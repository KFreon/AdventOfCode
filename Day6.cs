public class Day6 : BaseDay
{
    public override void Execute()
    {
        var text = File.ReadAllText(InputFile);
        var startOfPacketMarker = GetMarkerStart(text, 4);
        var startOfMessageMarker = GetMarkerStart(text, 14);
        WriteOutput(startOfPacketMarker, startOfMessageMarker);
    }

    private static int GetMarkerStart(string text, int markerSize)
    {
        var initialBuffer = text.Take(markerSize);
        var queue = new Queue<char>(initialBuffer);
        int startOfMarker = markerSize;
        foreach(var c in text.Skip(markerSize))
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
}