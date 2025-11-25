using System.Diagnostics.Eventing.Reader;

using System.Diagnostics;

var query = new EventLogQuery("System", PathType.LogName);
query.ReverseDirection = true;

var reader = new EventLogReader(query);

var count = 0;
while (count < 10)
{
    var eventRecord = reader.ReadEvent();
    if (eventRecord == null)
        break;

    Console.WriteLine($"Event ID: {eventRecord.Id}, Level: {eventRecord.LevelDisplayName}, TimeCreated: {eventRecord.TimeCreated}");
    count++;
}

// var reader = new EventLog
Console.WriteLine("Hello, World!");