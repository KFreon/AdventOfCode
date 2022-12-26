using System.Collections.Immutable;

public class Day13 : BaseDay
{
    record Pair(string Line1, string Line2);

    class Item
    {
        public List<Item?> Items = new List<Item?>();
        public List<int> Ints = new List<int>();
    }

    // Mabye a collection of lists of lists, etc
    // Then I can have a cascading test
    // Like do check, process, then do it again, until it's done

    public override void Execute()
    {
        var pairs = lines.Value
            .Chunk(3)
            .Where(x => x.Length > 1)
            .Select(x => new Pair(x[0], x[1]));

        var count = 0;
        var inOrders = new List<int>();
        foreach (var pair in pairs)
        {
            var isInOrder = ProcessPair(pair);
            var index = count + 1;

            Console.WriteLine($"{index}: {isInOrder}");
            if (isInOrder) inOrders.Add(index);
            count++;

            //var left = TrimLine(pair.Line1);
            //var right = TrimLine(pair.Line2);

            //var inCorrectOrder = DoTHings(left, right);
            //Console.WriteLine(inCorrectOrder);
        }

        WriteOutput(inOrders.Sum());
    }

    private static bool ProcessPair(Pair pair)
    {
        var l1 = ProcessLine(pair.Line1[1..^1]);
        var l2 = ProcessLine(pair.Line2[1..^1]);

        if (l1.Ints.Count > l2.Ints.Count) return false;
        return DoPair(l1, l2, false);
    }

    private static bool DoPair(Item left, Item right, bool onlyFirst)
    {
        for (int i = 0; i < left.Ints.Count; i++)
        {
            var leftInt = left.Ints[i];
            if (right.Ints.Count <= i) return false;

            var rightInt = right.Ints[i];

            var isLeftAnInt = leftInt != -1;
            var isRightAnInt = rightInt != -1;

            if (isLeftAnInt && isRightAnInt)
            {
                // both ints
                if (leftInt > rightInt) return false;

                if (onlyFirst) return true;
            }
            else if ((!isLeftAnInt && isRightAnInt) || (isLeftAnInt && !isRightAnInt))
            {
                // One is an int and the other isn't
                Item newLeft = isLeftAnInt ? new Item { Items = new List<Item?> { null }, Ints = new List<int> { leftInt } } : left.Items[i];
                Item newRight = isRightAnInt ? new Item { Items = new List<Item?> { null }, Ints = new List<int> { rightInt } } : right.Items[i];
                return DoPair(newLeft, newRight, true);
            }
            else
            {
                // both lists
                var results = new List<bool>();

                if (left.Items.Count > right.Items.Count) return false;

                for (int j = 0; j < left.Items.Count; j++)
                {
                    results.Add(DoPair(left.Items[i], right.Items[i], false));
                }
                if (results.Any(x => !x)) return false;
            }
        }

        return true;
    }

    private static Item ProcessLine(string line)
    {
        var item = new Item();
        var index = 0;
        while (index < line.Length)
        {
            var c = line[index];
            if (c == '[')
            {
                var section = new string(line.Skip(index + 1).TakeWhile(x => x != ']').ToArray());
                var subItem = ProcessLine(section!);
                item.Items.Add(subItem);
                item.Ints.Add(-1);
                index += section.Length + 2;  // brackets
            }
            else if (c != ',' && c != ']')
            {
                var section = new string(line.Skip(index).TakeWhile(x => x != ']' && x != ',').ToArray());
                item.Ints.Add(int.Parse(section));
                item.Items.Add(null);  //space it out
                index += section.Length;
            }
            else
            {
                index++;
            }
        }

        return item;
    }





    private static string[] TrimLine(string line)
    {
        var bits = line[1..^1].Split('[', StringSplitOptions.RemoveEmptyEntries);

        return bits;
    }

    private static bool DoTHings(string[] left, string[] right)
    {
        if (right.Length > left.Length)
        {
            // wrong order?
            return false;
        }

        // Loop over each list
        for (int i = 0; i < left.Length; i++)
        {
            var isLeftList = left[i].Contains(']');
            var isRightList = right[i].Contains(']');

            var leftThing = left[i].Split(',', 2, StringSplitOptions.RemoveEmptyEntries);
            var rightThing = right[i].Split(',', 2, StringSplitOptions.RemoveEmptyEntries);

            if (isLeftList && isRightList)
            {
                Console.WriteLine();
            }
            else if ((isLeftList && !isRightList) || (!isLeftList && isRightList))
            {
                // need to make one a list
            }
            else
            {
                // integers
                var leftInt = int.Parse(leftThing[0].Trim(']'));
                var rightInt = int.Parse(rightThing[0].Trim(']'));
                if (leftInt > rightInt)
                {
                    return false;
                }

                return DoTHings(leftThing[1..], rightThing[1..]);
            }
        }

        return true;
    }
}
