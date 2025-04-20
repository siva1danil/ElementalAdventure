namespace ElementalAdventure.Client.Game.Data;

public record class TileType(string TextureAtlas, string Texture);

public record class EntityType(string TextureAtlas, string TextureIdleLeft, string TextureIdleRight, string TextureWalkLeft, string TextureWalkRight, float Speed);
