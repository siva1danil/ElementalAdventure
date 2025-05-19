using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Components.Utils;

public class Camera {
    private Vector2 _center;
    private Vector2 _targetWorldSize;
    private Vector2 _screenSize;
    private bool _flipY;

    public Vector2 Center { set => _center = value; get => _center; }
    public Vector2 TargetWorldSize { set => _targetWorldSize = value; get => _targetWorldSize; }
    public Vector2 ScreenSize { set => _screenSize = value; get => _screenSize; }
    public bool FlipY { set => _flipY = value; get => _flipY; }


    public Camera(Vector2 center, Vector2 targetWorldSize, Vector2 screenSize, bool flipY = false) {
        _center = center;
        _targetWorldSize = targetWorldSize;
        _screenSize = screenSize;
        _flipY = flipY;
    }

    public Matrix4 GetViewMatrix() {
        float scaleX = _targetWorldSize.X / _screenSize.X, scaleY = _targetWorldSize.Y / _screenSize.Y;
        float scale = scaleX > scaleY ? scaleX : scaleY;
        return Matrix4.CreateOrthographicOffCenter(_center.X - _screenSize.X * scale / 2.0f, _center.X + _screenSize.X * scale / 2.0f, _flipY ? _center.Y + _screenSize.Y * scale / 2.0f : _center.Y - _screenSize.Y * scale / 2.0f, _flipY ? _center.Y - _screenSize.Y * scale / 2.0f : _center.Y + _screenSize.Y * scale / 2.0f, -1.0f, 1.0f);
    }
}