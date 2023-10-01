using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    private Item?[] itemGrid = Array.Empty<Item?>();
    private List<Item> heldItems = new();

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
        applyDimensionChange();
    }

    private void applyDimensionChange()
    {
        itemGrid = new Item?[width * height];
        if (GetNodeOrNull<CollisionShape2D>("BoundingBox") is { } boundingBox)
        {
            var rect = (RectangleShape2D) boundingBox.Shape;
            rect.Extents = 0.5f * ItemSlotSize * new Vector2(width, height);
            boundingBox.Position = rect.Extents;
        }
    }

    public InventoryFitResult FitItem(Vector2 pos, ItemLibrary.Properties properties)
    {
        var offset = 0.5f * ItemSlotSize * new Vector2(properties.Width - 1, properties.Height - 1);
        var searchPoint = pos - offset;
        if (!tryFindCoord(searchPoint, out var coord) ||
            width - coord.X < properties.Width ||
            height - coord.Y < properties.Height)
        {
            return InventoryFitResult.OutOfGrid;
        }
        var slotPos = toLocalPos(coord);
        var tiles = new InventoryFitResultTile[properties.Width * properties.Height];
        for (var j = 0; j < properties.Height; j++)
        {
            for (var i = 0; i < properties.Width; i++)
            {
                var c = new Coord(coord.X + i, coord.Y + j);
                tiles[j * properties.Width + i] = new InventoryFitResultTile(c, this[c] is null);
            }
        }
        return new InventoryFitResult(
            tiles.All(t => t.IsValid) ? ResultType.Valid : ResultType.Overlap,
            Position + slotPos + offset,
            tiles);
    }

    public bool TryFindItem(Vector2 globalPos, [NotNullWhen(true)] out Item? item)
    {
        if (!tryFindCoord(globalPos, out var coord))
        {
            item = default;
            return false;
        }

        item = this[coord];
        return item != null;
    }

    private bool tryFindCoord(Vector2 globalPos, out Coord coord)
    {
        var relativePos = globalPos - Position;

        var x = (int) (relativePos.x / ItemSlotSize);
        var y = (int) (relativePos.y / ItemSlotSize);

        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            coord = default;
            return false;
        }

        coord = new Coord(x, y);
        return true;
    }

    private Vector2 toLocalPos(Coord coord)
    {
        return new Vector2((coord.X + 0.5f) * ItemSlotSize, (coord.Y + 0.5f) * ItemSlotSize);
    }

    public void AddItem(Item item, IEnumerable<Coord> tiles, Vector2 globalPosition)
    {
        AddChild(item);
        item.GlobalPosition = globalPosition;

        heldItems.Add(item);
        foreach (var t in tiles)
        {
            this[t] = item;
        }

        item.Connect(nameof(Item.ItemDepleted), this, nameof(onItemDepleted), new Godot.Collections.Array(item));
    }

    private void onItemDepleted(Item item)
    {
        removeItem(item);
    }

    private void removeItem(Item item)
    {
        heldItems.Remove(item);
        for (var i = 0; i < itemGrid.Length; i++)
        {
            if (itemGrid[i] == item)
            {
                itemGrid[i] = null;
            }
        }
        item.QueueFree();
    }

    public void Populate(IEnumerable<ItemLibrary.ItemType> items)
    {
        var templates = GetNode<Templates>("/root/Templates");
        foreach (var type in items)
        {
            var item = templates.ItemScene.Instance<Item>();
            item.Type = type;
            if (!tryFindPosition(item.Properties, out var tiles, out var position))
            {
                throw new InvalidOperationException("Could not place requested items");
            }
            AddItem(item, tiles, position);
        }
    }

    private bool tryFindPosition(ItemLibrary.Properties properties, out IEnumerable<Coord> tiles, out Vector2 position)
    {
        for (var y = height - properties.Height; y >= 0; y--)
        {
            for (var x = width - properties.Width; x >= 0; x--)
            {
                var coords = new Coord[properties.Width * properties.Height];

                for (var j = 0; j < properties.Height; j++)
                {
                    for (var i = 0; i < properties.Width; i++)
                    {
                        coords[j * properties.Width + i] = new Coord(x + i, y + j);
                    }
                }

                if (coords.Select(c => this[c]).All(item => item is null))
                {
                    tiles = coords;
                    position = Position + toLocalPos(coords[0]) +
                        0.5f * ItemSlotSize * new Vector2(properties.Width - 1, properties.Height - 1);
                    return true;
                }
            }
        }

        tiles = Array.Empty<Coord>();
        position = default;

        return false;
    }
}
