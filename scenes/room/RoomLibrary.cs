using static RoomSteps;

public static class RoomLibrary
{
    public static RoomContents Empty() => Room();
    public static RoomContents SingleItem(ItemLibrary.ItemType item) => Room(Item(item));
    public static RoomContents HazardWithLoot(ItemLibrary.ItemType loot) => Room(Hazard(), Item(loot));
}
