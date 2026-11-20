using LinqApp.Models;

namespace LinqApp.Tasks;

/// <summary>
/// LinqObj37: Для каждой марки бензина найти минимальную и максимальную цену 1 литра (в копейках).
/// Формат: &lt;Марка&gt; &lt;МинЦена&gt; &lt;МаксЦена&gt;. Сортировка по убыванию марки.
/// </summary>
public static class LinqObj37
{
    public static List<string> Solve(IEnumerable<string> lines)
    {
        return lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new GasStationRecord(
                    Company: s[0],
                    Brand: int.Parse(s[1]),
                    Price: int.Parse(s[2]),
                    Street: s[3]);
            })
            .GroupBy(x => x.Brand)
            .Select(g => new
            {
                Brand = g.Key,
                MinPrice = g.Min(x => x.Price),
                MaxPrice = g.Max(x => x.Price)
            })
            .OrderByDescending(x => x.Brand)
            .Select(x => $"{x.Brand} {x.MinPrice} {x.MaxPrice}")
            .ToList();
    }

    public static List<string> SolveFiles(string inputPath, string outputPath)
    {
        var result = Solve(EncodingHelper.ReadWin1251Lines(inputPath));
        EncodingHelper.WriteWin1251Lines(outputPath, result);
        return result;
    }
}
