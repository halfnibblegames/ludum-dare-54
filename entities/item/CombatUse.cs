using System.Linq;

public interface ICombatUse
{
    public string Description { get; }
    public void Do(Encounter encounter);
}

public static class CombatUses
{
    public static ICombatUse Compose(params ICombatUse[] uses) => new CompositeCombatUse(uses);

    public static ICombatUse DamageHazard(int amount) => new DamageHazardCombatUse(amount);
    public static ICombatUse DamagePlayer(int amount) => new DamagePlayerCombatUse(amount);
    public static ICombatUse HealPlayer(int amount) => new HealPlayerCombatUse(amount);
}

sealed class CompositeCombatUse : ICombatUse
{
    private readonly ICombatUse[] uses;

    public string Description => string.Concat("; ", uses.Select(u => u.Description));

    public CompositeCombatUse(ICombatUse[] uses)
    {
        this.uses = uses;
    }

    public void Do(Encounter encounter)
    {
        foreach (var u in uses)
        {
            u.Do(encounter);
        }
    }
}

sealed class DamageHazardCombatUse : ICombatUse
{
    private readonly int amount;

    public string Description => $"Deal {amount} damage";

    public DamageHazardCombatUse(int amount)
    {
        this.amount = amount;
    }

    public void Do(Encounter encounter)
    {
        encounter.DamageHazard(amount);
    }
}

sealed class DamagePlayerCombatUse : ICombatUse
{
    private readonly int amount;

    public string Description => $"Deal {amount} damage to yourself";

    public DamagePlayerCombatUse(int amount)
    {
        this.amount = amount;
    }

    public void Do(Encounter encounter)
    {
        encounter.DamagePlayer(amount);
    }
}

sealed class HealPlayerCombatUse : ICombatUse
{
    private readonly int amount;

    public string Description => $"Heal {amount} damage";

    public HealPlayerCombatUse(int amount)
    {
        this.amount = amount;
    }

    public void Do(Encounter encounter)
    {
        encounter.HealPlayer(amount);
    }
}
