using System.Diagnostics.Eventing.Reader;
using static Crayon.Output;

using Helpers;

// var queryString = @"
//   *[System[(Level=1 or Level=2 or Level=3) and TimeCreated[timediff(@SystemTime) <= 86400000]]]
// ";

// or @Name='EventLog'
// or @Name='Microsoft-Windows-Kernel-PnP'
// or @Name='Microsoft-Windows-DriverFrameworks-UserMode'

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
var queryString = @"
  *[System/Provider[@Name='Microsoft-Windows-TPM-WMI'
    or @Name='Microsoft-Windows-Kernel-General'
    or @Name='Microsoft-Windows-Kernel-Power'
    or @Name='Microsoft-Windows-Kernel-Processor-Power'
    or @Name='User32'
    or @Name='WER-SystemErrorReporting'

    or @Name='Microsoft-Windows-WindowsUpdateClient'
    or @Name='Microsoft-Windows-UserPnp']]
";

var query = new EventLogQuery("System", PathType.LogName, queryString);
query.ReverseDirection = true;

var reader = new EventLogReader(query);
foreach (var eventRecord in reader.Iterator().Take(50))
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
  Console.WriteLine($"{Dim($"{eventRecord.TimeCreated} {levelString}\t[{eventRecord.ProviderName}]")} {eventRecord.Id} {eventRecord.TaskDisplayName}");
}