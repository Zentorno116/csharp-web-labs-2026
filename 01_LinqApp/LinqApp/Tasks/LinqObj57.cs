using LinqApp.Models;

namespace LinqApp.Tasks;

/// <summary>
/// LinqObj57: Для каждой школы вывести фамилии и инициалы первых 3 учащихся (в алфавитном порядке),
/// набравших менее 50 баллов по каждому предмету ЕГЭ.
/// Формат: &lt;Школа&gt; &lt;Фамилия&gt; &lt;Инициалы&gt;. Если нет ни одного — «Требуемые учащиеся не найдены».
/// </summary>
public static class LinqObj57
{
    public static List<string> Solve(IEnumerable<string> lines)
    {
        var result = lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new EgeRecord(
                    School: int.Parse(s[0]),
                    Surname: s[1],
                    Initials: s[2],
                    Math: int.Parse(s[3]),
                    Russian: int.Parse(s[4]),
                    Informatics: int.Parse(s[5]));
            })
            .Where(x => x.Math < 50 && x.Russian < 50 && x.Informatics < 50)
            .GroupBy(x => x.School)
            .OrderBy(g => g.Key)
            .SelectMany(g => g
                .OrderBy(x => $"{x.Surname} {x.Initials}", StringComparer.Ordinal)
                .Take(3))
            .Select(x => $"{x.School} {x.Surname} {x.Initials}")
            .ToList();

        return result.Count > 0 ? result : ["Требуемые учащиеся не найдены"];
    }

    public static List<string> SolveFiles(string inputPath, string outputPath)
    {
        var result = Solve(EncodingHelper.ReadWin1251Lines(inputPath));
        EncodingHelper.WriteWin1251Lines(outputPath, result);
        return result;
    }
}
