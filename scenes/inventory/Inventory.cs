using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Array = System.Array;
using static Constants;

[Tool]
public sealed class Inventory : Area2D
{
    private int width = 3;
    private int height = 3;

    [Export]
    public int Width
    {
        get => width;
        set
        {
            width = value;
            applyDimensionChange();
        }
    }

    [Export]
    public int Height
    {
        get => height;
        set
        {
            height = value;
            applyDimensionChange();
        }
    }

    [Export] private PackedScene itemSlotScene = null!;

    private Item?[] itemGrid = Array.Empty<Item?>();
    private List<Item> heldItems = new();
    private bool ready;

    public Item? this[Coord coord]
    {
        get => this[coord.X, coord.Y];
        set => this[coord.X, coord.Y] = value;
    }

    public Item? this[int x, int y]
    {
        get => itemGrid[toIndex(x, y)];
        set => itemGrid[toIndex(x, y)] = value;
    }

    private int toIndex(int x, int y)
    {
        if (x < 0 || x >= width)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }
        if (y < 0 || y >= height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return y * width + x;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ready = true;
        applyDimensionChange();
    }

    private void applyDimensionChange()
    {
        if (!ready) return;
        if (itemGrid.Length > 0)
        {
            GetTree().CallGroup("slots", "queue_free");
        }
        itemGrid = new Item?[width * height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var slot = itemSlotScene.Instance<Node2D>();
                slot.Position = new Vector2((x + 0.5f) * ItemSlotSize, (y + 0.5f) * ItemSlotSize);
                AddChild(slot);
            }
        }

        if (GetNode<CollisionShape2D>("BoundingBox") is { } boundingBox)
        {
            var rect = (RectangleShape2D) boundingBox.Shape;
            rect.Extents = 0.5f * ItemSlotSize * new Vector2(width, height);
            boundingBox.Position = rect.Extents;
        }
    }

    public InventoryFitResult FitItem(Vector2 pos, ItemLibrary.Properties properties)
    {
        var offset = 0.5f * ItemSlotSize * new Vector2(properties.Width - 1, properties.Height - 1);
        var d = pos - Position - offset;
        var x = (int) (d.x / ItemSlotSize);
        var y = (int) (d.y / ItemSlotSize);
        if (x < 0 || x >= width - properties.Width + 1 || y < 0 || y > width - properties.Height + 1)
        {
            return InventoryFitResult.OutOfGrid;
        }
        var slotPos = new Vector2((x + 0.5f) * ItemSlotSize, (y + 0.5f) * ItemSlotSize);
        var tiles = new InventoryFitResultTile[properties.Width * properties.Height];
        for (var j = 0; j < properties.Height; j++)
        {
            for (var i = 0; i < properties.Width; i++)
            {
                tiles[j * properties.Width + i] =
                    new InventoryFitResultTile(new Coord(x + i, y + j), this[x + i, y + j] is null);
            }
        }
        return new InventoryFitResult(
            tiles.All(t => t.IsValid) ? ResultType.Valid : ResultType.Overlap, Position + slotPos + offset, tiles);
    }
}
