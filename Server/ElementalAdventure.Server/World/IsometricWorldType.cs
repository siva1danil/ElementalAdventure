using ElementalAdventure.Common.Assets;

namespace ElementalAdventure.Server.World;

public class IsometricWorldType : IWorldType {
    private const int LayerFloor = 0, LayerMidground = 1, LayerDoors = 2, LayerWall = 3, LayerWallTop = 1;

    public int RoomWidth { get; private init; }
    public int RoomHeight { get; private init; }
    public int LayerCount => 4;
    public int MidgroundLayer => LayerMidground;

    public AssetID Floor { get; private init; }
    public AssetID WallBottom { get; private init; }
    public AssetID WallTop { get; private init; }
    public AssetID WallBottomConnected { get; private init; }
    public AssetID WallTopConnected { get; private init; }
    public AssetID WallLeft { get; private init; }
    public AssetID WallRight { get; private init; }
    public AssetID WallRightTop { get; private init; }
    public AssetID WallRightBottom { get; private init; }
    public AssetID WallLeftTop { get; private init; }
    public AssetID WallLeftBottom { get; private init; }
    public AssetID WallSideConnected { get; private init; }
    public AssetID WallCrossLbRb { get; private init; }
    public AssetID WallCrossLtLb { get; private init; }
    public AssetID WallCrossLtRt { get; private init; }
    public AssetID WallCrossRtRb { get; private init; }
    public AssetID WallCrossLtLbRb { get; private init; }
    public AssetID WallCrossLtRtRb { get; private init; }
    public AssetID WallCrossLbRtRb { get; private init; }
    public AssetID WallCrossLtLbRt { get; private init; }
    public AssetID WallCrossLtRtLbRb { get; private init; }
    public AssetID WallDoorwayHorizontalTop { get; private init; }
    public AssetID WallDoorwayHorizontalBottom { get; private init; }
    public AssetID WallDoorwayVerticalTop { get; private init; }
    public AssetID WallDoorwayVerticalBottom { get; private init; }
    public AssetID DoorHorizontalClosedTop { get; private init; }
    public AssetID DoorHorizontalClosedBottom { get; private init; }
    public AssetID DoorVerticalClosedTop { get; private init; }
    public AssetID DoorVerticalClosedBottom { get; private init; }

    public IsometricWorldType(
        int roomWidth,
        int roomHeight,
        AssetID floor,
        AssetID wallBottom,
        AssetID wallTop,
        AssetID wallBottomConnected,
        AssetID wallTopConnected,
        AssetID wallLeft,
        AssetID wallRight,
        AssetID wallRightTop,
        AssetID wallRightBottom,
        AssetID wallLeftTop,
        AssetID wallLeftBottom,
        AssetID wallSideConnected,
        AssetID wallCrossLbRb,
        AssetID wallCrossLtLb,
        AssetID wallCrossLtRt,
        AssetID wallCrossRtRb,
        AssetID wallCrossLtLbRb,
        AssetID wallCrossLtRtRb,
        AssetID wallCrossLbRtRb,
        AssetID wallCrossLtLbRt,
        AssetID wallCrossLtRtLbRb,
        AssetID wallDoorwayHorizontalTop,
        AssetID wallDoorwayHorizontalBottom,
        AssetID wallDoorwayVerticalTop,
        AssetID wallDoorwayVerticalBottom,
        AssetID doorHorizontalClosedTop,
        AssetID doorHorizontalClosedBottom,
        AssetID doorVerticalClosedTop,
        AssetID doorVerticalClosedBottom
    ) {
        RoomWidth = roomWidth;
        RoomHeight = roomHeight;
        Floor = floor;
        WallBottom = wallBottom;
        WallTop = wallTop;
        WallBottomConnected = wallBottomConnected;
        WallTopConnected = wallTopConnected;
        WallLeft = wallLeft;
        WallRight = wallRight;
        WallRightTop = wallRightTop;
        WallRightBottom = wallRightBottom;
        WallLeftTop = wallLeftTop;
        WallLeftBottom = wallLeftBottom;
        WallSideConnected = wallSideConnected;
        WallCrossLbRb = wallCrossLbRb;
        WallCrossLtLb = wallCrossLtLb;
        WallCrossLtRt = wallCrossLtRt;
        WallCrossRtRb = wallCrossRtRb;
        WallCrossLtLbRb = wallCrossLtLbRb;
        WallCrossLtRtRb = wallCrossLtRtRb;
        WallCrossLbRtRb = wallCrossLbRtRb;
        WallCrossLtLbRt = wallCrossLtLbRt;
        WallCrossLbRb = wallCrossLbRb;
        WallCrossLtLbRt = wallCrossLtLbRt;
        WallCrossLtRtLbRb = wallCrossLtRtLbRb;
        WallDoorwayHorizontalTop = wallDoorwayHorizontalTop;
        WallDoorwayHorizontalBottom = wallDoorwayHorizontalBottom;
        WallDoorwayVerticalTop = wallDoorwayVerticalTop;
        WallDoorwayVerticalBottom = wallDoorwayVerticalBottom;
        DoorHorizontalClosedTop = doorHorizontalClosedTop;
        DoorHorizontalClosedBottom = doorHorizontalClosedBottom;
        DoorVerticalClosedTop = doorVerticalClosedTop;
        DoorVerticalClosedBottom = doorVerticalClosedBottom;
    }

