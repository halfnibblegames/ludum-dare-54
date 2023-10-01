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
        hazardObj.Position = new Vector2(96, 72);
        var runner = new EncounterRunner(player, hazardObj, complete);

        cursor.Connect(nameof(ItemSelect.ItemChosen), runner, nameof(runner.OnItemChosen));

        roomNode.AddChild(runner);
        runner.AddChild(hazardObj);
        runner.AddChild(cursor);
    }

    private sealed class EncounterRunner : Node
    {
        private readonly Player player;
        private readonly Hazard hazard;
        private readonly Action complete;
        private readonly Encounter encounter;
        private bool waitingForInteraction;

        public EncounterRunner(Player player, Hazard hazard, Action complete)
        {
            this.player = player;
            this.hazard = hazard;
            this.complete = complete;
            encounter = new Encounter(player, hazard);
            startPlayerTurn();
        }

        public void OnItemChosen(Item item)
        {
            if (item.Type.CombatUse() is null || !waitingForInteraction)
            {
                return;
            }

            waitingForInteraction = false;
            player.DoAction(() => useItem(item), finishPlayerTurn);
        }

        private void useItem(Item item)
        {
            item.Type.CombatUse()!.Do(encounter);
            //item.Use();
        }

        private void startPlayerTurn()
        {
            waitingForInteraction = true;
        }

        private void finishPlayerTurn()
        {
            if (hazard.CurrentHealth > 0)
            {
                startHazardTurn();
                return;
            }

            QueueFree();
            complete();
        }

        private void startHazardTurn()
        {
            hazard.DoTurn(encounter, startPlayerTurn);
        }
    }
}
