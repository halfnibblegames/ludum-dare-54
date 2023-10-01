using System;
using Godot;

public class Character : Node2D
{
    [Signal]
    public delegate void HealthChanged(int newHealth, int maxHealth);

    [Export] public int CurrentHealth { get; protected set; }
    [Export] protected int MaxHealth { get; set; } = 12;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        EmitSignal(nameof(HealthChanged), CurrentHealth, MaxHealth);
    }

    public void Damage(int amountOfDamage)
    {
        if (CurrentHealth == 0)
        {
            return;
        }
        CurrentHealth = Math.Max(CurrentHealth - amountOfDamage, 0);
        // TODO: handle death
        EmitSignal(nameof(HealthChanged), CurrentHealth, MaxHealth);
    }

    public void Heal(int amountToHeal)
    {
        if (CurrentHealth == MaxHealth)
        {
            return;
        }
        CurrentHealth = Math.Min(CurrentHealth + amountToHeal, MaxHealth);
        EmitSignal(nameof(HealthChanged), CurrentHealth, MaxHealth);
    }
}
