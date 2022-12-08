using System.Diagnostics;

public class Day7 : BaseDay
{
    [DebuggerDisplay("{Name} - {Size} - {TotalSize}")]
    public class Item
    {
        // Files don't have Contents
        public List<Item> Contents { get; init; } = new List<Item>();
        public int Size { get; init; } = 0;
        public string Name { get; init; }
        public Item? Parent { get; init; }
        public int TotalSize => Size + Contents.Sum(c => c.TotalSize);

        public Item(string name, Item? parent, int size = 0, List<Item> contents = null)
        {
            Name = name;
            Parent = parent;
            Size = size;
            Contents = contents ?? new List<Item>();
        }
    }

    public override void Execute()
    {
        var lines = File.ReadAllLines(InputFile);

        var current = new Item("/", null, 0, new List<Item>());
        var root = current;

        foreach(var line in lines)
        {
            var bits = line.Trim('$').Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = bits[0];
            var args = bits.Last();

            switch(command)
            {
                case "ls":
                case "dir":
                    // Don't need to worry about these
                    break;
                case "cd":
                    if (args == "/") // Go to root
                    {
                        current = root;
                        break;
                    } 
                    else if(args == "..")  // Go up level
                    {
                        if (current?.Parent == null)
                        {
                            Console.WriteLine();
                        }

                        current = current?.Parent;
                        break;
                    }

                    // Go down into a folder
                    var match = current.Contents.Find(x => x.Name == args);
                    if (match == null)
                    {
                        var newFolder = new Item(args, current);
                        current.Contents.Add(newFolder);
                        current = newFolder;
                    } 
                    else
                    {
                        current = match;
                    }
                    break;
                default:
                    // SHOULD only be the numbers left
                    var fileSize = int.Parse(command);
                    var newFile = new Item(args, current, fileSize);
                    current.Contents.Add(newFile);
                    break;
            }
        }

        var sumOfThoseBelowThreshold = SumDirectoriesWhereSizeGreaterThan(new List<Item> { root }, 100_000);

        var usedSpace = 70_000_000 - root.TotalSize;
        var spaceRequiredToRemove = 30_000_000 - usedSpace;
        var bigEnoughDirectories = FindDirectoriesWhereTotalSizeIsEnough(new List<Item> { root }, spaceRequiredToRemove);
        var smallest = bigEnoughDirectories.MinBy(x => x.TotalSize);

        WriteOutput(sumOfThoseBelowThreshold, smallest.TotalSize);
    }

    private List<Item> FindDirectoriesWhereTotalSizeIsEnough(List<Item> items, int requiredSpace)
    {
        List<Item> bigEnough = new();
        foreach(var item in items.Where(x => x.Contents.Any()))
        {
            if (item.TotalSize >= requiredSpace)
            {
                bigEnough.Add(item);
            }

            bigEnough.AddRange(FindDirectoriesWhereTotalSizeIsEnough(item.Contents, requiredSpace));
        }

        return bigEnough;
    }

    private int SumDirectoriesWhereSizeGreaterThan(List<Item> items, int sizeNoGreaterThan)
    {
        var sum = 0;
        foreach(var item in items.Where(x => x.Contents.Any()))
        {
            if (item.TotalSize <= sizeNoGreaterThan)
            {
                sum += item.TotalSize;
            } 

            sum += SumDirectoriesWhereSizeGreaterThan(item.Contents, sizeNoGreaterThan);
        }
        return sum;
    }
}