using Godot;

public sealed class ItemSelect : Node
{
    [Signal]
    public delegate void ItemChosen(Item item);

    [Signal]
    public delegate void PunchChosen();

    private Node2D cursor = null!;
    private BottomHud hud = null!;
    private Vector2 mousePos = Vector2.Zero;
    private Inventory? inventory;
    private bool punch;

    public override void _Ready()
    {
        cursor = GetNode<Node2D>("Cursor");
        hud = GetNode<BottomHud>("BottomHud");
        mousePos = cursor.GetGlobalMousePosition();
        cursor.Position = mousePos;

        GetNode<InputMouseHint>("InventoryMouseHint").Prime();
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

        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left } mouseEvent)
        {
            return;
        }

        if (inventory is not null && inventory.TryFindItem(mouseEvent.Position, out var item))
        {
            EmitSignal(nameof(ItemChosen), item);
            GetTree().SetInputAsHandled();
        }

        if (punch)
        {
            EmitSignal(nameof(PunchChosen));
            GetTree().SetInputAsHandled();
        }
    }

    private void onAreaEntered(Area2D other)
    {
        if (other is Inventory inv)
        {
            inventory = inv;
        }

        if (other is PunchButton)
        {
            punch = true;
        }
    }

    private void onAreaExited(Area2D other)
    {
        if (other == inventory)
        {
            inventory = null;
            hud.ClearItem();
        }

        if (other is PunchButton)
        {
            punch = false;
        }
    }
}
