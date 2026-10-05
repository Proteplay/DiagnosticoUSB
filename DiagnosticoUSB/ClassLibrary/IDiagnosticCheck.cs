namespace DiagnosticoUSB.Core;

public enum Severity { Ok, Warning, Critical }

public record CheckResult(string Name, Severity Severity, string Message, IFix? Fix = null);

public interface IDiagnosticCheck {
    string Name { get; }
    Task<CheckResult> RunAsync();
}
