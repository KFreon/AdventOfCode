public abstract class BaseDay
{
    public string DayIndex => GetType().Name.Replace("Day", "");
    public string InputFile => Config.GetDayInput(DayIndex);
    public abstract void Execute();

    public void WriteOutput<T>(T part1Output, T part2Output, string prefix = "")
    {
        Console.WriteLine($"Day {DayIndex} -{prefix} Part1: {part1Output}");
        Console.WriteLine($"Day {DayIndex} -{prefix} Part2: {part2Output}");
        Console.WriteLine();
    }
}
