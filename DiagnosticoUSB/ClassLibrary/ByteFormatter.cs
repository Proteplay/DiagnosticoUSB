namespace DiagnosticoUSB.Core;

public static class ByteFormatter {
    private static readonly string[] Units = { "B", "KB", "MB", "GB", "TB" };

    public static string Format(long bytes) {
        double value = bytes;
        int unit = 0;

        while (value >= 1024 && unit < Units.Length - 1) {
            value /= 1024;
            unit++;
        }

        // Los bytes sueltos no llevan decimales; el resto, dos
        return unit == 0 ? $"{bytes} B" : $"{value:F2} {Units[unit]}";
    }
}