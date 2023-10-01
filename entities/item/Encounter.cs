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
        hazard.TakeDamage(amount);
    }

    public void HealHazard(int amount)
    {
        hazard.Heal(amount);
    }

    public void DamagePlayer(int amount)
    {
        player.TakeDamage(amount);
    }

    public void HealPlayer(int amount)
    {
        player.Heal(amount);
    }
}
