using System;
using Godot;
using static Constants;

public static class ItemLibrary
{
    public enum Key
    {
        Sword,
        Potion,
        Key
    }

    public static Properties Resolve(Key key) => key switch
    {
        Key.Sword => new Properties(1, 2, 0, 0),
        Key.Potion => new Properties(1, 1, 1, 0),
        Key.Key => new Properties(1, 1, 1, 1),
        _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
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
