# JSON Specifications Setup - Migration Guide

## Tóm tắt những thay đổi

Hệ thống đã được refactor để lưu tất cả specifications dưới dạng **JSON column** thay vì tạo các bảng riêng cho từng loại spec.

### Trước (Old):
- Tạo 7 bảng riêng: CpuSpec, MotherboardSpec, RamSpec, GpuSpec, PsuSpec, CaseSpec, StorageSpec
- Sử dụng `OwnsOne()` configuration trong EF Core

### Sau (New):
- **1 JSON column** `Specs` trong bảng `Products`
- Tất cả specifications được lưu trữ trong đối tượng `Specifications` duy nhất
- Giảm độ phức tạp của schema database

## Các file đã thay đổi

1. **Product.cs** - Thay thế các property riêng bằng 1 property `Specs` duy nhất
2. **AppDbContext.cs** - Cấu hình `jsonb` column thay vì `OwnsOne()`
3. **Specifications.cs** - Class mới chứa tất cả specs (auto-generated)
4. **SpecificationsBuilder.cs** - Helper class để build specs một cách fluent (tùy chọn)

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

**Cách 1: Trực tiếp**
```csharp
var product = new Product
{
    Name = "Intel Core i9",
    Brand = "Intel",
    Type = ProductTypeEnum.CPU,
    Price = 589.99m,
    Specs = new Specifications
    {
        CpuCores = 24,
        CpuThreads = 32,
        CpuSocket = "LGA1700",
        CpuBaseClockGhz = 3.2,
        CpuBoostClockGhz = 5.7,
        CpuTdpWatt = 253
    }
};
```

**Cách 2: Dùng Builder (Fluent)**
```csharp
var product = new Product
{
    Name = "Intel Core i9",
    Brand = "Intel",
    Type = ProductTypeEnum.CPU,
    Price = 589.99m,
    Specs = new SpecificationsBuilder()
        .WithCpuSpec(
            socket: "LGA1700",
            cores: 24,
            threads: 32,
            baseClockGhz: 3.2,
            boostClockGhz: 5.7,
            tdpWatt: 253)
        .Build()
};
```

### 2. Truy vấn Specifications trong LINQ

```csharp
// Tìm tất cả CPU có >= 8 cores
var cpus = context.Products
    .Where(p => p.Specs != null && p.Specs.CpuCores >= 8)
    .ToList();

// Tìm GPU với >= 8GB VRAM
var gpus = context.Products
    .Where(p => p.Specs != null && p.Specs.GpuVramGb >= 8)
    .ToList();
```

### 3. Cập nhật Specifications

```csharp
var product = await context.Products.FindAsync(productId);
if (product?.Specs != null)
{
    product.Specs.CpuCores = 32;
    product.Specs.CpuThreads = 64;
    await context.SaveChangesAsync();
}
```

## Database Query (PostgreSQL)

Vì sử dụng `jsonb`, bạn có thể truy vấn JSON trực tiếp trong PostgreSQL:

```sql
-- Tìm tất cả products có CPU cores >= 8
SELECT * FROM "Products"
WHERE "Specs"->'CpuCores' >= '8';

-- Tìm tất cả GPUs với VRAM >= 8GB
SELECT * FROM "Products"
WHERE "Specs"->'GpuVramGb' >= '8';

-- Cập nhật CPU cores
UPDATE "Products"
SET "Specs" = jsonb_set("Specs", '{CpuCores}', '32')
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
- Specifications có default value là empty object
- Các old spec tables có thể được xóa sau khi migration thành công
