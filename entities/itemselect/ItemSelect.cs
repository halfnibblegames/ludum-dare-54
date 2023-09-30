using Godot;

public sealed class ItemSelect : Area2D
{
    [Signal]
    public delegate void ItemChosen(Item item);

    private Vector2 mousePos = Vector2.Zero;
    private Inventory? inventory;

    public override void _Ready()
    {
        mousePos = GetGlobalMousePosition();
        Position = mousePos;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        Position = mousePos;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseMotion motionEvent)
        {
            mousePos = motionEvent.Position;
        }

        if (inventory is not null &&
            @event is InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left } mouseEvent &&
            inventory.TryFindItem(mouseEvent.Position, out var item))
        {
            EmitSignal(nameof(ItemChosen), item);
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
        if (other == inventory)
        {
            inventory = null;
        }
    }
}
