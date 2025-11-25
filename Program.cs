using System.Diagnostics.Eventing.Reader;
using static Crayon.Output;

using Helpers;

// var queryString = @"
//   *[System[(Level=1 or Level=2 or Level=3) and TimeCreated[timediff(@SystemTime) <= 86400000]]]
// ";

// Events pulled from:
// https://learn.microsoft.com/en-us/troubleshoot/windows-server/performance/troubleshoot-unexpected-reboots-system-event-logs
var providers = new[]
{
  // Noisy, not-super-helpful:
  // "Microsoft-Windows-TPM-WMI",

  "Microsoft-Windows-Kernel-Boot",
  "Microsoft-Windows-Kernel-General",
  "Microsoft-Windows-Kernel-Power",
  "Microsoft-Windows-Kernel-Processor-Power",
  "User32",
  "WER-SystemErrorReporting",
  "EventLog",
  "Microsoft-Windows-WHEA-Logger",

  "Microsoft-Windows-Kernel-PnP",
  // "Microsoft-Windows-DriverFrameworks-UserMode",
  "Microsoft-Windows-Ntfs",
  // "stornvme",
  "volmgr",

  "Microsoft-Windows-WindowsUpdateClient",
  "Microsoft-Windows-UserPnp"
};
var providerFilters = string.Join("\n    or ",
  providers.Select(p => $"@Name='{p}'"));
var queryString = $@"
  *[System/Provider[{providerFilters}]]
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