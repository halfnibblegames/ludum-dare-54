using Godot;

public class Player : Node2D
{
    [Signal]
    public delegate void HealthChanged(int newHealth, int maxHealth);

    [Export] private int currentHealth;
    [Export] private int maxHealth = 12;

    public override void _Ready()
    {
        currentHealth = maxHealth;
        EmitSignal(nameof(HealthChanged), currentHealth, maxHealth);
    }

    private void damage(int amountOfDamage)
    {
        currentHealth -= amountOfDamage;
        // TODO: handle death
        EmitSignal(nameof(HealthChanged), currentHealth, maxHealth);
    }
}
