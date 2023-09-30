using System;
using Godot;

public sealed class Main : Node
{
    [Export] private PackedScene hoveringItem = null!;

    public override void _Ready()
    {
        GD.Randomize();
        spawnRandomHoveringItem();
    }

    private void onItemPlaced(HoveringItem item)
    {
        RemoveChild(item);
        spawnRandomHoveringItem();
    }

    private void spawnRandomHoveringItem()
    {
        var types = Enum.GetValues(typeof(ItemLibrary.ItemType));
        var randomType = (ItemLibrary.ItemType) types.GetValue(GD.Randi() % types.Length);
        spawnHoveringItem(randomType);
    }

    private void spawnHoveringItem(ItemLibrary.ItemType type)
    {
        var newItem = hoveringItem.Instance<HoveringItem>();
        newItem.SetType(type);
        newItem.Connect(nameof(HoveringItem.ItemPlaced), this, nameof(onItemPlaced));
        AddChild(newItem);
    }
}
