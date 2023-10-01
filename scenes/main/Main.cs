using Godot;
using System.Linq;
using System.Threading.Tasks;

public sealed class Main : Node
{
    private const float shakeIntensityPerHealthLost = 0.15f;
    private const float baseShakeIntensity = 0.4f;

    private DungeonTraverser dungeonTraverser = null!;
    private AnimationPlayer animations = null!;
    private ShakeCamera shakeCamera = null!;

    public override async void _Ready()
    {
        GD.Randomize();
        var dungeon = Dungeon.Make();

        var dialogue = GetNode<Dialogue>("Dialogue");
        animations = GetNode<AnimationPlayer>("AnimationPlayer");
        shakeCamera = GetNode<ShakeCamera>("ShakeCamera");

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

        var room = GetNode<Room>("Room");
        room.Player = GetNode<Player>("Player");
        room.FillRoom(dungeonTraverser.CurrentRoom);
        await ToggleInventory(forceState: true);
    }

    private void onRoomExited()
    {
        dungeonTraverser.MoveForward();
        GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
    }

    private void onPlayerHealthChanged(int newHealth, int maxHealth, int healthChange)
    {
        GetNode<TopHud>("TopHud").UpdateHealth(newHealth, maxHealth);
        if (healthChange < 0)
        {
            var shakeIntensity = baseShakeIntensity - healthChange * shakeIntensityPerHealthLost;
            shakeCamera.Shake(shakeIntensity);
        }
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
