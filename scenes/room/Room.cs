using System.Collections.Generic;
using Godot;

public sealed class Room : Node2D
{
    [Signal]
    public delegate void RoomExited();

    private readonly Queue<IRoomStep> queuedSteps = new();
    private Templates templates = null!;

    // find a better way of injecting this?
    public Player Player { get; set; } = null!;

    public override void _Ready()
    {
        templates = GetNode<Templates>("/root/Templates");
    }

    public void FillRoom(RoomContents contents)
    {
        queuedSteps.Clear();
        foreach (var step in contents.Steps)
        {
            queuedSteps.Enqueue(step);
        }

        // TODO: animate in
        Player.Position = new Vector2(96, 24);

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
        step.Do(this, Player, templates, doNextStep);
    }

    private void finishRoom()
    {
        EmitSignal(nameof(RoomExited));
    }
}
