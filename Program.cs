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

/*
- <Event xmlns="http://schemas.microsoft.com/win/2004/08/events/event">
- <System>
  <Provider Name="Microsoft-Windows-TPM-WMI" Guid="{7d5387b0-cbe0-11da-a94d-0800200c9a66}" /> 
  <EventID>1801</EventID> 
  <Version>0</Version> 
  <Level>2</Level> 
  <Task>0</Task> 
  <Opcode>0</Opcode> 
  <Keywords>0x8000000000000000</Keywords> 
  <TimeCreated SystemTime="2025-11-25T15:38:46.8118362Z" /> 
  <EventRecordID>139287</EventRecordID> 
  <Correlation /> 
  <Execution ProcessID="19384" ThreadID="19196" /> 
  <Channel>System</Channel> 
  <Computer>TOOTHLESS</Computer> 
  <Security UserID="S-1-5-18" /> 
  </System>
- <EventData>
  <Data Name="DeviceAttributes">BaseBoardManufacturer:ASRock;FirmwareManufacturer:American Megatrends International, LLC.;FirmwareVersion:P2.20;OEMModelNumber:To Be Filled By O.E.M.;OEMModelBaseBoard:B550M Pro4;OEMModelSystemFamily:To Be Filled By O.E.M.;OEMManufacturerName:To Be Filled By O.E.M.;OEMModelSKU:To Be Filled By O.E.M.;OSArchitecture:amd64;</Data> 
  <Data Name="BucketId">2106b64953bae863e628e7e56f844e4186dc0216303d9dbff9ccdc4261c5e289</Data> 
  <Data Name="BucketConfidenceLevel" /> 
  <Data Name="UpdateType">0</Data> 
  <Data Name="HResult">0</Data> 
  </EventData>
  </Event>
*/
/* 
- <Event xmlns="http://schemas.microsoft.com/win/2004/08/events/event">
- <System>
  <Provider Name="Microsoft-Windows-Kernel-Power" Guid="{331c3b3a-2005-44c2-ac5e-77220c37d6b4}" /> 
  <EventID>41</EventID> 
  <Version>10</Version> 
  <Level>1</Level> 
  <Task>63</Task> 
  <Opcode>0</Opcode> 
  <Keywords>0x8000400000000002</Keywords> 
  <TimeCreated SystemTime="2025-11-25T15:33:38.3279369Z" /> 
  <EventRecordID>139034</EventRecordID> 
  <Correlation /> 
  <Execution ProcessID="4" ThreadID="8" /> 
  <Channel>System</Channel> 
  <Computer>TOOTHLESS</Computer> 
  <Security UserID="S-1-5-18" /> 
  </System>
- <EventData>
  <Data Name="BugcheckCode">0</Data> 
  <Data Name="BugcheckParameter1">0x0</Data> 
  <Data Name="BugcheckParameter2">0x0</Data> 
  <Data Name="BugcheckParameter3">0x0</Data> 
  <Data Name="BugcheckParameter4">0x0</Data> 
  <Data Name="SleepInProgress">0</Data> 
  <Data Name="PowerButtonTimestamp">0</Data> 
  <Data Name="BootAppStatus">0</Data> 
  <Data Name="Checkpoint">0</Data> 
  <Data Name="ConnectedStandbyInProgress">false</Data> 
  <Data Name="SystemSleepTransitionsToOn">0</Data> 
  <Data Name="CsEntryScenarioInstanceId">0</Data> 
  <Data Name="BugcheckInfoFromEFI">false</Data> 
  <Data Name="CheckpointStatus">0</Data> 
  <Data Name="CsEntryScenarioInstanceIdV2">0</Data> 
  <Data Name="LongPowerButtonPressDetected">false</Data> 
  <Data Name="LidReliability">false</Data> 
  <Data Name="InputSuppressionState">0</Data> 
  <Data Name="PowerButtonSuppressionState">0</Data> 
  <Data Name="LidState">3</Data> 
  <Data Name="WHEABootErrorCount">0</Data> 
  </EventData>
  </Event>
*/
/*
  - <Event xmlns="http://schemas.microsoft.com/win/2004/08/events/event">
- <System>
  <Provider Name="EventLog" /> 
  <EventID Qualifiers="32768">6008</EventID> 
  <Version>0</Version> 
  <Level>2</Level> 
  <Task>0</Task> 
  <Opcode>0</Opcode> 
  <Keywords>0x80000000000000</Keywords> 
  <TimeCreated SystemTime="2025-11-25T15:33:46.3051443Z" /> 
  <EventRecordID>139000</EventRecordID> 
  <Correlation /> 
  <Execution ProcessID="3092" ThreadID="4012" /> 
  <Channel>System</Channel> 
  <Computer>TOOTHLESS</Computer> 
  <Security /> 
  </System>
- <EventData>
  <Data>10:23:28 AM</Data> 
  <Data>‎11/‎25/‎2025</Data> 
  <Data /> 
  <Data /> 
  <Data>12</Data> 
  <Data /> 
  <Data /> 
  <Binary>E9070B00020019000A0017001C006501E9070B00020019000F0017001C0065013C0000003C000000000000000000000000000000000000000000000000000000</Binary> 
  </EventData>
  </Event>
*/
var queryString = @"
  *[System/Provider[@Name='Microsoft-Windows-TPM-WMI'
    or @Name='Microsoft-Windows-Kernel-Boot'
    or @Name='Microsoft-Windows-Kernel-General'
    or @Name='Microsoft-Windows-Kernel-Power'
    or @Name='Microsoft-Windows-Kernel-Processor-Power'
    or @Name='User32'
    or @Name='WER-SystemErrorReporting'
    or @Name='EventLog'

    or @Name='Microsoft-Windows-Kernel-PnP'
    or @Name='stornvme'

    or @Name='Microsoft-Windows-WindowsUpdateClient'
    or @Name='Microsoft-Windows-UserPnp']]
";

var query = new EventLogQuery("System", PathType.LogName, queryString);
query.ReverseDirection = true;

var reader = new EventLogReader(query);
foreach (var eventRecord in reader.Iterator()
  .Where((e) => e.TimeCreated > DateTime.Now.AddDays(-1))
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
  Console.WriteLine(descrString);

  // TODO: print out event data!
}