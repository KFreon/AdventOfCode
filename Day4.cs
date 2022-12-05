using System;

public class Day4 : BaseDay
{
    record MyRangePair(MyRange First, MyRange Second);
    record MyRange(int Start, int End)
    {
        public int Length => End - Start;
    }

    public override void Execute()
    {
        var lines = File.ReadLines(InputFile);

        Func<string, MyRange> getRange = (string stringRange) => {
            var bits = stringRange.Split('-').Select(x => int.Parse(x)).ToArray();
            return new MyRange(bits[0], bits[1]);
        };

        var part1 = lines
            .Select(l => l.Split(','))
            .Select(stringRanges =>
            {
                var bits = stringRanges.Select(getRange).ToArray();
                return new MyRangePair(bits[0], bits[1]);
            })
            .Select(rangePair => (rangePair.First.Start <= rangePair.Second.Start && rangePair.First.End >= rangePair.Second.End)
                || (rangePair.First.Start >= rangePair.Second.Start && rangePair.First.End <= rangePair.Second.End))
            .Where(x => x == true)
            .Count();

        // Pt2
        var part2 = lines
            .Select(l => l.Split(','))
            .Select(stringRanges =>
            {
                var bits = stringRanges.Select(getRange).ToArray();
                return new MyRangePair(bits[0], bits[1]);
            })
            .Select(ranges =>
            {
                var range1 = Enumerable.Range(ranges.First.Start, ranges.First.Length + 1);
                var range2 = Enumerable.Range(ranges.Second.Start, ranges.Second.Length + 1);
                return range1.Intersect(range2).Count();
            })
            .Where(x => x > 0)
            .Count();

        WriteOutput(part1, part2);
    }
}