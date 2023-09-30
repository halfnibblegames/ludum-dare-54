using Godot;

public class Main : Node
{
    private Inventory inventory = null!;

    public override void _Ready()
    {
        inventory = GetNode<Inventory>("Inventory");
    }
}
