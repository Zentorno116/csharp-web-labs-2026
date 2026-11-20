using System.Globalization;
using LinqApp.Models;

namespace LinqApp.Tasks;

/// <summary>
/// LinqObj27: 144-квартирный 9-этажный дом (4 подъезда, 4 квартиры на этаже).
/// Для каждого этажа с должниками вывести: &lt;Число задолжников&gt; &lt;Этаж&gt; &lt;Суммарный долг&gt;.
/// Сортировка по возрастанию числа задолжников, затем по возрастанию этажа.
/// </summary>
public static class LinqObj27
{
    public static List<string> Solve(IEnumerable<string> lines)
    {
        return lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new DebtorRecord(
                    Surname: s[0],
                    Apartment: int.Parse(s[1]),
                    Debt: decimal.Parse(s[2], CultureInfo.InvariantCulture));
            })
            .GroupBy(x => ((x.Apartment - 1) % 36) / 4 + 1)
            .Select(g => new
            {
                Count = g.Count(),
                Floor = g.Key,
                TotalDebt = g.Sum(x => x.Debt)
            })
            .OrderBy(x => x.Count)
            .ThenBy(x => x.Floor)
            .Select(x => $"{x.Count} {x.Floor} {x.TotalDebt.ToString("F2", CultureInfo.InvariantCulture)}")
            .ToList();
    }

    public static List<string> SolveFiles(string inputPath, string outputPath)
    {
        var result = Solve(EncodingHelper.ReadWin1251Lines(inputPath));
        EncodingHelper.WriteWin1251Lines(outputPath, result);
        return result;
    }
}
