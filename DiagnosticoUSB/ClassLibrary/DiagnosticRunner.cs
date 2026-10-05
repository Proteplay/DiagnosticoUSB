namespace DiagnosticoUSB.Core;

public class DiagnosticRunner {
    private readonly IEnumerable<IDiagnosticCheck> _checks;

    public DiagnosticRunner(IEnumerable<IDiagnosticCheck> checks) => _checks = checks;

    public async Task<List<CheckResult>> RunAllAsync() {
        var results = new List<CheckResult>();
        foreach (var check in _checks) {
            try {
                results.Add(await check.RunAsync());
            }
            catch (Exception ex) {
                // Un check que falla no debe tumbar el diagnóstico completo
                results.Add(new CheckResult(check.Name, Severity.Warning, $"No se pudo ejecutar: {ex.Message}"));
            }
        }
        return results;
    }
}