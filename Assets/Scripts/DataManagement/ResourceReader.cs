using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Utilities;

namespace DataManagement
{
    [Serializable]
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
        public float _attackWindUp;
        public float _attackSpeedYield;

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

    [Serializable]
    internal class HeroConfigCollection
    {
        public HeroConfig[] heroes;
    }

    [Serializable]
    public class SkillConfig
    {
        public string id;
        public string heroName;
        public string skillName;

        public string _skillDescription;
        public string _skillType;
        public float[] _baseSkillCost;
        public float[] _baseSkillCoolDown;
        public List<List<float>> _baseSkillValue;
        public float _skillRange;
        public float _castTime;
        public float _bulletWidth;
        public float _bulletSpeed;
        public float _destinationDistance;
    }

    [Serializable]
    internal class SkillConfigCollection
    {
        public SkillConfig[] skills;
    }

    [Serializable]
    public class EquipmentConfig
    {
        public string id;
        public string equipmentName;

        public string _usageDescription;
        public string _equipmentType;
        public int _cost;
        public float _passiveSkillCD;
        public float _activeSkillCD;

        public Dictionary<string, float> _attributeList;

        public string _passiveSkillDescription;
        public string _passiveSkillName;

        public string _activeSkillDescription;
        public string _activeSkillName;

        // JSON 字段（字符串）
        [JsonProperty("_uniqueEffect")]
        private string _uniqueEffectRaw;

