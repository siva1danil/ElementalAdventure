using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ElementalAdventure.Common.Logging;

public class Logger {
    static Logger() {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
    }

    [Conditional("TRACE")]
    public static void Trace(string message, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
        Write(LogLevel.Trace, message, member, file, line);
    }

    [Conditional("DEBUG")]
    public static void Debug(string message, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
        Write(LogLevel.Debug, message, member, file, line);
    }

    public static void Info(string message, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
        Write(LogLevel.Info, message, member, file, line);
    }

    public static void Warn(string message, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
        Write(LogLevel.Warn, message, member, file, line);
    }

    public static void Error(string message, [CallerMemberName] string member = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
        Write(LogLevel.Error, message, member, file, line);
    }

    private static void Write(LogLevel level, string message, string member, string file, int line) {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
        string source = Path.GetFileNameWithoutExtension(file);
        Console.WriteLine($"[{timestamp}] ({level.Name}) {source}.{member}:{line} : {message}");
    }

    public readonly struct LogLevel {
        public static readonly LogLevel Trace = new("Trace", 0);
        public static readonly LogLevel Debug = new("Debug", 1);
        public static readonly LogLevel Info = new("Info ", 2);
        public static readonly LogLevel Warn = new("Warn ", 3);
        public static readonly LogLevel Error = new("Error", 4);

        public string Name { get; private init; }
        public int Value { get; private init; }

        private LogLevel(string name, int value) {
            Name = name;
            Value = value;
        }
    }
}