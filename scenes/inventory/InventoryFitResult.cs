using System;
using Godot;

public readonly record struct InventoryFitResult(ResultType Result, Vector2 Position, InventoryFitResultTile[] Tiles)
{
    public static InventoryFitResult OutOfGrid =>
        new(ResultType.OutOfGrid, Vector2.Zero, Array.Empty<InventoryFitResultTile>());

    public void Commit(Inventory inventory, Item item)
    {
        if (!Result.CanCommit())
        {
            throw new InvalidOperationException();
        }

        item.GetParent().RemoveChild(item);
        inventory.AddChild(item);
        item.Position = Position - inventory.Position;

        foreach (var t in Tiles)
        {
            inventory[t.Tile] = item;
        }
    }
}

public readonly record struct InventoryFitResultTile(Coord Tile, bool IsValid);

public enum ResultType
{
    Valid,
    OutOfGrid,
    Overlap,
}

public static class ResultTypeExtensions
{
    public static bool CanPreview(this ResultType result) => result != ResultType.OutOfGrid;
    public static bool CanCommit(this ResultType result) => result == ResultType.Valid;
}
