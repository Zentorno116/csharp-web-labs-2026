using LinqApp;
using LinqApp.Tasks;
using Xunit;

namespace LinqApp.Tests;

public class LinqMultiSequenceTests
{
    [Fact]
    public void LinqObj77_CountsDistinctStoresAndCountriesPerRepresentedCategory()
    {
        string[] b =
        [
            "Электроника AB101-0001 Япония",
            "Электроника AB102-0002 Китай",
            "Электроника AB103-0003 Корея", // not in any store
            "Одежда CD201-0001 Турция",
            "Книги EF301-0001 Россия"       // not in any store
        ];
        string[] d =
        [
            "AB101-0001 15000 МВидео",
            "AB102-0002 9000 ДНС",
            "AB101-0001 14500 ДНС",
            "CD201-0001 3500 Зара"
        ];

        var res = LinqObj77.Solve(b, d);
        Assert.Equal(["2 Электроника 2", "1 Одежда 1"], res);
    }

    [Fact]
    public void LinqObj87_ComputesCartesianStreetStoreTotalsWithZeroForNoSales()
    {
        string[] a = ["1995 Ленина 101", "2001 Баумана 102", "1998 Ленина 103", "2000 Пушкина 104"];
        string[] d = ["AB101-0001 Магнит 120", "AB102-0002 Магнит 250", "AB101-0001 Пятерочка 130"];
        string[] e =
        [
            "Магнит 101 AB101-0001",
            "Магнит 101 AB101-0001", // duplicate purchase counted twice
            "Пятерочка 102 AB101-0001",
            "Магнит 103 AB102-0002"
        ];

        var res = LinqObj87.Solve(a, d, e);
        Assert.Equal(
            [
                "Баумана Магнит 0",
                "Баумана Пятерочка 130",
                "Ленина Магнит 490",
                "Ленина Пятерочка 0",
                "Пушкина Магнит 0",
                "Пушкина Пятерочка 0"
            ],
            res);
    }

    [Fact]
    public void LinqObj97_AppliesIntegerDiscountAndSumsByCountryAndConsumer()
    {
        string[] b = ["Техника AA111-0001 Китай", "Техника AA222-0002 Германия", "Мебель BB333-0003 Россия"];
        string[] c = ["15 ДНС 101", "10 МВидео 102"];
        string[] d = ["1055 ДНС AA111-0001", "2000 МВидео AA222-0002", "500 ДНС BB333-0003"];
        string[] e =
        [
            "ДНС 101 AA111-0001",     // 1055 - (1055*15/100=158) = 897
            "ДНС 101 BB333-0003",     // 500 - (500*15/100=75) = 425
            "МВидео 102 AA222-0002",  // 2000 - (2000*10/100=200) = 1800
            "МВидео 101 AA222-0002"   // no discount -> 2000
        ];

        var res = LinqObj97.Solve(b, c, d, e);
        Assert.Equal(["Германия 101 2000", "Германия 102 1800", "Китай 101 897", "Россия 101 425"], res);
    }

    [Fact]
    public void EncodingHelper_RoundTripsWindows1251Files()
    {
        var tmp = Path.Combine(Path.GetTempPath(), $"linq_test_{Guid.NewGuid():N}.txt");
        try
        {
            EncodingHelper.WriteWin1251Lines(tmp, ["Привет 1251", "Нет данных"]);
            Assert.Equal(["Привет 1251", "Нет данных"], EncodingHelper.ReadWin1251Lines(tmp));
        }
        finally
        {
            if (File.Exists(tmp)) File.Delete(tmp);
        }
    }
}