        // 外部使用统一枚举
        [JsonIgnore]
        public EquipmentUniqueEffect _uniqueEffect
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_uniqueEffectRaw))
                    return EquipmentUniqueEffect.None;

                if (Enum.TryParse(_uniqueEffectRaw, true, out EquipmentUniqueEffect result))
                    return result;

                return EquipmentUniqueEffect.None;
            }
        }
    }

    [Serializable]
    internal class EquipmentConfigCollection
    {
        public EquipmentConfig[] equipments;
    }
    
    [Serializable]
    public class HexConfig
    {
        public string id;
        public string hexName;
        public string quality;
        public string description;
        public string detail;
        public string iconName;
        public string iconBorderName;
    }

    [Serializable]
    internal class HexConfigCollection
    {
        public HexConfig[] hexes;
    }


    public static class ResourceReader
    {
        private static Dictionary<string, HeroConfig> _heroConfigMap;
        private static Dictionary<string, SkillConfig> _skillConfigMap;
        private static Dictionary<string, EquipmentConfig> _equipmentConfigMap;
        private static Dictionary<string, HexConfig> _hexConfigMap;

        /// <summary>
        /// 加载所有英雄配置
        /// </summary>
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
                var collection = JsonConvert.DeserializeObject<HeroConfigCollection>(jsonFile.text);
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
        /// 读取英雄配置
        /// </summary>
        public static HeroConfig ReadHeroConfig(string heroName)
        {
            if (_heroConfigMap == null) LoadAllHeroConfigs();

            if (string.IsNullOrEmpty(heroName))
            {
                Debug.LogWarning("ReadHeroConfig called with null or empty heroName.");
                return null;
            }

            if (_heroConfigMap.TryGetValue(heroName, out HeroConfig config))
                return config;

            Debug.LogWarning($"Hero config not found: '{heroName}'. Available heroes: {string.Join(", ", _heroConfigMap.Keys)}");
            return null;
        }

        /// <summary>
        /// 加载所有技能配置
        /// </summary>
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
                var collection = JsonConvert.DeserializeObject<SkillConfigCollection>(jsonFile.text);
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
        /// 读取技能配置
        /// </summary>
        public static SkillConfig ReadSkillConfig(string skillId)
        {
            if (_skillConfigMap == null) LoadAllSkillConfigs();

            if (string.IsNullOrEmpty(skillId))
            {
                Debug.LogWarning("ReadSkillConfig called with null or empty skillId.");
                return null;
            }

            if (_skillConfigMap.TryGetValue(skillId, out SkillConfig config))
                return config;

            Debug.LogWarning($"Skill config not found: '{skillId}'. Available skill IDs: {string.Join(", ", _skillConfigMap.Keys)}");
            return null;
        }

        /// <summary>
        /// 加载全部装备配置
        /// </summary>
        private static void LoadAllEquipmentConfigs()
        {
            if (_equipmentConfigMap != null) return;

            var jsonFile = Resources.Load<TextAsset>("Configs/EquipmentConfig");
            if (jsonFile == null)
            {
                Debug.LogError("EquipmentConfig.json not found in Resources/Configs/");
                _equipmentConfigMap = new Dictionary<string, EquipmentConfig>();
                return;
            }

            try
            {
                var collection = JsonConvert.DeserializeObject<EquipmentConfigCollection>(jsonFile.text);
                if (collection?.equipments == null || collection.equipments.Length == 0)
                {
                    Debug.LogError("EquipmentConfig.json is empty or missing 'equipments' array.");
                    _equipmentConfigMap = new Dictionary<string, EquipmentConfig>();
                    return;
                }

                _equipmentConfigMap = collection.equipments
                    .Where(e => !string.IsNullOrEmpty(e.id))
                    .ToDictionary(
                        e => e.id,
                        e => e,
                        StringComparer.OrdinalIgnoreCase
                    );
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse EquipmentConfig.json: {ex}");
                _equipmentConfigMap = new Dictionary<string, EquipmentConfig>();
            }
        }

        /// <summary>
        /// 读取装备配置
        /// </summary>
        public static EquipmentConfig ReadEquipmentConfig(string equipmentId)
        {
            if (_equipmentConfigMap == null) LoadAllEquipmentConfigs();

            if (string.IsNullOrEmpty(equipmentId))
            {
                Debug.LogWarning("ReadEquipmentConfig called with null or empty equipmentId.");
                return null;
            }

            if (_equipmentConfigMap.TryGetValue(equipmentId, out EquipmentConfig config))
                return config;

            Debug.LogWarning($"Equipment config not found: '{equipmentId}'. Available equipments: {string.Join(", ", _equipmentConfigMap.Keys)}");
            return null;
        }

        /// <summary>
        /// 加载全部海克斯配置
        /// </summary>
        private static void LoadAllHexConfigs()
        {
            if (_hexConfigMap != null) return;

            var jsonFile = Resources.Load<TextAsset>("Configs/HexConfig");
            if (jsonFile == null)
            {
                Debug.LogError("HexConfig.json not found in Resources/Configs/");
                _hexConfigMap = new Dictionary<string, HexConfig>();
                return;
            }

            try
            {
                var collection = JsonConvert.DeserializeObject<HexConfigCollection>(jsonFile.text);
                if (collection?.hexes == null || collection.hexes.Length == 0)
                {
                    Debug.LogError("HexConfig.json is empty or missing 'hexes' array.");
                    _hexConfigMap = new Dictionary<string, HexConfig>();
                    return;
                }

                _hexConfigMap = collection.hexes
                    .Where(h => !string.IsNullOrEmpty(h.id))
                    .ToDictionary(
                        h => h.id,
                        h => h,
                        StringComparer.OrdinalIgnoreCase
                    );
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse HexConfig.json: {ex}");
                _hexConfigMap = new Dictionary<string, HexConfig>();
            }
        }

        /// <summary>
        /// 读取海克斯配置
        /// </summary>
        public static HexConfig ReadHexConfig(string hexId)
        {
            if (_hexConfigMap == null) LoadAllHexConfigs();

            if (string.IsNullOrEmpty(hexId))
            {
                Debug.LogWarning("ReadHexConfig called with null or empty hexId.");
                return null;
            }

            if (_hexConfigMap.TryGetValue(hexId, out HexConfig config))
                return config;

            Debug.LogWarning($"Hex config not found: '{hexId}'. Available hexes: {string.Join(", ", _hexConfigMap.Keys)}");
            return null;
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        public static Sprite LoadImage(string imagePath, string subname)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                Debug.LogWarning("LoadImage called with null or empty imagePath.");
                return null;
            }

            // 加载整张图所有子Sprite
            var sprites = Resources.LoadAll<Sprite>(imagePath);
            if (sprites == null || sprites.Length == 0)
            {
                Debug.LogWarning($"No sprites found at path: {imagePath}");
                return null;
            }

            // 如果subname为空，直接返回第一个 sprite
            if (string.IsNullOrEmpty(subname))
            {
                return sprites[0];
            }

            // 匹配子 Sprite
            foreach (var sprite in sprites)
            {
                if (sprite.name == subname)
                {
                    return sprite;
                }
            }

            Debug.LogError($"Sprite with name '{subname}' not found in '{imagePath}'.");
            return null;
        }

        /// <summary>
        /// 加载图标
        /// </summary>
        public static Sprite LoadIcon(string name, string subname = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("LoadIcon called with null or empty name.");
                return null;
            }

            var iconName = $"{name}_Icon";
            var imagePath = $"Sprites/UI/Icons/{iconName}";

            return LoadImage(imagePath, subname);
        }
        
        /// <summary>
        /// 加载材质
        /// </summary>
        public static Material LoadMaterial(string materialName)
        {
            if (string.IsNullOrEmpty(materialName))
            {
                Debug.LogWarning("LoadMaterial called with null or empty materialName.");
                return null;
            }

            var materialPath = $"Materials/{materialName}";
            var material = Resources.Load<Material>(materialPath);

            if (material == null)
            {
                Debug.LogWarning($"Material not found: '{materialName}'. Make sure it exists in Assets/Resources/Materials/");
            }

            return material;
        }
        
        /// <summary>
        /// 加载并创建预制体
        /// </summary>
        public static GameObject LoadPrefab(string prefabPath, Transform parent = null)
        {
            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogWarning("InstantiatePrefab called with null or empty prefabPath.");
                return null;
            }

            var fullPath = $"Prefabs/{prefabPath}";

            var prefab = Resources.Load<GameObject>(fullPath);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found at path: Resources/{fullPath}");
                return null;
            }
            
            var go = GameObject.Instantiate(prefab, parent);
            go.SetActive(true);

            return go;
        }
    }
}
