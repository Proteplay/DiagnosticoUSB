using DiagnosticoUSB.Core;

public class RecycleBinTests {
    [Fact]
    public void Papelera_pequena_esta_ok_y_sin_solucion() {
        var r = RecycleBinCheck.Evaluate(10L * 1024 * 1024);   // 10 MB
        Assert.Equal(Severity.Ok, r.Severity);
        Assert.Null(r.Fix);
    }

    [Fact]
    public void Papelera_grande_da_aviso_y_ofrece_solucion() {
        var r = RecycleBinCheck.Evaluate(500L * 1024 * 1024);  // 500 MB
        Assert.Equal(Severity.Warning, r.Severity);
        Assert.NotNull(r.Fix);
    }
}
