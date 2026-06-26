namespace Program;
class EventPayload: EventPayloadAbstract
{
    public override string EventName => "SolaceEventPayload";

    public string Data {get; set;} 
} 