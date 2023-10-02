using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class RoomSteps
{
    public static RoomContents Room(params IRoomStep[] steps) => new(steps);

    public static IRoomStep Item(ItemLibrary.ItemType item) => new ItemDropStep(item);
    public static IRoomStep Hazard(HazardLibrary.HazardType hazard) => new EncounterStep(hazard);
    public static IRoomStep Dialogue(Portrait portrait, params string[] dialogue) =>
        Dialogue(dialogue.Select(s => new Sentence(portrait, s)).ToList());
    private static IRoomStep Dialogue(IReadOnlyList<Sentence> dialogue) => new DialogueStep(dialogue);
}

public interface IRoomStep
{
    bool WantsInventory { get; }
    void Do(Node roomNode, Player player, Templates templates, Action complete);
}
