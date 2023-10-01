using Godot;
using System.Linq;

public sealed class Main : Node
{
    private DungeonTraverser dungeonTraverser = null!;

    public override async void _Ready()
    {
        GD.Randomize();
        var dungeon = Dungeon.Make();

        var dialogue = GetNode<Dialogue>("Dialogue");
        var inventory = GetNode<Inventory>("Inventory");
        inventory.Visible = false;
        inventory.Populate(dungeon.StartingItems);
        
        dialogue.Visible = true;
        ;
        await dialogue
            .DisplayDialog(dungeon.Monologue.Select(x => new Sentence(Portrait.Player, x)).ToList());

        dialogue.Visible = false;
        
        dungeonTraverser = new DungeonTraverser(dungeon);

        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }

    private void onRoomExited()
    {
        dungeonTraverser.MoveForward();
        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }
}
