using Godot;
using System;
using System.Collections.Generic;

public sealed class Dialogue : Node2D
{
    [Signal] public delegate void DialogueFinished();

    private Label? text;
    private readonly Queue<Sentence> queuedDialogue = new();
    private bool hintPrimed;

    public override void _Ready()
    {
        text = GetNode<Label>("container/text");
        if (queuedDialogue.Count > 0)
        {
            startSentence(queuedDialogue.Dequeue());
        }
    }

    public override void _Process(float delta)
    {
        if (!Visible)
        {
            return;
        }
        text!.PercentVisible = Math.Min(text.PercentVisible + 0.5f * delta, 1.0f);
        if (text.PercentVisible >= 1 && !hintPrimed)
        {
            primeHint();
        }
    }

    private void primeHint()
    {
        GetNode<InputMouseHint>("MouseHint").Prime();
        hintPrimed = true;
    }

    private void resetHint()
    {
        GetNode<InputMouseHint>("MouseHint").Reset();
        hintPrimed = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (!Visible || text is null)
        {
            return;
        }

        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left })
        {
            return;
        }

        if (text.PercentVisible < 1)
        {
            text.PercentVisible = 1;
            primeHint();
            return;
        }

        if (queuedDialogue.Count > 0)
        {
            startSentence(queuedDialogue.Dequeue());
            return;
        }

        EmitSignal(nameof(DialogueFinished));
        Visible = false;
        resetHint();
    }

    public void DisplayDialog(IReadOnlyList<Sentence> dialog)
    {
        queuedDialogue.Clear();

        foreach (var sentence in dialog)
        {
            queuedDialogue.Enqueue(sentence);
        }

        if (queuedDialogue.Count == 0)
        {
            return;
        }

        if (text is not null)
        {
            startSentence(queuedDialogue.Dequeue());
        }

        Visible = true;
    }

    private void startSentence(Sentence sentence)
    {
        // TODO(will): Change portrait if we ever get this far
        text!.PercentVisible = 0;
        text.Text = sentence.Text;
        resetHint();
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
