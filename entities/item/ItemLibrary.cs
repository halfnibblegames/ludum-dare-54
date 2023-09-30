using System;
using Godot;
using static Constants;

public static class ItemLibrary
{
    public enum ItemType
    {
        Sword,
        Potion,
        Key,
        FlameSword,
        Crown,
        PonderingOrb,
        Crystal,
        Rope,
        Bomb
    }

    public static ItemType RandomType()
    {
        var types = Enum.GetValues(typeof(ItemType));
        return (ItemType) types.GetValue(GD.Randi() % types.Length);
    }

    public static Properties Resolve(ItemType itemType) => itemType switch
    {
        ItemType.Sword => new Properties(1, 2, 0, 0),
        ItemType.Potion => new Properties(1, 1, 1, 0),
        ItemType.Key => new Properties(1, 1, 1, 1),
        ItemType.FlameSword => new Properties(1, 2, 2, 0),
        ItemType.Crown => new Properties(2, 1, 0, 2),
        ItemType.PonderingOrb => new Properties(2, 2, 3, 0),
        ItemType.Crystal => new Properties(1, 2, 2, 2),
        ItemType.Bomb => new Properties(1, 1, 3, 2),
        ItemType.Rope => new Properties(1, 2, 0, 3),
        _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
    };

    public sealed record Properties(int Width, int Height, int SpriteX, int SpriteY)
    {
        public Rect2 SpriteRect => new(
            SpriteX * ItemSlotSize,
            SpriteY * ItemSlotSize,
            Width * ItemSlotSize,
            Height * ItemSlotSize);

        public Vector2 HalfSize = 0.5f * ItemSlotSize * new Vector2(Width, Height);
    }
}

public static class ItemTypeExtensions
{
    // TODO(will): Use actual values for scoring.
    public static string Description(this ItemLibrary.ItemType itemType) => itemType switch
    {
        ItemLibrary.ItemType.Key => "Opens doors",
        ItemLibrary.ItemType.Sword => "Basic Weapon",
        ItemLibrary.ItemType.FlameSword => "Oldest brother of the Lukewarm Shank.",
        ItemLibrary.ItemType.Crown => "Worn by a king with a comically large head.",
        ItemLibrary.ItemType.Potion => "Is it really day drinking if you can't see the sun?",
        ItemLibrary.ItemType.PonderingOrb => "An Orb. Feels Magical. Seems adequate for pondering.",
        ItemLibrary.ItemType.Crystal => "A fine mineral.",
        ItemLibrary.ItemType.Rope => "You never need a rope when you go on an adventure. Except when you do.",
        ItemLibrary.ItemType.Bomb => "Not useless, just extremely situational.",
        _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
    };
    
    // TODO(will): Use actual values for scoring.
    public static int Score(this ItemLibrary.ItemType itemType) => itemType switch
    {
        ItemLibrary.ItemType.Sword => 1,
        ItemLibrary.ItemType.Potion => -1,
        ItemLibrary.ItemType.Key => -10,
        ItemLibrary.ItemType.FlameSword => 10,
        ItemLibrary.ItemType.Crown => 100,
        ItemLibrary.ItemType.Crystal => 10,
        ItemLibrary.ItemType.PonderingOrb => 999,
        _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
    };
}
