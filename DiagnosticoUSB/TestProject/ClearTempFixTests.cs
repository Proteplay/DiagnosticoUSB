using DiagnosticoUSB.Core;

public class ClearTempFixTests : IDisposable {
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "DiagUSB_test_" + Guid.NewGuid());

    public ClearTempFixTests() {
        Directory.CreateDirectory(Path.Combine(_dir, "sub"));
        File.WriteAllText(Path.Combine(_dir, "a.tmp"), "hola");
        File.WriteAllText(Path.Combine(_dir, "sub", "b.tmp"), "mundo");
    }

    public void Dispose() {
        if (Directory.Exists(_dir)) Directory.Delete(_dir, true);
    }

    [Fact]
    public async Task Estimate_cuenta_los_archivos_sin_borrar() {
        var fix = new ClearTempFix(_dir);
        var estimate = await fix.EstimateAsync();

        Assert.Equal(2, estimate.Files);
        Assert.True(File.Exists(Path.Combine(_dir, "a.tmp")));   // sigue existiendo
    }

    [Fact]
    public async Task Apply_borra_los_archivos() {
        var fix = new ClearTempFix(_dir);
        var result = await fix.ApplyAsync();

        Assert.Equal(2, result.FilesDeleted);
        Assert.False(File.Exists(Path.Combine(_dir, "a.tmp")));
    }

    [Fact]
    public void Disco_critico_ofrece_solucion_y_disco_ok_no() {
        Assert.NotNull(DiskCheck.Evaluate("C:\\", 5).Fix);
        Assert.Null(DiskCheck.Evaluate("C:\\", 50).Fix);
    }
}
