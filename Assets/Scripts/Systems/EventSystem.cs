using System;
using Classes;
using Managers.EntityManagers;

namespace Systems
{
    public static class EventSystem
    {
        // Q技能释放事件
        public static event Action<Entity, Entity> OnQSkillRelease;
        public static void QSkillRelease(Entity owner = null, Entity target = null)
        {
            owner ??= HeroManager.hero;
            OnQSkillRelease?.Invoke(owner, target);
        }
        
        // W技能释放事件
        public static event Action<Entity, Entity> OnWSkillRelease;
        public static void WSkillRelease(Entity owner = null, Entity target = null)
        {
            owner ??= HeroManager.hero;
            OnWSkillRelease?.Invoke(owner, target);
        }
        
        // E技能释放事件
        public static event Action<Entity, Entity> OnESkillRelease;
        public static void ESkillRelease(Entity owner = null, Entity target = null)
        {
            owner ??= HeroManager.hero;
            OnESkillRelease?.Invoke(owner, target);
        }
        
        // R技能释放事件
        public static event Action<Entity, Entity> OnRSkillRelease;
        public static void RSkillRelease(Entity owner = null, Entity target = null)
        {
            owner ??= HeroManager.hero;
            OnRSkillRelease?.Invoke(owner, target);
        }
    }
}
