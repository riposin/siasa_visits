<#
This is script expects sqlite3.exe to be in the same folder than itself.
The script checks if the visits database exists; If so, it will try to rename it;
in case of error, an error will be displayed.
#>

$file = 'visits'
$ext = '.db'
$dtime = Get-Date -Format "yyyyMMddHHmmss"
$origname = $file + $ext
$newname = $file + $dtime + $ext
$cwd = Get-Location
<#
If (Test-Path $file){Remove-Item $file}
#>
$CreateFile = $True

If ([System.IO.File]::Exists("$cwd\$origname")) {
	Try {
		$FileStream = [System.IO.File]::Open("$cwd\$origname",'Open','Write')
		$FileStream.Close()
		$FileStream.Dispose()
		<#
		Remove-Item "$cwd\$origname"
		#>
		Rename-Item "$cwd\$origname" -NewName $newname
	} Catch {
		$CreateFile = $False
		Write-Output "The file $file$ext could not be renamed, probably because it is in use."
		<#Write-Output "Error: $($_.Exception.Message)"#>
		read-host "`r`nPress any key to exit..."
	}
}
if ($CreateFile){Get-Content .\db_script_new.sqlite | .\sqlite3.exe}
