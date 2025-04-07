using OpenTK.Graphics.OpenGL4;

using StbImageSharp;

namespace ElementalAdventure.Client.OpenGL;

public class Texture2D : IDisposable {
    private readonly int _id;
    private readonly int _width, _height;

    public int Id => _id;
    public int Width => _width;
    public int Height => _height;

    public Texture2D(byte[] data) {
        ImageResult image = ImageResult.FromMemory(data, ColorComponents.RedGreenBlueAlpha);
        (_width, _height) = (image.Width, image.Height);

        _id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _id);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Dispose() {
        GL.DeleteTexture(_id);
        GC.SuppressFinalize(this);
    }
}
