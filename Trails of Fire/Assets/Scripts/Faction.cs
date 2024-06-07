public enum Faction {Player, Npc, Environment, Enemy};

public static  class FactionHelpers
{
    public static bool IsHostile(this Faction f1, Faction f2)
    {
        switch (f1)
        {
            case Faction.Player:
                return (f2 == Faction.Enemy);
            case Faction.Npc:
                return false;
            case Faction.Environment:
                return (f2 != Faction.Player);
            case Faction.Enemy:
                return (f2 == Faction.Player);
            default:
                break;
        }

        return false;
    }
}