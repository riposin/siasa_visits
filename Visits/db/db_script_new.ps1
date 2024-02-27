<#
This is script expects sqlite3.exe to be in the same folder than itself.
The script checks if the visits database exists; If so, it will try to overwrite it;
in case of error, an error will be displayed.
#>

$file = 'visits.db'
<#
If (Test-Path $file){Remove-Item $file}
#>
$CreateFile = $True

If ([System.IO.File]::Exists($file)) {
	Try {
		$FileStream = [System.IO.File]::Open($file,'Open','Write')
		$FileStream.Close()
		$FileStream.Dispose()
		Remove-Item $file
	} Catch {
		$CreateFile = $False
		Write-Output "Error: $($_.Exception.Message)"
		read-host "`r`nPress ENTER to continue..."
	}
}

if ($CreateFile){Get-Content .\db_script_new.sqlite | .\sqlite3.exe}