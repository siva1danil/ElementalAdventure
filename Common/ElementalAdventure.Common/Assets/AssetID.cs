using System.Diagnostics;

namespace ElementalAdventure.Common.Assets;

public readonly struct AssetID(int value) : IEquatable<AssetID> {
    public static readonly Dictionary<int, string> DebugMappings = [];
    public static readonly AssetID None = new(0);

    public readonly int Value { get; } = value;
    public readonly string DebugName => DebugMappings.GetValueOrDefault(Value, Value.ToString());

    public AssetID(string value) : this(FNV1a(value)) {
        AddDebugMapping(DebugMappings, Value, value);
    }
    public bool Equals(AssetID other) => Value == other.Value;

    private static int FNV1a(string input) {
        const uint fnvPrime = 16777619;
        const uint fnvOffsetBasis = 2166136261;

        uint hash = fnvOffsetBasis;
        for (int i = 0; i < input.Length; i++) {
            hash ^= input[i];
            hash *= fnvPrime;
        }

        return (int)hash;
    }

    [Conditional("DEBUG")]
    private static void AddDebugMapping(Dictionary<int, string> mappings, int id, string name) {
        if (!mappings.ContainsKey(id))
            mappings[id] = name;
    }

    public override bool Equals(object? obj) => obj is AssetID other && Equals(other);
    public override int GetHashCode() => Value;
    public override string ToString() => $"AssetID[{DebugMappings.GetValueOrDefault(Value, Value.ToString())}]";
    public static bool operator ==(AssetID a, AssetID b) => a.Equals(b);
    public static bool operator !=(AssetID a, AssetID b) => !a.Equals(b);
}