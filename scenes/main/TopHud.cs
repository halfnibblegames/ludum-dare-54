using System;
using Godot;

public sealed class TopHud : Control
{
    private const int maxWidth = 58;

    public void UpdateHealth(int newHealth, int maxHealth)
    {
        // TODO: I'm not even sure this works???????
        var healthFill = GetNode<NinePatchRect>("HealthFill");
        var percentage = (float) newHealth / maxHealth;
        var pixelWidth = (int) Math.Round(percentage * maxWidth);
        healthFill.RectSize = new Vector2(pixelWidth, healthFill.RectSize.y);
    }

    public void UpdateScore(int newScore)
    {
        var scoreLabel = GetNode<Label>("Score");
        scoreLabel.Text = $"Score: {newScore}";
    }
}
