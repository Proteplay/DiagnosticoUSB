namespace DiagnosticoUSB.Core;

public record FixEstimate(long Bytes, int Files);
public record FixResult(long BytesFreed, int FilesDeleted, int FilesSkipped);

public interface IFix {
    string Name { get; }
    string Description { get; }
    Task<FixEstimate> EstimateAsync();   // simula, NO borra nada
    Task<FixResult> ApplyAsync();        // aplica la solución de verdad
}
