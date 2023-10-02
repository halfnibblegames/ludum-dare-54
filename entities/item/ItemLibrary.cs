using System;
using Godot;
using static Constants;
using static CombatUses;
using static ItemLibrary;
using static ItemLibrary.ItemType;

public static class ItemLibrary
{
    public enum ItemType
    {
        Sword,
        Potion,
        Key,
        Excalibur,
        Crown,
        Orb,
        Crystal,
        Rope,
        Bomb
    }

    public static ItemType RandomItem()
    {
        var types = Enum.GetValues(typeof(ItemType));
        return (ItemType) types.GetValue(GD.Randi() % types.Length);
    }

    public static Properties Resolve(ItemType itemType) => itemType switch
    {
        Sword => new Properties(1, 2, 0, 0),
        Potion => new Properties(1, 1, 1, 0),
        Key => new Properties(1, 1, 1, 1),
        Excalibur => new Properties(1, 2, 0, 0),
        Crown => new Properties(2, 1, 0, 2),
        Orb => new Properties(2, 2, 3, 0),
        Crystal => new Properties(1, 2, 2, 2),
        Bomb => new Properties(1, 1, 3, 2),
        Rope => new Properties(1, 2, 0, 3),
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
    public static string Description(this ItemType itemType) => itemType switch
    {
        Key => "Opens doors",
        Sword => "Basic Weapon",
        Excalibur => "Look, we really had no time to draw a different sword.",
        Crown => "Worn by a king with a comically large head.",
        Potion => "Is it really day drinking if you can't see the sun?",
        Orb => "An Orb. Feels Magical. Seems adequate for pondering.",
        Crystal => "A fine mineral.",
        Rope => "You never need a rope when you go on an adventure. Except when you do.",
        Bomb => "Not useless, just extremely situational.",
        _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
    };

    // TODO(will): Use actual values for scoring.
    public static int Score(this ItemType itemType) => itemType switch
    {
        Sword => 1,
        Potion => 0,
        Key => -10,
        Excalibur => 99,
        Crown => 99,
        Orb => 999,
        _ => 0
    };

    public static ICombatUse? CombatUse(this ItemType itemType) => itemType switch
    {
        Sword => DamageHazard(4),
        Excalibur => DamageHazard(99),
        Potion => HealPlayer(10),
        Bomb => Compose(DamageHazard(8), DamagePlayer(3)), // TODO(tom): consider randomized damage
        _ => null
    };

    public static int Durability(this ItemType itemType) => itemType switch
    {
        Sword => 7,
        Excalibur => 3,
        Potion => 3,
        _ => 1
    };
}
