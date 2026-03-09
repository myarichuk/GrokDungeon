using DnDGen.RollGen;
using DnDGen.RollGen.IoC;

namespace GrokDungeon.Services;

public class DiceService
{
    private readonly Dice _dice;

    public DiceService()
    {
        _dice = DiceFactory.Create();
    }

    public virtual int Roll(string expression)
    {
        // Handles "1d20", "2d6+3", etc.
        return _dice.Roll(expression).AsSum();
    }

    public bool Check(int bonus, int dc)
    {
        return (Roll("1d20") + bonus) >= dc;
    }
}
