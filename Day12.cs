public class Day12 : BaseDay
{
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private int[][] map;
    int currentX, currentY, endX, endY, mapHeight, mapWidth = 0;

    public override void Execute()
    {
        (map, currentX, currentY, endX, endY) = ParseMap(lines.Value);
        mapHeight = map.Length;
        mapWidth = map[0].Length;

        var moveCount = 0;

        while (!(currentX == endX && currentY == endY))
        {
            HillClimb();
            moveCount++;
        }

        WriteOutput(moveCount);
    }

    private void HillClimb()
    {
        var currentHeight = map[currentY][currentX];

        var scores = new List<(double BaseScore, int NextStepDirectionScore, int NewHeight, int NewX, int NewY)>();

        scores.Add(Move(currentHeight, Direction.Up));
        scores.Add(Move(currentHeight, Direction.Down));
        scores.Add(Move(currentHeight, Direction.Left));
        scores.Add(Move(currentHeight, Direction.Right));

        var maxBaseScore = scores.MaxBy(x => x.BaseScore);
        var maxStep = maxBaseScore;
        var maxes = scores.Where(x => x.BaseScore == maxBaseScore.BaseScore).ToList();
        if (maxes.Count > 1)
        {
            maxStep = maxes.MaxBy(x => x.NextStepDirectionScore);
        }

        currentX = maxStep.NewX;
        currentY = maxStep.NewY;
        Console.WriteLine($"{currentX} {currentY}");
    }

    private (double BaseScore, int NextStepDirectionScore, int NewHeight, int NewX, int NewY) Move(int currentHeight, Direction direction)
    {
        var(baseScore, newX, newY, newHeight) = CheckMove(currentHeight, direction, currentX, currentY, endX, endY, mapHeight, mapWidth, map);

        var xdiff = Math.Abs(currentX - endX);
        var ydiff = Math.Abs(currentY - endY);

        var biasDirection = xdiff > ydiff 
            ? xdiff > 0 
                ? Direction.Right : Direction.Left
            : ydiff > 0
                ? Direction.Up : Direction.Down;

        // Is there a step up in this direction anywhere at all?
        var up = map[..newY].Select(r => r.Skip(newX).First()).Reverse().ToArray();
        var down = map[newY..].Select(r => r.Skip(newX).First()).ToArray();
        var left = map[newY][..newX].Reverse().ToArray();
        var right = map[newY][newX..];


        var upScore = direction == Direction.Down ? 0 : GetScore(up, currentHeight);
        var downScore = direction == Direction.Up ? 0 : GetScore(down, currentHeight);
        var leftScore = direction == Direction.Right ? 0 : GetScore(left, currentHeight);
        var rightScore = direction == Direction.Left ? 0 : GetScore(right, currentHeight);

        if (biasDirection == Direction.Up) upScore *= 2;
        if (biasDirection == Direction.Down) downScore *= 2;
        if (biasDirection == Direction.Left) leftScore *= 2;
        if (biasDirection == Direction.Right) rightScore *= 2;

        var nextStepDirectionScore = upScore + downScore + leftScore + rightScore;
        return (baseScore, nextStepDirectionScore, newHeight, newX, newY);
    }

    private static int GetScore(int[] bits, int currentHeight)
    {
        var itemsInDirectionOfGoodSize = bits.TakeWhile(x => x == currentHeight || x == (currentHeight + 1));
        var scoreInDirection = itemsInDirectionOfGoodSize.Select(x => x == currentHeight ? 1 : 5).Sum();
        return scoreInDirection;
    }

    private static (double Score, int NewX, int NewY, int NewHeight) CheckMove(int currentHeight, Direction direction, int currentX, int currentY, int endX, int endY, int mapHeight, int mapWidth, int[][] map)
    {
        var stepY = direction switch
        {
            Direction.Up => -1,
            Direction.Down => 1,
            _ => 0
        };

        var stepX = direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
            _ => 0
        };

        var newX = currentX + stepX;
        var newY = currentY + stepY;

        // NEED to move in direction, THEN check it out


        double score = 0;

        // Can we even go in the requested direction
        if (direction == Direction.Up && newY >= 0) score = 1;
        if (direction == Direction.Down && newY < mapHeight) score = 1;
        if (direction == Direction.Left && newX >= 0) score = 1;
        if (direction == Direction.Right && newX < mapWidth) score = 1;

        if (score == 0) return (0, 0, 0,0);

        // Bounds check done, but is it too hight?
        var totalGoalDiffFromNew = Math.Sqrt(Math.Pow(Math.Abs(endX - newX), 2) + Math.Pow(Math.Abs(endY - newY), 2));
        var totalGoalDiffFromCurrent = Math.Sqrt(Math.Pow(Math.Abs(endX - currentX), 2) + Math.Pow(Math.Abs(endY - currentY), 2));

        var diff = totalGoalDiffFromCurrent - totalGoalDiffFromNew;

        var heightDiff = map[newY][newX] - currentHeight;
        if (heightDiff == 0) score += diff > 0 ? 0 : 0;
        if (heightDiff == 1) score += 100;
        if (heightDiff < 0) score -= 10000;
        if (heightDiff > 1) score -= 10000;

        return (score, newX, newY, map[newY][newX]);
    }

    private static (int[][] Map, int StartX, int StartY, int EndX, int EnxY) ParseMap(string[] lines)
    {
        int startX = 0;
        int startY = 0;
        int endX = 0;
        int endY = 0;
        var map = lines.Select((l, y) => l.Select((c, x) =>
        {
            if (c == 'S')
            {
                startX = x;
                startY = y;
                return (int)'a';
            }
            else if (c == 'E')
            {
                endX = x;
                endY = y;
                return (int)'z';
            }
            return (int)c;
        })
        .ToArray())
            .ToArray();

        return (map, startX, startY, endX, endY);
    }
}
