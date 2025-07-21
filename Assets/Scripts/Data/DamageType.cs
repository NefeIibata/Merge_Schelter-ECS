using System;

namespace Data
{
    [Flags]
    public enum DamageType
    {
        None = 0,
        Match = 1 << 0,
        Bonus = 1 << 1 
        // Rocket = 1 << 2, Bomb = 1 << 3
    }
}
