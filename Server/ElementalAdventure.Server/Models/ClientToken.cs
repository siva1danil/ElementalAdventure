namespace ElementalAdventure.Server.Models;

public readonly struct ClientToken(string provider, string token, long uid) {
    public readonly string Provider = provider;
    public readonly string Token = token.Length == 36 ? token : throw new ArgumentException("Token must be 36 characters long", nameof(token));
    public readonly long Uid = uid;
}