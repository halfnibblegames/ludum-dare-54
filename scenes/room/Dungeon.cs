using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HazardLibrary.HazardType;
using static ItemLibrary.ItemType;
using static RoomSteps;

public static class DungeonRoomParser
{
    public static IReadOnlyDictionary<Coord, RoomContents> Linear(out Coord entrance)
    {
        var rooms = new[]
        {
            Room(
                Dialogue(
                    Portrait.Player,
                    "Phew, that was a close call! I'm glad I managed to escape these nondescript monsters in time to arrive at the first monologue unscathed.",
                    "I used up all my inventory except for my trusty Tutorial Sword. Wait, why's there a lampshade hung over there?"
                ),
                Dialogue(
                    Portrait.Narrator,
                    "And thus began the journey of this unexpected hero!"
                )
            ),
            Room(
                Item(Potion),
                Dialogue(
                    Portrait.Player,"A potion? Here? Guess my straight edge phase ends today."
                )
            ),
            Room(
                Item(Bomb),
                Dialogue(
                    Portrait.Player,
                    "It's a common misconception that gunpowder is a recent invention."
                ),
                Dialogue(
                    Portrait.Narrator,
                    "The hero is correct, the earliest possible reference to gunpowder appeared in 142 AD during the Eastern Han dynasty."
                ),
                Dialogue(
                    Portrait.Player,
                    "Yeah the devs totally knew that piece of trivia."
                )
            ),
            Room(
                Dialogue(
                    Portrait.Player, 
                    "I feel like I'm about to fight an enemy.",
                    "I think now is a good time to explain some battle mechanics out loud."
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "I'm sure the player is not mashing the skip button during this interaction."
                ),
                Dialogue(
                    Portrait.Player, 
                    "Right? I'm sure they are reading that they should click on the items they wants me to use.",
                    "Whatever the word 'click' means in this context."
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "Unlike gunpowder, personal computers did not exist in the era this game takes place in."
                ),
                Hazard(OvergrownVines),
                Item(Rope)),
            Room(
                Item(Sword)),
            Room(
                Dialogue(
                    Portrait.Slime, 
                    "The first slime you killed couldn't deal damage because he was a tutorial. He was also my father."
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "This is a story about not getting to know your heroes."
                ),
                Dialogue(
                    Portrait.Player, 
                    "Shut up, narrator, the devs didn't even draw you a portrait."
                ),
                Hazard(Slime), 
                Item(Sword)
            ),
            Room(
                Hazard(Beholder),
                Item(PonderingOrb)
            )
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

        ItemLibrary.ItemType loot = ItemLibrary.RandomItem();
        return c switch
        {
            'X' => Room(Hazard(Spider), Item(loot)),
            '$' => Room(Item(ItemLibrary.RandomItem())),
            _ => Room()
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
    Coord Entrance
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
            Entrance: entrance
        );
    }
}
