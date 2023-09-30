using System;
using Godot;
using Array = System.Array;

[Tool]
public sealed class Inventory : Node
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

    [Export] private Item?[] items = Array.Empty<Item?>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        applyDimensionChange();
    }

    private void applyDimensionChange()
    {
        GD.Print("Apply dimension change");
        items = new Item?[width * height];
        GetTree().CallGroup("slots", "queue_free");
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var slot = itemSlotScene.Instance<Node2D>();
                slot.Position = new Vector2((x + 0.5f) * 12, (y + 0.5f) * 12);
                AddChild(slot);
            }
        }
    }

    public Item? this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= width)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }
            if (y < 0 || y >= height)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            return items[y * width + x];
        }
    }
}
