public interface IPayloadPublisherService<T> where T : new()
{
    void Publish(T payload);
}