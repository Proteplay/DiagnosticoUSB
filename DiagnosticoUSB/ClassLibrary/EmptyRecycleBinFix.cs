namespace DiagnosticoUSB.Core;

public class EmptyRecycleBinFix : IFix {
    public string Name => "Vaciar la Papelera";
    public string Description =>
        "Elimina DEFINITIVAMENTE todo el contenido de la Papelera de reciclaje. Esta acción no se puede deshacer.";

    public Task<FixEstimate> EstimateAsync() {
        var (bytes, items) = RecycleBinNative.Query();
        return Task.FromResult(new FixEstimate(bytes, (int)items));
    }

    public Task<FixResult> ApplyAsync() => Task.Run(() => {
        var (bytes, items) = RecycleBinNative.Query();
        if (items == 0) return new FixResult(0, 0, 0);

        int hr = RecycleBinNative.SHEmptyRecycleBin(
            IntPtr.Zero, null,
            RecycleBinNative.SHERB_NOCONFIRMATION |
            RecycleBinNative.SHERB_NOPROGRESSUI |
            RecycleBinNative.SHERB_NOSOUND);

        if (hr != 0)
            throw new InvalidOperationException($"No se pudo vaciar la Papelera (código 0x{hr:X}).");

        return new FixResult(bytes, (int)items, 0);
    });
}
