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
        var numberOfDistinctLocations = CountNumberOfDistinctLocationsOccupiedByTail(lines.Value);
        var numberOfDistinctLocations_chain = CountDistinctLocationsForTailInChain(lines.Value);

        WriteOutput(numberOfDistinctLocations, numberOfDistinctLocations_chain);
    }

    private static int CountDistinctLocationsForTailInChain(string[] lines)
    {
        var knots = Enumerable.Range(0, 10).Select(x => new Position(0,0)).ToArray();
        var tailMoves = new List<Position> { new Position(0, 0) };

        foreach (var line in lines)
        {
            var direction = (Direction)line[0];
            var distance = int.Parse(line[2..]);

            for (var step = 0; step < distance; step++)
            {
                knots[0] = MoveHead(direction, knots[0]);
                var currentTailPosition = knots.Last() with { };

                for (var knotIndex = 1; knotIndex < knots.Length; knotIndex++)
                {
                    var previousKnot = knots[knotIndex - 1] with { };
                    var thisKnotPosition = knots[knotIndex] with { };
                    knots[knotIndex] = GetNewPosition(previousKnot, thisKnotPosition);
                }

                var newTailPosition = knots.Last();
                if (currentTailPosition != newTailPosition)
                {
                    tailMoves.Add(newTailPosition with { });
                }
            }
        }

        // example = 36
        return tailMoves.Distinct().Count();
    }

    private static Position MoveHead(Direction direction, Position head)
    {
        switch (direction)
        {
            case Direction.Left:
                head = head with { X = head.X - 1 };
                break;
            case Direction.Right:
                head = head with { X = head.X + 1 };
                break;
            case Direction.Up:
                head = head with { Y = head.Y + 1 };
                break;
            case Direction.Down:
                head = head with { Y = head.Y - 1 };
                break;
        }

        return head;
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
                headPosition = MoveHead(direction, headPosition);

                var xDiff = headPosition.X - previousHeadPosition.X;
                var yDiff = headPosition.Y - previousHeadPosition.Y;

                var absX = Math.Abs(xDiff);
                var absY = Math.Abs(yDiff);

                if (absX > 1 || absY > 1)
                {
                    previousHeadPosition = previous;
                    tailMoves.Add(previousHeadPosition);
                }
            }
        }

        return tailMoves.Distinct().Count();
    }

    private static Position GetNewPosition(Position previousKnotCurrentPosition, Position thisOne)
    {
        var xDiff = previousKnotCurrentPosition.X - thisOne.X;
        var yDiff = previousKnotCurrentPosition.Y - thisOne.Y;

        var absX = Math.Abs(xDiff);
        var absY = Math.Abs(yDiff);

        if (absX > 1 || absY > 1)
        {
            // Needs to move towards it

            //xxxxxx
            //xxxftx

            //xxxxxt
            //xxxfxx
            return thisOne with { X = thisOne.X + Math.Clamp(xDiff, -1, 1), Y = thisOne.Y + Math.Clamp(yDiff, -1, 1) };
        }
        return thisOne with { };
    }
}
