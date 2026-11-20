using System.Text;

namespace LinqApp;

public static class EncodingHelper
{
    static EncodingHelper()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static Encoding Win1251 => Encoding.GetEncoding(1251);

    public static string[] ReadWin1251Lines(string path) =>
        File.ReadAllLines(path, Win1251);

    public static void WriteWin1251Lines(string path, IEnumerable<string> lines)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllLines(path, lines, Win1251);
    }
}
