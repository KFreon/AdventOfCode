public static class Day15Helpers
{
    public static int GetManhattanDistance(int x1, int x2, int y1, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }
}

public class Day15 : BaseDay
{
    record Item(int Row, int Column, bool IsOccupied);
    record Beacon(int Row, int Column):Item(Row, Column, true);
    record Sensor(int Row, int Column, Beacon ClosestBeacon) :Item(Row, Column, true)
    {
        private int _range = -1;
        public int Range
        {
            get
            {
                if (_range == -1 )
                    _range = Day15Helpers.GetManhattanDistance(Row, ClosestBeacon.Row, Column, ClosestBeacon.Column);
                
                return _range;
            }
        }

        public bool IsPointWithinRange(Item point)
        {
            var distance = Day15Helpers.GetManhattanDistance(Row, point.Row, Column, point.Column);
            return distance <= _range;
        }
    }

    public override void Execute()
    {
        var (sensors, beacons) = ParseInput(lines.Value);

        // figure out how to get a coverage map
        // find Manhattan distance out from the sensor to beacon
        // Then wrap around to get coverage
        // repeat for all sensors

        int minRow = 0, minCol = 0, maxRow = 0, maxCol = 0;
        minRow = new[] { sensors.Min(x => x.Row - x.Range), beacons.Min(x => x.Row) }.Min();
        maxRow = new[] { sensors.Max(x => x.Row + x.Range), beacons.Max(x => x.Row) }.Max();

        minCol = new[] { sensors.Min(x => x.Column - x.Range), beacons.Min(x => x.Column) }.Min();
        maxCol = new[] { sensors.Max(x => x.Column + x.Range), beacons.Max(x => x.Column) }.Max();

        var numberOfColumnsConsidered = Enumerable.Range(minCol, Math.Abs(minCol - maxCol));
        var consideredRow = 2_000_000;
        var coveredAreaCount = 0;
        foreach(var col in numberOfColumnsConsidered)
        {
            var point = new Item(consideredRow, col, false);
            if (sensors.Any(x => x.Column == point.Column && x.Row == point.Row)
                || beacons.Any(x => x.Column == point.Column && x.Row == point.Row))
                continue;

            if (sensors.Any(s => s.IsPointWithinRange(point)))
                coveredAreaCount++;
        };

        WriteOutput(coveredAreaCount);
    }

    private static (Sensor[] Sensors, Beacon[] Beacons) ParseInput(string[] lines)
    {
        var sensors = new List<Sensor>();
        var beacons = new List<Beacon>();
        foreach (var line in lines)
        {
            var bits = line.Split(':');
            var x = bits[0];
            var y= bits[1];
            var sensorBits = x.Split(',');
            var xText = sensorBits[0]["Sensor at x=".Length..];
            var yText = sensorBits[1].Replace("y=", "");
            var sensor = (row: int.Parse(yText),  col: int.Parse(xText));

            var beaconBits = y.Split(',');
            var xBText = beaconBits[0][" closest beacon is at x=".Length..];
            var yBText = beaconBits[1].Replace("y=", "");
            var beacon = (row: int.Parse(yBText), col: int.Parse(xBText));

            var actualBeacon = new Beacon(beacon.row, beacon.col);
            beacons.Add(actualBeacon);
            sensors.Add(new Sensor(sensor.row, sensor.col, actualBeacon));
        }

        return (sensors.ToArray(), beacons.ToArray());
    }
}
