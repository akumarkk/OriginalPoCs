using System.Text.Json;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        Console.WriteLine("=== 1. SERIALIZATION (Object -> JSON) ===");

        // Create instances of the derived classes
        Aircraft commercial = new CommercialAircraft 
        { 
            Model = "Boeing 737", 
            CruisingSpeedKnots = 484, 
            PassengerCapacity = 189 
        };

        Aircraft cargo = new CargoAircraft 
        { 
            Model = "Boeing 747 Freighter", 
            CruisingSpeedKnots = 502, 
            MaxPayloadTons = 137.7 
        };

        // Put them in a base-type array
        Aircraft[] fleet = { commercial, cargo };
        string jsonOutput = JsonSerializer.Serialize(fleet, options);

        Console.WriteLine(jsonOutput);


        Console.WriteLine("\n=== 2. DESERIALIZATION (JSON -> Object) ===");

        // JSON string containing the "aircraftType" discriminator property
        string jsonInput = @"[
            {
                ""model"": ""Airbus A320"",
                ""cruisingSpeedKnots"": 470,
                ""passengerCapacity"": 180
            },
            {
                ""model"": ""Antonov An-124"",
                ""cruisingSpeedKnots"": 460,
                ""maxPayloadTons"": 150.0
            }
        ]";

        // Deserialize back into the base type array
        Aircraft[] deserializedFleet = JsonSerializer.Deserialize<Aircraft[]>(jsonInput, options);

        foreach (var craft in deserializedFleet)
        {
            Console.WriteLine($"\nInstantiated Object Type: {craft.GetType().Name}");
            Console.WriteLine($"Model: {craft.Model} (Speed: {craft.CruisingSpeedKnots} knots)");
            Console.WriteLine($"Type Discriminator: {(craft is CommercialAircraft ? "commercial" : craft is CargoAircraft ? "cargo" : "unknown")}");
            // Pattern matching to access subclass-specific properties safely
            if (craft is CommercialAircraft passengerPlane)
            {
                Console.WriteLine($"Passenger Seats: {passengerPlane.PassengerCapacity}");
            }
            else if (craft is CargoAircraft freighter)
            {
                Console.WriteLine($"Max Cargo Payload: {freighter.MaxPayloadTons} tons");
            }
        }
    }
}

