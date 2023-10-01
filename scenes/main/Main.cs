using Godot;
using System.Linq;

public sealed class Main : Node
{
    private const float shakeIntensityPerHealthLost = 0.15f;
    private const float baseShakeIntensity = 0.4f;

    private DungeonTraverser dungeonTraverser = null!;
    private AnimationPlayer animations = null!;
    private ShakeCamera shakeCamera = null!;

    public override void _Ready()
    {
        GD.Randomize();
        var dungeon = Dungeon.Make();
        dungeonTraverser = new DungeonTraverser(dungeon);

        animations = GetNode<AnimationPlayer>("AnimationPlayer");
        shakeCamera = GetNode<ShakeCamera>("ShakeCamera");

        ToggleInventory(forceState: false, skipAnimation: true);

        var inventory = GetNode<Inventory>("Inventory");
        inventory.Populate(dungeon.StartingItems);

        var dialogue = GetNode<Dialogue>("Dialogue");
        dialogue
            .DisplayDialog(dungeon.Monologue.Select(x => new Sentence(Portrait.Player, x)).ToList());

        var room = GetNode<Room>("Room");
        room.Player = GetNode<Player>("Player");
    }

    private void onDialogueFinished()
    {
        var room = GetNode<Room>("Room");
        room.FillRoom(dungeonTraverser.CurrentRoom);
        ToggleInventory(forceState: true);
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
    private void ToggleInventory(bool? forceState = null, bool skipAnimation = false)
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

        if (skipAnimation)
        {
            animations.Advance(animations.CurrentAnimationLength);
        }
    }
}
