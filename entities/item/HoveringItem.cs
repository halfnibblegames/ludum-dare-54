using Godot;

public sealed class HoveringItem : Area2D
{
    [Signal]
    public delegate void ItemPicked();

    [Signal]
    public delegate void ItemPlaced(HoveringItem item);

    private Item item = null!;
    private Vector2 mousePos = Vector2.Zero;
    private Vector2? snapPos;
    private Inventory? inventory;
    private bool pickedUp;
    private bool discard;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        item = GetNode<Item>("Item");
        onItemPropertiesChanged();
    }

    public override void _Process(float delta)
    {
        if (!pickedUp) return;
        tryUpdateSnapPos();
        Position = snapPos ?? mousePos;
    }

    private void tryUpdateSnapPos()
    {
        if (inventory is null) return;
        var result = inventory.FitItem(mousePos, item.Properties);
        snapPos = result.Result.CanPreview() ? result.Position : null;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion motionEvent)
        {
            mousePos = motionEvent.Position;
        }

        var mouseClicked = @event is InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left };
        if (!mouseClicked)
        {
            return;
        }

        if (!pickedUp)
        {
            pickUp();
            return;
        }

        if (discard)
        {
            EmitSignal(nameof(ItemPlaced), this);
            GetTree().SetInputAsHandled();
            return;
        }

        if (inventory is null)
        {
            return;
        }

        var result = inventory.FitItem(mousePos, item.Properties);
        if (!result.Result.CanCommit())
        {
            return;
        }

        result.Commit(inventory, item);
        EmitSignal(nameof(ItemPlaced), this);
        GetTree().SetInputAsHandled();
    }

    private void pickUp()
    {
        pickedUp = true;
        EmitSignal(nameof(ItemPicked));
    }

    private void onItemPropertiesChanged()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (GetNode<CollisionShape2D>("BoundingBox") is { } boundingBox && item is not null)
        {
            var rect = (RectangleShape2D) boundingBox.Shape;
            rect.Extents = item.Properties.HalfSize;
        }
    }

    private void onAreaEntered(Area2D other)
    {
        if (other is Inventory inv)
        {
            inventory = inv;
        }

        if (other is ItemDiscard)
        {
            discard = true;
        }
    }

    private void onAreaExited(Area2D other)
    {
        if (other == inventory)
        {
            inventory = null;
            snapPos = null;
        }

        if (other is ItemDiscard)
        {
            discard = false;
        }
    }

    public void SetItem(ItemLibrary.ItemType type)
    {
        GetNode<Item>("Item").Type = type;
    }
}
