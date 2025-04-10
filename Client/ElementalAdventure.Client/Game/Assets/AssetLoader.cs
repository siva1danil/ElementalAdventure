namespace ElementalAdventure.Client.Game.Assets;

public class AssetLoader {
    private readonly string _root;

    public AssetLoader(string root) {
        _root = root;
    }

    public string LoadText(string path) => File.ReadAllText(Path.Combine(_root, path));
    public byte[] LoadBinary(string path) => File.ReadAllBytes(Path.Combine(_root, path));
}