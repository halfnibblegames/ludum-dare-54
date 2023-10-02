using System;
using System.Collections.Generic;
using Godot;

public sealed class Room : Node2D
{
    private const float initialY = -16;
    private const float restY = 32;
    private const float finalY = 124;
    private const float movementSpeed = 96;

    [Signal]
    public delegate void RoomExited();

    private readonly Queue<IRoomStep> queuedSteps = new();
    private Templates templates = null!;
    private State state = State.Done;

    // find a better way of injecting this?
    public Player Player { get; set; } = null!;

    public bool WantsInventory { get; private set; }

    public override void _Ready()
    {
        templates = GetNode<Templates>("/root/Templates");
    }

    public override void _Process(float delta)
    {
        if (state is State.Steps or State.Done)
        {
            return;
        }

        Player.Position = new Vector2(Player.Position.x, Player.Position.y + delta * movementSpeed);
        if (state == State.Entering && Player.Position.y >= restY)
        {
            Player.Position = new Vector2(Player.Position.x, restY);
            state = State.Steps;
            doNextStep();
        }

        if (state == State.Exiting && Player.Position.y >= finalY)
        {
            finishRoom();
        }
    }

    public void FillRoom(RoomContents contents)
    {
        if (state != State.Done)
        {
            throw new InvalidOperationException();
        }

        queuedSteps.Clear();
        foreach (var step in contents.Steps)
        {
            queuedSteps.Enqueue(step);
        }

        enterRoom();
    }

    public void Clear()
    {
        Player.Position = new Vector2(Player.Position.x, finalY);
        finishRoom();
    }

    private void enterRoom()
    {
        Player.Position = new Vector2(96, initialY);
        state = State.Entering;
    }

    private void doNextStep()
    {
        if (queuedSteps.Count == 0)
        {
            exitRoom();
            return;
        }

        var step = queuedSteps.Dequeue();
        step.Do(this, Player, templates, doNextStep);
        WantsInventory = step.WantsInventory;
    }

    private void exitRoom()
    {
        state = State.Exiting;
        WantsInventory = false;
    }

    private void finishRoom()
    {
        state = State.Done;
        EmitSignal(nameof(RoomExited));
    }

    private enum State
    {
        Entering,
        Steps,
        Exiting,
        Done
    }
}
