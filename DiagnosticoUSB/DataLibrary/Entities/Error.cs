namespace DiagnosticoUSB.Data.Entities;

public class Error {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string NivelGravedad { get; set; } = "";

    public int DiagnosticoId { get; set; }
    public Diagnostico Diagnostico { get; set; } = null!;
}
