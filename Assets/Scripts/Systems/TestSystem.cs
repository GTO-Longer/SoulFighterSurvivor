using System.Linq;
using Classes.Buffs;
using DataManagement;
using Managers.EntityManagers;
using UnityEngine;

namespace Systems
{
    public class TestSystem : MonoBehaviour
    {
        public static TestSystem Instance { get; private set; }
        public bool InfinityMagicPoint;
        public bool InfinityHealthPoint;
        public bool SkillNoCoolDown;
        public bool LevelUp;
        public bool Gain20000Coin;
        public bool PathFindTest;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (InfinityMagicPoint)
            {
                HeroManager.hero.TakeMagicRecover(999);
            }
            
            if (InfinityHealthPoint)
            {
                HeroManager.hero.healthPoint.Value = HeroManager.hero.maxHealthPoint.Value;
            }
            
            if (SkillNoCoolDown)
            {
                foreach (var skill in HeroManager.hero.skillList)
                {
                    skill.coolDownTimer = 999f;
                }
            }

            if (LevelUp)
            {
                HeroManager.hero.LevelUp();
                LevelUp = false;
            }

            if (Gain20000Coin)
            {
                HeroManager.hero.coins.Value += 20000;
                Gain20000Coin = false;
            }

            if (PathFindTest)
            {
                var entities = GameObject.FindObjectsOfType<EntityData>()
                    .Select(e => e.entity)
                    .Where(e => e != null && e.isAlive)
                    .ToList();

                foreach (var entity in entities)
                {
                    var ghostPoro = new GhostPoro(entity, entity);
                    ghostPoro.buffIcon = null;
                    entity.GainBuff(ghostPoro);
                    
                    for (var j = 0; j < 20000 / Mathf.Max(1, entities.Count - 1); j++)
                    {
                        var x = Mathf.Sin(j * 0.001f) * Mathf.Cos(j * 0.002f) * Mathf.Asin(entities.Count * 0.1145141919810f);
                    }
                }
            }
        }
    }
}