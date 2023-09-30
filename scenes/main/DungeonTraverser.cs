public sealed class DungeonTraverser
{
    private readonly DungeonLayout layout;

    private int i;

    public RoomContents CurrentRoom => layout.Rooms[i];

    public DungeonTraverser(DungeonLayout layout)
    {
        this.layout = layout;
    }

    public void MoveForward()
    {
        i++;
    }
}
