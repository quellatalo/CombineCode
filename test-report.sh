#!/usr/bin/env sh
rootDir=${PWD}
outDir="$rootDir/TestResults"
reportDir="$outDir/Reports"


echo "Test and generate reports"

echo "Checking environment..."
dotnet tool restore

if [ -d "$outDir" ]; then
    echo "cleaning $outDir..."
	rm -rf "$outDir"
fi

echo "Testing .NET solution..."
export CI=true
dotnet test -c Release -- --coverage --results-directory "$outDir" --coverage-output-format cobertura
dotnet tool run reportgenerator "-reports:TestResults/**/*.xml" "-targetDir:$reportDir" "-reportTypes:HtmlInline_AzurePipelines"
echo "Reports generated in $reportDir"
read -rp "Press enter to exit"
