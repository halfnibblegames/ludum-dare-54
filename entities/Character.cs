using System;
using Godot;

public abstract class Character : Node2D
{
    private const float rednessDecayPerSecond = 4;
    private const float rednessStep = 0.333f;
    private const float moveDuration = 0.1f;
    private const float moveDistance = 3;
    private const float pauseDuration = 0.4f;

    [Signal]
    public delegate void HealthChanged(int newHealth, int maxHealth, int healthChange);

    protected abstract Vector2 Forward { get; }

    [Export] public int CurrentHealth { get; protected set; }
    [Export] protected int MaxHealth { get; set; } = 12;

    private CurrentAction? currentAction;
    private Node2D? offset;
    private float redness;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        EmitSignal(nameof(HealthChanged), CurrentHealth, MaxHealth, 0);
        offset = GetNodeOrNull<Node2D>("Offset");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        updateRedness(delta);
        updateCurrentAction(delta);
    }

    private void updateRedness(float delta)
    {
        redness = Math.Max(0, redness - delta * rednessDecayPerSecond);
        if (offset is not null)
        {
            var steppedRedness = rednessStep * Mathf.Round(redness / rednessStep);
            var color = new Color(1, 1 - steppedRedness, 1 - steppedRedness);
            offset.Modulate = color;
        }
    }

    private void updateCurrentAction(float delta)
    {
        currentAction?.Update(delta);
        if (currentAction?.Completed ?? false)
        {
            currentAction = null;
        }

        if (offset is not null)
        {
            offset.Position = currentAction is null ? Vector2.Zero : currentAction.Forwardness * Forward * moveDistance;
        }
    }

    public void DoAction(Action @do, Action complete)
    {
        if (currentAction is not null)
        {
            throw new InvalidOperationException();
        }

        currentAction = new CurrentAction(@do, complete);
    }

    public void TakeDamage(int amountOfDamage)
    {
        if (CurrentHealth == 0)
        {
            return;
        }

        var oldHealth = CurrentHealth;
        CurrentHealth = Math.Max(CurrentHealth - amountOfDamage, 0);
        redness = 1;
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

    private sealed class CurrentAction
    {
        private readonly Action @do;
        private readonly Action complete;

        private bool actionDone;
        private float pausedForSeconds;

        public bool Completed { get; private set; }
        public float Forwardness { get; private set; }

        public CurrentAction(Action @do, Action complete)
        {
            this.@do = @do;
            this.complete = complete;
        }

        public void Update(float delta)
        {
            if (Completed) return;

            if (pausedForSeconds > 0)
            {
                pausedForSeconds -= delta;
                if (pausedForSeconds > 0)
                {
                    return;
                }
            }

            if (actionDone)
            {
                Forwardness = Mathf.Max(0, Forwardness - delta / moveDuration);
                if (Forwardness <= 0)
                {
                    complete();
                    Completed = true;
                }
            }
            else
            {
                Forwardness = Mathf.Min(1, Forwardness + delta / moveDuration);
                if (Forwardness >= 1)
                {
                    @do();
                    pausedForSeconds = pauseDuration;
                    actionDone = true;
                }
            }
        }
    }
}
