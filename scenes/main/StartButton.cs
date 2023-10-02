using Godot;
using System;

public class StartButton : Label
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Console.WriteLine("asd");
    }
    
    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: (int)ButtonList.Left })
            return;

        GetTree().SetInputAsHandled();
        GetTree().ChangeScene("res://scenes/main/main.tscn");
    }
}
