using LinqApp.Tasks;
using Xunit;

namespace LinqApp.Tests;

public class LinqSingleSequencePart1Tests
{
    [Fact]
    public void LinqObj07_ReturnsBestMonthPerYearAndHandlesTieAndMissing()
    {
        string[] data =
        [
            "12 2025 3 101",
            "15 2025 5 101",
            "15 2025 1 101",
            "20 2026 2 101",
            "8 2026 6 101",
            "30 2025 4 202"
        ];

        var res = LinqObj07.Solve(101, data);
        Assert.Equal(["2026 2 20", "2025 1 15"], res);

        var missing = LinqObj07.Solve(999, data);
        Assert.Equal(["Нет данных"], missing);
    }

    [Fact]
    public void LinqObj17_CountsDistinctSchoolsPerYearSorted()
    {
        string[] data =
        [
            "12 2024 Иванов",
            "12 2024 Петров",
            "45 2024 Сидоров",
            "7 2025 Смирнов",
            "19 2025 Кузнецов",
            "33 2025 Попов",
            "5 2023 Соколов",
            "12 2023 Морозов"
        ];

        var res = LinqObj17.Solve(data);
        Assert.Equal(["2 2023", "2 2024", "3 2025"], res);
    }

    [Fact]
    public void LinqObj27_AggregatesByFloorAcrossEntrancesAndOmitsEmptyFloors()
    {
        // Apt 1 -> Floor 1, Apt 38 -> Ent 2 Floor 1; Apt 5 -> Floor 2; Apt 35, 72, 144 -> Floor 9
        string[] data =
        [
            "Иванов 1 1250.50",
            "Петров 38 749.50",
            "Сидоров 5 300.00",
            "Козлов 72 512.75",
            "Новиков 144 487.25",
            "Волков 35 100.00"
        ];

        var res = LinqObj27.Solve(data);
        Assert.Equal(["1 2 300.00", "2 1 2000.00", "3 9 1100.00"], res);
    }

    [Fact]
    public void LinqObj37_FindsMinMaxPricePerBrandDescending()
    {
        string[] data =
        [
            "Лукойл 95 5850 Ленина",
            "Татнефть 95 5790 Баумана",
            "Газпром 92 5210 Ленина",
            "Лукойл 92 5290 Пушкина",
            "Татнефть 98 6900 Кремлевская"
        ];

        var res = LinqObj37.Solve(data);
        Assert.Equal(["98 6900 6900", "95 5790 5850", "92 5210 5290"], res);
    }
}
