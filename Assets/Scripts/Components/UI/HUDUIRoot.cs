using DataManagement;
using MVVM.ViewModels;
using UnityEngine;
using Utilities;

namespace Components.UI
{
    public class HUDUIRoot : MonoBehaviour
    {
        public static HUDUIRoot Instance;
        
        [HideInInspector] public TargetAttributeViewModel targetAttributes;
        [HideInInspector] public HeroAttributeViewModel heroAttributes;
        [HideInInspector] public SkillViewModel skillInfo;
        [HideInInspector] public BuffViewModel buffInfo;
        [HideInInspector] public BuffViewModel targetBuffInfo;
        [HideInInspector] public HexInfoViewModel hexInfo;
        [HideInInspector] public AttributeViewModel attributeInfo;

        private void Awake()
        {
            Instance = this;
            
            heroAttributes = ResourceReader.LoadPrefab("UI/heroAttributes", transform).GetComponent<HeroAttributeViewModel>();
            targetAttributes = ResourceReader.LoadPrefab("UI/TargetAttributes", transform).GetComponent<TargetAttributeViewModel>();
            skillInfo = ResourceReader.LoadPrefab("UI/SkillInfo", transform).GetComponent<SkillViewModel>();
            buffInfo = ResourceReader.LoadPrefab("UI/BuffInfo", transform).GetComponent<BuffViewModel>();
            targetBuffInfo = ResourceReader.LoadPrefab("UI/BuffInfo", transform).GetComponent<BuffViewModel>();
            hexInfo = ResourceReader.LoadPrefab("UI/HexInfo", transform).GetComponent<HexInfoViewModel>();
            attributeInfo = ResourceReader.LoadPrefab("UI/AttributeInfo", transform).GetComponent<AttributeViewModel>();
        }

        private void Start()
        {
            heroAttributes.Initialize();
            targetAttributes.Initialize();
            skillInfo.Initialize();
            buffInfo.Initialize(Team.Hero);
            targetBuffInfo.Initialize(Team.Enemy);
            hexInfo.Initialize();
            attributeInfo.Initialize();
        }
    }
}