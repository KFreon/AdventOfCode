public class Config
{
    public static string GetDayInput(string day) => Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + $"/Inputs/Day{day}Input.txt";
}