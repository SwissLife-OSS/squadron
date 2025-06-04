using System;
using System.Diagnostics;
using Docker.DotNet.Models;
using Newtonsoft.Json;

namespace Squadron;

internal sealed class Logger
{
    private readonly ContainerResourceSettings _settings;
    private readonly TraceSource _traceSource;

    internal Logger(SourceLevels level, ContainerResourceSettings settings)
    {
        _settings = settings;

        Trace.AutoFlush = true;
        _traceSource = new TraceSource("Squadron", level);
        _traceSource.Listeners.Clear();
        var writerTraceListener = new TextWriterTraceListener(Console.Out)
        {
            TraceOutputOptions = TraceOptions.None
        };
        _traceSource.Listeners.Add(writerTraceListener);
    }

    public void ContainerLogs(string logs)
    {
        Warning($"Container logs{Environment.NewLine}{logs}");
    }

    public void StartParameters(CreateContainerParameters startParams)
    {
        var startParamsJson = JsonConvert.SerializeObject(startParams, Formatting.Indented);
        Verbose($"Container parameters{Environment.NewLine}{startParamsJson}");
    }

    public void ContainerStatus(Status status)
    {
        Verbose($"Container status: IsReady {status.IsReady}, Message: {status.Message}");
    }

    public void Error(string message, Exception ex)
    {
        _traceSource.TraceEvent(TraceEventType.Error, 1, $"{CreateMessage(message)}{Environment.NewLine}{ex}");
    }

    public void Warning(string message)
    {
        _traceSource.TraceEvent(TraceEventType.Warning, 2, CreateMessage(message));
    }

    public void Information(string message)
    {
        _traceSource.TraceEvent(TraceEventType.Information, 3, CreateMessage(message));
    }

    public void Verbose(string message, Exception ex = null)
    {
        _traceSource.TraceEvent(TraceEventType.Verbose, 4, ex is null
            ? CreateMessage(message)
            : $"{CreateMessage(message)}{Environment.NewLine}{ex}");
    }

    private string CreateMessage(string message)
    {
        return $"[{DateTime.UtcNow}] [{_settings.ImageFullname}][{_settings.UniqueContainerName}]: {message}";
    }
}