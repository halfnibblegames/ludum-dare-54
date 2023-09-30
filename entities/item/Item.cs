using Godot;

public sealed class Item : Node2D
{
    [Signal]
    public delegate void PropertiesChanged();

    [Signal]
    public delegate void ItemDepleted();

    private ItemLibrary.ItemType type = ItemLibrary.ItemType.Sword;

    public ItemLibrary.Properties Properties { get; private set; } = null!;

    [Export]
    public ItemLibrary.ItemType Type
    {
        get => type;
        set
        {
            type = value;
            Properties = ItemLibrary.Resolve(type);
            applyProperties();
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Properties = ItemLibrary.Resolve(type);
        applyProperties();
    }

    private void applyProperties()
    {
        if (GetNodeOrNull<Sprite>("Sprite") is { } sprite)
        {
            sprite.RegionRect = Properties.SpriteRect;
        }
        EmitSignal(nameof(PropertiesChanged));
    }

    public void Use()
    {
        EmitSignal(nameof(ItemDepleted));
    }
}
