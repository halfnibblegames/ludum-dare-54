using Godot;

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

    public void PrimeChestHint()
    {
        GetNode<InputMouseHint>("ChestMouseHint").Prime();
    }

    public void PrimeDropHint()
    {
        GetNode<InputMouseHint>("ChestMouseHint").Reset();
        GetNode<InputMouseHint>("InventoryMouseHint").Prime();
    }
}
