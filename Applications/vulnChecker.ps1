# Get all project files in the current directory and subdirectories
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

$projects = Get-ChildItem -Recurse -Filter *.csproj
$vulnerableProjects = @()

Write-Host "Found $($projects.Count) projects. Starting vulnerability scan..." -ForegroundColor Cyan

foreach ($project in $projects) {
    Write-Host "`n------------------------------------------------------------" -ForegroundColor Gray
    Write-Host "Checking: $($project.Name)" -ForegroundColor Yellow
    Write-Host "Path: $($project.FullName)" -ForegroundColor DarkGray
    
    # Run the dotnet list command and capture the output.
    $output = dotnet list $project.FullName package --vulnerable --include-transitive --interactive
    
    # Display the output
    Write-Output $output
    
    # Check if the output indicates vulnerabilities
    if ($output -match "vulnerable") {
        $vulnerabilities = $output | Select-String "Top-level Package|Transitive Package" -Context 0, 999 | ForEach-Object { $_.Line + "`n" + ($_.Context.PostContext -join "`n") }
        $vulnerableProjects += [pscustomobject]@{
            Name = $project.Name
            Path = $project.FullName
            Vulnerabilities = $vulnerabilities
        }
    }
}

$stopwatch.Stop()
$elapsed = $stopwatch.Elapsed

Write-Host "`nScan Complete." -ForegroundColor Cyan
Write-Host "Time taken: $($elapsed.Hours)h $($elapsed.Minutes)m $($elapsed.Seconds)s" -ForegroundColor Cyan

if ($vulnerableProjects.Count -gt 0) {
    Write-Host "`n--- Vulnerable Projects Summary ---" -ForegroundColor Red
    foreach ($proj in $vulnerableProjects) {
        Write-Host "`nProject: $($proj.Name)" -ForegroundColor Yellow
        Write-Host "Path: $($proj.Path)" -ForegroundColor DarkGray
        Write-Host "Vulnerabilities:" -ForegroundColor Red
        $proj.Vulnerabilities | Write-Output
    }
} else {
    Write-Host "`nNo vulnerable projects found." -ForegroundColor Green
}