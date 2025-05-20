using System.Diagnostics;

namespace ElementalAdventure.Common.Assets;

public class AssetManager : IDisposable {
    private readonly Dictionary<Type, Dictionary<AssetID, object>> _assets;

    public AssetManager() {
        _assets = [];
    }

    public void Add<T>(AssetID key, T asset) where T : notnull {
        Debug.WriteLine($"Adding asset of type {typeof(T)} with key {key}.");
        Type type = typeof(T);
        if (!_assets.ContainsKey(type)) _assets[type] = [];
        else if (_assets[type].ContainsKey(key)) throw new ArgumentException($"Asset of type {type} with key {key} already exists.");
        _assets[type][key] = asset;
    }

    public T Get<T>(AssetID key) where T : notnull {
        Type type = typeof(T);
        if (!_assets.ContainsKey(type)) throw new ArgumentException($"There are no assets of type {type}.");
        else if (!_assets[type].ContainsKey(key)) throw new ArgumentException($"There is no asset of type {type} with key {key}.");
        return (T)_assets[type][key];
    }

    public bool TryGet<T>(AssetID key, out T? asset) where T : notnull {
        Type type = typeof(T);
        asset = default;
        if (!_assets.ContainsKey(type)) return false;
        else if (!_assets[type].ContainsKey(key)) return false;
        asset = (T)_assets[type][key];
        return true;
    }

    public void Dispose() {
        foreach (KeyValuePair<Type, Dictionary<AssetID, object>> entry in _assets) {
            foreach (KeyValuePair<AssetID, object> asset in entry.Value) {
                if (asset.Value is IDisposable disposable) {
                    Debug.WriteLine($"Disposing asset of type {entry.Key} with key {asset.Key}.");
                    disposable.Dispose();
                }
            }
            entry.Value.Clear();
        }
        _assets.Clear();
        GC.SuppressFinalize(this);
    }
}