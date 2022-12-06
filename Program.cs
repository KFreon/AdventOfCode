using System.Reflection;
using System.Runtime.CompilerServices;

var days = typeof(Day1).Assembly.GetTypes()
    .Where(x => !x.IsAbstract && x.Name.Contains("Day"))
    .Select(x => new { name = x.Name, type = Activator.CreateInstance(x) as BaseDay })
    .OrderBy(x => x.name);

foreach (var day in days)
    day.type!.Execute();
