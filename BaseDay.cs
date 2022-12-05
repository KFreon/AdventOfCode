public abstract class BaseDay
{
    public string DayIndex => GetType().Name.Replace("Day", "");
    public string InputFile => Config.GetDayInput(DayIndex);
    public abstract void Execute();

    public void WriteOutput<T>(T part1Output, T part2Output)
    {
        Console.WriteLine($"Day {DayIndex} - Part1: {part1Output}");
        Console.WriteLine($"Day {DayIndex} - Part2: {part2Output}");
        Console.WriteLine();
    }
}
