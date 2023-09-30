using Godot;

public class HoveringItem : Node2D
{
    private Item item = null!;
    private Vector2 mousePos = Vector2.Zero;
    [Export] private Inventory? inventory;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        item = GetNode<Item>("Item");
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
    }
}
