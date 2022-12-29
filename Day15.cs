using System.Diagnostics;
using System.Net.Mail;
using BenchmarkDotNet.Disassemblers;

public static class Day15Helpers
{
    public static int GetManhattanDistance(int x1, int x2, int y1, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }
}

public class Day15 : BaseDay
{
    record Item(int Row, int Column, bool IsOccupied)
    {
        public bool KindaEquals(Item inc) => Row == inc.Row && Column == inc.Column;
    }
    record Beacon(int Row, int Column) : Item(Row, Column, true);
    record Sensor(int Row, int Column, Beacon ClosestBeacon) : Item(Row, Column, true)
    {
        public int TriggerRange { get; private set; }
        public int Adjustment { get; private set; }
        private int _range = -1;
        public int Range
        {
            get
            {
                if (_range == -1)
                {
                    _range = Day15Helpers.GetManhattanDistance(Row, ClosestBeacon.Row, Column, ClosestBeacon.Column);
                    Adjustment = _range / 3;
                    TriggerRange = Adjustment * 2;
                }

                return _range;
            }
        }

        public (bool IsInRange, int Distance) IsPointWithinRange(Item point)
        {
            var distance = Day15Helpers.GetManhattanDistance(Row, point.Row, Column, point.Column);
            return (distance <= _range, distance);
        }

        public bool IsPointWithinRange_Bare(Item point)
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

        var watch = Stopwatch.StartNew();

        var sens = sensors; //.Take(1).ToArray();

        Console.WriteLine("Part2");
        //var tuningFreq = Part2(sens);
        var tuningFreq = Part2_BruteForce(sens, beacons);
        //watch.Stop();
        //Console.WriteLine($"Elapsed: {watch.Elapsed}");
        //watch.Start();

        //Console.WriteLine();
        //Console.WriteLine();
        //Console.WriteLine("Part2_BruteForce");
        //Part2_BruteForce(sens, beacons);
        watch.Stop();
        Console.WriteLine($"Elapsed: {watch.Elapsed}");
        //var coveredAreaCount = Part1(sensors, beacons, minCol, maxCol);

        Console.WriteLine($"{tuningFreq.Item1}, {tuningFreq.Item2}");

        WriteOutput(0, tuningFreq.Item3);
    }

    private static long Part2(Sensor[] sensors)
    {
        int size = 20;
        foreach(var sensor in sensors)
        {
            var matches = sensors.Select(x =>
            {
                var considerations = new List<(int, int)>();
                if (x != sensor)
                {
                    var dist = Day15Helpers.GetManhattanDistance(sensor.Row, x.Row, sensor.Column, x.Column) - (sensor.Range + x.Range);
                    if (dist == 1)
                    {
                        var rowDiff = sensor.Row - x.Row;
                        var colDiff = sensor.Column - x.Column;
                        var center = (r: rowDiff / 2, c: colDiff / 2);

                        if (center.r >= 0 && center.r <= size && center.c >= 0 && center.c >= size)
                            considerations.Add(center);

                    }
                }
                return considerations;
            })
                .Where(x => x.Any()).ToArray();

            Console.WriteLine(  );
        }

        return -1;
    }

