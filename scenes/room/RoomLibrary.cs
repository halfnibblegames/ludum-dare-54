using static RoomSteps;

public static class RoomLibrary
{
    public static RoomContents RandomSingleItem() => Room(Item(ItemLibrary.RandomType()));
}
