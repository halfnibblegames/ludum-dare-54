public sealed class RoomContents
{
    public IRoomStep[] Steps { get; }

    public RoomContents(IRoomStep[] steps)
    {
        Steps = steps;
    }
}

public static class RoomSteps
{
    public static RoomContents Room(params IRoomStep[] steps) => new(steps);

    public static IRoomStep Item(ItemLibrary.ItemType item) => new ItemDropStep(item);
    public static IRoomStep Hazard() => new EncounterStep();
}

public interface IRoomStep {}

public sealed record EncounterStep : IRoomStep;

public sealed record ItemDropStep(ItemLibrary.ItemType Item) : IRoomStep;
