using Godot;

public sealed class Player : Character
{
    [Signal] public delegate void PlayerDied();

    protected override Vector2 Forward => Vector2.Down;

    protected override void OnDeath()
    {
        EmitSignal(nameof(PlayerDied));
    }
}
