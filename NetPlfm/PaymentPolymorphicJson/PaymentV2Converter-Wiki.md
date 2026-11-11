# Why `PaymentV2Converter` is required

## Problem

`DapPaymentV2` derives from `PaymentV2`. With `System.Text.Json`, a value typed as the base class behaves differently when serializing and deserializing.

```csharp
PaymentV2 payment = new DapPaymentV2 { ... };
```

| Direction | Default behavior (no converter) |
|---|---|
| Serialize `PaymentV2` | Uses the **declared** type. Derived members such as `paymentDetails` are dropped. |
| Deserialize `PaymentV2` | Always creates a plain `PaymentV2`. The serializer cannot infer the derived type from JSON shape. |

`System.Text.Json` only handles derived types when polymorphism is configured explicitly, either with `[JsonPolymorphic]` / `[JsonDerivedType]` or with a custom converter. This sample uses a converter so the JSON needs no `$type` discriminator.

## Results without the converter

```text
Serialized:
{
  "paymentId": "PAY-001",
  "amount": 110,
  "currency": "USD",
  "status": "Completed"
}
Round-trip type: PaymentV2
From string type: PaymentV2
```

- `paymentDetails` is lost on serialization, which is silent data loss.
- Both deserializations return `PaymentV2`, so the `is DapPaymentV2` check fails and the line printing `base`/`tax` never runs.

## Results with the converter

```text
Serialized:
{
  "paymentDetails": {
    "baseAmount": 100,
    "taxAmount": 10
  },
  "paymentId": "PAY-001",
  "amount": 110,
  "currency": "USD",
  "status": "Completed"
}
Round-trip type: DapPaymentV2
From string type: DapPaymentV2
PAY-002: base=200.00, tax=20.50
```

- `paymentDetails` is preserved.
- Both deserializations return `DapPaymentV2`.
- The derived-type check passes and the `base`/`tax` line prints.

## How the converter works

Registered through `JsonSerializerOptions.Converters`. It applies only when the requested type is exactly `PaymentV2`.

- **Write**: serializes `value.GetType()` (the runtime type) instead of the declared type.
- **Read**: parses the JSON into a `JsonDocument`. It picks `DapPaymentV2` if a `paymentDetails` property exists, and `PaymentV2` otherwise.
- **`Inner(options)`**: returns a cached copy of the options with this converter removed. Without it, `Read`/`Write` would call back into themselves for `PaymentV2` and recurse forever.

## Trade-offs

- Type selection is based on JSON shape. Every new derived type needs a new rule in `Read`.
- Existing payloads need no change and no `$type` field.
- Alternative: `[JsonPolymorphic]` + `[JsonDerivedType]` on `PaymentV2`. It is less code, but the JSON must carry a type discriminator (default `$type`).
