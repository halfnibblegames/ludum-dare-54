using Godot;

public sealed class InputMouseHint : Node2D
{
    private bool ready;
    private bool primed;

    public override void _Ready()
    {
        ready = true;
        if (primed)
        {
            Prime();
            primed = false;
        }
    }

    public void Prime()
    {
        if (!ready)
        {
            primed = true;
            return;
        }

        Reset();
        GetNode<Timer>("Timer").Start();
        Visible = false;
    }

    public void Reset()
    {
        GetNode<Timer>("Timer").Stop();
        Visible = false;
    }

    private void onTimerFinished()
    {
        Visible = true;
    }
}
