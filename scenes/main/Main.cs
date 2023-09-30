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

    private void onRoomExited()
    {
        dungeonTraverser.MoveForward();
        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }
}
