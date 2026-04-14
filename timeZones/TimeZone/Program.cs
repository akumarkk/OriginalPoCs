namespace  Program;
class Program
{
    public static void Main()
    {
        DateTime dt = DateTime.Parse("2023-01-25T23:58:37");
        DateTime utc = TimeZoneInfo.ConvertTimeToUtc(dt, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));

        Console.WriteLine($"dt {dt} utc {utc}");
    }
    
}