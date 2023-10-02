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
                    "Right? I'm sure they are reading that they should click on the items they want me to use.",
                    "Whatever the word 'click' means in this context."
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "Unlike gunpowder, personal computers did not exist in the era this game takes place in."
                ),
                Hazard(OvergrownVines),
                Dialogue(
                    Portrait.Player, 
                    "With limited space, I feel like there should be a way to not pick up these items."
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "There is one! Drag the items to the trash can below the mute button.",
                    "Just don't mute the music. The devs are particularly proud of it"
                ),
                Item(Rope),
                Dialogue(
                    Portrait.Player, 
                    "That rope was still in greyscale."
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "The devs were running out of time towards the end."
                )
            ),
            Room(
                Item(Sword),
                Dialogue(
                    Portrait.Player, 
                    "Why do I need another sword?"
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "Swords break, so you can take that into consideration when fighting.",
                        "That's part of inventory management. You know, the theme of the jam."
                ),
                Dialogue(
                    Portrait.Player, 
                    "I don't know what preserves have to do with anything.",
                    "But thanks anyway, disembodied voice I insist on interacting with."
                )
            ),
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
                Dialogue(
                    Portrait.Beholder, 
                    "Now you will fight a larger monster, in order to prove your worth."
                ),
                Hazard(Beholder),
                Dialogue(
                    Portrait.Player, 
                    "Hmm, even though that Sword has the exact same sprite, it does look somehow special."
                ),
                Item(Excalibur)
            ),
            Room(
                Item(Potion),
                Item(Potion),
                Item(Potion),
                Item(Sword),
                Item(Sword)
            ),
            Room(
                Hazard(Beholder),
                Item(Crown),
                Item(Potion),
                Item(Potion)
            ),
            Room(
                Hazard(Slime),
                Item(Potion)
            ),
            Room(
                Hazard(Beholder),
                Item(Sword),
                Item(Potion),
                Item(Potion)
            ),
            Room(
                Hazard(Slime)
            ),
            Room(
                Hazard(Slime)
            ),
            Room(
                Hazard(Slime),
                Item(Sword),
                Item(Sword)
            ),
            Room(
                Dialogue(
                    Portrait.Beholder, 
                    "I'm the final boss. You can tell that because I'm substantially larger."
                ),
                Dialogue(
                    Portrait.Player, 
                    "...",
                    "You look the same."
                ),
                Hazard(Beholder),
                Item(Orb)
            ),
            Room(
                Dialogue(
                    Portrait.Player, 
                    "Wait, I thought it was over."
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "Not yet. The grind continues. Go get that bread."
                ),
                Hazard(Beholder),
                Item(Orb)
            ),
            Room(
            Hazard(Slime),
                Item(Potion)
            ),
            Room(
                Hazard(Slime)
            ),
            Room(
                Hazard(Slime),
                Item(Crown)
            ),
            Room(
                Hazard(Slime)
            ),
            Room(
                Hazard(Beholder),
                Item(Orb)
            ),
            Room(
                Dialogue(
                    Portrait.Narrator, 
                    "And at last, our hero finds the Beholder who killed his parents."
                ),
                Dialogue(
                    Portrait.Beholder, 
                    "Ha, I'm the Beholder who killed your parents."
                ),
                Dialogue(
                    Portrait.Player, 
                    "At last I found you, beholder who killed my parents."
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "He will now have his revenge."
                ),
                Dialogue(
                    Portrait.Beholder, 
                    "Ha, You will never have your revenge."
                ),
                Dialogue(
                    Portrait.Player, 
                    "I will have my revenge!"
                ),
                Hazard(Beholder),
                Item(Orb),
                Dialogue(
                    Portrait.Narrator, 
                    "And at last, our hero killed the Beholder who killed his parents."
                ),
                Dialogue(
                    Portrait.Player, 
                    "Is that the reason I entered this dungeon?"
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "Originally the devs intended this to be a journey where you saved your mother from cancer.",
                    "They scraped that idea and settled for something with more meta-humour and self awareness.",
                    "You know, time constraints and all that."
                ),
                Dialogue(
                    Portrait.Player, 
                    "I see. Can we be sure that was the Beholder who killed my parents tho?"
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "..."
                ),
                Dialogue(
                    Portrait.Player, 
                    "...",
                    "Well, I guess at least I made one friend in you, narrator"
                ),
                Dialogue(
                    Portrait.Narrator, 
                    "Perhaps true friendship is the disembodied apparitions found along the way"
                ),
                Dialogue(
                    Portrait.Slime, 
                    "The narrator and L0nk lived happily ever after in a relationship that transcends the fourth wall.",
                    "After the tragic death of my father and grandfather, I now need to do freelancing as a narrator",
                    "It's a tough job, but I need to pay for my college tuition somehow"
                )
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
