using System.Text;
using LinqApp;
using LinqApp.Tasks;

Console.OutputEncoding = Encoding.UTF8;

var baseDir = AppContext.BaseDirectory;
var dataDir = Path.Combine(baseDir, "Data");
var outDir = Path.Combine(baseDir, "Output");

SampleDataSingle.Generate(dataDir);
SampleDataMulti.Generate(dataDir);

var runners = new Dictionary<string, (string Title, Func<List<string>> Run)>
{
    ["7"]  = ("LinqObj7  (Фитнес-центр, K=101)",       () => LinqObj07.SolveFiles(101, Path.Combine(dataDir, "obj07.txt"), Path.Combine(outDir, "obj07_out.txt"))),
    ["17"] = ("LinqObj17 (Абитуриенты и школы)",       () => LinqObj17.SolveFiles(Path.Combine(dataDir, "obj17.txt"), Path.Combine(outDir, "obj17_out.txt"))),
    ["27"] = ("LinqObj27 (Задолжники по этажам)",      () => LinqObj27.SolveFiles(Path.Combine(dataDir, "obj27.txt"), Path.Combine(outDir, "obj27_out.txt"))),
    ["37"] = ("LinqObj37 (Мин/макс цены АЗС)",         () => LinqObj37.SolveFiles(Path.Combine(dataDir, "obj37.txt"), Path.Combine(outDir, "obj37_out.txt"))),
    ["47"] = ("LinqObj47 (АЗС с >= 2 марками)",        () => LinqObj47.SolveFiles(Path.Combine(dataDir, "obj47.txt"), Path.Combine(outDir, "obj47_out.txt"))),
    ["57"] = ("LinqObj57 (ЕГЭ < 50 по всем)",          () => LinqObj57.SolveFiles(Path.Combine(dataDir, "obj57.txt"), Path.Combine(outDir, "obj57_out.txt"))),
    ["67"] = ("LinqObj67 (Двоечники по классам)",      () => LinqObj67.SolveFiles(Path.Combine(dataDir, "obj67.txt"), Path.Combine(outDir, "obj67_out.txt"))),
    ["77"] = ("LinqObj77 (B, D: категории/магазины)",  () => LinqObj77.SolveFiles(Path.Combine(dataDir, "obj77_B.txt"), Path.Combine(dataDir, "obj77_D.txt"), Path.Combine(outDir, "obj77_out.txt"))),
    ["87"] = ("LinqObj87 (A, D, E: улицы x магазины)", () => LinqObj87.SolveFiles(Path.Combine(dataDir, "obj87_A.txt"), Path.Combine(dataDir, "obj87_D.txt"), Path.Combine(dataDir, "obj87_E.txt"), Path.Combine(outDir, "obj87_out.txt"))),
    ["97"] = ("LinqObj97 (B, C, D, E: скидки/страны)", () => LinqObj97.SolveFiles(Path.Combine(dataDir, "obj97_B.txt"), Path.Combine(dataDir, "obj97_C.txt"), Path.Combine(dataDir, "obj97_D.txt"), Path.Combine(dataDir, "obj97_E.txt"), Path.Combine(outDir, "obj97_out.txt")))
};

void PrintTask(string key)
{
    var (title, run) = runners[key];
    Console.WriteLine($"\n=== {title} ===");
    foreach (var line in run())
        Console.WriteLine("  " + line);
}

if (args.Contains("--all") || Console.IsInputRedirected)
{
    Console.WriteLine("Вариант: Замилов Тимур Рамилевич (7, 17, 27, 37, 47, 57, 67, 77, 87, 97)");
    foreach (var key in runners.Keys)
        PrintTask(key);
    Console.WriteLine($"\nФайлы результатов (windows-1251) сохранены в: {outDir}");
    return;
}

while (true)
{
    Console.WriteLine("\n--- LINQ Задачи (Замилов Т.Р.: 7,17,27,37,47,57,67,77,87,97) ---");
    Console.WriteLine("Введите номер задачи, 'all' для запуска всех или '0' для выхода:");
    var input = Console.ReadLine()?.Trim().ToLowerInvariant();
    if (input is "0" or "q" or "exit" or null) break;
    if (input == "all")
    {
        foreach (var key in runners.Keys) PrintTask(key);
    }
    else if (runners.ContainsKey(input))
    {
        PrintTask(input);
    }
    else
    {
        Console.WriteLine("Неизвестный номер. Доступны: 7, 17, 27, 37, 47, 57, 67, 77, 87, 97, all.");
    }
}
