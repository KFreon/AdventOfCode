public class Day11 : BaseDay
{
    enum OperationType
    {
        Add = '+',
        Multiply = '*'
    }

    record Operation(OperationType OperationType, uint Amount);

    class Monkey
    {
        public int Index { get; set; }
        public List<ulong> Items { get; set; }
        public Operation Operation { get; set; }
        public uint TestDivideAmount { get; set; }
        public int TrueTarget { get; set; }
        public int FalseTarget { get; set; }
        public ulong ItemInspectionCount { get; set; }

        public Monkey(string[] lines)
        {
            Index = int.Parse(lines[0]["Monkey ".Length..^1]);

            Items = lines[1]["  Starting items:".Length..]
                .Split(',')
                .Select(ulong.Parse)
                .ToList();

            var operationChar = lines[2]["  Operation: new = old ".Length];
            var operation = (OperationType)operationChar;
            var amount = lines[2][("  Operation: new = old ".Length + 1)..];
            Operation = new Operation(operation, amount.Contains("old") ? 0 : uint.Parse(amount));

            TestDivideAmount = uint.Parse(lines[3]["  Test: divisible by ".Length..]);

            TrueTarget = int.Parse(lines[4]["    If true: throw to monkey ".Length..]);
            FalseTarget = int.Parse(lines[5]["    If false: throw to monkey ".Length..]);
        }

        public bool Test(ulong item) => item % TestDivideAmount == 0L;

        public ulong Inspect_AndGetNewWorryLevelForItem(ulong item)
        {
            ItemInspectionCount++;
            return Operation.OperationType switch
            {
                OperationType.Add => item + (Operation.Amount == 0 ? item : Operation.Amount),
                OperationType.Multiply => item * (Operation.Amount == 0 ? item : Operation.Amount),
                _ => throw new NotImplementedException()
            };
        }

        public void TakeTurn(Monkey[] allMonkeys, bool areWeWorried, ulong allTestValues)
        {
            if (!Items.Any())
            {
                return;
            }

            foreach (var item in Items)
            {
                var newWorryLevelForItem = Inspect_AndGetNewWorryLevelForItem(item);
                ulong relief = areWeWorried 
                    ? newWorryLevelForItem % allTestValues
                    : (ulong)Math.Round(newWorryLevelForItem / 3d, MidpointRounding.ToZero);

                var panicThrow = Test(relief);
                allMonkeys[panicThrow ? TrueTarget : FalseTarget].Items.Add(relief);
            }
            Items.Clear();
        }
    }

    public override void Execute()
    {
        var monkeyBusiness = GetMonkeyBusiness(lines.Value, false, 20);
        var fear = GetMonkeyBusiness(lines.Value, true, 10_000);

        WriteOutput(monkeyBusiness, fear);
    }

    private static ulong GetMonkeyBusiness(string[] lines, bool areWeWorried, int numberOfRounds)
    {
        var monkeys = lines.Chunk(7).Select(x => new Monkey(x)).ToArray();
        var allTestValues = monkeys.Aggregate(1UL, (agg, x) => x.TestDivideAmount * agg);

        for (int round = 0; round < numberOfRounds; round++)
        {
            for (int monkey = 0; monkey < monkeys.Length; monkey++)
            {
                monkeys[monkey].TakeTurn(monkeys, areWeWorried, allTestValues);
            }
        }

        var top2 = monkeys.OrderByDescending(x => x.ItemInspectionCount).Take(2);
        var monkeyBusiness = top2.Aggregate(1UL, (agg, monkey) => agg * monkey.ItemInspectionCount);
        return monkeyBusiness;
    }
}
