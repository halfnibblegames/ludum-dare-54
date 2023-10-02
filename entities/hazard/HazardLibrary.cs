using System;
using Godot;
using static HazardLibrary;
using static HazardLibrary.HazardType;

public static class HazardLibrary
{
    public enum HazardType
    {
        Spider,
        Slime,
        Beholder,
        OvergrownVines
    }

    public static HazardType RandomHazard()
    {
        var types = Enum.GetValues(typeof(HazardType));
        return (HazardType) types.GetValue(GD.Randi() % types.Length);
    }
}

public static class HazardTypeExtensions
{
    public static int MaxHealth(this HazardType hazardType) => hazardType switch
    {
        Spider => 6,
        Slime => 8,
        Beholder => 12,
        OvergrownVines => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(hazardType), hazardType, null)
    };

    public static int AttackDamage(this HazardType hazardType) => hazardType switch
    {
        Spider => 3,
        Slime => 2,
        Beholder => 3,
        OvergrownVines => 0,
        _ => throw new ArgumentOutOfRangeException(nameof(hazardType), hazardType, null)
    };

    public static int Score(this HazardType hazardType) => hazardType switch
    {
        Spider => 8,
        Slime => 5,
        Beholder => 12,
        OvergrownVines => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(hazardType), hazardType, null)
    };

    public static int IndexOfTexture(this HazardType hazardType) => hazardType switch
    {
        Spider => 0,
        Slime => 0,
        Beholder => 1,
        OvergrownVines => 0,
        _ => throw new ArgumentOutOfRangeException(nameof(hazardType), hazardType, null)
    };
}
