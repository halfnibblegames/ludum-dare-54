using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HazardLibrary.HazardType;
using static ItemLibrary.ItemType;

public static class DungeonRoomParser
{
    public static IReadOnlyDictionary<Coord, RoomContents> Linear(out Coord entrance)
    {
        var rooms = new[]
        {
            RoomLibrary.SingleItem(Potion),
            RoomLibrary.SingleItem(Bomb),
            RoomLibrary.HazardWithLoot(OvergrownVines, Rope),
            RoomLibrary.SingleItem(Sword),
            RoomLibrary.HazardWithLoot(Slime, Sword),
            RoomLibrary.HazardWithLoot(Spider, PonderingOrb)
        };

        var dict = new Dictionary<Coord, RoomContents>();
        for (var i = 0; i < rooms.Length; i++)
        {
            dict.Add(new Coord(i, 0), rooms[i]);
        }

        entrance = new Coord(0, 0);
        return dict;
    }

    public static IReadOnlyDictionary<Coord, RoomContents> FromMap(string map, out Coord entrance)
    {
        var splitMap = map
            .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        var y = Enumerable.Range(1, splitMap.Length)
            .FirstOrDefault(i => splitMap[i - 1].Contains('P')) - 1;

        if (y == -1)
        {
            throw new InvalidOperationException(
                "You created a map with no entrance. Maybe you should take a breather.");
        }

        var x = splitMap[y].IndexOf("P", StringComparison.Ordinal);
        entrance = new Coord(x, y);
        var dict = new Dictionary<Coord, RoomContents>();

        for (var j = 0; j < splitMap.Length; j++)
        {
            for (var i = 0; i < splitMap[j].Length; i++)
            {
                if (parseContents(splitMap[j][i]) is { } contents)
                {
                    dict.Add(new Coord(i, j), contents);
                }
            }
        }

        return dict;
    }

    private static RoomContents? parseContents(char c)
    {
        if (c is ' ')
            return null;

        return c switch
        {
            'X' => RoomLibrary.HazardWithLoot(Spider, ItemLibrary.RandomItem()),
            '$' => RoomLibrary.SingleItem(ItemLibrary.RandomItem()),
            _ => RoomLibrary.Empty()
        };
    }
}

public sealed class DungeonRoom
{
    private readonly Dungeon dungeon;
    private readonly Coord coords;

    public RoomContents Contents { get; }

    public DungeonRoom? North => dungeon[coords.Above];
    public DungeonRoom? East => dungeon[coords.Right];
    public DungeonRoom? South => dungeon[coords.Below];
    public DungeonRoom? West => dungeon[coords.Left];

    public DungeonRoom(Dungeon dungeon, Coord coords, RoomContents contents)
    {
        this.dungeon = dungeon;
        this.coords = coords;
        Contents = contents;
    }
}

public sealed record Dungeon(
    string Name,
    IReadOnlyList<ItemLibrary.ItemType> StartingItems,
    IReadOnlyDictionary<Coord, RoomContents> Rooms,
    Coord Entrance,
    IReadOnlyList<string> Monologue
)
{
    public DungeonRoom? this[Coord coords] =>
        Rooms.TryGetValue(coords, out var contents)
            ? new DungeonRoom(this, coords, contents)
            : null;

    public DungeonRoom EntranceRoom => this[Entrance] ?? throw new InvalidDataException();

    public static Dungeon Make()
    {
        /*
        const string firstDungeonMap = @"
 $.E
 ...
.X.
P
";
        */

        return new Dungeon(
            Name: "A Tale of Encumbrance",
            StartingItems: new [] { Sword },
            Rooms: DungeonRoomParser.Linear(out var entrance), // DungeonRoomParser.FromMap(firstDungeonMap, out var entrance),
            Entrance: entrance,
            Monologue: new List<string>
            {
                "Phew, that was a close call! I'm glad I managed to escape these nondescript monsters in time to arrive at the first monologue unscathed.",
                "I used up all my inventory except for my trusty Tutorial Sword. Wait, why's there a lampshade hung over there?"
            }
        );
    }
}
