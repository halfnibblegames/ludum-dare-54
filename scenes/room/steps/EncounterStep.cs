using System;
using Godot;

public sealed class EncounterStep : IRoomStep
{
    public void Do(Node roomNode, Templates templates, Action complete)
    {
        complete();
    }
}
