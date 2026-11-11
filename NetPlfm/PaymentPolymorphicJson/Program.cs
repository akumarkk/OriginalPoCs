using System.Text.Json;

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    AllowOutOfOrderMetadataProperties = true
};

// 1. Instantiate DapPaymentV2 as PaymentV2, serialize and deserialize
PaymentV2 payment = new DapPaymentV2
{
    PaymentId = "PAY-001",
    Amount = 110m,
    Currency = "USD",
    Status = "Completed",
    PaymentDetails = new DapPaymentDetailsV2 { BaseAmount = 100m, TaxAmount = 10m }
};

string json = JsonSerializer.Serialize(payment, options);
Console.WriteLine("Serialized:");
Console.WriteLine(json);

PaymentV2? roundTrip = JsonSerializer.Deserialize<PaymentV2>(json, options);
Console.WriteLine($"Round-trip type: {roundTrip?.GetType().Name}");

// 2. Deserialize from a raw JSON string carrying the DapPaymentV2 type discriminator
const string dapJson = """
{
  "paymentId": "PAY-002",
  "amount": 220.50,
  "currency": "EUR",
  "status": "Pending",
  "paymentDetails": { "baseAmount": 200.00, "taxAmount": 20.50 }
}
""";

PaymentV2? fromString = JsonSerializer.Deserialize<PaymentV2>(dapJson, options);
Console.WriteLine($"From string type: {fromString?.GetType().Name}");

if (fromString is DapPaymentV2 dap)
{
    Console.WriteLine($"{dap.PaymentId}: base={dap.PaymentDetails.BaseAmount}, tax={dap.PaymentDetails.TaxAmount}");
}
