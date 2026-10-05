using DiagnosticoUSB.Core;

public class DiskCheckTests {
    [Theory]
    [InlineData(50, Severity.Ok)]
    [InlineData(15, Severity.Warning)]
    [InlineData(5, Severity.Critical)]
    public void Evaluate_devuelve_la_severidad_correcta(double libre, Severity esperada) {
        Assert.Equal(esperada, DiskCheck.Evaluate("C:\\", libre).Severity);
    }
}
