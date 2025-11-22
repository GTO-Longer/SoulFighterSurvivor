using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DataManagement
{
    [System.Serializable]
    public class HeroConfig
    {
        public string heroName;
        
        // 基础属性
        public float _baseMaxHealthPoint;
        public float _baseMaxMagicPoint;
        public float _baseAttackDamage;
        public float _baseAttackSpeed;
        public float _baseAttackDefense;
        public float _baseMagicDefense;
        public float _baseHealthRegeneration;
        public float _baseMagicRegeneration;
        public float _baseAttackRange;
        public float _baseMovementSpeed;
        public float _baseScale;

        // 成长属性
        public float _maxHealthPointGrowth;
        public float _maxMagicPointGrowth;
        public float _attackDamageGrowth;
        public float _attackSpeedGrowth;
        public float _attackDefenseGrowth;
        public float _magicDefenseGrowth;
        public float _healthRegenerationGrowth;
        public float _magicRegenerationGrowth;
        
        // 技能名
        public string _passiveSkill;
        public string _QSkill;
        public string _WSkill;
        public string _ESkill;
        public string _RSkill;
    }

    [System.Serializable]
    internal class HeroConfigCollection
    {
        public HeroConfig[] heroes;
    }

    [System.Serializable]
    public class SkillConfig
    {
        public string id;
        public string heroName;
        public string skillName;
        
        public string _skillDescription;
        public string _skillType;
        public float[] _baseSkillCost;
        public float[] _baseSkillCoolDown;
        public float[][] _baseSkillValue;
        public float _skillRange;
        
        public string[] _skillBulletType;
        public string[] _skillUsageType;
    }

    [System.Serializable]
    internal class SkillConfigCollection
    {
        public SkillConfig[] skills;
    }

    public static class ConfigReader
    {
        // === Hero Config ===
        private static Dictionary<string, HeroConfig> _heroConfigMap;

        private static void LoadAllHeroConfigs()
        {
            if (_heroConfigMap != null) return;

            var jsonFile = Resources.Load<TextAsset>("Configs/HeroConfig");
            if (jsonFile == null)
            {
                Debug.LogError("HeroConfig.json not found in Resources/Configs/");
                _heroConfigMap = new Dictionary<string, HeroConfig>();
                return;
            }

            try
            {
                var collection = JsonUtility.FromJson<HeroConfigCollection>(jsonFile.text);
                if (collection?.heroes == null || collection.heroes.Length == 0)
                {
                    Debug.LogError("HeroConfig.json is empty or missing 'heroes' array.");
                    _heroConfigMap = new Dictionary<string, HeroConfig>();
                    return;
                }

                _heroConfigMap = collection.heroes.ToDictionary(
                    h => h.heroName,
                    h => h,
                    StringComparer.OrdinalIgnoreCase
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse HeroConfig.json: {ex}");
                _heroConfigMap = new Dictionary<string, HeroConfig>();
            }
        }

        /// <summary>
        /// 从 Resources/Configs/HeroConfig.json 中读取指定英雄的配置
        /// </summary>
        public static HeroConfig ReadHeroConfig(string heroName)
        {
            LoadAllHeroConfigs();

            if (string.IsNullOrEmpty(heroName))
            {
                Debug.LogWarning("ReadHeroConfig called with null or empty heroName.");
                return null;
            }

            if (_heroConfigMap.TryGetValue(heroName, out HeroConfig config))
            {
                return config;
            }

            Debug.LogWarning($"Hero config not found: '{heroName}'. Available heroes: {string.Join(", ", _heroConfigMap.Keys)}");
            return null;
        }

        // === Skill Config ===
        private static Dictionary<string, SkillConfig> _skillConfigMap;

        private static void LoadAllSkillConfigs()
        {
            if (_skillConfigMap != null) return;

            var jsonFile = Resources.Load<TextAsset>("Configs/SkillConfig");
            if (jsonFile == null)
            {
                Debug.LogError("SkillConfig.json not found in Resources/Configs/");
                _skillConfigMap = new Dictionary<string, SkillConfig>();
                return;
            }

            try
            {
                var collection = JsonUtility.FromJson<SkillConfigCollection>(jsonFile.text);
                if (collection?.skills == null || collection.skills.Length == 0)
                {
                    Debug.LogError("SkillConfig.json is empty or missing 'skills' array.");
                    _skillConfigMap = new Dictionary<string, SkillConfig>();
                    return;
                }

                _skillConfigMap = collection.skills
                    .Where(s => !string.IsNullOrEmpty(s.id))
                    .ToDictionary(
                        s => s.id,
                        s => s,
                        StringComparer.OrdinalIgnoreCase
                    );
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse SkillConfig.json: {ex}");
                _skillConfigMap = new Dictionary<string, SkillConfig>();
            }
        }

        /// <summary>
        /// 从 Resources/Configs/SkillConfig.json 中读取指定技能ID的配置
        /// </summary>
        public static SkillConfig ReadSkillConfig(string skillId)
        {
            LoadAllSkillConfigs();

            if (string.IsNullOrEmpty(skillId))
            {
                Debug.LogWarning("ReadSkillConfig called with null or empty skillId.");
                return null;
            }

            if (_skillConfigMap.TryGetValue(skillId, out SkillConfig config))
            {
                return config;
            }

            Debug.LogWarning($"Skill config not found: '{skillId}'. Available skill IDs: {string.Join(", ", _skillConfigMap.Keys)}");
            return null;
        }
    }
}