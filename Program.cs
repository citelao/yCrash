using System.Diagnostics.Eventing.Reader;
using static Crayon.Output;

using Helpers;

var query = new EventLogQuery("System", PathType.LogName);
query.ReverseDirection = true;

var reader = new EventLogReader(query);
foreach (var eventRecord in reader.Iterator().Take(10))
{
    Console.WriteLine($"Event ID: {eventRecord.Id}, Level: {eventRecord.LevelDisplayName}, TimeCreated: {eventRecord.TimeCreated}");
}