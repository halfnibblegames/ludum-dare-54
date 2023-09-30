using System.Linq;
using Godot;

public sealed class Main : Node
{
    private DungeonTraverser dungeonTraverser = null!;

    [Export] private PackedScene hoveringItem = null!;

    public override void _Ready()
    {
        GD.Randomize();
        dungeonTraverser = new DungeonTraverser(Dungeon.Dungeons().First());

        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }

    private void onItemPlaced(HoveringItem item)
    {
        RemoveChild(item);
        GetNode<Room>("Room").StepCompleted();
    }

    private void onItemDropped(ItemLibrary.ItemType type)
    {
        spawnHoveringItem(type);
    }

    private void spawnHoveringItem(ItemLibrary.ItemType type)
    {
        var newItem = hoveringItem.Instance<HoveringItem>();
        newItem.SetType(type);
        newItem.Connect(nameof(HoveringItem.ItemPlaced), this, nameof(onItemPlaced));
        AddChild(newItem);
    }

    private void onEncounterStarted()
    {

    }

    private void onRoomExited()
    {
        dungeonTraverser.MoveForward();
        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }
}
