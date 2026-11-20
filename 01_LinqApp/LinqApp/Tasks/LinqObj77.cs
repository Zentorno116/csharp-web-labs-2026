using LinqApp.Models;

namespace LinqApp.Tasks;

/// <summary>
/// LinqObj77 (B, D): Для каждой категории товаров определить количество магазинов, предлагающих
/// товары данной категории, и количество стран-производителей этих товаров, представленных в магазинах.
/// Формат: &lt;Число магазинов&gt; &lt;Категория&gt; &lt;Число стран&gt;.
/// </summary>
public static class LinqObj77
{
    public static List<string> Solve(IEnumerable<string> linesB, IEnumerable<string> linesD)
    {
        var products = linesB
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new ProductRecord(Category: s[0], Article: s[1], Country: s[2]);
            });

        var prices = linesD
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new PriceRecord(Article: s[0], Price: int.Parse(s[1]), Store: s[2]);
            });

        return products
            .Join(prices,
                b => b.Article,
                d => d.Article,
                (b, d) => new { b.Category, b.Country, d.Store })
            .GroupBy(x => x.Category)
            .Select(g => new
            {
                Category = g.Key,
                StoreCount = g.Select(x => x.Store).Distinct().Count(),
                CountryCount = g.Select(x => x.Country).Distinct().Count()
            })
            .OrderByDescending(x => x.StoreCount)
            .ThenBy(x => x.Category, StringComparer.Ordinal)
            .Select(x => $"{x.StoreCount} {x.Category} {x.CountryCount}")
            .ToList();
    }

    public static List<string> SolveFiles(string pathB, string pathD, string outputPath)
    {
        var result = Solve(
            EncodingHelper.ReadWin1251Lines(pathB),
            EncodingHelper.ReadWin1251Lines(pathD));
        EncodingHelper.WriteWin1251Lines(outputPath, result);
        return result;
    }
}
