$wd = "$($PSScriptRoot)\..\src\"
$cache = "$($PSScriptRoot)\..\tmp"
$sln = Get-ChildItem $wd -File -Filter "*.sln" | Select-Object -First 1 -ExpandProperty FullName

Write-Host "Working with solution: $sln"

Get-ChildItem -Path $cache -Directory -Recurse -Force `
 | Where-Object { $_.Name -eq "TestResults" } `
 | ForEach-Object { `
	Write-Host "Removing folder: $($_.FullName)" -ForegroundColor Green `
	&& Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction SilentlyContinue `
} && dotnet test $sln --collect:"XPlat Code Coverage" --results-directory "$cache\testresults" ; `
	dotnet-coverage merge "$cache\testresults\**\*.cobertura.xml" -o "$cache\merged.xml" -f cobertura `
	&& reportgenerator -reports:"$cache\merged.xml" -targetdir:"$cache\report" -reporttypes:Html ; `
	explorer "$cache\report\index.html"; Write-Host "Opening $cache\report\index.html"