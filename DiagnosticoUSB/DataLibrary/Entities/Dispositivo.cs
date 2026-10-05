namespace DiagnosticoUSB.Data.Entities;

public class Dispositivo {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string SistemaOperativo { get; set; } = "";

    public List<Diagnostico> Diagnosticos { get; set; } = new();
}