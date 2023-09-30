using Godot;
using static Constants;

public class ItemSlot : Area2D
{
    [Signal]
    public delegate void ItemDropped(Item? item);

    [Export] private PackedScene itemScene = null!;
    private Item? item;

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (Engine.EditorHint) return;
        if (item is not null) return;

        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left } e)
        {
            var d = e.Position - Position;
            const float r = 0.5f * ItemSlotSize;
            if (d.x is >= -r and < r && d.y is >= -r and < r)
            {
                fillWithItem();
            }
        }
    }

    private void fillWithItem()
    {
        item = itemScene.Instance<Item>();
        AddChild(item);
        EmitSignal(nameof(ItemDropped), item);
    }
}
