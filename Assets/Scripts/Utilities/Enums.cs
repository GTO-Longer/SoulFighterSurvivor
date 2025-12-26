using System;

namespace Utilities
{
    public enum EntityType{
        None = 0,
        Hero,
        Enemy
    }

    public enum DamageType
    {
        None = 0,
        AD,
        AP,
        Real
    }

    public enum DataType
    {
        None = 0,
        Float,
        Int,
        Percentage
    }

    public enum SkillType
    {
        None = -1,
        PassiveSkill = 0,
        QSkill,
        WSkill,
        ESkill,
        RSkill,
        DSkill,
        FSkill
    }

    public enum AttributeType
    {
        None = 0,
        AttackDamage,
        AbilityPower,
        AttackDefense,
        MagicDefense,
        AttackSpeed,
        AbilityHaste,
        CriticalRate,
        MovementSpeed,
        HealthRegeneration,
        HealAndShieldPower,
        AttackPenetration,
        MagicPenetration,
        LifeSteel,
        OmniVamp,
        AttackRange,
        CriticalDamage,
    }

    public enum EquipmentAttributeType
    {
        None = 0,
        maxHealthPoint,
        percentageMaxHealthPoint,
        maxMagicPoint,
        percentageMaxMagicPoint,
        attackSpeed,
        percentageAttackSpeed,
        attackDamage,
        percentageAttackDamage,
        abilityPower,
        percentageAbilityPower,
        abilityHaste,
        percentageAbilityHaste,
        attackDefense,
        percentageAttackDefense,
        magicDefense,
        percentageMagicDefense,
        attackPenetration,
        percentageAttackPenetration,
        magicPenetration,
        percentageMagicPenetration,
        criticalRate,
        criticalDamage,
        movementSpeed,
        percentageMovementSpeed,
        percentageHealthRegeneration,
        percentageMagicRegeneration,
        omnivamp,
        lifeSteal,
        percentageScaleBonus,
        fortune
    }

    public enum EquipmentType
    {
        None = 0,
        Starter,
        Anvil,
        Legend,
        Prismatic,
        Task
    }

    public enum BulletType
    {
        None = 0,
        /// <summary>
        /// 直线型
        /// </summary>
        Straight,
        /// <summary>
        /// 锁定型
        /// </summary>
        Aimed,
        /// <summary>
        /// 弹道
        /// </summary>
        Projectile,
        /// <summary>
        /// 即刻生效
        /// </summary>
        Immediate
    }

    public enum Team
    {
        None = 0,
        Hero,
        Enemy
    }

    public enum EquipmentUniqueEffect
    {
        None = 0,
        Guardian,
        CursedBlade,
        MPBoost,
        MagicPenetration,
        Hydra
    }

    public enum Quality
    {
        None = 0,
        Silver,
        Gold,
        Prismatic
    }

    public enum UsageType
    {
        None = 0,
        Fighter,
        Wizard,
        Tank,
        Assassin,
        Shooter
    }
}