public sealed class DungeonTraverser
{
    private DungeonRoom currentRoom;

    public RoomContents CurrentRoom => currentRoom.Contents;

    public DungeonTraverser(Dungeon dungeon)
    {
        currentRoom = dungeon.EntranceRoom;
    }

    public bool MoveForward()
    {
        var next = currentRoom.East;
        if (next == null)
        {
            return false;
        }

        currentRoom = next;
        return true;
    }
}
