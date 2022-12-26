public class Day14 : BaseDay
{
    record Point(int Row, int Column, bool IsFilled, bool IsRock = false, bool IsFloor = false);
    const int Width = 1000;
    const int DefaultHeight = 200;

    public override void Execute()
    {
        var map = ParseInput(lines.Value);
        var atRestCount = RunSimulation(map, DefaultHeight - 1);

        // Add floor
        var maxRow = map.SelectMany(r => r.Where(c => c.IsFilled).Select(c => c.Row)).Max();
        var floor = maxRow + 2;
        for(int col = 0;col < Width;col++)
            map[floor][col] = map[floor][col] with { IsFilled = true, IsFloor = true };

        var blockedEmitCount = RunSimulation(map, floor + 1);
        DrawMap(map);
        
        // I don't know why the main count is wrong the second time...I don't know how it can be.
        var manualCheck = map.SelectMany(x => x.Where(t => t.IsFilled && !t.IsRock && !t.IsFloor).Select(t => t)).Count();

        WriteOutput(atRestCount, manualCheck);
    }

    private static void DrawMap(Point[][] map)
    {
        var nonFloorFilledCols = map.SelectMany(r => r.Where(p => !p.IsFloor && p.IsFilled).Select(p => p.Column));
        var minCol = nonFloorFilledCols.Min();
        var maxCol = nonFloorFilledCols.Max();

        var floor = map.SelectMany(x => x.Where(t => t.IsFloor).Select(t => t.Row)).First() + 1;

        var section = map[..floor].Select(x => x.Where(r => r.Column >= (minCol - 2) && r.Column <= (maxCol + 2)).ToArray()).ToArray();


        foreach(var row in section)
        {
            foreach (var col in row)
            {
                Console.Write(col.IsFilled 
                    ? col.IsRock || col.IsFloor
                        ? 'x' 
                        : 'o'
                    : '.'
                    );
            }
            Console.WriteLine();
        }
    }

    private static int RunSimulation(Point[][] map, int height)
    {
        var atRestCount = 0;

        // emit sand from 500,0
        var sandTile = new Point(0, 500, true);
        while (sandTile.Row < height)
        {
            var downTile = map[sandTile.Row + 1][sandTile.Column];
            var downLeft = map[sandTile.Row + 1][sandTile.Column - 1];
            var downRight = map[sandTile.Row + 1][sandTile.Column + 1];
            if (!downTile.IsFilled)
                sandTile = downTile with { IsFilled = true };
            else if (!downLeft.IsFilled)
                sandTile = downLeft with { IsFilled = true };
            else if (!downRight.IsFilled)
                sandTile = downRight with { IsFilled = true };
            else
            {
                // At Rest
                map[sandTile.Row][sandTile.Column] = sandTile with { IsFilled = true };
                sandTile = new Point(0, 500, true);
                atRestCount++;

                // Spawner blocked
                if (map[sandTile.Row][sandTile.Column].IsFilled)
                {
                    break;
                }
            }
        }

        return atRestCount;
    }

    private static Point[][] ParseInput(string[] lines)
    {

        var map = new Point[DefaultHeight][];
        for (int row = 0; row < DefaultHeight; row++)
            map[row] = Enumerable.Range(0, Width).Select(c => new Point(row, c, false)).ToArray();

        foreach (var line in lines)
        {
            // each line is a path, but I don't think that really matters
            var points = ParseLine(line);
            for (int i = 0; i < points.Length - 1; i++)
            {
                // Take the junctions, and fill in the gaps on the map
                // Could probably somehow do line intersections, but lazy
                var point = points[i];
                var nextPoint = points[i + 1];
                var rowMin = point.Row < nextPoint.Row ? point.Row : nextPoint.Row;
                var columnMin = point.Column < nextPoint.Column ? point.Column : nextPoint.Column;
                var rowCount = Math.Abs(point.Row - nextPoint.Row);
                var columnCount = Math.Abs(point.Column - nextPoint.Column);
                var rowBits = Enumerable.Range(rowMin, rowCount + 1).ToArray();
                var columnBits = Enumerable.Range(columnMin, columnCount + 1).ToArray();

                if (rowBits.Length > 1)
                    foreach(var row in rowBits)
                        map[row][columnMin] = new Point(row, columnMin, true, true);
                else if (columnBits.Length > 1)
                    foreach(var col in columnBits)
                        map[rowMin][col] = new Point(rowMin, col, true, true);
            }
        }

        return map;
    }

    private static Point[] ParseLine(string line)
    {
        var points = line.Split("->")
            .Select(x =>
            {
                var trimmed = x.Trim();
                var colText = trimmed[..3];
                var rowText = trimmed[4..];
                return new Point(int.Parse(rowText), int.Parse(colText), true, true);
            })
            .ToArray();

        return points;
    }
}
