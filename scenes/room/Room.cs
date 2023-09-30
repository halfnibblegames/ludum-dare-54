using Godot;
using System;

public sealed class Room : Node2D
{
    [Signal]
    public delegate void ItemDropped(ItemLibrary.ItemType type);

    public override void _Ready()
    {

    }
}
