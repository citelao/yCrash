using System.Diagnostics.Eventing.Reader;

namespace Helpers;

public static class EventExtensions
{
    public static IEnumerable<EventRecord> Iterator(this EventLogReader reader)
    {
        EventRecord eventRecord;
        while ((eventRecord = reader.ReadEvent()) != null)
        {
            yield return eventRecord;
        }

        yield break;
    }
}