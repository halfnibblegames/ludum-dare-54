using System;
using Godot;

public sealed class EncounterStep : IRoomStep
{
    private readonly HazardLibrary.HazardType hazard;

    public EncounterStep(HazardLibrary.HazardType hazard)
    {
        this.hazard = hazard;
    }

    public void Do(Node roomNode, Player player, Templates templates, Action complete)
    {
        var cursor = templates.ItemSelectScene.Instance<ItemSelect>();
        var hazardObj = templates.HazardScene.Instance<Hazard>();
        hazardObj.Type = hazard;
        var runner = new EncounterRunner(player, hazardObj, complete);

        cursor.Connect(nameof(ItemSelect.ItemChosen), runner, nameof(runner.OnItemChosen));

        GD.Print($"Beware! A {hazard}");

        roomNode.AddChild(runner);
        runner.AddChild(cursor);
        runner.AddChild(hazardObj);
    }

    private sealed class EncounterRunner : Node
    {
        private readonly Hazard hazard;
        private readonly Action complete;
        private readonly Encounter encounter;

        public EncounterRunner(Player player, Hazard hazard, Action complete)
        {
            this.hazard = hazard;
            this.complete = complete;
            encounter = new Encounter(player, hazard);
        }

        public void OnItemChosen(Item item)
        {
            if (item.Type.CombatUse() is null)
            {
                return;
            }

            item.Type.CombatUse()!.Do(encounter);
            //item.Use();

            if (hazard.CurrentHealth <= 0)
            {
                QueueFree();
                complete();
                return;
            }

            hazard.DoTurn(encounter);
        }
    }
}
