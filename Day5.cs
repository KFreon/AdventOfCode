using System.Diagnostics;
using System.Text;

public class Day5 : BaseDay
{
    public override void Execute()
    {
        string[] lines = File.ReadAllLines(InputFile);

        var originalStacks = ParseStacks(lines);

        var stacks = originalStacks.Select(x => x.ToList())
            .ToArray();

        var originalTotalNumberOfContainers = stacks.Sum(x => x.Count);

        // Parse instructions
        var instructions = lines.Skip(stacks.Length)
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => new Instruction(x))
            .ToList();

        RunInstructions(stacks, instructions, isCraneOver9000: false);
        var part1 = GetTopContainers(stacks, originalTotalNumberOfContainers);

        // Reset
        stacks = originalStacks.Select(x => x.ToList())
            .ToArray();

        RunInstructions(stacks, instructions, isCraneOver9000: true);
        var part2 = GetTopContainers(stacks, originalTotalNumberOfContainers);

        WriteOutput(part1, part2);
    }

    private static IEnumerable<IEnumerable<char>> ParseStacks(string[] lines)
    {
        var stackLines = lines.TakeWhile(x => !string.IsNullOrEmpty(x)).SkipLast(1);
        var containerLines = stackLines.Select(line => line.Chunk(4).Select(t => t[1]));
        var originalStacks = containerLines
            .SelectMany(containers => containers.Select((container, stackIndex) => new { container, stackIndex }))
            .GroupBy(x => x.stackIndex)
            .Select(x => x.Select(t => t.container).Where(c => c != ' '));

        return originalStacks;
    }

    static void RunInstructions(List<char>[] stacks, List<Instruction> instructions, bool isCraneOver9000)
    {
        foreach (var instruction in instructions)
        {
            var containersInStack = stacks[instruction.FromStack - 1];

            // Take and remove
            var containersToMove = containersInStack.Take(instruction.NumberToMove);
            stacks[instruction.FromStack - 1] = containersInStack.Skip(instruction.NumberToMove).ToList();

            stacks[instruction.ToStack - 1].InsertRange(0, isCraneOver9000 ? containersToMove : containersToMove.Reverse());
        }
    }

    static string GetTopContainers(List<char>[] stacks, int originalTotalNumberOfContainers)
    {
        var containersOnTop = stacks.Select(contents => contents.FirstOrDefault())
            .Select(x => x == default ? ' ' : x)
            .ToArray();

        var endTotalNumberOfContainers = stacks.Sum(x => x.Count);
        if (endTotalNumberOfContainers != originalTotalNumberOfContainers)
        {
            global::System.Console.WriteLine();
        }
        return string.Join("", containersOnTop);
    }

    // 1 based indexing
    [DebuggerDisplay("{NumberToMove}, {FromStack}, {ToStack}")]
    class Instruction
    {
        public int NumberToMove { get; init; }
        public int FromStack { get; init; }
        public int ToStack { get; init; }

        private static string[] StringsToRemove = new[] { "move", "from", "to" };


        public Instruction(string input)
        {
            var sb = new StringBuilder(input);
            foreach (var removal in StringsToRemove)
            {
                sb = sb.Replace(removal, "");
            }

            var sanitised = sb.ToString();
            var numbers = sanitised.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            if (numbers.Count != 3)
            {
                Console.WriteLine();
            }
            NumberToMove = numbers[0];
            FromStack = numbers[1];
            ToStack = numbers[2];
        }
    }
}