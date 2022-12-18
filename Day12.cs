public class Day12 : BaseDay
{
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public bool Visited { get; set; }

        public Point(int x, int y, int height, bool visited)
        {
            X = x;
            Y = y;
            Height = height;
            Visited = visited;
        }
    }

    

    public override void Execute()
    {
        //var part1 = RunPart1(lines.Value);
        var part2 = RunPart2(lines.Value);

        WriteOutput(0, part2);
    }

    private static int RunPart1(string[] lines)
    {
        var (map, start, goal) = ParseMap(lines);

        var currentPoints = new HashSet<Point>();
        var nextPoints = new HashSet<Point>();

        var moveCount = 0;

        currentPoints.Add(start);

        while (!map[goal.Y][goal.X].Visited)
        {
            foreach (var point in currentPoints)
            {
                foreach (var item in GetNextPoints(map, point, false))
                    nextPoints.Add(item);
                point.Visited = true;
            }
            currentPoints.Clear();
            foreach (var pt in nextPoints) currentPoints.Add(pt);
            nextPoints.Clear();
            //WriteMap(map);
            moveCount++;
        }

        return moveCount - 1;
    }

    private static int RunPart2(string[] lines)
    {
        var (map, _, start) = ParseMap(lines);
        var currentPoints = new HashSet<Point>();
        var nextPoints = new HashSet<Point>();
        var moveCount = 0;

        currentPoints.Add(start);

        while (true)
        {
            if (currentPoints.Any(x => x.Height == (int)'a'))
            {
                break;
            }

            foreach (var point in currentPoints)
            {
                foreach (var item in GetNextPoints(map, point, true))
                    nextPoints.Add(item);
                point.Visited = true;
            }

            currentPoints.Clear();
            foreach (var pt in nextPoints) currentPoints.Add(pt);
            nextPoints.Clear();
            WriteMap(map);
            moveCount++;
        }

        return moveCount;
    }

    private static void WriteMap(Point[][] map)
    {
        Console.Clear();
        foreach(var row in map)
        {
            foreach(var point in row)
            {
                Console.Write(point.Visited ? '.' : (char)point.Height);
            }
            Console.Write(Environment.NewLine);
        }
    }


    private static List<Point> GetNextPoints(Point[][] map, Point point, bool part2)
    {
        var nextPoints = new List<Point>();

        // up
        if (point.Y > 0)
            MaybeAddPoint(map, point, 0, -1, nextPoints, part2);

        // down
        if (point.Y < map.Length -1)
            MaybeAddPoint(map, point, 0, 1, nextPoints, part2);

        // left
        if (point.X > 0)
            MaybeAddPoint(map, point, -1, 0, nextPoints, part2);

        // right
        if (point.X < map[0].Length-1)
            MaybeAddPoint(map, point, 1, 0, nextPoints, part2);

        return nextPoints;
    }

    private static void MaybeAddPoint(Point[][] map, Point point, int stepX, int stepY, List<Point> nextPoints, bool part2)
    {
        var next = map[point.Y + stepY][point.X + stepX];
        if (!next.Visited && CanVisit(next, point.Height, part2)) nextPoints.Add(next);
    }

    private static bool CanVisit(Point point, int currentHeight, bool part2)
    {
        var diff = point.Height - currentHeight;
        return part2 
            ? diff >= -1
            : diff <= 1;
    }

    private static (Point[][] Map, Point Start, Point Goal) ParseMap(string[] lines)
    {
        Point? start = null, end = null;
        var map = lines.Select((l, y) => l.Select((c, x) =>
        {
            if (c == 'S')
            {
                var point = new Point(x, y, (int)'a', false);
                start = point;
                return point;
            }
            else if (c == 'E')
            {
                var point = new Point(x, y, (int)'z', false);
                end = point;
                return point;
            }
            return new Point(x, y, (int)c, false);
        })
        .ToArray())
            .ToArray();

        return (map, start!, end!);
    }
}
