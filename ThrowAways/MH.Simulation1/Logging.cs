using Godot;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

public static class GodotLogging
{
    public static ILoggerFactory GodotLoggerFactory()
    {
        var lf = new LoggerFactory();
        lf.AddProvider(new GodotLoggerProvider());
        return lf;
    }
}
public class GodotLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, GodotLogger> _loggers = [];
    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, (name) => new GodotLogger());
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}
public class GodotLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        GD.Print($"[{eventId.Id,2}: {logLevel,-12}] {formatter(state, exception)}");
    }
}