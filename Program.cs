var days = typeof(Day1).Assembly.GetTypes()
    .Where(x => !x.IsAbstract && x.Name.Contains("Day"))
    .Select(x => new { name = x.Name, type = Activator.CreateInstance(x) as BaseDay, dayIndex = int.Parse(x.Name.Replace("Day", "")) })
    .OrderBy(x => x.dayIndex);

//foreach (var day in days)
//    day.type!.Execute();

new Day12().Execute();
//days.Last().type.Execute();

//BenchmarkRunner.Run<Day6>();