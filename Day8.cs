using System;

public class Day8 : BaseDay
{
    record Tree(int Height, bool IsVisible, int Score = 0);

    public override void Execute()
    {
        var lines = File.ReadAllLines(InputFile);

        var height = lines.Length;
        var width = lines[0].Length;

        var trees = new Tree[height][];

        // Initial setup
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                var isEdge = row == 0 || row == height - 1
                    || column == 0 || column == width - 1;

                var currentTree = lines[row][column].ToString();
                if (trees[row] == default)
                {
                    trees[row] = new Tree[width];
                }
                trees[row][column] = new Tree(int.Parse(currentTree), isEdge);
            }
        }

        var numberOfTreesVisible_Indexes = GetUsingIndexes(height, width, trees);
        var numberOfTreesVisible = GetUsingDumbMethod(height, width, trees);
        //var bestSpot_Indexes = GetScoreUsingIndexes(height, width, trees);
        var bestSpot = GetScoreUsingDumbMethod(height, width, trees);

        WriteOutput(numberOfTreesVisible, bestSpot);
        WriteOutput(numberOfTreesVisible_Indexes);
    }

    private static int GetScoreUsingDumbMethod(int height, int width, Tree[][] trees)
    {
        for (var row = 1; row < height - 1; row++)
        {
            for (var column = 1; column < width - 1; column++)
            {
                var current = trees[row][column];
                var currentHeight = current.Height;

                // top
                var top = 0;
                for (top = row - 1; top > 0; top--)
                {
                    if (trees[top][column].Height >= currentHeight)
                    {
                        break;
                    }
                }

                var topScore = row - top;

                // bottom
                var bottom = 0;
                for (bottom = row + 1; bottom < height - 1; bottom++)
                {
                    if (trees[bottom][column].Height >= currentHeight)
                    {
                        break;
                    }
                }

                var bottomScore = bottom - row;

                // left
                var left = 0;
                for (left = column - 1; left > 0; left--)
                {
                    if (trees[row][left].Height >= currentHeight)
                    {
                        break;
                    }
                }

                var leftScore = column - left;

                // right
                var right = 0;
                for (right = column + 1; right < height - 1; right++)
                {
                    if (trees[row][right].Height >= currentHeight)
                    {
                        break;
                    }
                }

                var rightScore = right - column;

                var totalScore = topScore * bottomScore * leftScore * rightScore;
                trees[row][column] = current with { Score = totalScore };
            }
        }

        var bestSpot = trees.SelectMany(x => x.Select(t => t.Score)).Max();
        return bestSpot;
    }

    private static int GetUsingDumbMethod(int height, int width, Tree[][] trees)
    {
        var maxHeightsTop = trees[0].Clone() as Tree[];
        var maxHeightsBottom = trees.Last().Clone() as Tree[];

        var maxHeightsLeft = trees.Select(x => x[0]).ToArray();
        var maxHeightsRight = trees.Select(x => x.Last()).ToArray();

        // Need to come in from edges, so do top left, then again bottom right
        // Start at 1 because the edges are all visible
        for (int row = 1; row < height - 1; row++)
        {
            for (int column = 1; column < width - 1; column++)
            {
                var currentTree = trees[row][column];
                var maxTop = maxHeightsTop[column];
                var maxLeft = maxHeightsLeft[row];

                var isVisibleFromTop = currentTree.Height > maxTop.Height;
                var isVisibleFromLeft = currentTree.Height > maxLeft.Height;

                if (isVisibleFromTop)
                {
                    trees[row][column] = currentTree with { IsVisible = true };
                    maxHeightsTop[column] = currentTree;
                }

                if (isVisibleFromLeft)
                {
                    trees[row][column] = currentTree with { IsVisible = true };
                    maxHeightsLeft[row] = currentTree;
                }
            }
        }

        // Bottom/right
        for (int row = height - 2; row > 0; row--)
        {
            for (int column = width - 2; column > 0; column--)
            {
                var currentTree = trees[row][column];
                var maxBottom = maxHeightsBottom[column];
                var maxRight = maxHeightsRight[row];

                var isVisibleFromBottom = currentTree.Height > maxBottom.Height;
                var isVisibleFromRight = currentTree.Height > maxRight.Height;

                if (isVisibleFromBottom)
                {
                    trees[row][column] = currentTree with { IsVisible = true };
                    maxHeightsBottom[column] = currentTree;
                }

                if (isVisibleFromRight)
                {
                    trees[row][column] = currentTree with { IsVisible = true };
                    maxHeightsRight[row] = currentTree;
                }
            }
        }

        // Overlapping corners
        var numberOfTreesVisible = trees.SelectMany(x => x.Select(t => t.IsVisible)).Where(x => x == true).Count();
        return numberOfTreesVisible;
    }

    private static int GetUsingIndexes(int height, int width, Tree[][] trees)
    {
        var treesWithVisibility = trees.Select((treeRow, row) => treeRow.Select((tree, column) =>
        {
            var isEdge = row == 0 || row == height - 1
                    || column == 0 || column == width - 1;

            var isVisible = isEdge;
            if (!isVisible)
            {
                var visibleFromLeft = treeRow[..column].All(t => t.Height < tree.Height);
                var visibleFromRight = treeRow[(column + 1)..].All(t => t.Height < tree.Height);
                var visibleFromTop = trees[..row].Select(x => x[column]).All(t => t.Height < tree.Height);
                var visibleFromBottom = trees[(row + 1)..].Select(x => x[column]).All(t => t.Height < tree.Height);
                isVisible = visibleFromLeft || visibleFromRight || visibleFromTop || visibleFromBottom;
            }
            if (isVisible) return tree with { IsVisible = true };
            return tree;
        }).ToArray()
                ).ToArray();

        var howManyVisible = treesWithVisibility.SelectMany(x => x.Select(t => t.IsVisible)).Count(x => x == true);
        return howManyVisible;
    }
}
