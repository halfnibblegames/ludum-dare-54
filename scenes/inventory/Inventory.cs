using System;
using Godot;
using Array = System.Array;
using static Constants;

[Tool]
public sealed class Inventory : Node2D
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
    private bool ready;

    public Item? this[int x, int y]
    {
        get => items[toIndex(x, y)];
        set => items[toIndex(x, y)] = value;
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
        if (items.Length > 0)
        {
            GetTree().CallGroup("slots", "queue_free");
        }
        items = new Item?[width * height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var slot = itemSlotScene.Instance<Node2D>();
                slot.Position = new Vector2((x + 0.5f) * ItemSlotSize, (y + 0.5f) * ItemSlotSize);
                if (!Engine.EditorHint)
                {
                    slot.Connect(
                        nameof(ItemSlot.ItemDropped),
                        this,
                        nameof(onItemDropped),
                        new Godot.Collections.Array(x, y));
                }
                AddChild(slot);
            }
        }
    }

    private void onItemDropped(Item? item, int x, int y)
    {
        if (this[x, y] is not null)
        {
            throw new InvalidOperationException();
        }

        this[x, y] = item;
    }
}
