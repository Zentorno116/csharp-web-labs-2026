using LinqApp.Models;

namespace LinqApp.Tasks;

/// <summary>
/// LinqObj67: Найти всех двоечников (хотя бы одна двойка по любому предмету).
/// Формат: &lt;Класс&gt; &lt;Фамилия&gt; &lt;Инициалы&gt; &lt;Число двоек&gt;.
/// Сортировка по убыванию классов, затем по алфавиту фамилий и инициалов.
/// </summary>
public static class LinqObj67
{
    public static List<string> Solve(IEnumerable<string> lines)
    {
        var result = lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new SchoolGradeRecord(
                    GradeClass: int.Parse(s[0]),
                    Subject: s[1],
                    Surname: s[2],
                    Initials: s[3],
                    Mark: int.Parse(s[4]));
            })
            .Where(x => x.Mark == 2)
            .GroupBy(x => new { x.GradeClass, x.Surname, x.Initials })
            .Select(g => new
            {
                g.Key.GradeClass,
                g.Key.Surname,
                g.Key.Initials,
                TwosCount = g.Count()
            })
            .OrderByDescending(x => x.GradeClass)
            .ThenBy(x => $"{x.Surname} {x.Initials}", StringComparer.Ordinal)
            .Select(x => $"{x.GradeClass} {x.Surname} {x.Initials} {x.TwosCount}")
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
