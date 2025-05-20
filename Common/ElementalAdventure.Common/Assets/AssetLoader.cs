namespace ElementalAdventure.Common.Assets;

public class AssetLoader {
    private readonly string _path;

    public AssetLoader(string path) {
        _path = path;
    }

    public string LoadText(string path) => File.ReadAllText(Path.Combine(_path, path));
    public byte[] LoadBinary(string path) => File.ReadAllBytes(Path.Combine(_path, path));
}