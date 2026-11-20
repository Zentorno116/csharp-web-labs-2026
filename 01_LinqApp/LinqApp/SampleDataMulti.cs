namespace LinqApp;

public static class SampleDataMulti
{
    public static void Generate(string dataDir)
    {
        // LinqObj77 (B, D)
        EncodingHelper.WriteWin1251Lines(Path.Combine(dataDir, "obj77_B.txt"), [
            "Электроника AB101-0001 Япония",
            "Электроника AB102-0002 Китай",
            "Электроника AB103-0003 Корея",
            "Одежда CD201-0001 Турция",
            "Книги EF301-0001 Россия"
        ]);
        EncodingHelper.WriteWin1251Lines(Path.Combine(dataDir, "obj77_D.txt"), [
            "AB101-0001 15000 МВидео",
            "AB102-0002 9000 ДНС",
            "AB101-0001 14500 ДНС",
            "CD201-0001 3500 Зара"
        ]);

        // LinqObj87 (A, D, E)
        EncodingHelper.WriteWin1251Lines(Path.Combine(dataDir, "obj87_A.txt"), [
            "1995 Ленина 101",
            "2001 Баумана 102",
            "1998 Ленина 103",
            "2000 Пушкина 104"
        ]);
        EncodingHelper.WriteWin1251Lines(Path.Combine(dataDir, "obj87_D.txt"), [
            "AB101-0001 Магнит 120",
            "AB102-0002 Магнит 250",
            "AB101-0001 Пятерочка 130"
        ]);
        EncodingHelper.WriteWin1251Lines(Path.Combine(dataDir, "obj87_E.txt"), [
            "Магнит 101 AB101-0001",
            "Магнит 101 AB101-0001",
            "Пятерочка 102 AB101-0001",
            "Магнит 103 AB102-0002"
        ]);

        // LinqObj97 (B, C, D, E)
        EncodingHelper.WriteWin1251Lines(Path.Combine(dataDir, "obj97_B.txt"), [
            "Техника AA111-0001 Китай",
            "Техника AA222-0002 Германия",
            "Мебель BB333-0003 Россия"
        ]);
        EncodingHelper.WriteWin1251Lines(Path.Combine(dataDir, "obj97_C.txt"), [
            "15 ДНС 101",
            "10 МВидео 102"
        ]);
        EncodingHelper.WriteWin1251Lines(Path.Combine(dataDir, "obj97_D.txt"), [
            "1055 ДНС AA111-0001",
            "2000 МВидео AA222-0002",
            "500 ДНС BB333-0003"
        ]);
        EncodingHelper.WriteWin1251Lines(Path.Combine(dataDir, "obj97_E.txt"), [
            "ДНС 101 AA111-0001",
            "ДНС 101 BB333-0003",
            "МВидео 102 AA222-0002",
            "МВидео 101 AA222-0002"
        ]);
    }
}