    public void MapMaskToLayers(AssetID[,,] layer, Generator.TileMask[,] mask) {
        if (layer.GetLength(0) != LayerCount)
            throw new ArgumentException($"Layer count mismatch: expected {LayerCount}, got {layer.GetLength(0)}");
        if (layer.GetLength(1) != mask.GetLength(0) || layer.GetLength(2) != mask.GetLength(1))
            throw new ArgumentException($"Mask size mismatch: expected {mask.GetLength(0)}x{mask.GetLength(1)}, got {layer.GetLength(1)}x{layer.GetLength(2)}");

        for (int y = 1; y < mask.GetLength(0) - 1; y++) {
            for (int x = 1; x < mask.GetLength(1) - 1; x++) {
                // TileMask.Floor => Floor
                if (mask[y, x] == Generator.TileMask.Floor)
                    layer[LayerFloor, y, x] = Floor;

                // TileMask.Wall => WallBottom, WallTop
                if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.Floor) {
                    layer[LayerWall, y - 1, x] = WallBottom;
                    layer[LayerFloor, y - 1, x] = AssetID.None; // TODO: half tile
                }
                if (mask[y, x] == Generator.TileMask.Wall && mask[y + 1, x] == Generator.TileMask.Floor)
                    layer[LayerWallTop, y, x] = WallTop;
                if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.Floor && mask[y + 1, x] == Generator.TileMask.Floor) {
                    layer[LayerWallTop, y, x] = WallBottomConnected;
                    layer[LayerWall, y - 1, x] = WallTopConnected;
                }

                // TileMask.Wall => WallLeft, WallRight, WallSideConnected
                if (mask[y, x] == Generator.TileMask.Wall && mask[y, x + 1] == Generator.TileMask.Floor)
                    layer[LayerWall, y, x] = WallLeft;
                if (mask[y, x] == Generator.TileMask.Wall && mask[y, x - 1] == Generator.TileMask.Floor)
                    layer[LayerWall, y, x] = WallRight;
                if (mask[y, x] == Generator.TileMask.Wall && mask[y, x + 1] == Generator.TileMask.Floor && mask[y, x - 1] == Generator.TileMask.Floor)
                    layer[LayerWall, y, x] = WallSideConnected;

                // TileMask.Wall => WallRightTop, WallLeftTop, WallRightBottom, WallLeftBottom
                if (mask[y, x] == Generator.TileMask.Wall && mask[y + 1, x] == Generator.TileMask.Wall && mask[y, x - 1] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.None && mask[y, x + 1] == Generator.TileMask.None)
                    layer[LayerWall, y, x] = WallRightTop;
                if (mask[y, x] == Generator.TileMask.Wall && mask[y + 1, x] == Generator.TileMask.Wall && mask[y, x + 1] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.None && mask[y, x - 1] == Generator.TileMask.None)
                    layer[LayerWall, y, x] = WallLeftTop;
                if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.Wall && mask[y, x - 1] == Generator.TileMask.Wall && mask[y + 1, x] == Generator.TileMask.None && mask[y, x + 1] == Generator.TileMask.None)
                    layer[LayerWall, y - 1, x] = WallRightBottom;
                if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.Wall && mask[y, x + 1] == Generator.TileMask.Wall && mask[y + 1, x] == Generator.TileMask.None && mask[y, x - 1] == Generator.TileMask.None)
                    layer[LayerWall, y - 1, x] = WallLeftBottom;

                // TileMask.Wall => WallCrossLtLb, WallCrossLtRt, WallCrossRtRb, WallCrossLbRb
                if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.Wall && mask[y + 1, x] == Generator.TileMask.Wall && mask[y, x - 1] == Generator.TileMask.Wall && mask[y, x + 1] == Generator.TileMask.None) {
                    layer[LayerWall, y - 1, x] = WallCrossLtLb;
                    layer[LayerWall, y, x] = WallRight;
                }
                if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.Wall && mask[y + 1, x] == Generator.TileMask.Wall && mask[y, x - 1] == Generator.TileMask.None && mask[y, x + 1] == Generator.TileMask.Wall) {
                    layer[LayerWall, y - 1, x] = WallCrossRtRb;
                    layer[LayerWall, y, x] = WallLeft;
                }
                if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.Wall && mask[y + 1, x] == Generator.TileMask.None && mask[y, x - 1] == Generator.TileMask.Wall && mask[y, x + 1] == Generator.TileMask.Wall)
                    layer[LayerWall, y - 1, x] = WallCrossLtRt;
                if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.None && mask[y + 1, x] == Generator.TileMask.Wall && mask[y, x - 1] == Generator.TileMask.Wall && mask[y, x + 1] == Generator.TileMask.Wall)
                    layer[LayerWall, y, x] = WallCrossLbRb;

                // TileMask.Wall => WallCrossLtLbRb, WallCrossLtRtRb, WallCrossLbRtRb, WallCrossLtLbRt
                if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.Wall && mask[y + 1, x] == Generator.TileMask.Wall && mask[y, x - 1] == Generator.TileMask.Wall && mask[y, x + 1] == Generator.TileMask.Wall) {
                    if (mask[y - 1, x - 1] == Generator.TileMask.Floor && mask[y - 1, x + 1] == Generator.TileMask.None && mask[y + 1, x - 1] == Generator.TileMask.Floor && mask[y + 1, x + 1] == Generator.TileMask.Floor) {
                        layer[LayerWall, y - 1, x] = WallCrossLtLbRb;
                        layer[LayerWall, y, x] = WallSideConnected;
                    }
                    if (mask[y - 1, x - 1] == Generator.TileMask.Floor && mask[y - 1, x + 1] == Generator.TileMask.Floor && mask[y + 1, x - 1] == Generator.TileMask.None && mask[y + 1, x + 1] == Generator.TileMask.Floor) {
                        layer[LayerWall, y - 1, x] = WallCrossLtRtRb;
                        layer[LayerWall, y, x] = WallLeft;
                    }
                    if (mask[y - 1, x - 1] == Generator.TileMask.None && mask[y - 1, x + 1] == Generator.TileMask.Floor && mask[y + 1, x - 1] == Generator.TileMask.Floor && mask[y + 1, x + 1] == Generator.TileMask.Floor) {
                        layer[LayerWall, y - 1, x] = WallCrossLbRtRb;
                        layer[LayerWall, y, x] = WallSideConnected;
                    }
                    if (mask[y - 1, x - 1] == Generator.TileMask.Floor && mask[y - 1, x + 1] == Generator.TileMask.Floor && mask[y + 1, x - 1] == Generator.TileMask.Floor && mask[y + 1, x + 1] == Generator.TileMask.None) {
                        layer[LayerWall, y - 1, x] = WallCrossLtLbRt;
                        layer[LayerWall, y, x] = WallRight;
                    }
                }

                // TileMask.Wall => WallCross
                if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x] == Generator.TileMask.Wall && mask[y + 1, x] == Generator.TileMask.Wall && mask[y, x - 1] == Generator.TileMask.Wall && mask[y, x + 1] == Generator.TileMask.Wall) {
                    if (mask[y, x] == Generator.TileMask.Wall && mask[y - 1, x - 1] == Generator.TileMask.Floor && mask[y - 1, x + 1] == Generator.TileMask.Floor && mask[y + 1, x - 1] == Generator.TileMask.Floor && mask[y + 1, x + 1] == Generator.TileMask.Floor) {
                        layer[LayerWall, y - 1, x] = WallCrossLtRtLbRb;
                        layer[LayerWall, y, x] = WallSideConnected;
                    }
                }

                // TileMask.Door => WallDoorwayHorizontalTop, WallDoorwayHorizontalBottom, WallDoorwayVerticalTop, WallDoorwayVerticalBottom
                if (mask[y, x] == Generator.TileMask.Door && mask[y - 1, x] == Generator.TileMask.Floor && mask[y + 1, x] == Generator.TileMask.Floor) {
                    layer[LayerFloor, y, x] = Floor;
                    layer[LayerWall, y - 1, x] = WallDoorwayHorizontalTop;
                    layer[LayerWallTop, y, x] = WallDoorwayHorizontalBottom;
                }
                if (mask[y, x] == Generator.TileMask.Door && mask[y, x - 1] == Generator.TileMask.Floor && mask[y, x + 1] == Generator.TileMask.Floor) {
                    layer[LayerFloor, y - 1, x] = Floor;
                    layer[LayerFloor, y, x] = Floor;
                    layer[LayerWall, y - 1, x] = AssetID.None;
                    layer[LayerWall, y, x] = WallDoorwayVerticalBottom;
                    layer[LayerWallTop, y - 1, x] = WallDoorwayVerticalTop;
                }
            }
        }
    }
}