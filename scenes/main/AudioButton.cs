using Godot;

public sealed class AudioButton : TextureRect
{
    private AudioStreamPlayer? audio;
    private AnimationPlayer? animation;

    private bool muted;

    [Export]
    public bool Muted
    {
        get => muted;
        set
        {
            if (muted == value) return;
            muted = value;

            Character.ShouldPlaySound = !muted;
            
            if (muted)
            {
                audio?.Stop();
                animation?.Play("default");
            }
            else
            {
                audio?.Play();
                animation?.PlayBackwards("default");
            }
        }
    }

    public override void _Ready()
    {
        audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        animation = GetNode<AnimationPlayer>("AnimationPlayer");
        if (muted)
        {
            audio?.Stop();
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: (int) ButtonList.Left })
        {
            Muted = !Muted;
            GetTree().SetInputAsHandled();
        }
    }
}
