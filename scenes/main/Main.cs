using Godot;

public sealed class Main : Node
{
    [Export] private PackedScene hoveringItem = null!;

    public override void _Ready()
    {
        GD.Randomize();
        spawnHoveringItem();
    }

    private void onItemPlaced(HoveringItem item)
    {
        RemoveChild(item);
        spawnHoveringItem();
    }

    private void spawnHoveringItem()
    {
        var newItem = hoveringItem.Instance<HoveringItem>();
        newItem.RandomizeItem();
        newItem.Connect(nameof(HoveringItem.ItemPlaced), this, nameof(onItemPlaced));
        AddChild(newItem);
    }
}
