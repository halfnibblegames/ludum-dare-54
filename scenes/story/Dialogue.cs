using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed class Dialogue : Node2D
{
    private Label text = null!;
    private Queue<Sentence> queuedDialogue = new();
    private TaskCompletionSource<bool>? tsc;

    public override void _Ready()
    {
        text = GetNode<Label>("container/text");
    }

    public override void _Process(float delta)
    {
        if (!Visible)
        {
            return;
        }
        text.PercentVisible = Math.Min(text.PercentVisible + 0.5f * delta, 1.0f);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left })
        {
            return;
        }

        if (text.PercentVisible < 1)
        {
            text.PercentVisible = 1;
            return;
        }

        if (queuedDialogue.Count > 0)
        {
            startSentence(queuedDialogue.Dequeue());
            return;
        }

        tsc?.SetResult(true);
        tsc = null;
    }

    public Task DisplayDialog(IReadOnlyList<Sentence> dialog)
    {
        tsc?.SetResult(true);
        tsc = null;
        queuedDialogue.Clear();

        foreach (var sentence in dialog)
        {
            queuedDialogue.Enqueue(sentence);
        }

        if (queuedDialogue.Count > 0)
        {
            startSentence(queuedDialogue.Dequeue());
        }

        tsc = new TaskCompletionSource<bool>();
        return tsc.Task;
    }

    private void startSentence(Sentence sentence)
    {
        // TODO(will): Change portrait if we ever get this far
        text.PercentVisible = 0;
        text.Text = sentence.Text;
    }
}

public sealed record Sentence(
    Portrait Speaker,
    string Text
);

public enum Portrait
{
    Player
}
