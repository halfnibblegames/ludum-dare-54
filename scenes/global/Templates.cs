using Godot;

public sealed class Templates : Node
{
    [Export] public PackedScene HoveringItemScene { get; set; } = null!;
    [Export] public PackedScene ItemSelectScene { get; set; } = null!;
}
