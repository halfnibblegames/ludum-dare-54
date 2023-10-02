using System;
using Godot;
using static HazardLibrary;

public sealed class Hazard : Character
{
    [Export]
    private Texture[] MonsterTextures = Array.Empty<Texture>();

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
            if (MonsterTextures.Length > 0)
            {
                GetNode<Sprite>("Offset/Sprite").Texture = MonsterTextures[type.IndexOfTexture()];
            }
        }
    }

    public void DoTurn(Encounter encounter, Action completed)
    {
        DoAction(() => encounter.DamagePlayer(Type.AttackDamage()), completed);
    }

    public override void _Ready()
    {
        GetNode<Sprite>("Offset/Sprite").Texture = MonsterTextures[type.IndexOfTexture()];
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        var healthPercentage = (float) CurrentHealth / MaxHealth;
        var barWidth = Mathf.RoundToInt(healthPercentage * 46);
        GetNode<NinePatchRect>("HealthFill").MarginRight = barWidth + 17;
    }
}
