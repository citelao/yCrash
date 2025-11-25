using System.Diagnostics.Eventing.Reader;
using static Crayon.Output;

using Helpers;

// var queryString = @"
//   *[System[(Level=1 or Level=2 or Level=3) and TimeCreated[timediff(@SystemTime) <= 86400000]]]
// ";

// or @Name='EventLog'
// or @Name='Microsoft-Windows-Kernel-PnP'
// or @Name='Microsoft-Windows-DriverFrameworks-UserMode'
// Microsoft-Windows-Ntfs

// Events pulled from:
// https://learn.microsoft.com/en-us/troubleshoot/windows-server/performance/troubleshoot-unexpected-reboots-system-event-logs
var queryString = @"
  *[System/Provider[@Name='Microsoft-Windows-TPM-WMI'
    or @Name='Microsoft-Windows-Kernel-Boot'
    or @Name='Microsoft-Windows-Kernel-General'
    or @Name='Microsoft-Windows-Kernel-Power'
    or @Name='Microsoft-Windows-Kernel-Processor-Power'
    or @Name='User32'
    or @Name='WER-SystemErrorReporting'
    or @Name='EventLog'
    or @Name='Microsoft-Windows-WHEA-Logger'

    or @Name='Microsoft-Windows-Kernel-PnP'
    or @Name='stornvme'
    or @Name='volmgr'

    or @Name='Microsoft-Windows-WindowsUpdateClient'
    or @Name='Microsoft-Windows-UserPnp']]
";

var query = new EventLogQuery("System", PathType.LogName, queryString);
query.ReverseDirection = true;

var verbose = false;

var reader = new EventLogReader(query);
foreach (var eventRecord in reader.Iterator()
  // .Where((e) => e.TimeCreated > DateTime.Now.AddDays(-1))
  .Where((e) => verbose || e.Level <= 3)
  .Take(500))
{
  var levelString = eventRecord.Level switch
  {
      1 => Red(eventRecord.LevelDisplayName),
      2 => Red(eventRecord.LevelDisplayName),
      3 => Yellow(eventRecord.LevelDisplayName),
      4 => Cyan("Info"),
      5 => Green(eventRecord.LevelDisplayName),
      _ => eventRecord.LevelDisplayName
  };

  var description = eventRecord.FormatDescription();
  var descriptionFirstLine = description?.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
  var descriptionRest = description?.Substring(descriptionFirstLine.Length).TrimStart() ?? null;
  var hasMore = !string.IsNullOrEmpty(descriptionRest);

  Console.WriteLine($"{Dim($"{eventRecord.TimeCreated} {levelString}\t[{eventRecord.ProviderName}]")} {eventRecord.Id} {eventRecord.TaskDisplayName}");

  var descrString = eventRecord.Level <= 3
    ? description
    : Dim(descriptionFirstLine + (hasMore ? " ..." : ""));
  Console.WriteLine($"    {descrString}");

  // TODO: print out event data!
}