using Godot;
using System.Linq;
using System.Threading.Tasks;

public sealed class Main : Node
{
    private DungeonTraverser dungeonTraverser = null!;
    private AnimationPlayer animations = null!;

    public override async void _Ready()
    {
        GD.Randomize();
        var dungeon = Dungeon.Make();

        var dialogue = GetNode<Dialogue>("Dialogue");
        animations = GetNode<AnimationPlayer>("AnimationPlayer");

        var sceneAnimationTasks = new[]
        {
            ToggleInventory(forceState: false),
            dialogue
                .DisplayDialog(dungeon.Monologue.Select(x => new Sentence(Portrait.Player, x)).ToList())
        };

        var inventory = GetNode<Inventory>("Inventory");
        inventory.Populate(dungeon.StartingItems);

        await Task.WhenAll(sceneAnimationTasks);

        dungeonTraverser = new DungeonTraverser(dungeon);

        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
        await ToggleInventory(forceState: true);
    }

    private void onRoomExited()
    {
        dungeonTraverser.MoveForward();
        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }

    private void onPlayerHealthChanged(int newHealth, int maxHealth)
    {
        GetNode<TopHud>("TopHud").UpdateHealth(newHealth, maxHealth);
    }

    private bool currentInventoryVisibility = true;
    private async Task ToggleInventory(bool? forceState = null)
    {
        var inventory = GetNode<Inventory>("Inventory");
        var shouldBeVisible = forceState ?? inventory.Position.x < 0;
        if (shouldBeVisible == currentInventoryVisibility)
            return;
        currentInventoryVisibility = shouldBeVisible;

        if (shouldBeVisible)
        {
            animations.PlayBackwards("InventoryClose");
        }
        else
        {
            animations.Play("InventoryClose");
        }

        // TODO(will): Wait for player input
        await Task.Delay(50);
    }
}
