using System.Runtime.InteropServices;
using System.Security.Principal;

namespace DiagnosticoUSB.Core;

internal static class RecycleBinNative {
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern int SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint flags);

    public const uint SHERB_NOCONFIRMATION = 0x1;
    public const uint SHERB_NOPROGRESSUI = 0x2;
    public const uint SHERB_NOSOUND = 0x4;

    // Hay que incluir archivos ocultos y de sistema, porque la Papelera los usa
    private static readonly EnumerationOptions Todo = new() {
        RecurseSubdirectories = true,
        IgnoreInaccessible = true,
        AttributesToSkip = 0
    };

    public static (long Bytes, long Items) Query() {
        string sid = WindowsIdentity.GetCurrent().User!.Value;
        long bytes = 0, items = 0;

        foreach (var drive in DriveInfo.GetDrives()) {
            if (drive.DriveType != DriveType.Fixed || !drive.IsReady) continue;

            string carpeta = Path.Combine(drive.Name, "$Recycle.Bin", sid);
            if (!Directory.Exists(carpeta)) continue;

            var raiz = new DirectoryInfo(carpeta);

            // Cada elemento borrado tiene un archivo $I (datos) y un $R (contenido)
            foreach (var e in raiz.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly))
                if (e.Name.StartsWith("$I", StringComparison.OrdinalIgnoreCase)) items++;

            foreach (var f in raiz.EnumerateFiles("*", Todo))
                if (!f.Name.StartsWith("$I", StringComparison.OrdinalIgnoreCase))
                    bytes += f.Length;
        }

        return (bytes, items);
    }
}