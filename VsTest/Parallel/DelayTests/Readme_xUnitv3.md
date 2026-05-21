##### MTP (Without the Visual Studio Runner)


```
<!-- 1. xUnit v3 projects MUST be executables -->
    <OutputType>Exe</OutputType>

<!-- 2. Tell the SDK to route `dotnet test` through MTP instead of VSTest -->
    <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>


<ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
    
    <!-- 3. Use the v3 metapackage (includes MTP v2 support by default) -->
    <PackageReference Include="xunit.v3" Version="3.2.2" />
    
    <!-- ❌ NO Microsoft.NET.Test.Sdk needed -->
    <!-- ❌ NO xunit.runner.visualstudio needed -->
  </ItemGroup>
```