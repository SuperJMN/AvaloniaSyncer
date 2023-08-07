using System;
using Microsoft.Extensions.Logging;

namespace AvaloniaSyncer;

public class LoggerAdapter : ILogger
{
    private readonly Serilog.ILogger _logger;

    public LoggerAdapter(Serilog.ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        // Serilog no admite el concepto de ámbitos (scopes) directamente,
        // por lo que en esta implementación simplemente retornamos nulo.
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        // Convertimos el nivel de registro de Microsoft.Extensions.Logging a Serilog.Level
        return _logger.IsEnabled(ConvertToSerilogLevel(logLevel));
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        // Convertimos el nivel de registro de Microsoft.Extensions.Logging a Serilog.Level
        var serilogLogLevel = ConvertToSerilogLevel(logLevel);

        // Llamamos a Serilog con el mensaje y, si corresponde, la excepción
        if (formatter != null)
        {
            _logger.Write(serilogLogLevel, exception, formatter(state, exception));
        }
        else
        {
            _logger.Write(serilogLogLevel, exception, state.ToString());
        }
    }

    private static Serilog.Events.LogEventLevel ConvertToSerilogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => Serilog.Events.LogEventLevel.Verbose,
            LogLevel.Debug => Serilog.Events.LogEventLevel.Debug,
            LogLevel.Information => Serilog.Events.LogEventLevel.Information,
            LogLevel.Warning => Serilog.Events.LogEventLevel.Warning,
            LogLevel.Error => Serilog.Events.LogEventLevel.Error,
            LogLevel.Critical => Serilog.Events.LogEventLevel.Fatal,
            _ => Serilog.Events.LogEventLevel.Information,
        };
    }
}