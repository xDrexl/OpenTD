using Godot;
using OpenTD.Simulation.Configuration;

namespace OpenTD.Presentation;

public sealed partial class MapView : Node2D
{
    private const int TileColumns = 18;
    private const int TileRows = 11;

    public void Initialize(MapConfiguration configuration)
    {
        DrawTerrain(configuration.TerrainSeed);
        var points = new Vector2[configuration.Path.Count];
        for (var index = 0; index < configuration.Path.Count; index++)
        {
            points[index] = new Vector2(
                configuration.Path[index].X,
                configuration.Path[index].Y);
        }

        GetNode<Line2D>("PathBorder").Points = points;
        GetNode<Line2D>("Path").Points = points;
        GetNode<Sprite2D>("Spawn").Position = points[0];
        GetNode<Sprite2D>("Base").Position = points[^1];
        DrawPathMarkers(points);
        DrawObstacles(configuration);
    }

    private void DrawTerrain(int terrainSeed)
    {
        var terrain = GetNode<TileMapLayer>("Terrain");
        for (var y = 0; y < TileRows; y++)
        {
            for (var x = 0; x < TileColumns; x++)
            {
                var atlasX = PositiveModulo(terrainSeed + x * 31 + y * 47, 3);
                terrain.SetCell(new Vector2I(x, y), 0, new Vector2I(atlasX, 0));
            }
        }
    }

    private void DrawPathMarkers(Vector2[] points)
    {
        var parent = GetNode<Node2D>("PathMarkers");
        for (var index = 1; index < points.Length - 1; index++)
        {
            var marker = new Sprite2D
            {
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                Texture = GD.Load<Texture2D>("res://assets/generated/path_marker.svg"),
                Position = points[index],
            };
            parent.AddChild(marker);
        }
    }

    private void DrawObstacles(MapConfiguration configuration)
    {
        var parent = GetNode<Node2D>("Obstacles");
        if (configuration.Obstacles is null)
        {
            return;
        }

        foreach (var obstacle in configuration.Obstacles)
        {
            var size = obstacle.Maximum - obstacle.Minimum;
            var sprite = new Sprite2D
            {
                TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
                Texture = GD.Load<Texture2D>("res://assets/generated/obstacle.svg"),
                Position = new Vector2(
                    (obstacle.Minimum.X + obstacle.Maximum.X) / 2,
                    (obstacle.Minimum.Y + obstacle.Maximum.Y) / 2),
                Scale = new Vector2(size.X / 72, size.Y / 72),
            };
            parent.AddChild(sprite);
        }
    }

    private static int PositiveModulo(int value, int divisor) =>
        (value % divisor + divisor) % divisor;
}
