using BenchmarkDotNet.Attributes;

[SimpleJob]
[MemoryDiagnoser]
public class Day6 : BaseDay
{
    public override void Execute()
    {
        var text = File.ReadAllText(InputFile);
        Span<char> span = text.ToCharArray();

        var part1 = GetMarkerFromSpan(span, 4);
        var part2 = GetMarkerFromSpan(span, 14);

        var startOfPacketMarker = GetMarkerStart(text, 4);
        var startOfMessageMarker = GetMarkerStart(text, 14);

        WriteOutput(startOfPacketMarker, startOfMessageMarker, "Original");
        WriteOutput(part1, part2, "Spans");
    }

    private static int GetMarkerFromSpan(Span<char> span, int markerSize)
    {
        var slice = span.Slice(0, markerSize);
        var result = 0;

        for (int i = 1; i < span.Length; i++)
        {
            if (!HasDuplicate(slice))
            {
                result = i + markerSize - 1;
                break;
            }

            slice = span.Slice(i, markerSize);
        }

        return result;
    }

    private static bool HasDuplicate(Span<char> slice)
    {
        for (int i = 0; i < slice.Length - 1; i++)
        {
            for (int j = i + 1; j < slice.Length; j++)
            {
                if (slice[i] == slice[j]) return true;
            }
        }

        return false;
    }

    private static int GetMarkerStart(string text, int markerSize)
    {
        var initialBuffer = text.Take(markerSize);
        var queue = new Queue<char>(initialBuffer);
        int startOfMarker = markerSize;
        foreach (var c in text.Skip(markerSize))
        {
            if (queue.Distinct().Count() == markerSize)
            {
                break;
            }

            queue.Dequeue();
            queue.Enqueue(c);
            startOfMarker++;
        }

        return startOfMarker;
    }

    [Benchmark]
    public (int part1, int part2) Spans()
    {
        var source = File.ReadAllText(@"C:\Source\Personal\AdventOfCode2022\Inputs\Day6Input.txt");
        Span<char> span = source.ToCharArray();

        var part1 = GetMarkerFromSpan(span, 4);
        var part2 = GetMarkerFromSpan(span, 14);

        return (part1, part2);
    }

    [Benchmark]
    public (int part1, int part2) Original()
    {
        var source = File.ReadAllText(@"C:\Source\Personal\AdventOfCode2022\Inputs\Day6Input.txt");
        var startOfPacketMarker = GetMarkerStart(source, 4);
        var startOfMessageMarker = GetMarkerStart(source, 14);

        return (startOfPacketMarker, startOfMessageMarker);
    }
}