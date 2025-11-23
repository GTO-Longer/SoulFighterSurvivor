using System;
using System.Collections;
using System.Collections.Generic;
using Classes;
using UnityEngine;

namespace Systems
{
    public class EventSystem
    {
        // 持续更新事件
        public event Action<Entity, Entity> EntityUpdateEvent;
        public void EntityUpdate(Entity owner)
        {
            EntityUpdateEvent?.Invoke(owner, null);
        }
        
        // Q技能释放事件
        public event Action<Entity, Entity> OnQSkillRelease;
        public void QSkillRelease(Entity owner, Entity target)
        {
            OnQSkillRelease?.Invoke(owner, target);
        }
        
        // W技能释放事件
        public event Action<Entity, Entity> OnWSkillRelease;
        public void WSkillRelease(Entity owner, Entity target)
        {
            OnWSkillRelease?.Invoke(owner, target);
        }
        
        // E技能释放事件
        public event Action<Entity, Entity> OnESkillRelease;
        public void ESkillRelease(Entity owner, Entity target)
        {
            OnESkillRelease?.Invoke(owner, target);
        }
        
        // R技能释放事件
        public event Action<Entity, Entity> OnRSkillRelease;
        public void RSkillRelease(Entity owner, Entity target)
        {
            OnRSkillRelease?.Invoke(owner, target);
        }
        
        // 击杀单位
        public event Action<Entity, Entity> OnKillEntity;
        public void KillEntity(Entity owner, Entity target)
        {
            OnKillEntity?.Invoke(owner, target);
        }
        
        // 技能命中
        public event Action<Entity, Entity> OnSkillHit;
        public void SkillHit(Entity owner, Entity target)
        {
            OnSkillHit?.Invoke(owner, target);
        }
    }
}
