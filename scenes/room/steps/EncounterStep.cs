using System;
using Godot;

public sealed class EncounterStep : IRoomStep
{
    private readonly HazardLibrary.HazardType hazard;

    public bool WantsInventory => true;

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
        var runner = new EncounterRunner(player, hazardObj, cursor, complete);

        cursor.Connect(nameof(ItemSelect.ItemChosen), runner, nameof(runner.OnItemChosen));
        cursor.Connect(nameof(ItemSelect.PunchChosen), runner, nameof(runner.OnPunchChosen));

        roomNode.AddChild(runner);
        runner.AddChild(hazardObj);
        runner.AddChild(cursor);

        roomNode.Connect(nameof(Room.RoomExited), runner, nameof(runner.Quit));
    }

    private sealed class EncounterRunner : Node
    {
        private readonly Player player;
        private readonly Hazard hazard;
        private readonly ItemSelect cursor;
        private readonly Action complete;
        private readonly Encounter encounter;
        private bool waitingForInteraction;

        public EncounterRunner(Player player, Hazard hazard, ItemSelect cursor, Action complete)
        {
            this.player = player;
            this.hazard = hazard;
            this.cursor = cursor;
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
            cursor.Disable();
            player.HidePunchButton();
            player.DoAction(() => useItem(item), finishPlayerTurn);
        }

        public void OnPunchChosen()
        {
            if (!waitingForInteraction)
            {
                return;
            }

            waitingForInteraction = false;
            cursor.Disable();
            player.HidePunchButton();
            player.DoAction(punch, finishPlayerTurn);
        }

        private void useItem(Item item)
        {
            item.Type.CombatUse()!.Do(encounter);
            item.Use();
        }

        private void punch()
        {
            encounter.DamageHazard(1);
        }

        private void startPlayerTurn()
        {
            player.PreviewDamage(hazard.Type.AttackDamage());
            player.ShowPunchButton();
            waitingForInteraction = true;
            cursor.Enable();
        }

        private void finishPlayerTurn()
        {
            if (hazard.CurrentHealth > 0)
            {
                startHazardTurn();
                return;
            }

            player.AddScore(hazard.Type.Score());
            player.PreviewDamage(0);
            QueueFree();
            complete();
        }

        private void startHazardTurn()
        {
            hazard.DoTurn(encounter, startPlayerTurn);
        }

        public void Quit()
        {
            QueueFree();
        }
    }
}
