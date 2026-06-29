using System.Text.Json;

namespace Program;

class PayloadPublisherService<T> : IPayloadPublisherService<T> where T : new()
{
    public void Publish(T payload)
    {
        string jsonString = JsonSerializer.Serialize(payload);
        Console.WriteLine($"Publishing payload: {jsonString}");


    }
}