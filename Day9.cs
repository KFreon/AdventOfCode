using System;

public class Day9 : BaseDay
{
    record Position(int X, int Y);

    enum Direction
    {
        Up = 'U',
        Down = 'D',
        Left = 'L',
        Right = 'R'
    }

    public override void Execute()
    {
        var lines = File.ReadAllLines(InputFile);
        var numberOfDistinctLocations = CountNumberOfDistinctLocationsOccupiedByTail(lines);

        WriteOutput(numberOfDistinctLocations);
    }

    private static int CountNumberOfDistinctLocationsOccupiedByTail(string[] lines)
    {
        var headPosition = new Position(X: 0, Y: 0);
        var previousHeadPosition = new Position(X: 0, Y: 0);
        var tailMoves = new List<Position> { previousHeadPosition };

        foreach (var line in lines)
        {
            var direction = (Direction)line[0];
            var distance = int.Parse(line[2..]);
            for (int i = 0; i < distance; i++)
            {
                var previous = headPosition;
                switch (direction)
                {
                    case Direction.Left:
                        headPosition = headPosition with { X = headPosition.X - 1 };
                        break;
                    case Direction.Right:
                        headPosition = headPosition with { X = headPosition.X + 1 };
                        break;
                    case Direction.Up:
                        headPosition = headPosition with { Y = headPosition.Y + 1 };
                        break;
                    case Direction.Down:
                        headPosition = headPosition with { Y = headPosition.Y - 1 };
                        break;
                }

                var xDiff = headPosition.X - previousHeadPosition.X;
                var yDiff = headPosition.Y - previousHeadPosition.Y;

                var absX = Math.Abs(xDiff);
                var absY = Math.Abs(yDiff);

                if (absX > 1 || absY> 1) 
                {
                    previousHeadPosition = previous;
                    tailMoves.Add(previousHeadPosition);
                }
            }
        }

        return tailMoves.Distinct().Count();
    }
}
