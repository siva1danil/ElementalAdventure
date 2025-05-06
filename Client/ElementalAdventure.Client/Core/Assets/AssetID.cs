using System.Diagnostics;

namespace ElementalAdventure.Client.Core.Assets;

public readonly struct AssetID(int value) : IEquatable<AssetID> {
    public static readonly Dictionary<int, string> DebugMappings = [];
    public static readonly AssetID None = new(0);

    public readonly int Value { get; } = value;

    public AssetID(object value) : this(value.GetHashCode()) {
        if (Debugger.IsAttached && !DebugMappings.ContainsKey(Value))
            DebugMappings[Value] = value.ToString() ?? "null";
    }
    public bool Equals(AssetID other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is AssetID other && Equals(other);
    public override int GetHashCode() => Value;
    public override string ToString() => $"AssetID{{{DebugMappings.GetValueOrDefault(Value, Value.ToString())}}}";
    public static bool operator ==(AssetID a, AssetID b) => a.Equals(b);
    public static bool operator !=(AssetID a, AssetID b) => !a.Equals(b);
}