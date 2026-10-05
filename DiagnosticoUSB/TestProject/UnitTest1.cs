using DiagnosticoUSB.Core;

public class RamCheckTests {
    [Theory]
    [InlineData(40, Severity.Ok)]
    [InlineData(80, Severity.Warning)]
    [InlineData(95, Severity.Critical)]
    public void Evaluate_devuelve_la_severidad_correcta(double uso, Severity esperada) {
        Assert.Equal(esperada, RamCheck.Evaluate(uso).Severity);
    }
}
