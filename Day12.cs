record Point(int X, int Y, int Height);

public class Day12 : BaseDay
{
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
        var goalDirectionX = endX - currentX;
        var goalDirectionY = endY - currentY;
        var currentHeight = map[currentY][currentX];

        var wantToMoveUp = goalDirectionY < 0;
        var wantToMoveDown = goalDirectionY > 0;
        var wantToMoveLeft = goalDirectionX < 0;
        var wantToMoveRight = goalDirectionX > 0;

        var hasMoved = false;

        if (wantToMoveUp && currentY > 0)
        {
            currentY = Move(currentY, currentHeight, 0, -1);
            hasMoved = true;
        }
        else if (wantToMoveDown && currentY < mapHeight - 2)
        {
            currentY = Move(currentY, currentHeight, 0, 1);
            hasMoved = true;
        }

        if (!hasMoved && wantToMoveLeft && currentX > 0)
        {
            currentX = Move(currentX, currentHeight, -1, 0);
        }
        else if (!hasMoved && wantToMoveRight && currentX < mapWidth - 2)
        {
            currentX = Move(currentX, currentHeight, 1, 0);
        }
    }

    private int Move(int target, int currentHeight, int stepX, int stepY)
    {
        var heightDiff = map[currentY + stepY][currentX + stepX] - currentHeight;
        if (heightDiff == 0 || heightDiff == 1)
        {
            return target + stepX + stepY;
        }
        return target;
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
