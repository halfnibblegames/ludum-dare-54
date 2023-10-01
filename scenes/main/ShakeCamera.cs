using Godot;

public sealed class ShakeCamera : Camera2D
{
    private const float maxShakeOffset = 5;
    private const float noiseSpeed = 60;
    private const float decay = 0.02f;

    private readonly OpenSimplexNoise noise = new();
    private float amount = 0;
    private float t;

    public override void _Ready()
    {
        GD.Randomize();
        noise.Seed = (int) GD.Randi();
        noise.Period = 4;
        noise.Octaves = 2;
    }

    public override void _Process(float delta)
    {
        amount *= Mathf.Pow(decay, delta);
        if (amount < 0.001f)
        {
            amount = 0;
            // Avoid t getting too big by just resetting it.
            t = 0;
            return;
        }

        t += noiseSpeed * delta;

        Offset = new Vector2(
            maxShakeOffset * amount * noise.GetNoise1d(t),
            maxShakeOffset * amount * noise.GetNoise2d(noise.Seed, t));
    }

    public void Shake(float intensity)
    {
        amount = Mathf.Min(amount + intensity, 1);
    }
}
