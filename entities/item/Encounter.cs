using Godot;

public sealed class Encounter
{
    private readonly Player player;
    private readonly Hazard hazard;

    public Encounter(Player player, Hazard hazard)
    {
        this.player = player;
        this.hazard = hazard;
    }

    public void DamageHazard(int amount)
    {
        hazard.Damage(amount);
        GD.Print($"Ka-pow! {hazard.Type} reduced to {hazard.CurrentHealth} health");
    }

    public void HealHazard(int amount)
    {
        hazard.Heal(amount);
        GD.Print($"Uh-oh. {hazard.Type} increased to {hazard.CurrentHealth} health");
    }

    public void DamagePlayer(int amount)
    {
        player.Damage(amount);
        GD.Print($"Ouch! Player reduced to {player.CurrentHealth} health");
    }

    public void HealPlayer(int amount)
    {
        player.Heal(amount);
        GD.Print($"Sparkles :3 Player increased to {player.CurrentHealth} health");
    }
}
