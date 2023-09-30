
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public abstract record RoomEvent
{
    public sealed record None : RoomEvent
    {
        public static readonly None Instance = new();
    }

    public sealed record Exit : RoomEvent
    {
        public static readonly None Instance = new();
    }

    // TODO: Add necessary battle shenanigans.
    public sealed record Battle : RoomEvent;

    // TODO: Add necessary trasure shenanigans.
    public sealed record Treasure : RoomEvent;
    
}



public sealed record DungeonRoom(
    DungeonRoom? North,
    DungeonRoom? East,
    DungeonRoom? South,
    DungeonRoom? West,
    RoomEvent Event
)
{
    public static DungeonRoom FromMap(string map)
    {
        var splitMap = map
            .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        var indexOfEntrance = map.IndexOf("P", StringComparison.Ordinal);
        if (indexOfEntrance == -1)
            throw new InvalidOperationException("You created a map with no entrance. Maybe you should take a breather.");

        var y = Regex
            .Matches(map.Substring(0, indexOfEntrance), Environment.NewLine)
            .Count;

        var x = splitMap[y].IndexOf("P", StringComparison.Ordinal);

        return new DungeonRoom(
            North: FromMap(splitMap, (x, y - 1)),
            East: FromMap(splitMap, (x + 1, y)),
            South: FromMap(splitMap, (x, y + 1)),
            West: FromMap(splitMap, (x - 1, y)),
            Event: RoomEvent.None.Instance
        );
    }
    
    private  static DungeonRoom? FromMap(string[] map, (int, int) currentRoom)
    {
        var (x, y) = currentRoom;
        var eventString = map.ElementAtOrDefault(y)?.ElementAtOrDefault(x);
        if (eventString is null || eventString == ' ')
            return null;

        RoomEvent roomEvent = eventString switch
        { 
            'X' => new RoomEvent.Battle(),
            '$' => new RoomEvent.Treasure(),
            _ => RoomEvent.None.Instance
        };

        return new DungeonRoom(
            North: FromMap(map, (x, y - 1)),
            East: FromMap(map, (x + 1, y)),
            South: FromMap(map, (x, y + 1)),
            West: FromMap(map, (x - 1, y)),
            Event: roomEvent
        );
    }
}

public sealed record Dungeon(
    string Name,
    IReadOnlyList<ItemLibrary.ItemType> StartingItems,
    DungeonRoom Rooms,
    IReadOnlyList<string> Monologue
)
{
    public static IEnumerable<Dungeon> Dungeons()
    {
        const string firstDungeonMap = @"
 $.E
 ...
.X.
P
";

        yield return new(
            Name: "A Tale of Encumbrance",
            StartingItems: new [] { ItemLibrary.ItemType.Sword },
            Rooms: DungeonRoom.FromMap(firstDungeonMap),
            Monologue: new List<string>
            {
                "Phew, that was a close call! I'm glad I managed to escape these nondescript monsters in time to arrive at the first monologue unscathed.",
                "I used up all my inventory except for my trusty Tutorial Sword. Wait, why's there a lampshade hung over there?"
            }
        );
    }
}
