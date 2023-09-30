using System;
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

        if (GetNode<CollisionShape2D>("BoundingBox") is { } boundingBox)
        {
            var rect = (RectangleShape2D) boundingBox.Shape;
            rect.Extents = 0.5f * ItemSlotSize * new Vector2(width, height);
            boundingBox.Position = rect.Extents;
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

    public InventoryFitResult FitItem(Vector2 pos, ItemLibrary.Properties properties)
    {
        var offset = 0.5f * ItemSlotSize * new Vector2(properties.Width - 1, properties.Height - 1);
        var d = pos - Position - offset;
        var x = (int) (d.x / ItemSlotSize);
        var y = (int) (d.y / ItemSlotSize);
        if (x < 0 || x >= width - properties.Width + 1 || y < 0 || y > width - properties.Height + 1)
        {
            return InventoryFitResult.NoFit;
        }
        var slotPos = new Vector2((x + 0.5f) * ItemSlotSize, (y + 0.5f) * ItemSlotSize);
        return new InventoryFitResult(true, Position + slotPos + offset);
    }
}

public record struct InventoryFitResult(bool Fits, Vector2 Position)
{
    public static InventoryFitResult NoFit => new(false, Vector2.Zero);
}
