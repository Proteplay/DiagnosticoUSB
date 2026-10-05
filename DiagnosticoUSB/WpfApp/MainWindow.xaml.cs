using System.Windows;
using System.Windows.Media;
using DiagnosticoUSB.Core;
using DiagnosticoUSB.Data;

namespace WpfApp;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        Loaded += async (_, _) => await CargarHistorialAsync();
    }

    private async void BtnRun_Click(object sender, RoutedEventArgs e) {
        BtnRun.IsEnabled = false;
        Progress.Visibility = Visibility.Visible;
        StatusText.Text = "Ejecutando diagnóstico...";

        try {
            var runner = new DiagnosticRunner(new IDiagnosticCheck[]
            {
                new RamCheck(),
                new DiskCheck()
            });

            var results = await runner.RunAllAsync();
            ResultsList.ItemsSource = results.Select(r => new ResultRow(r)).ToList();

            int id;
            using (var db = new DiagnosticoContext()) {
                var repo = new DiagnosticoRepository(db);
                id = await repo.GuardarAsync(results);
            }

            StatusText.Text = $"Diagnóstico guardado (id {id}).";
            await CargarHistorialAsync();
        }
        catch (Exception ex) {
            StatusText.Text = "Error durante el diagnóstico.";
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            Progress.Visibility = Visibility.Collapsed;
            BtnRun.IsEnabled = true;
        }
    }

    private async void BtnFix_Click(object sender, RoutedEventArgs e) {
        if (((FrameworkElement)sender).Tag is not ResultRow row || row.Fix is null) return;

        try {
            var estimate = await row.Fix.EstimateAsync();
            var ask = MessageBox.Show(
                $"{row.Fix.Description}\n\nSe liberarían unos {FormatBytes(estimate.Bytes)} ({estimate.Files} archivos).\n\n¿Aplicar?",
                row.Fix.Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (ask != MessageBoxResult.Yes) return;

            var done = await row.Fix.ApplyAsync();
            MessageBox.Show(
                $"Liberados {FormatBytes(done.BytesFreed)}.\nBorrados: {done.FilesDeleted}. Omitidos (en uso): {done.FilesSkipped}.",
                row.Fix.Name, MessageBoxButton.OK, MessageBoxImage.Information);

            StatusText.Text = "Solución aplicada. Vuelve a ejecutar el diagnóstico para comprobar el resultado.";
        }
        catch (Exception ex) {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e) => await CargarHistorialAsync();

    private async Task CargarHistorialAsync() {
        using var db = new DiagnosticoContext();
        var repo = new DiagnosticoRepository(db);
        var lista = await repo.HistorialAsync();

        HistorialList.ItemsSource = lista
            .Select(d => new HistorialRow(d.Id, d.Fecha.ToString("dd/MM/yyyy HH:mm"), d.Dispositivo.Nombre, d.Resultado))
            .ToList();
    }

    private static string FormatBytes(long bytes) => bytes switch {
        >= 1_073_741_824 => $"{bytes / 1_073_741_824.0:F1} GB",
        >= 1_048_576 => $"{bytes / 1_048_576.0:F1} MB",
        _ => $"{bytes / 1024.0:F0} KB"
    };
}

// Fila que se muestra en la lista de resultados
public class ResultRow {
    public ResultRow(CheckResult r) {
        Name = r.Name;
        Message = r.Message;
        Fix = r.Fix;
        Brush = r.Severity switch {
            Severity.Critical => Brushes.Crimson,
            Severity.Warning => Brushes.Orange,
            _ => Brushes.SeaGreen
        };
    }

    public string Name { get; }
    public string Message { get; }
    public IFix? Fix { get; }
    public Brush Brush { get; }
    public Visibility FixVisibility => Fix is null ? Visibility.Collapsed : Visibility.Visible;
}

// Fila que se muestra en el historial
public record HistorialRow(int Id, string Fecha, string Dispositivo, string Resultado);