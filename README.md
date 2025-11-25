# yCrash

**yCrash** is a simple tool to read the Windows Event Logs when diagnosing weird
shutdowns.

## Usage

```pwsh
dotnet run
```

Example output

![Terminal window with some error message spew](/doc/img/output.png)

## Do you want this?

Windows Event Viewer (built-in app) and Reliability History (search `View reliability history` in Start) are both okay, but they are both slower than they should be. I built this to see if there was info I was missing because the existing views were mediocre.

I found:

* [Con] This did not directly indicate whether my crashes were based on overheating (there were no secret logs I missed in Event Viewer).
* [Pro] This tool runs **much** faster than using either tool; all lags are UI-related.
* [Pro] This is a simpler to read through than Event Viewer, in many ways.

Sharing in case others find this useful.

## See also

* MSDN [Troubleshoot unexpected reboots using system event logs](https://learn.microsoft.com/en-us/troubleshoot/windows-server/performance/troubleshoot-unexpected-reboots-system-event-logs)
* MSDN [`System.Diagnostics.Eventing.Reader`](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.eventing.reader?view=windowsdesktop-10.0)
* [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor?tab=readme-ov-file)