namespace DiagnosticoUSB.Core;

public class DiskCheck : IDiagnosticCheck {
    public string Name => "Disco";

    public Task<CheckResult> RunAsync() {
        string root = Path.GetPathRoot(Environment.SystemDirectory)!; // normalmente C:\
        var drive = new DriveInfo(root);
        double freePercent = 100.0 * drive.AvailableFreeSpace / drive.TotalSize;
        return Task.FromResult(Evaluate(root, freePercent));
    }

    public static CheckResult Evaluate(string unidad, double freePercent) => freePercent switch {
        <= 10 => new("Disco", Severity.Critical, $"Espacio libre crítico en {unidad}: {freePercent:F0}%", new ClearTempFix()),
        <= 20 => new("Disco", Severity.Warning, $"Poco espacio libre en {unidad}: {freePercent:F0}%", new ClearTempFix()),
        _ => new("Disco", Severity.Ok, $"Espacio libre correcto en {unidad}: {freePercent:F0}%")
    };
}