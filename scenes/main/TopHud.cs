using System;
using Godot;

public sealed class TopHud : Control
{
    private const int maxWidth = 58;

    private int health;
    private int maxHealth;

    public override void _Ready()
    {
        UpdateScore(0);
    }

    public void UpdateHealth(int newHealth, int newMaxHealth)
    {
        health = newHealth;
        maxHealth = newMaxHealth;

        updateBarWidth(GetNode<NinePatchRect>("HealthFill"), health);
        updateBarWidth(GetNode<NinePatchRect>("HealthPreview"), health);
    }

    public void PreviewDamage(int amount)
    {
        updateBarWidth(GetNode<NinePatchRect>("HealthFill"), Math.Max(0, health - amount));
    }

    public void DiscardDamagePreview()
    {
        updateBarWidth(GetNode<NinePatchRect>("HealthFill"), health);
    }

    private void updateBarWidth(Control rect, int h)
    {
        var percentage = (float) h / maxHealth;
        var pixelWidth = (int) Math.Round(percentage * maxWidth);
        rect.RectSize = new Vector2(pixelWidth, rect.RectSize.y);
    }

    public void UpdateScore(int newScore)
    {
        var scoreLabel = GetNode<Label>("Score");
        scoreLabel.Text = $"Score: {newScore}";
    }
}
