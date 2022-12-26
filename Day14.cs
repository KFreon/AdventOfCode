using System.Runtime.InteropServices;

public class Day14 : BaseDay
{
    record Point(int Row, int Column, bool IsFilled);
    const int Width = 600;
    const int Height = 200;

    public override void Execute()
    {
        var map = ParseInput(lines.Value);
        var atRestCount = 0;

        // emit sand from 500,0
        var sandTile = new Point(0, 500, true);
        while (sandTile.Row < Height - 1)
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
                map[sandTile.Row][sandTile.Column] = sandTile;
                sandTile = new Point(0, 500, true);
                atRestCount++;
            }
        }

        WriteOutput(atRestCount);
    }


    private static Point[][] ParseInput(string[] lines)
    {

        var map = new Point[Height][];
        for (int row = 0; row < Height; row++)
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
                        map[row][columnMin] = new Point(row, columnMin, true);
                else if (columnBits.Length > 1)
                    foreach(var col in columnBits)
                        map[rowMin][col] = new Point(rowMin, col, true);
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
                return new Point(int.Parse(rowText), int.Parse(colText), true);
            })
            .ToArray();

        return points;
    }
}
