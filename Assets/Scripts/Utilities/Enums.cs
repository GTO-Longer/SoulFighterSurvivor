using System;

namespace Utilities
{
    public enum EntityType{
        None = 0,
        Hero,
        Enemy
    }

    public enum DataType
    {
        None = 0,
        Float,
        Int,
        Percentage
    }

    [Serializable]
    public enum SkillType
    {
        None = -1,
        PassiveSkill = 0,
        QSkill,
        WSkill,
        ESkill,
        RSkill,
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

    public enum SkillUsageType
    {
        None = 0,
        /// <summary>
        /// 控制
        /// </summary>
        Control,
        /// <summary>
        /// 伤害
        /// </summary>
        Damage,
        /// <summary>
        /// 增益
        /// </summary>
        Buff,
        /// <summary>
        /// 位移
        /// </summary>
        Displacement,
        /// <summary>
        /// 负面效果
        /// </summary>
        Debuff,
        /// <summary>
        /// 数值提升
        /// </summary>
        Stat
    }

    public enum Team
    {
        None = 0,
        Hero,
        Enemy
    }
}