    private static long Part2_lol_memory(Sensor[] sensors, Beacon[] beacons)
    {
        var size = 4_000_000;
        var map = GetEmptyMap_Bool(size);

        foreach (var sensor in sensors)
        {
            // for a sensor, block out evertying in it's range in the map
            var minRow = sensor.Row - sensor.Range;
            var minCol = sensor.Column - sensor.Range;
            var maxRow = sensor.Row + sensor.Range;
            var maxCol = sensor.Column + sensor.Range;

            // Bottom right
            for (int row = sensor.Row, count = 0; row <= maxRow; row++, count++)
            {
                if (row >= size) continue;

                for (int col = sensor.Column; col <= maxCol - count; col++)
                {
                    if (col >= size) continue;
                    map[row][col] = true;
                }
            }

            // Top right
            for (int row = sensor.Row, count = 0; row >= minRow; row--, count++)
            {
                if (row < 0) continue;

                for (int col = sensor.Column; col <= maxCol - count; col++)
                {
                    if (col >= size) continue;
                    map[row][col] = true;
                }
            }

            // Bottom left
            for (int row = sensor.Row, count = 0; row <= maxRow; row++, count++)
            {
                if (row >= size) continue;

                for (int col = sensor.Column; col >= minCol + count; col--)
                {
                    if (col < 0) continue;
                    map[row][col] = true;
                }
            }

            // Top left
            for (int row = sensor.Row, count = 0; row >= minRow; row--, count++)
            {
                if (row < 0) continue;

                for (int col = sensor.Column; col >= minCol + count; col--)
                {
                    if (col < 0) continue;
                    map[row][col] = true;
                }
            }
        }

        //DrawMap(map);

        var unoccupiedCells = map.SelectMany((r, row) => r.Select((i, col) => new { IsOccupied = i, row, col})).Where(x => !x.IsOccupied).ToArray();

        Console.WriteLine($"{unoccupiedCells.Length}, {unoccupiedCells[0].row}, {unoccupiedCells[0].col}");

        return -1;
    }

    private static Item[][] GetEmptyMap(int size)
    {
        return Enumerable.Range(0, size).Select((x, row) => new Item[size].Select((t, col) => new Item(row, col, false)).ToArray()).ToArray();
    }

    private static bool[][] GetEmptyMap_Bool(int size)
    {
        return Enumerable.Range(0, size).Select((x, row) => new bool[size]).ToArray();
    }

    private static void DrawMap(Item[][] map)
    {
        foreach(var row in map)
        {
            foreach(var col in row)
            {
                Console.Write(col == null ? '0' : col.IsOccupied ? 'x' : '.');
            }
            Console.WriteLine();
        }
    }

    private static (int, int, long) Part2_BruteForce(Sensor[] sensors, Beacon[] beacons)
    {
        // This isn't feasible. It takes so long.
        int min = 0, max = 4_000_000;
        int r = 0, c = 0;
        long tuning = -1;

        //for(int row = min; row < max; row++)
        Parallel.For(min, max, new ParallelOptions { MaxDegreeOfParallelism = 8 }, row =>
        {
            if (tuning > 0) return;

            for (int col = min; col < max; col++)
            {
                var point = new Item(row, col, false);
                bool? isInRange = null;
                int? distance = null;
                Sensor? sensorInRange = null;

                foreach (var sensor in sensors)
                {
                    (isInRange, distance) = sensor.IsPointWithinRange(point);
                    if (isInRange.Value)
                    {
                        sensorInRange = sensor;
                        break;
                    }
                }

                // No sensors in range
                if (sensorInRange == null)
                {
                    tuning = col * 4_000_000L + row;
                    r = row;
                    c = col;
                    return;
                }

                // do some tricks
                if (distance!.Value < sensorInRange.TriggerRange)
                {
                    col += sensorInRange.Adjustment;
                }
            }
        });

        //DrawMap(map);

        return (r, c, tuning);
    }

    private static int Part1(Sensor[] sensors, Beacon[] beacons, int minCol, int maxCol)
    {
        var numberOfColumnsConsidered = Enumerable.Range(minCol, Math.Abs(minCol - maxCol));
        var consideredRow = 2_000_000;
        var coveredAreaCount = 0;
        foreach (var col in numberOfColumnsConsidered)
        {
            var point = new Item(consideredRow, col, false);
            if (sensors.Any(x => x.KindaEquals(point))
                || beacons.Any(x => x.KindaEquals(point)))
                continue;

            if (sensors.Any(s => s.IsPointWithinRange(point).IsInRange))
                coveredAreaCount++;
        };

        return coveredAreaCount;
    }

    private static (Sensor[] Sensors, Beacon[] Beacons) ParseInput(string[] lines)
    {
        var sensors = new List<Sensor>();
        var beacons = new List<Beacon>();
        foreach (var line in lines)
        {
            var bits = line.Split(':');
            var x = bits[0];
            var y = bits[1];
            var sensorBits = x.Split(',');
            var xText = sensorBits[0]["Sensor at x=".Length..];
            var yText = sensorBits[1].Replace("y=", "");
            var sensor = (row: int.Parse(yText), col: int.Parse(xText));

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
