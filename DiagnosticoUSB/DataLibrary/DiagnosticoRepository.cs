using DiagnosticoUSB.Core;
using DiagnosticoUSB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiagnosticoUSB.Data;

public class DiagnosticoRepository {
    private readonly DiagnosticoContext _db;

    public DiagnosticoRepository(DiagnosticoContext db) {
        _db = db;
        _db.Database.EnsureCreated();   // crea el archivo .db y las tablas si no existen
    }

    public async Task<int> GuardarAsync(List<CheckResult> resultados, int planId = 1) {
        string nombrePc = Environment.MachineName;

        var dispositivo = await _db.Dispositivos.FirstOrDefaultAsync(d => d.Nombre == nombrePc)
            ?? new Dispositivo {
                Nombre = nombrePc,
                SistemaOperativo = System.Runtime.InteropServices.RuntimeInformation.OSDescription
            };

        var problemas = resultados.Where(r => r.Severity != Severity.Ok).ToList();

        var diagnostico = new Diagnostico {
            Dispositivo = dispositivo,
            PlanId = planId,
            Resultado = problemas.Count == 0
                ? "Sin problemas"
                : $"{problemas.Count} problema(s) detectado(s)",
            Errores = problemas.Select(p => new Error {
                Nombre = p.Name,
                Descripcion = p.Message,
                NivelGravedad = p.Severity.ToString()
            }).ToList()
        };

        _db.Diagnosticos.Add(diagnostico);
        await _db.SaveChangesAsync();
        return diagnostico.Id;
    }

    public Task<List<Diagnostico>> HistorialAsync() =>
        _db.Diagnosticos
            .Include(d => d.Dispositivo)
            .Include(d => d.Errores)
            .OrderByDescending(d => d.Fecha)
            .ToListAsync();
}
