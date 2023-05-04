$full = $args[0]

# COMMON PATHS 

$rootFolder = (Get-Item -Path "./" -Verbose).FullName

$rootPath = (Get-Item -Path "..")

Write-Host("rootFolder:"+$rootFolder)
Write-Host("rootPath:"+$rootPath)

# List of solutions used only in development mode
$solutionPaths = @(
		"../framework"
		"../modules/file-storing-database",
		"../modules/file-storing-management",
		"../modules/map-tenancy-management",
		"../modules/tenant-group-management",
		"../modules/dbconnections",
		"../modules/Identity",
		"../modules/IdentityServer",
		"../modules/openiddict",
		"../modules/audit-logging",
		"../modules/account",
		"../modules/minid"
	)

if ($full -eq "-f")
{
	# List of additional solutions required for full build
#	$solutionPaths += (
#		"../modules/client-simulation",
#		"../modules/virtual-file-explorer",
#	) 
}else{ 
	Write-Host ""
	Write-Host ":::::::::::::: !!! You are in development mode !!! ::::::::::::::" -ForegroundColor red -BackgroundColor  yellow
	Write-Host "" 
} 