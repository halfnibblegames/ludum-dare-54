using static HazardLibrary;
using static ItemLibrary;
using static RoomSteps;

public static class RoomLibrary
{
    public static RoomContents Empty() => Room();
    public static RoomContents SingleItem(ItemType item) => Room(Item(item));
    public static RoomContents HazardWithLoot(HazardType hazard, ItemType loot) => Room(Hazard(hazard), Item(loot));
}
