using System.Linq;
using Godot;

public sealed class Main : Node
{
    private DungeonTraverser dungeonTraverser = null!;

    public override void _Ready()
    {
        GD.Randomize();
        var dungeon = Dungeon.Dungeons().First();
        GetNode<Inventory>("Inventory").Populate(dungeon.StartingItems);
        dungeonTraverser = new DungeonTraverser(dungeon);

        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }

    private void onRoomExited()
    {
        dungeonTraverser.MoveForward();
        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }
}
