public class Day2 : BaseDay
{
    public override void Execute()
    {
        var lines = File.ReadLines(InputFile);

        var player1Map = new List<char> { 'A', 'B', 'C' };
        var player2Map = new List<char> { 'X', 'Y', 'Z' };  // First round, x = rock, y = paper, etc

        var part1 = lines
        .Select(x => new
        {
            player1 = player1Map.IndexOf(x[0]),
            player2 = player2Map.IndexOf(x[2])
        })
    .Select(x =>
    {
        var score = 0;
        var adjustedPlayer1Score = (x.player1 + 1) % 3;
        if (x.player1 == x.player2)
        {
            // tie
            score = 3;
        }
        else if (adjustedPlayer1Score == x.player2)
        {
            // win
            score = 6;
        }

        return new { player1 = x.player1, player2 = x.player2, score = score + x.player2 + 1 };
    })
    .Sum(x => x.score);

        // second round X = lose, Y = tie, Z = win
        var part2 = lines
        .Select(x => new { player1 = player1Map.IndexOf(x[0]), endState = player2Map.IndexOf(x[2]) })
        .Select(x =>
        {
            var player2Move = 0;
            if (x.endState == 1)
            {
                // tie
                player2Move = x.player1;
            }
            else if (x.endState == 0)
            {
                player2Move = (x.player1 + 2) % 3;
            }
            else
            {
                player2Move = (x.player1 + 1) % 3;
            }

            return new { player1 = x.player1, endState = x.endState, score = (x.endState * 3) + player2Move + 1 };
        })
        .Sum(x => x.score);

        WriteOutput(part1, part2);
    }
}