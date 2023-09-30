using System;
using Godot;

public sealed class EncounterStep : IRoomStep
{
    public void Do(Node roomNode, Templates templates, Action complete)
    {
        var cursor = templates.ItemSelectScene.Instance<ItemSelect>();
        var runner = new EncounterRunner(complete);

        cursor.Connect(nameof(ItemSelect.ItemChosen), runner, nameof(runner.OnItemChosen));

        roomNode.AddChild(runner);
        runner.AddChild(cursor);
    }

    private sealed class EncounterRunner : Node
    {
        private readonly Action complete;

        public EncounterRunner(Action complete)
        {
            this.complete = complete;
            // TODO(tom): instantiate encounter and process turns until combat over
        }

        public void OnItemChosen(Item item)
        {
            if (item.Type.CombatUse() is null)
            {
                return;
            }

            // TODO(tom): apply use to encounter
            item.Use();
            complete();
        }
    }
}
