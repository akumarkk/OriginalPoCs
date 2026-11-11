using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

/// <summary>
/// Picks DapPaymentV2 when "paymentDetails" is present, otherwise PaymentV2; writes the runtime type.
/// </summary>
public class PaymentV2Converter : JsonConverter<PaymentV2>
{
    private JsonSerializerOptions? _inner;

    // Copy of the options without this converter, to avoid infinite recursion
    private JsonSerializerOptions Inner(JsonSerializerOptions options)
    {
        if (_inner is null)
        {
            var copy = new JsonSerializerOptions(options);
            copy.Converters.Remove(this);
            _inner = copy;
        }
        return _inner;
    }

    public override PaymentV2? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        var target = root.TryGetProperty("paymentDetails", out _) ? typeof(DapPaymentV2) : typeof(PaymentV2);
        return (PaymentV2?)root.Deserialize(target, Inner(options));
    }

    public override void Write(Utf8JsonWriter writer, PaymentV2 value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), Inner(options));
    }
}

/// <summary>
/// Base class representing payment information
/// </summary>
public class PaymentV2
{
    /// <summary>
    /// Unique identifier for the payment
    /// </summary>
    [JsonPropertyName("paymentId")]
    public string PaymentId { get; set; }

    /// <summary>
    /// Total amount for the payment
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code (e.g., USD, EUR)
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    /// <summary>
    /// Status of the payment
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; }
}

/// <summary>
/// Derived payment class representing DAP payment information
/// </summary>
public class DapPaymentV2 : PaymentV2
{
    /// <summary>
    /// Payment transaction and breakdown values
    /// </summary>
    [JsonPropertyName("paymentDetails")]
    public DapPaymentDetailsV2 PaymentDetails { get; set; }
}

/// <summary>
/// Detailed payment breakdown and transaction information for DAP
/// </summary>
public class DapPaymentDetailsV2
{
    /// <summary>
    /// Base amount for the payment transaction
    /// </summary>
    [JsonPropertyName("baseAmount")]
    public decimal BaseAmount { get; set; }

    /// <summary>
    /// Tax amount associated with the payment
    /// </summary>
    [JsonPropertyName("taxAmount")]
    public decimal TaxAmount { get; set; }
}