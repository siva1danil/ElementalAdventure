using OpenTK.Graphics.OpenGL4;

using StbImageSharp;

namespace ElementalAdventure.Client.Core.Resources.OpenGL;

public class Texture2D : IDisposable {
    private readonly int _id;
    private readonly int _width, _height;

    public int Id => _id;
    public int Width => _width;
    public int Height => _height;

    public Texture2D(byte[] data, bool interpolate = false) {
        ImageResult image = ImageResult.FromMemory(data, ColorComponents.RedGreenBlueAlpha);
        (_width, _height) = (image.Width, image.Height);

        _id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _id);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, interpolate ? (int)TextureMinFilter.Linear : (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, interpolate ? (int)TextureMagFilter.Linear : (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public Texture2D(byte[] rgba, int width, int height, bool interpolate = false) {
        (_width, _height) = (width, height);
        if (rgba.Length != _width * _height * 4)
            throw new ArgumentException($"Invalid RGBA data length: {rgba.Length}, expected: {_width * _height * 4}");

        _id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _id);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, rgba);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, interpolate ? (int)TextureMinFilter.Linear : (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, interpolate ? (int)TextureMagFilter.Linear : (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Dispose() {
        GL.DeleteTexture(_id);
        GC.SuppressFinalize(this);
    }
}
