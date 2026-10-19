using DiagnosticoUSB.Core;

public class ByteFormatterTests {
    [Theory]
    [InlineData(0, "0 B")]
    [InlineData(900, "900 B")]
    [InlineData(1024, "1,00 KB")]
    [InlineData(1_048_576, "1,00 MB")]
    [InlineData(1_073_741_824, "1,00 GB")]
    [InlineData(1_649_267_441, "1,54 GB")]
    public void Format_cambia_de_unidad_cada_1024(long bytes, string esperado) {
        Assert.Equal(esperado, ByteFormatter.Format(bytes));
    }
}