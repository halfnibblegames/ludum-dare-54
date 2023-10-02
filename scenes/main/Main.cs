using System.Linq;
using Godot;

public sealed class Main : Node
{
    private const float shakeIntensityPerHealthLost = 0.15f;
    private const float baseShakeIntensity = 0.4f;

    private DungeonTraverser dungeonTraverser = null!;
    private AnimationPlayer animations = null!;
    private ShakeCamera shakeCamera = null!;

    private bool gameOver;
    private int score;

    public override void _Ready()
    {
        GD.Randomize();
        var dungeon = Dungeon.Make();
        dungeonTraverser = new DungeonTraverser(dungeon);

        animations = GetNode<AnimationPlayer>("AnimationPlayer");
        shakeCamera = GetNode<ShakeCamera>("ShakeCamera");

        var inventory = GetNode<Inventory>("Inventory");
        inventory.Populate(dungeon.StartingItems);

        var room = GetNode<Room>("Room");
        room.Player = GetNode<Player>("Player");

        room.FillRoom(dungeonTraverser.CurrentRoom);
        ToggleInventory(forceState: room.WantsInventory, skipAnimation: true);
    }

    public override void _Process(float delta)
    {
        var wantsInventory = GetNode<Room>("Room").WantsInventory;
        if (wantsInventory != currentInventoryVisibility)
        {
            ToggleInventory();
        }
    }

    private void onRoomExited()
    {
        if (gameOver) return;
        if (dungeonTraverser.MoveForward())
        {
            GetNode<Room>("Room").FillRoom(dungeonTraverser.CurrentRoom);
            return;
        }
        addLootScore();
        doGameOver();
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

    private void onPlayerDied()
    {
        doGameOver();
        var room = GetNode<Room>("Room");
        room.Clear();
    }

    private void addLootScore()
    {
        score += GetNode<Inventory>("Inventory").HeldItems.Sum(i => i.Type.Score());
    }

    private void doGameOver()
    {
        gameOver = true;
        var overlay = GetNode<CanvasItem>("GameOverOverlay");
        overlay.GetNode<Label>("GameOver/Score").Text = score.ToString();
        overlay.Visible = true;
    }

    private void onScoreAdded(int amount)
    {
        score += amount;
        GetNode<TopHud>("TopHud").UpdateScore(score);
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
