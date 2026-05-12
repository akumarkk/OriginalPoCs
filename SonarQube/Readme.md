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


###### Accessing from Host
```
New-NetIPAddress -InterfaceAlias "vEthernet (anikris-Internal)" -IPAddress 192.168.10.1 -PrefixLength 24

# also off firewall for traffic from host to vm!
Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False
```