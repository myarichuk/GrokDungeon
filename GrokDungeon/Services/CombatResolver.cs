using DefaultEcs;
using GrokDungeon.Models;

namespace GrokDungeon.Services;

public class CombatResolver
{
    private readonly DiceService _dice;

    public CombatResolver(DiceService dice)
    {
        _dice = dice;
    }

    public string ResolveAttack(Entity attacker, Entity defender)
    {
        if (!attacker.Has<StatsComponent>() || !defender.Has<ArmorClassComponent>())
            return "Combat invalid: missing stats.";

        var attStats = attacker.Get<StatsComponent>();
        var defAc = defender.Get<ArmorClassComponent>().Value;
        
        // Calculate modifier (Score - 10) / 2
        int strMod = (attStats.Strength - 10) / 2;
        
        int attackRoll = _dice.Roll("1d20");
        int totalHit = attackRoll + strMod;

        string weaponName = attacker.Has<WeaponComponent>() ? attacker.Get<WeaponComponent>().Name : "fists";
        string damageDice = attacker.Has<WeaponComponent>() ? attacker.Get<WeaponComponent>().DamageDice : "1d4";

        if (attackRoll == 20) // Crit
        {
            int damage = _dice.Roll(damageDice) + _dice.Roll(damageDice) + strMod;
            ApplyDamage(defender, damage);
            return $"CRITICAL HIT! {GetName(attacker)} strikes {GetName(defender)} with {weaponName} for {damage} damage!";
        }
        else if (attackRoll == 1)
        {
            return $"CRITICAL MISS! {GetName(attacker)} fumbles their attack.";
        }
        else if (totalHit >= defAc)
        {
            int damage = _dice.Roll(damageDice) + strMod;
            ApplyDamage(defender, damage);
            return $"{GetName(attacker)} hits {GetName(defender)} with {weaponName} for {damage} damage.";
        }
        else
        {
            return $"{GetName(attacker)} attacks {GetName(defender)} but misses (Rolled {totalHit} vs AC {defAc}).";
        }
    }

    private void ApplyDamage(Entity entity, int amount)
    {
        if (entity.Has<HealthComponent>())
        {
            var hp = entity.Get<HealthComponent>();
            hp.Current -= amount;
            if (hp.Current < 0) hp.Current = 0;
            entity.Set(hp);
        }
    }

    private string GetName(Entity e) => e.Has<NameComponent>() ? e.Get<NameComponent>().Value : "Unknown";
}