using Godot;

public sealed class Player : Character
{
    [Signal] public delegate void PlayerDied();

    protected override Vector2 Forward => Vector2.Down;

    protected override void OnDeath()
    {
        EmitSignal(nameof(PlayerDied));
    }

    public void ShowPunchButton()
    {
        GetNode<CanvasItem>("PunchButton").Visible = true;
    }

    public void HidePunchButton()
    {
        GetNode<CanvasItem>("PunchButton").Visible = false;
    }

    public void StartWalking()
    {
        GetNode<AnimationPlayer>("Offset/Sprite/AnimationPlayer").Play("default");
        
    }
    
    public void StopWalking()
    {
        GetNode<AnimationPlayer>("Offset/Sprite/AnimationPlayer").Stop();        
    }
}
