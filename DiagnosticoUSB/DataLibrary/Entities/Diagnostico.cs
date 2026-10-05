namespace DiagnosticoUSB.Data.Entities;

public class Diagnostico {
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Resultado { get; set; } = "";

    public int DispositivoId { get; set; }
    public Dispositivo Dispositivo { get; set; } = null!;

    public int PlanId { get; set; }
    public PlanDiagnostico Plan { get; set; } = null!;

    public List<Error> Errores { get; set; } = new();
}