using LinqApp.Models;

namespace LinqApp.Tasks;

/// <summary>
/// LinqObj7: Для каждого года посещений клиента K найти месяц с максимальной продолжительностью занятий
/// (при равенстве — наименьший номер месяца). Сортировка по убыванию года.
/// </summary>
public static class LinqObj07
{
    public static List<string> Solve(int clientCode, IEnumerable<string> lines)
    {
        var result = lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new FitnessRecord(
                    Hours: int.Parse(s[0]),
                    Year: int.Parse(s[1]),
                    Month: int.Parse(s[2]),
                    ClientId: int.Parse(s[3]));
            })
            .Where(x => x.ClientId == clientCode)
            .GroupBy(x => x.Year)
            .Select(g => g
                .OrderByDescending(x => x.Hours)
                .ThenBy(x => x.Month)
                .First())
            .OrderByDescending(x => x.Year)
            .Select(x => $"{x.Year} {x.Month} {x.Hours}")
            .ToList();

        return result.Count > 0 ? result : ["Нет данных"];
    }

    public static List<string> SolveFiles(int clientCode, string inputPath, string outputPath)
    {
        var result = Solve(clientCode, EncodingHelper.ReadWin1251Lines(inputPath));
        EncodingHelper.WriteWin1251Lines(outputPath, result);
        return result;
    }
}
