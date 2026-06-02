###### dk commands
```


docker build --build-arg DT_TOKEN="dt0c01.." -t dt-test-cve-musl-v2:latest .


docker run --rm -p 8081:8080 --name local-dt-test-musl dt-test-cve-musl-v2:latest                                                    
```