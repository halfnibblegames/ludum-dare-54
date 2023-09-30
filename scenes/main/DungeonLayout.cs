public sealed class DungeonLayout
{
    public RoomContents[] Rooms { get; }

    public DungeonLayout(params RoomContents[] rooms)
    {
        Rooms = rooms;
    }
}
