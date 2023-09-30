using System;
using Godot;

public static class RoomSteps
{
    public static RoomContents Room(params IRoomStep[] steps) => new(steps);

    public static IRoomStep Item(ItemLibrary.ItemType item) => new ItemDropStep(item);
    public static IRoomStep Hazard() => new EncounterStep();
}

public interface IRoomStep
{
    void Do(Node roomNode, Templates templates, Action complete);
}
