using System.Diagnostics;
using Nuke.Common;

class NukeTraceListener : TextWriterTraceListener
{
    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
        string message)
    {
        TraceEvent(eventCache, source, eventType, id, "{0}", message);
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
        string format, params object[] args)
    {
        switch (eventType)
        {
            case TraceEventType.Critical:
            case TraceEventType.Error:
                Logger.Error(format, args);
                break;
            case TraceEventType.Information:
                Logger.Info(format, args);
                break;
            case TraceEventType.Verbose:
                Logger.Trace(format, args);
                break;
            case TraceEventType.Warning:
                Logger.Warn(format, args);
                break;
            case TraceEventType.Resume:
            case TraceEventType.Start:
            case TraceEventType.Stop:
            case TraceEventType.Suspend:
            case TraceEventType.Transfer:
                Logger.Trace(format, args);
                break;
        }
    }
}