using Godot;

public sealed class Item : Node2D
{
    [Signal]
    public delegate void PropertiesChanged();

    [Signal]
    public delegate void ItemDepleted();

    private ItemLibrary.ItemType type = ItemLibrary.ItemType.Sword;
    private int chargesUsed;

    public ItemLibrary.Properties Properties { get; private set; } = null!;

    private ColorRect durabilityRect = null!;

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

    public override void _Ready()
    {
        Properties = ItemLibrary.Resolve(type);
        applyProperties();
        durabilityRect = GetNode<ColorRect>("DurabilityRect");
    }

    public override void _Process(float delta)
    {
        if (chargesUsed == 0)
        {
            durabilityRect.Visible = false;
            return;
        }

        var totalCharges = (float) type.Durability();
        var percentage = 1 - chargesUsed / totalCharges;
        durabilityRect.RectPosition = new Vector2(-Properties.HalfSize.x, Properties.HalfSize.y - 2);
        durabilityRect.RectSize = new Vector2(percentage * 2 * Properties.HalfSize.x, 1);
        durabilityRect.Visible = true;
    }

    private void applyProperties()
    {
        if (GetNodeOrNull<Sprite>("Sprite") is { } sprite)
        {
            sprite.RegionRect = Properties.SpriteRect;
        }

        chargesUsed = 0;
        EmitSignal(nameof(PropertiesChanged));
    }

    public void Use()
    {
        chargesUsed++;
        if (chargesUsed >= type.Durability())
        {
            EmitSignal(nameof(ItemDepleted));
        }
    }
}
