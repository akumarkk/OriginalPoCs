###### config
```

>az functionapp runtime config set --name seFuncApp --resource-group sefuncapp --runtime-version 8.0
Argument '--runtime-version' is in preview and under development. Reference and support levels: https://aka.ms/CLI_refstatus
{
  "name": "dotnet-isolated",
  "version": "8.0"
}

>az functionapp runtime config show --name seFuncApp --resource-group sefuncapp
{
  "name": "dotnet-isolated",
  "version": "8.0"
}
```