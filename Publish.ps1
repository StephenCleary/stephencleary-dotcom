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
$destination = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($dte.Solution.FullName), "index.html")
$wc.DownloadFile($url, $destination)

# Switch back to the original configuration.
$origConfig.Activate()
