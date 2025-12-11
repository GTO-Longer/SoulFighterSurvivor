using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Classes;
using DataManagement;
using Newtonsoft.Json;

namespace Managers
{
    public class HexManager : MonoBehaviour
    {
        public static HexManager Instance;
        private Dictionary<string, Hex> hexMap;
        public List<Hex> hexList { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                hexMap = new Dictionary<string, Hex>(StringComparer.OrdinalIgnoreCase);
                hexList = new List<Hex>();
                LoadAllHexes();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 获取所有装备配置
        /// </summary>
        private void LoadAllHexes()
        {
            // 读取全部装备配置
            var allHexConfigs = GetAllHexConfigs();

            Debug.Log($"[HexManager] 加载海克斯数量：{allHexConfigs.Count}");

            var assembly = Assembly.GetExecutingAssembly();

            foreach (var config in allHexConfigs)
            {
                // 根据海克斯id找到对应的装备类
                var className = "Classes.Hexes." + config.id;
                var type = assembly.GetType(className);

                if (type == null)
                {
                    Debug.LogWarning($"[HexManager] 未找到海克斯类：{className}，该海克斯将跳过。");
                    continue;
                }

                try
                {
                    // 要求装备类必须有无参构造函数
                    var instance = Activator.CreateInstance(type);
                    hexMap[config.id] = (Hex)instance;
                    hexList.Add(hexMap[config.id]);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[HexManager] 海克斯实例创建失败：{config.id} → {className}\n{ex}");
                }
            }
        }
        
        private List<HexConfig> GetAllHexConfigs()
        {
            var list = new List<HexConfig>();

            var jsonText = Resources.Load<TextAsset>("Configs/HexConfig");
            if (jsonText == null)
            {
                Debug.LogError("[HexManager] HexConfig.json文件");
                return list;
            }

            try
            {
                var collection = JsonConvert.DeserializeObject<HexConfigCollection>(jsonText.text);
                if (collection?.hexes != null)
                {
                    list.AddRange(collection.hexes);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[HexManager] 解析HexConfig.json失败：{e}");
            }

            return list;
        }

        /// <summary>
        /// 根据装备id获取海克斯
        /// </summary>
        public Hex GetHex(string id)
        {
            if (hexMap.TryGetValue(id, out var hex))
            {
                return hex;
            }

            Debug.LogWarning($"[HexManager] 未找到海克斯实例：{id}");
            return null;
        }
    }
}
