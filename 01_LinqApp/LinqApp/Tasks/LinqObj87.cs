using LinqApp.Models;

namespace LinqApp.Tasks;

/// <summary>
/// LinqObj87 (A, D, E): Для каждой улицы проживания (из A) и каждого магазина (из E) определить
/// суммарную стоимость покупок в данном магазине, сделанных потребителями с этой улицы (0 при отсутствии).
/// Формат: &lt;Улица&gt; &lt;Магазин&gt; &lt;Суммарная стоимость&gt;.
/// </summary>
public static class LinqObj87
{
    public static List<string> Solve(
        IEnumerable<string> linesA,
        IEnumerable<string> linesD,
        IEnumerable<string> linesE)
    {
        var consumers = linesA
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new ConsumerRecord(int.Parse(s[0]), s[1], int.Parse(s[2]));
            })
            .ToList();

        var prices = linesD
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new PriceRecord(s[0], s[1], int.Parse(s[2]));
            })
            .ToList();

        var purchases = linesE
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l =>
            {
                var s = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new PurchaseRecord(s[0], int.Parse(s[1]), s[2]);
            })
            .ToList();

        var streets = consumers.Select(c => c.Street).Distinct();
        var stores = purchases.Select(p => p.Store).Distinct();

        var purchaseCosts = purchases
            .Join(consumers,
                e => e.ConsumerId,
                a => a.ConsumerId,
                (e, a) => new { a.Street, e.Store, e.Article })
            .Join(prices,
                x => new { x.Store, x.Article },
                d => new { d.Store, d.Article },
                (x, d) => new { x.Street, x.Store, d.Price });

        return streets
            .SelectMany(_ => stores, (street, store) => new { Street = street, Store = store })
            .GroupJoin(purchaseCosts,
                pair => pair,
                pc => new { pc.Street, pc.Store },
                (pair, pcs) => new
                {
                    pair.Street,
                    pair.Store,
                    TotalCost = pcs.Sum(p => p.Price)
                })
            .OrderBy(x => x.Street, StringComparer.Ordinal)
            .ThenBy(x => x.Store, StringComparer.Ordinal)
            .Select(x => $"{x.Street} {x.Store} {x.TotalCost}")
            .ToList();
    }

    public static List<string> SolveFiles(string pathA, string pathD, string pathE, string outputPath)
    {
        var result = Solve(
            EncodingHelper.ReadWin1251Lines(pathA),
            EncodingHelper.ReadWin1251Lines(pathD),
            EncodingHelper.ReadWin1251Lines(pathE));
        EncodingHelper.WriteWin1251Lines(outputPath, result);
        return result;
    }
}
