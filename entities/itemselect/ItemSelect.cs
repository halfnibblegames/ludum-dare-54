using Godot;

public sealed class ItemSelect : Node
{
    [Signal]
    public delegate void ItemChosen(Item item);

    private Node2D cursor = null!;
    private BottomHud hud = null!;
    private Vector2 mousePos = Vector2.Zero;
    private Inventory? inventory;

    public override void _Ready()
    {
        cursor = GetNode<Node2D>("Cursor");
        hud = GetNode<BottomHud>("BottomHud");
        mousePos = cursor.GetGlobalMousePosition();
        cursor.Position = mousePos;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        cursor.Position = mousePos;

        if (inventory?.TryFindItem(mousePos, out var item) ?? false)
        {
            hud.SetItem(item.Type);
        }
        else
        {
            hud.ClearItem();
        }
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
            GetTree().SetInputAsHandled();
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
            hud.ClearItem();
        }
    }
}
