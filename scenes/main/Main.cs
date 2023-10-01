using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public sealed class Main : Node
{
    private DungeonTraverser dungeonTraverser = null!;

    public override async void _Ready()
    {
        GD.Randomize();
        var dungeon = Dungeon.Make();

        var dialogue = GetNode<Dialogue>("Dialogue");
        dialogue.Visible = true;

        var sceneAnimationTasks = new[]
        {
            ToggleInventory(forceState: false),
            dialogue
                .DisplayDialog(dungeon.Monologue.Select(x => new Sentence(Portrait.Player, x)).ToList())
        };

        var inventory = GetNode<Inventory>("Inventory");
        inventory.Populate(dungeon.StartingItems);

        await Task.WhenAll(sceneAnimationTasks);
        dialogue.Visible = false;

        dungeonTraverser = new DungeonTraverser(dungeon);

        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
        await ToggleInventory(forceState: true);
    }

    private void onRoomExited()
    {
        dungeonTraverser.MoveForward();
        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }

    private bool currentInventoryVisibility = true;
    private async Task ToggleInventory(bool? forceState = null)
    {
        var inventory = GetNode<Inventory>("Inventory");
        var shouldBeVisible = forceState ?? inventory.Position.x < 0;
        if (shouldBeVisible == currentInventoryVisibility)
            return;

        var targetX = shouldBeVisible ? 5 : -100;
        var step = shouldBeVisible ? 1 : -1;
        do
        {
            // TODO: Make this animation less stupid
            inventory.Position = inventory.Position with { x = inventory.Position.x + step };
            await Task.Delay(1);
        } while (Math.Abs(inventory.Position.x - targetX) > 0.01);

        // TODO(will): Wait for player input
        await Task.Delay(1000);
    }
}
