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
        OvergrownVines => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(hazardType), hazardType, null)
    };

    public static int AttackDamage(this HazardType hazardType) => hazardType switch
    {
        Spider => 100,
        Slime => 100,
        OvergrownVines => 0,
        _ => throw new ArgumentOutOfRangeException(nameof(hazardType), hazardType, null)
    };
}
