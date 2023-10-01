using System;
using Godot;

public sealed class ItemDropStep : IRoomStep
{
    private readonly ItemLibrary.ItemType item;

    public ItemDropStep(ItemLibrary.ItemType item)
    {
        this.item = item;
    }

    public void Do(Node roomNode, Player player, Templates templates, Action complete)
    {
        var drop = templates.ItemDropScene.Instance<ItemDrop>();
        drop.SetItem(item);
        var hover = drop.GetNode<HoveringItem>("HoveringItem");
        var listener = new SignalListener(complete);
        hover.Connect(nameof(HoveringItem.ItemPlaced), listener, nameof(listener.OnItemPlaced));
        roomNode.AddChild(listener);
        listener.AddChild(drop);

        roomNode.Connect(nameof(Room.RoomExited), listener, nameof(listener.Quit));
    }

    private sealed class SignalListener : Node
    {
        private readonly Action complete;

        public SignalListener(Action complete)
        {
            this.complete = complete;
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
