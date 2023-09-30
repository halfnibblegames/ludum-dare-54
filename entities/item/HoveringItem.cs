using Godot;

public class HoveringItem : Area2D
{
    public Item Item = null!;
    private Vector2 mousePos = Vector2.Zero;
    private Vector2? snapPos = null;
    private Inventory? inventory;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Item = GetNode<Item>("Item");
        onItemPropertiesChanged();
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
        var result = inventory.FitItem(mousePos, Item.Properties);
        snapPos = result.Fits ? result.Position : null;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion motionEvent)
        {
            mousePos = motionEvent.Position;
        }
    }

    private void onItemPropertiesChanged()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (GetNode<CollisionShape2D>("BoundingBox") is { } boundingBox && Item is not null)
        {
            var rect = (RectangleShape2D) boundingBox.Shape;
            rect.Extents = Item.Properties.HalfSize;
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
}
