using System;
using Godot;

public sealed class ItemDropStep : IRoomStep
{
    private readonly ItemLibrary.ItemType item;

    public bool WantsInventory => true;

    public ItemDropStep(ItemLibrary.ItemType item)
    {
        this.item = item;
    }

    public void Do(Node roomNode, Player player, Templates templates, Action complete)
    {
        var drop = templates.ItemDropScene.Instance<ItemDrop>();
        drop.SetItem(item);
        var hover = drop.GetNode<HoveringItem>("HoveringItem");
        var listener = new SignalListener(drop, complete);
        hover.Connect(nameof(HoveringItem.ItemPicked), listener, nameof(listener.OnItemPicked));
        hover.Connect(nameof(HoveringItem.ItemPlaced), listener, nameof(listener.OnItemPlaced));
        roomNode.AddChild(listener);
        listener.AddChild(drop);

        roomNode.Connect(nameof(Room.RoomExited), listener, nameof(listener.Quit));
    }

    private sealed class SignalListener : Node
    {
        private readonly ItemDrop drop;
        private readonly Action complete;

        public SignalListener(ItemDrop drop, Action complete)
        {
            this.drop = drop;
            this.complete = complete;
        }

        public override void _Ready()
        {
            drop.PrimeChestHint();
        }

        public void OnItemPicked()
        {
            drop.PrimeDropHint();
        }

        // ReSharper disable once UnusedParameter.Local
        public void OnItemPlaced(HoveringItem item)
        {
            QueueFree();
            complete();
        }

        public void Quit()
        {
            QueueFree();
        }
    }
}
