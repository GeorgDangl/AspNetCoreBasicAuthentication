$testProjects = "AspNetCoreBasicAuthentication.Tests"
$testRuns = 1;

& dotnet restore
& dotnet build -c Debug

foreach ($testProject in $testProjects){
    & cd "$PSScriptRoot\$testProject"
    & dotnet.exe xunit `
        -nobuild `
        -xml "$PSScriptRoot\results_$testRuns.testresults"   
			
    $testRuns++
}

& cd $PSScriptRoot
