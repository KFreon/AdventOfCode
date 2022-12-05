public class Day1 : BaseDay
{
    public override void Execute()
    {
        var lines = File.ReadLines(InputFile);
        var elves = new List<List<int>>();

        var currentElf = new List<int>();

        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                elves.Add(currentElf);
                currentElf = new List<int>();
            }
            else
            {
                currentElf.Add(int.Parse(line));
            }
        }

        var totalCalories = elves.Select((x, i) => new { index = i, sum = x.Sum() }).ToList();
        var maxCalories = totalCalories.Max(x => x.sum);
        var index = totalCalories.Find(x => x.sum == maxCalories);

        var top3 = totalCalories.OrderByDescending(x => x.sum);
        var totalTop3 = top3.Take(3).Sum(t => t.sum);

        WriteOutput(index.sum, totalTop3);
    }
}