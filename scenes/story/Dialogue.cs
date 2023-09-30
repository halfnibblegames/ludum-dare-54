using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Dialogue : Node2D
{
    public async Task DisplayDialog(IReadOnlyList<Sentence> dialog)
    {
        var text = GetNode<Label>("container/text");
        
        foreach (var sentence in dialog)
        {
            // TODO(will): Change portrait if we ever get this far
            text.PercentVisible = 0;
            text.Text = sentence.Text;

            do
            {
                text.PercentVisible = Math.Min(text.PercentVisible + 0.01f, 1.0f);
                await ToSignal(GetTree(), "process_frame");
            } while (text.PercentVisible < 1.0f);
            
            // TODO(will): Wait for player input
        }
    }
}

public sealed record Sentence(
    Portrait Speaker,
    string Text
);

public enum Portrait
{
    Player
}
