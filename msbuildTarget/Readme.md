###### dotnet build-server


```
dotnet build-server shutdown
```

```
dotnet build -v d
```

```
<Project>
  <Target Name='MyCustomGlobalInjection' BeforeTargets='BeforeBuild'>
    <Message Importance='high' Text='--- [MyTool GLOBAL] Injecting Custom Logic into $(ProjectName) ---' />
    <Warning Text='--- [MyTool GLOBAL] [warn] Injecting Custom Logic ---' />
  </Target>
</Project>
```