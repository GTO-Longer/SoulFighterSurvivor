using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Classes;
using Components.UI;
using DataManagement;
using Managers.EntityManagers;
using Newtonsoft.Json;
using Systems;
using Utilities;

namespace Managers
{
    public class HexManager : MonoBehaviour
    {
        public static HexManager Instance;
        private Dictionary<string, Hex> hexMap;
        public List<Hex> hexList { get; private set; }
        private int choiceTime;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                hexMap = new Dictionary<string, Hex>(StringComparer.OrdinalIgnoreCase);
                hexList = new List<Hex>();
                LoadAllHexes();
                choiceTime = 0;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (((int)HeroManager.hero.level.Value >= 3 && choiceTime == 0) ||
                ((int)HeroManager.hero.level.Value >= 7 && choiceTime == 1) ||
                ((int)HeroManager.hero.level.Value >= 11 && choiceTime == 2) ||
                ((int)HeroManager.hero.level.Value >= 15 && choiceTime == 3))
            {
                var targetHexList = hexList.FindAll(
                    hex => !HeroManager.hero.hexList.Contains(hex) && 
                           hex.hexQuality == Quality.Prismatic &&
                           hex.canChoose);
                
                if (!PanelUIRoot.Instance.isChoiceOpen && ToolFunctions.GetRandomUniqueItems(targetHexList, 3, out var results))
                {
                    choiceTime += 1;
                    var choices = new Choice[3];
                    for (var index = 0; index < results.Count; index++)
                    {
                        var result = results[index];
                        choices[index] = new Choice(result.hexName, result.hexDescription, result.hexIcon, result.hexIconBorder, () =>
                        {
                            HeroManager.hero.GetHex(result);
                        }, result.hexQuality);
                    }
                    ChoiceSystem.Instance.MakeChoice(choices);
                }
            }
        }

        /// <summary>
        /// 获取所有海克斯配置
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
        /// 根据海克斯id获取海克斯
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
