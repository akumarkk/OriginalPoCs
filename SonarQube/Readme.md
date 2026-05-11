##### "Three-Step" Sequence
- Begin: 
```
dotnet sonarscanner begin /k:"SonarTest" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="xxx"
```

- Build: 
```
dotnet build
```

End: 
```
dotnet sonarscanner end /d:sonar.token="xxx"
```