namespace Program;

using Moq;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var mockPublisherService = new Mock<IPayloadPublisherService<EventPayloadAbstract>>();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");

        }
    }
}