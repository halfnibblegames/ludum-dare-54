using System;
using Godot;

public class HoveringItem : Area2D
{
    [Signal]
    public delegate void ItemPlaced(HoveringItem item);

    private Item item = null!;
    private Vector2 mousePos = Vector2.Zero;
    private Vector2? snapPos = null;
    private Inventory? inventory;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        item = GetNode<Item>("Item");
        onItemPropertiesChanged();
        mousePos = GetGlobalMousePosition();
        Position = mousePos;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
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

        if (inventory is not null &&
            @event is InputEventMouseButton { Pressed: true, ButtonIndex: (int)ButtonList.Left })
        {
            var result = inventory.FitItem(mousePos, item.Properties);
            if (result.Result.CanCommit())
            {
                result.Commit(inventory, item);
                EmitSignal(nameof(ItemPlaced), this);
            }
        }
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
    }

    private void onAreaExited(Area2D other)
    {
        if (other is Inventory)
        {
            inventory = null;
            snapPos = null;
        }
    }

    public void RandomizeItem()
    {
        var types = Enum.GetValues(typeof(ItemLibrary.ItemType));
        var randomType = (ItemLibrary.ItemType) types.GetValue(GD.Randi() % types.Length);
        GetNode<Item>("Item").Type = randomType;
    }
}
