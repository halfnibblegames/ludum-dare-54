using Godot;

public sealed class Main : Node
{
    private DungeonTraverser dungeonTraverser = null!;

    [Export] private PackedScene hoveringItem = null!;

    public override void _Ready()
    {
        GD.Randomize();
        dungeonTraverser = new DungeonTraverser(
            new DungeonLayout(
                RoomLibrary.RandomSingleItem(),
                RoomLibrary.RandomSingleItem(),
                RoomLibrary.RandomSingleItem(),
                RoomLibrary.RandomSingleItem()
            ));

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

    private void onRoomExited()
    {
        dungeonTraverser.MoveForward();
        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }
}
