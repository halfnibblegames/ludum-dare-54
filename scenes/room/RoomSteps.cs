using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class RoomSteps
{
    public static RoomContents Room(params IRoomStep[] steps) => new(steps);

    public static IRoomStep Item(ItemLibrary.ItemType item) => new ItemDropStep(item);
    public static IRoomStep Hazard(HazardLibrary.HazardType hazard) => new EncounterStep(hazard);
    public static IRoomStep PlayerDialogue(params string[] dialogue) =>
        Dialogue(dialogue.Select(s => new Sentence(Portrait.Player, s)).ToList());
    public static IRoomStep Dialogue(IReadOnlyList<Sentence> dialogue) => new DialogueStep(dialogue);
}

public interface IRoomStep
{
    bool WantsInventory { get; }
    void Do(Node roomNode, Player player, Templates templates, Action complete);
}
