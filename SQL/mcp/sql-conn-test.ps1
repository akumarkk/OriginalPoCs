try { . "c:\Users\krisrepo\AppData\Local\Programs\Microsoft VS Code\e7fb5e96c0\resources\app\out\vs\workbench\contrib\terminal\common\scripts\shellIntegration.ps1" } catch {}

Get-ChildItem -File -Recurse -Filter .env | Select-Object FullName
Get-ChildItem -File -Recurse -Filter *.env | Select-Object FullName

[System.Environment]::GetEnvironmentVariable("MSSQL_CONNECTION_STRING", "User")
[System.Environment]::GetEnvironmentVariable("MSSQL_CONNECTION_STRING", "Process")
[System.Environment]::GetEnvironmentVariable("MSSQL_CONNECTION_STRING", "Machine")
[System.Environment]::GetEnvironmentVariable("MSSQL_CONNECTION_STRING")

Resolve-DnsName db-name
Test-NetConnection -ComputerName db-name -Port 1433
Test-NetConnection -ComputerName db-name -Port 1433

$connectionString = "Data Source=tcp:db-name,1433;Initial Catalog=db-catalog-name;Persist Security Info=True;User ID=db-catalog-user-name;Password=<redacted>;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;Command Timeout=0"
$sqlConn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
try {
    $sqlConn.Open()
    "Connection successful"
    $sqlConn.Close()
} catch {
    $_.Exception.Message
}

$connectionString = "Server=tcp:db-name,1433;Initial Catalog=db-catalog-name;Persist Security Info=True;User ID=db-catalog-user-name;Password=<redacted>;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;Command Timeout=0"
Add-Type -AssemblyName System.Data
$sqlConn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
try {
    $sqlConn.Open()
    "Connection successful"
    $sqlConn.Close()
} catch {
    $_.Exception.Message
}

Add-Type -AssemblyName System.Data
$connectionString = 'Data Source=tcp:db-name,1433;Initial Catalog=db-catalog-name;Persist Security Info=True;User ID=db-catalog-user-name;Password=<redacted>;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;Command Timeout=0'
$sqlConn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
try { $sqlConn.Open(); "SUCCESS"; $sqlConn.Close() } catch { "FAILURE: " + $_.Exception.Message }

[System.Environment]::GetEnvironmentVariable("MSSQL_CONNECTION_STRING", "User") | Out-File -FilePath .env
Get-Content .env
Set-Content -Path .env -Value "MSSQL_CONNECTION_STRING='Data Source=tcp:db-name,1433;Initial Catalog=db-catalog-name;Persist Security Info=True;User ID=db-catalog-user-name;Password=<redacted>;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;Command Timeout=0'"
Get-Content .env
Set-Content -Path .env -Value "MSSQL_CONNECTION_STRING=Data Source=tcp:db-name,1433;Initial Catalog=db-catalog-name;Persist Security Info=True;User ID=db-catalog-user-name;Password=<redacted>;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;Command Timeout=0"
Get-Content .env