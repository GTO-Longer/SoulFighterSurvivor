using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Classes;
using DataManagement;
using Managers.EntityManagers;
using Newtonsoft.Json;

namespace Managers
{
    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager Instance;
        public Dictionary<string, Equipment> EquipmentMap { get; private set; }

        private void Awake()
        {
            Instance = this;
            EquipmentMap = new Dictionary<string, Equipment>(StringComparer.OrdinalIgnoreCase);
            LoadAllEquipments();
        }

        private void Start()
        {
            GetEquipment("Guardian_sOrb").OnEquipmentGet(HeroManager.hero);
        }

        /// <summary>
        /// 获取所有装备配置
        /// </summary>
        private void LoadAllEquipments()
        {
            // 读取全部装备配置
            var allEquipmentConfigs = GetAllEquipmentConfigs();

            Debug.Log($"[EquipmentManager] 加载装备数量：{allEquipmentConfigs.Count}");

            var assembly = Assembly.GetExecutingAssembly();

            foreach (var config in allEquipmentConfigs)
            {
                // 根据装备 id 找到对应的装备类
                var className = "Classes.Equipments." + config.id;
                var type = assembly.GetType(className);

                if (type == null)
                {
                    Debug.LogWarning($"[EquipmentManager] 未找到装备类：{className}，该装备将跳过。");
                    continue;
                }

                try
                {
                    // 要求装备类必须有无参构造函数
                    var instance = Activator.CreateInstance(type);
                    EquipmentMap[config.id] = (Equipment)instance;
                    Debug.Log($"[EquipmentManager] 创建装备实例成功：{config.id + " : " + EquipmentMap[config.id].equipmentName}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EquipmentManager] 装备实例创建失败：{config.id} → {className}\n{ex}");
                }
            }
        }
        private List<EquipmentConfig> GetAllEquipmentConfigs()
        {
            var list = new List<EquipmentConfig>();

            var jsonText = Resources.Load<TextAsset>("Configs/EquipmentConfig");
            if (jsonText == null)
            {
                Debug.LogError("[EquipmentManager] 找不到 EquipmentConfig.json 文件");
                return list;
            }

            try
            {
                var collection = JsonConvert.DeserializeObject<EquipmentConfigCollection>(jsonText.text);
                if (collection?.equipments != null)
                {
                    list.AddRange(collection.equipments);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[EquipmentManager] 解析 EquipmentConfig.json 失败：{e}");
            }

            return list;
        }

        /// <summary>
        /// 根据装备id获取装备
        /// </summary>
        public Equipment GetEquipment(string id)
        {
            if (EquipmentMap.TryGetValue(id, out var eq))
            {
                return eq;
            }

            Debug.LogWarning($"[EquipmentManager] 未找到装备实例：{id}");
            return null;
        }
    }
}
