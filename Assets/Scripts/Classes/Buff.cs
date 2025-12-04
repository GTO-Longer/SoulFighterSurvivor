using System;
using DataManagement;
using MVVM.ViewModels;
using UnityEngine;
using Utilities;

namespace Classes
{
    public class Buff
    {
        public Entity owner;
        public Entity source;
        public string buffName;
        public string buffDescription;
        /// <summary>
        /// buff层数
        /// </summary>
        protected int buffCount;
        /// <summary>
        /// buff最大层数
        /// </summary>
        protected int buffMaxCount;
        /// <summary>
        /// buff持续时间
        /// </summary>
        public float buffDuration;
        /// <summary>
        /// buff持续时间计时器
        /// </summary>
        public float buffDurationTimer;
        public Sprite buffIcon;
        /// <summary>
        /// 获取buff
        /// </summary>
        public Action OnBuffGet;
        /// <summary>
        /// buff生效
        /// </summary>
        public Action OnBuffEffect;
        /// <summary>
        /// buff失效
        /// </summary>
        public Action OnBuffRunOut;

        public bool isUnique;

        private Action<Entity> buffTimer;

        /// <summary>
        /// Buff构造函数
        /// </summary>
        /// <param name="ownerEntity">buff拥有者</param>
        /// <param name="sourceEntity">buff来源</param>
        /// <param name="name">buff名称</param>
        /// <param name="description">buff描述</param>
        /// <param name="maxCount">buff最大层数</param>
        /// <param name="duration">buff持续时间</param>
        public Buff(Entity ownerEntity, Entity sourceEntity, string name, string description, int maxCount, float duration)
        {
            owner = ownerEntity;
            source = sourceEntity;
            buffName = name;
            buffDescription = description;
            buffMaxCount = maxCount;
            buffDuration = duration;
            buffDurationTimer = 0;
            
            buffTimer = (_) =>
            {
                if (buffDuration > 0)
                {
                    buffDurationTimer += Time.deltaTime;
                    if (buffDurationTimer >= buffDuration)
                    {
                        RemoveBuff();
                    }
                }
            };

            owner.EntityUpdateEvent += buffTimer;
        }

        public Buff GetBuff()
        {
            if (owner.team == Team.Hero)
            {
                HeroAttributeViewModel.Instance.CreateBuffUI(this);
            }
            
            OnBuffGet?.Invoke();
            return this;
        }

        public void RemoveBuff()
        {
            if (owner.team == Team.Hero)
            {
                RemoveBuffUI();
            }
            OnBuffRunOut?.Invoke();
            owner.buffList.Remove(this);
            owner.EntityUpdateEvent -= buffTimer;
            owner = null;
        }

        public void RemoveBuffUI()
        {
            HeroAttributeViewModel.Instance.DeleteBuffUI(this);
        }
    }
}