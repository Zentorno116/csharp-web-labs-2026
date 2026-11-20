using LinqApp.Models;

namespace LinqApp.Tasks;

/// <summary>
/// LinqObj17: Для каждого года вывести число различных школ абитуриентов, поступивших в этом году.
/// Формат: &lt;Число школ&gt; &lt;Год&gt;. Сортировка по возрастанию числа школ, затем по возрастанию года.
/// </summary>
public static class LinqObj17
{
    public static List<string> Solve(IEnumerable<string> lines)
    {
        return lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new ApplicantRecord(
                    School: int.Parse(s[0]),
                    Year: int.Parse(s[1]),
                    Surname: s[2]);
            })
            .GroupBy(x => x.Year)
            .Select(g => new
            {
                Year = g.Key,
                SchoolCount = g.Select(x => x.School).Distinct().Count()
            })
            .OrderBy(x => x.SchoolCount)
            .ThenBy(x => x.Year)
            .Select(x => $"{x.SchoolCount} {x.Year}")
            .ToList();
    }

    public static List<string> SolveFiles(string inputPath, string outputPath)
    {
        var result = Solve(EncodingHelper.ReadWin1251Lines(inputPath));
        EncodingHelper.WriteWin1251Lines(outputPath, result);
        return result;
    }
}
