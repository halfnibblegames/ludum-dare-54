using Godot;

public class Item : Node
{
    private ItemLibrary.Key type = ItemLibrary.Key.Sword;

    private ItemLibrary.Properties properties = null!;

    [Export]
    public ItemLibrary.Key Type
    {
        get => type;
        set
        {
            type = value;
            applyProperties();
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        properties = ItemLibrary.Resolve(type);
        applyProperties();
    }

    private void applyProperties()
    {
        if (GetNodeOrNull<Sprite>("Sprite") is { } sprite)
        {
            sprite.RegionRect = properties.SpriteRect;
        }
    }
}
