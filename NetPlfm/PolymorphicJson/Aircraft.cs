using System;
using System.Text.Json;
using System.Text.Json.Serialization;

// 1. Configure the base class for polymorphism
[JsonPolymorphic(TypeDiscriminatorPropertyName = "aircraftType")]
[JsonDerivedType(typeof(CommercialAircraft), typeDiscriminator: "commercial")]
[JsonDerivedType(typeof(CargoAircraft), typeDiscriminator: "cargo")]
public class Aircraft
{
    public string Model { get; set; }
    public int CruisingSpeedKnots { get; set; }
}

// 2. Derived class for passenger/commercial planes
public class CommercialAircraft : Aircraft
{
    public int PassengerCapacity { get; set; }
}

// 3. Derived class for cargo/freight planes
public class CargoAircraft : Aircraft
{
    public double MaxPayloadTons { get; set; }
}