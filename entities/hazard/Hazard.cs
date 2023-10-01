using System;
using Godot;
using static HazardLibrary;

public sealed class Hazard : Character
{
    protected override Vector2 Forward => Vector2.Up;

    private HazardType type = HazardType.Spider;

    [Export]
    public HazardType Type
    {
        get => type;
        set
        {
            type = value;
            MaxHealth = type.MaxHealth();
            CurrentHealth = MaxHealth;
        }
    }

    public void DoTurn(Encounter encounter, Action completed)
    {
        DoAction(() => encounter.DamagePlayer(Type.AttackDamage()), completed);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        var healthPercentage = (float) CurrentHealth / MaxHealth;
        var barWidth = Mathf.RoundToInt(healthPercentage * 46);
        GetNode<NinePatchRect>("HealthFill").MarginRight = barWidth + 17;
    }
}
