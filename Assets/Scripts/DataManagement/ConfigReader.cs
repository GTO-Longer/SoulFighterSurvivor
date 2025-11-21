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
    }

    [System.Serializable]
    internal class HeroConfigCollection
    {
        public HeroConfig[] heroes;
    }

    public static class ConfigReader
    {
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

                // 构建 heroName -> config 的字典，用于快速查找
                _heroConfigMap = collection.heroes.ToDictionary(
                    h => h.heroName,
                    h => h,
                    StringComparer.OrdinalIgnoreCase
                );
            }
            catch (System.Exception ex)
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
    }
}