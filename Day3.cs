public class Day3 : BaseDay
{
    public override void Execute()
    {
        // (((int)'Z') - 96) + 31 + 27
        var part1 = lines.Value.Select(line =>
        {
            var priorities = line.Select(c =>
            {
                int priority = c - 96;
                priority = priority < 0 ? priority + 31 + 27 : priority;
                return priority;
            });
            return new { priorities, bits = line.Select(c => c).ToArray(), length = line.Length };
        })
        .Select(backpack =>
        {
            var compartmentSize = backpack.length / 2;
            var compartment1 = backpack.priorities.Take(compartmentSize);
            var compartment2 = backpack.priorities.Skip(compartmentSize);
            var shared = compartment1.Intersect(compartment2).First();
            return shared;
        })
        .Sum();


        // Part2
        var priorities = lines.Value.Select(
            line => line.Select(c =>
            {
                int priority = c - 96;
                priority = priority < 0 ? priority + 31 + 27 : priority;
                return priority;
            }
        ).ToList()).ToList();

        var currentIndex = 0;
        var sum = 0;
        while (currentIndex < priorities.Count)
        {
            var currentGroup = priorities.Skip(currentIndex).Take(3).ToList();
            // Find the shared ones
            // Lol this is so bad
            var sharedElement = currentGroup[0].Intersect(currentGroup[1]).Intersect(currentGroup[2]).First();
            sum += sharedElement;

            currentIndex += 3;
        }

        WriteOutput(part1, sum);
    }
}