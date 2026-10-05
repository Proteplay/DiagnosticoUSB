using System.Management;

namespace DiagnosticoUSB.Core;

public class RamCheck : IDiagnosticCheck {
    public string Name => "Memoria RAM";

    public Task<CheckResult> RunAsync() {
        double percent = GetUsedPercent();
        return Task.FromResult(Evaluate(percent));
    }

    private static double GetUsedPercent() {
        using var searcher = new ManagementObjectSearcher(
            "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");

        foreach (ManagementObject os in searcher.Get()) {
            double total = Convert.ToDouble(os["TotalVisibleMemorySize"]);
            double free = Convert.ToDouble(os["FreePhysicalMemory"]);
            return 100.0 * (total - free) / total;
        }

        throw new InvalidOperationException("No se pudo leer la memoria del sistema.");
    }

    // Separada para poder testearla sin depender del PC real
    public static CheckResult Evaluate(double usedPercent) => usedPercent switch {
        >= 90 => new("Memoria RAM", Severity.Critical, $"Uso de RAM crítico: {usedPercent:F0}%"),
        >= 75 => new("Memoria RAM", Severity.Warning, $"Uso de RAM alto: {usedPercent:F0}%"),
        _ => new("Memoria RAM", Severity.Ok, $"Uso de RAM normal: {usedPercent:F0}%")
    };
}