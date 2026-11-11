using System.Text.Json.Serialization;
using System.Collections.Generic;

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