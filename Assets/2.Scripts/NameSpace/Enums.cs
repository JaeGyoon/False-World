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

    /*public enum ObjectID
    {
        None = 0,

        Warrior = 1001,
        Pyromancer = 1002,

        Slime = 2001,
        SpinySnail = 2002,



    }*/

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

    public enum StatModifierType
    {
        Add = 0,
        Multiply = 1,
        Override = 2,
    }

    public enum StatType
    {
        MaxHealth = 0,

        AttackDamage = 1,
        MagicDamage = 2,

        Defence = 3,
        Resistance = 4,

        AttackSpeed = 5,
        MoveSpeed = 6,

        CriticalChance = 7,
        CriticalDamage = 8,

        CooldownReduction = 9,

        LifeSteal = 10,
    }

    public enum StatModifierOrder
    {
        Equipment = 100,

        Passive = 200,

        Buff = 300,

        Debuff = 400,

        Override = 1000
    }

    public enum ItemType
    {
        Equipment = 0,

        Consumable = 1,

        Material = 2,

        Quest = 3,
    }

    public enum EquipmentType
    {
        Weapon = 0,

        Armor = 1,

        Accessory = 2,
    }

    public enum EquipmentSlotType
    {
        Weapon = 0,

        Helmet = 1,

        Armor_Top = 2,
        Armor_Bottom = 3,

        Bag = 4,

        Ring = 5,
        Necklace = 6,
    }

    public enum EquipmentRarityType
    {
        Common = 0,

        Uncommon = 1,

        Rare = 2,

        Epic = 3,

        Legendary = 4,
    }

}
    


