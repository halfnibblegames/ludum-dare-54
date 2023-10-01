using System;
using Godot;

public class Character : Node2D
{
    [Signal]
    public delegate void HealthChanged(int newHealth, int maxHealth, int healthChange);

    [Export] public int CurrentHealth { get; protected set; }
    [Export] protected int MaxHealth { get; set; } = 12;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        EmitSignal(nameof(HealthChanged), CurrentHealth, MaxHealth, 0);
    }

    public void Damage(int amountOfDamage)
    {
        if (CurrentHealth == 0)
        {
            return;
        }

        var oldHealth = CurrentHealth;
        CurrentHealth = Math.Max(CurrentHealth - amountOfDamage, 0);
        // TODO: handle death
        EmitSignal(nameof(HealthChanged), CurrentHealth, MaxHealth, CurrentHealth - oldHealth);
    }

    public void Heal(int amountToHeal)
    {
        if (CurrentHealth == MaxHealth)
        {
            return;
        }

        var oldHealth = CurrentHealth;
        CurrentHealth = Math.Min(CurrentHealth + amountToHeal, MaxHealth);
        EmitSignal(nameof(HealthChanged), CurrentHealth, MaxHealth, CurrentHealth - oldHealth);
    }
}
