using LinqApp.Models;

namespace LinqApp.Tasks;

/// <summary>
/// LinqObj47: Найти все АЗС, предлагавшие не менее 2 марок бензина.
/// Формат: &lt;Компания&gt; &lt;Улица&gt; &lt;Число марок&gt;. Сортировка по компании, затем по улице.
/// Если нет подходящих АЗС — вывести «Нет».
/// </summary>
public static class LinqObj47
{
    public static List<string> Solve(IEnumerable<string> lines)
    {
        var result = lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new GasStationRecord(
                    Company: s[1],
                    Brand: int.Parse(s[3]),
                    Price: int.Parse(s[0]),
                    Street: s[2]);
            })
            .GroupBy(x => new { x.Company, x.Street })
            .Select(g => new
            {
                g.Key.Company,
                g.Key.Street,
                BrandCount = g.Select(x => x.Brand).Distinct().Count()
            })
            .Where(x => x.BrandCount >= 2)
            .OrderBy(x => x.Company, StringComparer.Ordinal)
            .ThenBy(x => x.Street, StringComparer.Ordinal)
            .Select(x => $"{x.Company} {x.Street} {x.BrandCount}")
            .ToList();

        return result.Count > 0 ? result : ["Нет"];
    }

    public static List<string> SolveFiles(string inputPath, string outputPath)
    {
        var result = Solve(EncodingHelper.ReadWin1251Lines(inputPath));
        EncodingHelper.WriteWin1251Lines(outputPath, result);
        return result;
    }
}
