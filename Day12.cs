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

    private Point[][] map;
    Point start, goal;
    int mapHeight, mapWidth;

    private HashSet<Point> currentPoints = new HashSet<Point>();
    private HashSet<Point> nextPoints = new HashSet<Point>();

    public override void Execute()
    {
        (map, start, goal) = ParseMap(lines.Value);
        mapHeight = map.Length;
        mapWidth = map[0].Length;

        var moveCount = 0;

        currentPoints.Add(start);

        while (!map[goal.Y][goal.X].Visited)
        {
            foreach(var point in currentPoints)
            {
                foreach (var item in GetNextPoints(map, point))
                    nextPoints.Add(item);
                point.Visited = true;
            }
            currentPoints.Clear();
            foreach (var pt in nextPoints) currentPoints.Add(pt);
            nextPoints.Clear();
            WriteMap(map);
            moveCount++;
        }

        WriteOutput(moveCount - 1);
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


    private static List<Point> GetNextPoints(Point[][] map, Point point)
    {
        var nextPoints = new List<Point>();

        // up
        if (point.Y > 0)
            MaybeAddPoint(map, point, 0, -1, nextPoints);

        // down
        if (point.Y < map.Length -1)
            MaybeAddPoint(map, point, 0, 1, nextPoints);

        // left
        if (point.X > 0)
            MaybeAddPoint(map, point, -1, 0, nextPoints);

        // right
        if (point.X < map[0].Length-1)
            MaybeAddPoint(map, point, 1, 0, nextPoints);

        return nextPoints;
    }

    private static void MaybeAddPoint(Point[][] map, Point point, int stepX, int stepY, List<Point> nextPoints)
    {
        var next = map[point.Y + stepY][point.X + stepX];
        if (!next.Visited && CanVisit(next, point.Height)) nextPoints.Add(next);
    }

    private static bool CanVisit(Point point, int currentHeight)
    {
        return Math.Abs(point.Height - currentHeight) <= 1;
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
