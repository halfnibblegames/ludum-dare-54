using Godot;

public sealed class TopHud : Control
{
    private const int FillStartingPixel = 17;
    public void UpdateHealth(int newHealth)
    {
        // TODO: I'm not even sure this works???????
        var healthFill = GetNode<NinePatchRect>("HealthFill");
        var oldHealth = healthFill.MarginRight - FillStartingPixel;
        healthFill.MarginRight = FillStartingPixel + newHealth;
        if (oldHealth > newHealth)
        {
            // TODO: SHAKE THE SCREEN
        }
        else
        {
            // TODO: SPARKLES AND PARTICLES IDK MAKE IT PRETTY
        }
    }

    public void UpdateScore(int newScore)
    {
        var scoreLabel = GetNode<Label>("Score");
        scoreLabel.Text = $"Score: {newScore}";
    }
}
