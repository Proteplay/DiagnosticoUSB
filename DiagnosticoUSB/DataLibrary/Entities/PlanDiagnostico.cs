namespace DiagnosticoUSB.Data.Entities;

public class PlanDiagnostico {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int Nivel { get; set; }
}