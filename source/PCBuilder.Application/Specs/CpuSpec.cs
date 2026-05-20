namespace PCBuilder.Application;

public class CpuSpec
{
    public string? Socket { get; set; }
    public int? Cores { get; set; }
    public int? Threads { get; set; }
    public double? BaseClockGhz { get; set; }
    public double? BoostClockGhz { get; set; }
    public int? TdpWatt { get; set; }
}
