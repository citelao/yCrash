using System.Diagnostics.Eventing.Reader;
using static Crayon.Output;

using Helpers;

// var queryString = @"
//   *[System[(Level=1 or Level=2 or Level=3) and TimeCreated[timediff(@SystemTime) <= 86400000]]]
// ";

// or @Name='EventLog'
// or @Name='Microsoft-Windows-Kernel-PnP'
// or @Name='Microsoft-Windows-DriverFrameworks-UserMode'
var queryString = @"
  *[System/Provider[@Name='TPM-WMI'
    or @Name='Kernel-General'
    or @Name='Kernel-Power'
    or @Name='User32'
    or @Name='WindowsUpdateClient'
    or @Name='WER-SystemErrorReporting'

    or @Name='Microsoft-Windows-WindowsUpdateClient'
    or @Name='Microsoft-Windows-UserPnp']]
";

var query = new EventLogQuery("System", PathType.LogName, queryString);
query.ReverseDirection = true;

var reader = new EventLogReader(query);
foreach (var eventRecord in reader.Iterator().Take(50))
{
    Console.WriteLine($"{Dim($"[{eventRecord.ProviderName}]")} {eventRecord.Id} {eventRecord.TaskDisplayName}, Level: {eventRecord.LevelDisplayName}, TimeCreated: {eventRecord.TimeCreated}");
}