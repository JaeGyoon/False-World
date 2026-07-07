using UnityEngine;

namespace FalseWorld
{
    public enum SceneName
    {
        Bootstrap = 0,
        Lobby = 1,
        Loading = 2,
        Stage = 3,
    }

    public enum EntityType
    {
        None = 0,

        Player = 1,

        Enemy = 2,

        NPC = 3,

        Projectile = 4,

        Weapon = 5,

        Item = 6,

        Skill = 7,
    }

    public enum EntityID
    {
        None = 0,

        Player = 1,

        Slime = 2001,
        SpinySnail = 2002,

    }

    // AI가 어떤 방식으로 행동하는지
    public enum AIBehaviorType
    {
        Passive = 0,

        Aggressive = 1,

        Ranged = 2,

        Charge = 3,

        Boss = 4,

    }

    public enum SpawnState
    {
        Idle = 0,

        Patrol = 1,

        Sleeping = 2,

        Alert = 3,
    }
}


