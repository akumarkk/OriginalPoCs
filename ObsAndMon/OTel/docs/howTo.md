```
 docker run -d -p 4317:4317 -v "${PWD}/otel-config.yml:/etc/otelcol/config.yaml" -p 4318:4318 -
-name otel-collector otel/opentelemetry-collector-contrib:0.154.0
```