using System.Diagnostics;

public class Day10 : BaseDay
{
    record Instruction(string Name, int CycleCost);
    record XRegisterAtCycle(int Cycle, int Value);

    Instruction[] Instructions = new[] { new Instruction("noop", 1), new Instruction("addx", 2) };
    List<XRegisterAtCycle> InstructionsAtCycle = new();

    int[] CpuCyclesOfInterest = new[] { 20, 60, 100, 140, 180, 220 };
    char[][] CRT = new char[6][];
    int CrtLength = 40;

    public override void Execute()
    {
        var cycleCount = 0;
        foreach (var line in lines.Value)
        {
            var matchingInstruction = Instructions.FirstOrDefault(x => x.Name == line[..4]);
            if (matchingInstruction == null)
            {
                Debugger.Break();
            }

            cycleCount += matchingInstruction.CycleCost;
            if (matchingInstruction.CycleCost > 1)
            {
                var valueToAdd = int.Parse(line[4..]);
                var previous = InstructionsAtCycle.LastOrDefault()?.Value ?? 1;
                InstructionsAtCycle.Add(new(cycleCount, previous + valueToAdd));
            }
        }

        // Find the cycles of interest
        var signalStrengths = CpuCyclesOfInterest.Select(x => ValueAtCycle(x) * x);
        var total = signalStrengths.Sum();

        for (int CRTRow = 0; CRTRow < CRT.Length; CRTRow++)
        {
            if (CRT[CRTRow] == default) CRT[CRTRow] = Enumerable.Range(0, CrtLength).Select(x => '.').ToArray();

            for (int crtPositionInRow = 0; crtPositionInRow < CrtLength; crtPositionInRow++)
            {
                var cycle = (CRTRow * CrtLength) + crtPositionInRow + 1;
                var value  = ValueAtCycle(cycle);
                var spritePositionInRow = value % CrtLength;
                var drawPixel = Math.Abs(spritePositionInRow - crtPositionInRow) <= 1;
                if (drawPixel)
                {
                    CRT[CRTRow][crtPositionInRow] = '0';
                }
            }
        }

        var display = Environment.NewLine + string.Join(Environment.NewLine, CRT.Select(x => string.Join("", x)));

        WriteOutput(total.ToString(), display);
    }

    private int ValueAtCycle(int cycle) => InstructionsAtCycle.Where(x => x.Cycle < cycle).LastOrDefault()?.Value ?? 1;
}
