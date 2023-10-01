using Godot;
using static HazardLibrary;

public sealed class Hazard : Character
{
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

    public void DoTurn(Encounter encounter)
    {
        encounter.DamagePlayer(Type.AttackDamage());
    }
}
