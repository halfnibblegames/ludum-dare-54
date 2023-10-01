using System;
using Godot;

public sealed class TopHud : Control
{
    private const int maxWidth = 58;

    public override void _Ready()
    {

        var audioButton = GetNode<TextureRect>("AudioButton");
        // TODO: Handle clicks here????
    }

    public void UpdateHealth(int newHealth, int maxHealth)
    {
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
