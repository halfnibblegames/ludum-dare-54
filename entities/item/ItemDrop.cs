using Godot;
using System;

public class ItemDrop : Node2D
{
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("OpeningAnimation").Play("default");
    }

    public void SetItem(ItemLibrary.ItemType item)
    {
        GetNode<HoveringItem>("HoveringItem").SetItem(item);
        GetNode<BottomHud>("BottomHud").SetItem(item);
    }
}
