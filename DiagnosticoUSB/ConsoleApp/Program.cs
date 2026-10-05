using DiagnosticoUSB.Core;
using DiagnosticoUSB.Data;

var runner = new DiagnosticRunner(new IDiagnosticCheck[]
{
    new RamCheck(),
    new DiskCheck()
});

var results = await runner.RunAllAsync();
foreach (var r in results)
    Console.WriteLine($"[{r.Severity}] {r.Name}: {r.Message}");

using (var db = new DiagnosticoContext()) {
    var repo = new DiagnosticoRepository(db);
    int id = await repo.GuardarAsync(results);
    Console.WriteLine($"\nDiagnóstico guardado (id {id}).");

    Console.WriteLine("\nHistorial:");
    foreach (var d in await repo.HistorialAsync())
        Console.WriteLine($"  #{d.Id} {d.Fecha:dd/MM/yyyy HH:mm} - {d.Dispositivo.Nombre} - {d.Resultado}");
}

foreach (var r in results.Where(x => x.Fix is not null)) {
    var fix = r.Fix!;
    var estimate = await fix.EstimateAsync();

    Console.WriteLine();
    Console.WriteLine($"Solución disponible: {fix.Name}");
    Console.WriteLine($"  {fix.Description}");
    Console.WriteLine($"  Se liberarían unos {FormatBytes(estimate.Bytes)} ({estimate.Files} archivos).");
    Console.Write("  ¿Aplicar? (s/n): ");

    if (Console.ReadLine()?.Trim().ToLower() == "s") {
        var done = await fix.ApplyAsync();
        Console.WriteLine($"  Liberados {FormatBytes(done.BytesFreed)}. " +
                          $"Borrados: {done.FilesDeleted}. Omitidos (en uso): {done.FilesSkipped}.");
    }
    else {
        Console.WriteLine("  No se ha borrado nada.");
    }
}

static string FormatBytes(long bytes) => bytes switch {
    >= 1_073_741_824 => $"{bytes / 1_073_741_824.0:F1} GB",
    >= 1_048_576 => $"{bytes / 1_048_576.0:F1} MB",
    _ => $"{bytes / 1024.0:F0} KB"
};