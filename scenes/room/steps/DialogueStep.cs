using System;
using System.Collections.Generic;
using Godot;

sealed class DialogueStep : IRoomStep
{
    private readonly IReadOnlyList<Sentence> dialogue;

    public bool WantsInventory => false;

    public DialogueStep(IReadOnlyList<Sentence> dialogue)
    {
        this.dialogue = dialogue;
    }

    public void Do(Node roomNode, Player player, Templates templates, Action complete)
    {
        var dialogue = templates.DialogueScene.Instance<Dialogue>();
        dialogue.DisplayDialog(this.dialogue);
        var listener = new SignalListener(complete);
        dialogue.Connect(nameof(Dialogue.DialogueFinished), listener, nameof(listener.OnDialogueFinished));
        roomNode.AddChild(listener);
        listener.AddChild(dialogue);

        roomNode.Connect(nameof(Room.RoomExited), listener, nameof(listener.Quit));
    }

    private sealed class SignalListener : Node
    {
        private readonly Action complete;

        public SignalListener(Action complete)
        {
            this.complete = complete;
        }

        public void OnDialogueFinished()
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
