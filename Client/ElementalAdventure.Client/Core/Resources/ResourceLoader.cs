namespace ElementalAdventure.Client.Core.Resources;

public class ResourceLoader {
    private readonly string _root;

    public ResourceLoader(string root) {
        _root = root;
    }

    public string LoadText(string path) => File.ReadAllText(Path.Combine(_root, path));
    public byte[] LoadBinary(string path) => File.ReadAllBytes(Path.Combine(_root, path));
}