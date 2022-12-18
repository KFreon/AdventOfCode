public class Day13 : BaseDay
{
    record Pair(string Line1, string Line2);

    record Item(Item[] Items, bool IsProcessed);

    // Mabye a collection of lists of lists, etc
    // Then I can have a cascading test
    // Like do check, process, then do it again, until it's done

    public override void Execute()
    {
        var pairs = lines.Value
            .Chunk(3)
            .Where(x => x.Length > 1)
            .Select(x => new Pair(x[0], x[1]));

        foreach (var pair in pairs)
        {
        }
    }

    private static void ParseLine(string line)
    {
        var bits = line[1..^2] // Ignore outer brackets
            .Split(',');

        if (bits.Length == 0 ) 
        {
            // empty array
        }


        foreach(var bit in bits)
        {
            if (bit[0] == '[')
            {
                if (bit.Length > 1 && bit[1] == '[')
                {
                    // nested list
                } 
                else
                {
                    // list
                }
                
            }
            else if (int.TryParse(bit, out var integer))
            {
                // integer
            }
        }
    }
}
