namespace PCBuilder.Domain;

/// <summary>
/// Helper class to build product specifications fluently
/// </summary>
public class SpecificationsBuilder
{
    private readonly Specifications _specs = new();

    public SpecificationsBuilder WithCpuSpec(
        string? socket = null,
        int? cores = null,
        int? threads = null,
        double? baseClockGhz = null,
        double? boostClockGhz = null,
        int? tdpWatt = null)
    {
        _specs.CpuSocket = socket;
        _specs.CpuCores = cores;
        _specs.CpuThreads = threads;
        _specs.CpuBaseClockGhz = baseClockGhz;
        _specs.CpuBoostClockGhz = boostClockGhz;
        _specs.CpuTdpWatt = tdpWatt;
        return this;
    }

    public SpecificationsBuilder WithMotherboardSpec(
        string? socket = null,
        string? chipset = null,
        string? formFactor = null,
        int? memorySlots = null)
    {
        _specs.MotherboardSocket = socket;
        _specs.MotherboardChipset = chipset;
        _specs.MotherboardFormFactor = formFactor;
        _specs.MotherboardMemorySlots = memorySlots;
        return this;
    }

    public SpecificationsBuilder WithRamSpec(
        string? type = null,
        int? capacityGb = null,
        int? speedMhz = null,
        int? modules = null)
    {
        _specs.RamType = type;
        _specs.RamCapacityGb = capacityGb;
        _specs.RamSpeedMhz = speedMhz;
        _specs.RamModules = modules;
        return this;
    }

    public SpecificationsBuilder WithGpuSpec(
        int? vramGb = null,
        string? memoryType = null,
        int? tdpWatt = null,
        int? lengthMm = null)
    {
        _specs.GpuVramGb = vramGb;
        _specs.GpuMemoryType = memoryType;
        _specs.GpuTdpWatt = tdpWatt;
        _specs.GpuLengthMm = lengthMm;
        return this;
    }

    public SpecificationsBuilder WithPsuSpec(
        int? wattage = null,
        string? efficiency = null,
        string? modularType = null)
    {
        _specs.PsuWattage = wattage;
        _specs.PsuEfficiency = efficiency;
        _specs.PsuModularType = modularType;
        return this;
    }

    public SpecificationsBuilder WithCaseSpec(
        string? formFactor = null,
        int? fanSlots = null,
        int? maxGpuLengthMm = null)
    {
        _specs.CaseFormFactor = formFactor;
        _specs.CaseFanSlots = fanSlots;
        _specs.CaseMaxGpuLengthMm = maxGpuLengthMm;
        return this;
    }

    public SpecificationsBuilder WithStorageSpec(
        string? type = null,
        int? capacityGb = null,
        string? @interface = null,
        int? readSpeedMb = null)
    {
        _specs.StorageType = type;
        _specs.StorageCapacityGb = capacityGb;
        _specs.StorageInterface = @interface;
        _specs.StorageReadSpeedMb = readSpeedMb;
        return this;
    }

    public Specifications Build() => _specs;
}
