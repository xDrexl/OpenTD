using Godot;

namespace OpenTD.Presentation;

public sealed partial class MapView : Node2D
{
    private const int TileColumns = 18;
    private const int TileRows = 11;

    public override void _Ready()
    {
        var terrain = GetNode<TileMapLayer>("Terrain");
        for (var y = 0; y < TileRows; y++)
        {
            for (var x = 0; x < TileColumns; x++)
            {
                var atlasX = (x * 3 + y * 5) % 3;
                terrain.SetCell(new Vector2I(x, y), 0, new Vector2I(atlasX, 0));
            }
        }
    }
}
