# JSON Specifications Setup - Migration Guide

## Tóm tắt những thay đổi

Hệ thống đã được refactor để lưu tất cả specifications dưới dạng **JSON column** thay vì tạo các bảng riêng cho từng loại spec.

### Trước (Old):
- Tạo 7 bảng riêng: CpuSpec, MotherboardSpec, RamSpec, GpuSpec, PsuSpec, CaseSpec, StorageSpec
- Sử dụng `OwnsOne()` configuration trong EF Core

### Sau (New):
- **1 JSON column** `SpecsJson` trong bảng `Products` (string chứa JSON)
- Lưu các specs bằng cách serialize các class typed (ví dụ `CpuSpec`, `GpuSpec`)
- Giảm độ phức tạp của schema database

## Các file đã thay đổi

1. **Product.cs** - Thay thế các property riêng bằng `SpecsJson` string và helper `GetSpecs<T>()/SetSpecs<T>()`
2. **AppDbContext.cs** - Cấu hình `jsonb` column cho `SpecsJson`
3. **SpecificationsBuilder.cs** - (tùy chọn) có thể xóa nếu không dùng
4. **ProductService.cs** - Ví dụ service để tạo product với typed spec

## Database Migration

Chạy lệnh sau để tạo migration:

```bash
dotnet ef migrations add MigrateToJsonSpecifications -p source/PCBuilder.Infrastructure -s source/PCBuilder.API
```

Hoặc nếu dùng PowerShell:

```powershell
Add-Migration MigrateToJsonSpecifications -Project source/PCBuilder.Infrastructure -StartupProject source/PCBuilder.API
```

Sau đó áp dụng migration:

```bash
dotnet ef database update -p source/PCBuilder.Infrastructure -s source/PCBuilder.API
```

## Cách sử dụng

### 1. Tạo Product với Specifications

**Ví dụ lưu bằng `SpecsJson`**

```csharp
var product = new Product
{
    Name = "Intel Core i9",
    Brand = "Intel",
    Type = ProductTypeEnum.CPU,
    Price = 589.99m,
};

// dùng strongly-typed spec class
var cpuSpec = new CpuSpec
{
    Cores = 24,
    Threads = 32,
    Socket = "LGA1700",
    BaseClockGhz = 3.2,
    BoostClockGhz = 5.7,
    TdpWatt = 253
};

product.SetSpecs(cpuSpec);

context.Products.Add(product);
await context.SaveChangesAsync();
```

### 2. Truy vấn Specifications trong LINQ

```csharp
// Tìm tất cả CPU có >= 8 cores (client-side cast)
var cpus = context.Products
    .AsEnumerable()
    .Select(p => p.GetSpecs<CpuSpec>())
    .Where(s => s != null && s.Cores >= 8)
    .ToList();

// Hoặc dùng JSON operators trong DB (PostgreSQL) để filter server-side
// xem phần Database Query bên dưới
```

### 3. Cập nhật Specifications

```csharp
var product = await context.Products.FindAsync(productId);
var cpu = product?.GetSpecs<CpuSpec>();
if (product != null && cpu != null)
{
    cpu.Cores = 32;
    cpu.Threads = 64;
    product.SetSpecs(cpu);
    await context.SaveChangesAsync();
}
```

## Database Query (PostgreSQL)

Vì sử dụng `jsonb`, bạn có thể truy vấn JSON trực tiếp trong PostgreSQL:

```sql
-- Tìm tất cả products có CPU cores >= 8 (PostgreSQL jsonb)
SELECT * FROM "Products"
WHERE ("SpecsJson"->>'Cores')::int >= 8;

-- Tìm tất cả GPUs với VRAM >= 8GB
SELECT * FROM "Products"
WHERE ("SpecsJson"->>'VramGb')::int >= 8;

-- Cập nhật CPU cores
UPDATE "Products"
SET "SpecsJson" = jsonb_set("SpecsJson", '{Cores}', '32')
WHERE "Id" = 1;
```

## Lợi ích

✅ **Giảm số lượng bảng** - Từ 8 bảng xuống còn 1 column  
✅ **Linh hoạt hơn** - Dễ thêm spec mới mà không cần migration phức tạp  
✅ **Performance tốt hơn** - Ít join queries, có thể index JSON fields  
✅ **Code đơn giản hơn** - Không cần navigate qua nhiều entities  
✅ **Query tốt hơn** - JSON operators hỗ trợ nhiều truy vấn thông minh

## Migration từ dữ liệu cũ (nếu có)

Nếu bạn đã có dữ liệu trong các bảng spec cũ, thêm logic sau vào migration:

```csharp
migrationBuilder.Sql(@"
    UPDATE ""Products""
    SET ""Specs"" = jsonb_build_object(
        'CpuSocket', cpu.""Socket"",
        'CpuCores', cpu.""Cores"",
        -- ... map tất cả fields
    )
    FROM ""CpuSpec"" cpu
    WHERE -- matching logic
");
```

## Ghi chú

- Column type là `jsonb` cho PostgreSQL, `json` cho SQL Server
 - Các old spec tables có thể được xóa sau khi migration thành công
