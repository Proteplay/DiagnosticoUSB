namespace DiagnosticoUSB.Core;

public class ClearTempFix : IFix {
    private readonly string _path;

    // El parámetro permite testearlo con una carpeta de prueba
    public ClearTempFix(string? path = null) => _path = path ?? Path.GetTempPath();

    public string Name => "Limpiar archivos temporales";
    public string Description => $"Elimina los archivos temporales de {_path}. Los que estén en uso se omiten.";

    private static readonly EnumerationOptions Options = new() {
        RecurseSubdirectories = true,
        IgnoreInaccessible = true,
        AttributesToSkip = FileAttributes.ReparsePoint | FileAttributes.System
    };

    public Task<FixEstimate> EstimateAsync() {
        long bytes = 0;
        int files = 0;

        if (Directory.Exists(_path)) {
            foreach (var f in new DirectoryInfo(_path).EnumerateFiles("*", Options)) {
                bytes += f.Length;
                files++;
            }
        }
        return Task.FromResult(new FixEstimate(bytes, files));
    }

    public Task<FixResult> ApplyAsync() => Task.Run(() => {
        long freed = 0;
        int deleted = 0, skipped = 0;

        if (!Directory.Exists(_path)) return new FixResult(0, 0, 0);

        var root = new DirectoryInfo(_path);

        foreach (var f in root.EnumerateFiles("*", Options).ToList()) {
            try {
                long len = f.Length;
                f.Delete();
                freed += len;
                deleted++;
            }
            catch (IOException) { skipped++; }                  // archivo en uso
            catch (UnauthorizedAccessException) { skipped++; }  // sin permisos
        }

        // Quitar carpetas que hayan quedado vacías (las más profundas primero)
        foreach (var d in root.EnumerateDirectories("*", Options)
                              .OrderByDescending(x => x.FullName.Length).ToList()) {
            try { if (!d.EnumerateFileSystemInfos().Any()) d.Delete(); }
            catch { /* si no se puede, se ignora */ }
        }

        return new FixResult(freed, deleted, skipped);
    });
}