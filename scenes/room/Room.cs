using System;
using System.Collections.Generic;
using Godot;

public sealed class Room : Node2D
{
    [Signal]
    public delegate void ItemDropped(ItemLibrary.ItemType type);

    [Signal]
    public delegate void RoomExited();

    private readonly Queue<IRoomStep> queuedSteps = new();

    public void FillRoom(RoomContents contents)
    {
        queuedSteps.Clear();
        foreach (var step in contents.Steps)
        {
            queuedSteps.Enqueue(step);
        }

        doNextStep();
    }

    public void StepCompleted()
    {
        doNextStep();
    }

    private void doNextStep()
    {
        if (queuedSteps.Count == 0)
        {
            finishRoom();
            return;
        }

        var step = queuedSteps.Dequeue();
        switch (step)
        {
            case ItemDropStep itemDrop:
                dropItem(itemDrop.Item);
                break;
            default:
                throw new InvalidOperationException($"Step cannot be handled: {step.GetType()}");
        }
    }

    private void finishRoom()
    {
        EmitSignal(nameof(RoomExited));
    }

    private void dropItem(ItemLibrary.ItemType item)
    {
        EmitSignal(nameof(ItemDropped), item);
    }
}
