public sealed class RoomContents
{
    public IRoomStep[] Steps { get; }

    public RoomContents(IRoomStep[] steps)
    {
        Steps = steps;
    }
}
