using System;
using Components.UI;
using DataManagement;
using UnityEngine;
using UnityEngine.UI;
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
        public Property<int> buffCount = new();
        /// <summary>
        /// buff最大层数
        /// </summary>
        public int buffMaxCount;
        /// <summary>
        /// buff持续时间
        /// </summary>
        public float buffDuration;
        /// <summary>
        /// buff持续时间计时器
        /// </summary>
        public float buffDurationTimer;
        public Property<float> buffLeftDuration;
        public Sprite buffIcon;
        /// <summary>
        /// 获取buff
        /// </summary>
        public Action OnBuffGet;
        /// <summary>
        /// buff失效
        /// </summary>
        public Action OnBuffRunOut;

        public bool isUnique;
        /// <summary>
        /// 是否为灼烧类buff
        /// </summary>
        public bool isBurn;
        /// <summary>
        /// 灼烧类buff的通用效果
        /// </summary>
        public Action<Entity> Burn;

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
            buffLeftDuration = new Property<float>();
            
            buffTimer = (_) =>
            {
                if (buffDuration > 0)
                {
                    buffDurationTimer += Time.deltaTime;
                    buffLeftDuration.Value = buffDurationTimer / buffDuration;
                    
                    if (buffDurationTimer >= buffDuration)
                    {
                        RemoveBuff();
                    }
                }
            };
        }

        public Buff GetBuff(bool repeat = false)
        {
            if (!repeat)
            {
                if (owner.team == Team.Hero)
                {
                    var buffUI = HUDUIRoot.Instance.buffInfo.CreateBuffUI(this);
                    buffUI.GetComponent<Image>().sprite = buffIcon;
                }
                else if(Equals(owner, HUDUIRoot.Instance.targetAttributes.checkTarget.Value))
                {
                    var buffUI = HUDUIRoot.Instance.targetBuffInfo.CreateBuffUI(this);
                    buffUI.GetComponent<Image>().sprite = buffIcon;
                }

                owner.EntityUpdateEvent += buffTimer;
            }

            OnBuffGet?.Invoke();
            return this;
        }

        public void RemoveBuff()
        {
            RemoveBuffUI();
            OnBuffRunOut?.Invoke();
            owner.buffList.Remove(this);
            owner.EntityUpdateEvent -= buffTimer;
            owner = null;
        }

        public void RemoveBuffUI()
        {
            if (owner.team == Team.Hero)
            {
                HUDUIRoot.Instance.buffInfo.DeleteBuffUI(this);
            }
            else
            {
                HUDUIRoot.Instance.targetBuffInfo.DeleteBuffUI(this);
            }
        }
    }
}