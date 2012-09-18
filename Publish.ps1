# Run this script by typing ".\Publish.ps1" in the Package Manager Console.

# Determine the current configuration and "Release" configuration.
$build = $dte.Solution.SolutionBuild
$origConfig = $build.ActiveConfiguration
$relConfig = $build.SolutionConfigurations.Item("Release")

# Switch to "Release" and build.
$relConfig.Activate()
$build.Build($true)

# Run the site and download the HTML.
$dte.ExecuteCommand("Debug.StartWithoutDebugging")
$url = (Get-Project).Properties.Item("WebApplication.BrowseURL").Value
$wc = New-Object System.Net.WebClient
$solutionDirectory = [System.IO.Path]::GetDirectoryName($dte.Solution.FullName)
$destination = [System.IO.Path]::Combine($solutionDirectory, "index.html")
$wc.DownloadFile($url, $destination)

# Copy the HTML and supporting files to gh-pages.
$target = [System.IO.Path]::Combine($solutionDirectory, "..\\gh-pages")
Move-Item $destination $target -Force
Copy-Item ([System.IO.Path]::Combine([System.IO.Path]::Combine($solutionDirectory, "StephenCleary.com"), "Content")) $target -Force -Recurse
Copy-Item ([System.IO.Path]::Combine([System.IO.Path]::Combine($solutionDirectory, "StephenCleary.com"), "Scripts")) $target -Force -Recurse

# Switch back to the original configuration.
$origConfig.Activate()
