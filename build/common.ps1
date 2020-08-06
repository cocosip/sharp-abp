$full = $args[0]

# COMMON PATHS 

$rootFolder = (Get-Item -Path "./" -Verbose).FullName

# List of solutions used only in development mode
$solutionPaths = @(
		"../framework"
#		"../modules/users",
#		"../modules/permission-management",
	)

if ($full -eq "-f")
{
	# List of additional solutions required for full build
#	$solutionPaths += (
#		"../modules/client-simulation",
#		"../modules/virtual-file-explorer",
#	) 
}else{ 
	Write-host ""
	Write-host ":::::::::::::: !!! You are in development mode !!! ::::::::::::::" -ForegroundColor red -BackgroundColor  yellow
	Write-host "" 
} 