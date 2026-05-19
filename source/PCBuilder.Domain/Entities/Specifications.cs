namespace PCBuilder.Domain;

/// <summary>
/// Contains all product specifications stored as JSON
/// </summary>
public class Specifications
{
    // CPU Specifications
    public string? CpuSocket { get; set; }
    public int? CpuCores { get; set; }
    public int? CpuThreads { get; set; }
    public double? CpuBaseClockGhz { get; set; }
    public double? CpuBoostClockGhz { get; set; }
    public int? CpuTdpWatt { get; set; }

    // Motherboard Specifications
    public string? MotherboardSocket { get; set; }
    public string? MotherboardChipset { get; set; }
    public string? MotherboardFormFactor { get; set; }
    public int? MotherboardMemorySlots { get; set; }

    // RAM Specifications
    public string? RamType { get; set; }
    public int? RamCapacityGb { get; set; }
    public int? RamSpeedMhz { get; set; }
    public int? RamModules { get; set; }

    // GPU Specifications
    public int? GpuVramGb { get; set; }
    public string? GpuMemoryType { get; set; }
    public int? GpuTdpWatt { get; set; }
    public int? GpuLengthMm { get; set; }

    // PSU Specifications
    public int? PsuWattage { get; set; }
    public string? PsuEfficiency { get; set; }
    public string? PsuModularType { get; set; }

    // Case Specifications
    public string? CaseFormFactor { get; set; }
    public int? CaseFanSlots { get; set; }
    public int? CaseMaxGpuLengthMm { get; set; }

    // Storage Specifications
    public string? StorageType { get; set; }
    public int? StorageCapacityGb { get; set; }
    public string? StorageInterface { get; set; }
    public int? StorageReadSpeedMb { get; set; }
}
