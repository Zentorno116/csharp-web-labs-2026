using LinqApp.Models;

namespace LinqApp.Tasks;

/// <summary>
/// LinqObj97 (B, C, D, E): Для каждой страны-производителя и каждого потребителя определить
/// суммарную стоимость (с учетом скидки) купленных им товаров из этой страны.
/// Размер скидки вычисляется целочисленно: p * d / 100.
/// Формат: &lt;Страна&gt; &lt;Код потребителя&gt; &lt;Суммарная стоимость&gt;.
/// </summary>
public static class LinqObj97
{
    public static List<string> Solve(
        IEnumerable<string> linesB,
        IEnumerable<string> linesC,
        IEnumerable<string> linesD,
        IEnumerable<string> linesE)
    {
        var products = linesB
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new ProductRecord(Category: s[0], Article: s[1], Country: s[2]);
            });

        var discounts = linesC
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new DiscountRecord(DiscountPercent: int.Parse(s[0]), Store: s[1], ConsumerId: int.Parse(s[2]));
            });

        var prices = linesD
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new PriceRecord(Article: s[2], Store: s[1], Price: int.Parse(s[0]));
            });

        var purchases = linesE
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new PurchaseRecord(Store: s[0], ConsumerId: int.Parse(s[1]), Article: s[2]);
            });

        return purchases
            .Join(products,
                e => e.Article,
                b => b.Article,
                (e, b) => new { e.Store, e.ConsumerId, e.Article, b.Country })
            .Join(prices,
                x => new { x.Store, x.Article },
                d => new { d.Store, d.Article },
                (x, d) => new { x.Store, x.ConsumerId, x.Country, d.Price })
            .GroupJoin(discounts,
                x => new { x.Store, x.ConsumerId },
                c => new { c.Store, c.ConsumerId },
                (x, cs) => new { x.Country, x.ConsumerId, x.Price, Discount = cs.Select(c => c.DiscountPercent).FirstOrDefault() })
            .Select(x => new
            {
                x.Country,
                x.ConsumerId,
                FinalPrice = x.Price - (x.Price * x.Discount / 100)
            })
            .GroupBy(x => new { x.Country, x.ConsumerId })
            .Select(g => new
            {
                g.Key.Country,
                g.Key.ConsumerId,
                TotalCost = g.Sum(x => x.FinalPrice)
            })
            .OrderBy(x => x.Country, StringComparer.Ordinal)
            .ThenBy(x => x.ConsumerId)
            .Select(x => $"{x.Country} {x.ConsumerId} {x.TotalCost}")
            .ToList();
    }

    public static List<string> SolveFiles(string pathB, string pathC, string pathD, string pathE, string outputPath)
    {
        var result = Solve(
            EncodingHelper.ReadWin1251Lines(pathB),
            EncodingHelper.ReadWin1251Lines(pathC),
            EncodingHelper.ReadWin1251Lines(pathD),
            EncodingHelper.ReadWin1251Lines(pathE));
        EncodingHelper.WriteWin1251Lines(outputPath, result);
        return result;
    }
}
