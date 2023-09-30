using System;
using Godot;

public sealed class ItemDropStep : IRoomStep
{
    private readonly ItemLibrary.ItemType item;

    public ItemDropStep(ItemLibrary.ItemType item)
    {
        this.item = item;
    }

    public void Do(Node roomNode, Templates templates, Action complete)
    {
        var newItem = templates.HoveringItemScene.Instance<HoveringItem>();
        newItem.SetType(item);
        var listener = new SignalListener(roomNode, complete);
        newItem.Connect(nameof(HoveringItem.ItemPlaced), listener, nameof(listener.OnItemPlaced));
        roomNode.AddChild(listener);
        roomNode.AddChild(newItem);
    }

    private sealed class SignalListener : Node
    {
        private readonly Node roomNode;
        private readonly Action complete;

        public SignalListener(Node roomNode, Action complete)
        {
            this.roomNode = roomNode;
            this.complete = complete;
        }

        public void OnItemPlaced(HoveringItem item)
        {
            roomNode.RemoveChild(item);
            complete();
        }
    }
}
