using DiagnosticoUSB.Core;
using DiagnosticoUSB.Data;

public class RepositoryTests : IDisposable {
    private readonly string _file = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");

    public void Dispose() {
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (File.Exists(_file)) File.Delete(_file);
    }

    [Fact]
    public async Task Guarda_solo_los_problemas_como_errores() {
        using var db = new DiagnosticoContext(_file);
        var repo = new DiagnosticoRepository(db);

        var resultados = new List<CheckResult>
        {
            new("Memoria RAM", Severity.Ok, "bien"),
            new("Disco", Severity.Critical, "mal")
        };

        await repo.GuardarAsync(resultados);
        var historial = await repo.HistorialAsync();

        Assert.Single(historial);
        Assert.Single(historial[0].Errores);
        Assert.Equal("Disco", historial[0].Errores[0].Nombre);
    }
}