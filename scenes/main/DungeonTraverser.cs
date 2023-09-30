public sealed class DungeonTraverser
{
    private DungeonRoom currentRoom;

    public RoomContents CurrentRoom => currentRoom.Contents;

    public DungeonTraverser(Dungeon dungeon)
    {
        currentRoom = dungeon.EntranceRoom;
    }

    public void MoveForward()
    {
        currentRoom = currentRoom.North ?? currentRoom.East ?? currentRoom.South ?? currentRoom.West ?? currentRoom;
    }
}
