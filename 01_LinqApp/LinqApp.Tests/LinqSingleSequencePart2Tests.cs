using LinqApp.Tasks;
using Xunit;

namespace LinqApp.Tests;

public class LinqSingleSequencePart2Tests
{
    [Fact]
    public void LinqObj47_ReturnsStationsWithAtLeastTwoBrandsOrNo()
    {
        string[] data =
        [
            "5200 Татнефть Баумана 92",
            "5800 Татнефть Баумана 95",
            "6950 Татнефть Баумана 98",
            "5300 Лукойл Ленина 92",
            "5900 Лукойл Ленина 95",
            "5250 Газпром Мира 92"
        ];

        var res = LinqObj47.Solve(data);
        Assert.Equal(["Лукойл Ленина 2", "Татнефть Баумана 3"], res);

        var none = LinqObj47.Solve(["5250 Газпром Мира 92"]);
        Assert.Equal(["Нет"], none);
    }

    [Fact]
    public void LinqObj57_TakesFirstThreeFailingStudentsPerSchoolAlphabetically()
    {
        string[] data =
        [
            "19 Иванов А.Б. 40 45 30",
            "19 Абрамов В.Г. 35 49 20",
            "19 Сидоров Д.Е. 48 41 49",
            "19 Петров Ж.З. 10 20 30",
            "19 Орлов И.К. 50 40 40",
            "7 Яковлев М.Н. 42 38 44",
            "7 Белов О.П. 30 30 30"
        ];

        var res = LinqObj57.Solve(data);
        Assert.Equal(
            ["7 Белов О.П.", "7 Яковлев М.Н.", "19 Абрамов В.Г.", "19 Иванов А.Б.", "19 Петров Ж.З."],
            res);

        var none = LinqObj57.Solve(["7 Белов О.П. 50 50 50"]);
        Assert.Equal(["Требуемые учащиеся не найдены"], none);
    }

    [Fact]
    public void LinqObj67_FindsStudentsWithTwosSortedByClassDescThenName()
    {
        string[] data =
        [
            "10 Алгебра Иванов А.Б. 2",
            "10 Геометрия Иванов А.Б. 2",
            "10 Информатика Петров В.Г. 5",
            "11 Алгебра Смирнов Д.Е. 2",
            "11 Геометрия Абрамов Ж.З. 2",
            "11 Информатика Абрамов Ж.З. 2",
            "11 Алгебра Абрамов Ж.З. 2"
        ];

        var res = LinqObj67.Solve(data);
        Assert.Equal(["11 Абрамов Ж.З. 3", "11 Смирнов Д.Е. 1", "10 Иванов А.Б. 2"], res);

        var none = LinqObj67.Solve(["10 Информатика Петров В.Г. 5"]);
        Assert.Equal(["Требуемые учащиеся не найдены"], none);
    }
}
