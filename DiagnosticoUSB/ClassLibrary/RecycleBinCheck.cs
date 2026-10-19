namespace DiagnosticoUSB.Core;

public class RecycleBinCheck : IDiagnosticCheck {
    public string Name => "Papelera";

    public Task<CheckResult> RunAsync() {
        var (bytes, _) = RecycleBinNative.Query();
        return Task.FromResult(Evaluate(bytes));
    }

    // Separada para poder testearla sin tocar la Papelera real
    public static CheckResult Evaluate(long bytes) {
        string size = ByteFormatter.Format(bytes);

        return bytes >= 100L * 1024 * 1024
            ? new CheckResult("Papelera", Severity.Warning,
                $"La Papelera ocupa {size}. Vaciarla liberaría ese espacio.", new EmptyRecycleBinFix())
            : new CheckResult("Papelera", Severity.Ok, $"La Papelera ocupa {size}.");
    }
}
