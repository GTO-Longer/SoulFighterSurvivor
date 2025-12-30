using System;
using Classes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVVM.ViewModels
{
    public class StateBarViewModel : MonoBehaviour
    {
        public Action ViewModelInitialization(Entity entity)
        {
            Action UnBind = () => { };
            gameObject.SetActive(true);

            UnBind += Binder.BindFillAmountImmediate(transform.Find("HPBarBackground/HPBar").GetComponent<Image>(), entity.healthPointProportion);
            UnBind += Binder.BindFillAmountSmooth(transform.Find("HPBarBackground/HPBarSmooth").GetComponent<Image>(), 0.2f, entity.healthPointProportion);
            UnBind += Binder.BindText(transform.Find("LevelBackground/Level").GetComponent<TMP_Text>(), entity.level, "{0:F0}");
            UnBind += Binder.BindFillAmountImmediate(transform.Find("ExpBackground/ExpMask").GetComponent<Image>(), entity.experienceProportion);
            
            UnBind += Binder.BindFillAmountImmediate(transform.Find("ControlBackground/ControlBar").GetComponent<Image>(), entity.ControlProportion);
            UnBind += Binder.BindActive(transform.Find("ControlBackground").gameObject, entity.isControlled, false);

            var MPBar = transform.Find("MPBarBackground/MPBar").GetComponent<Image>();
            if (entity._baseMaxMagicPoint > 1)
            {
                UnBind += Binder.BindFillAmountImmediate(MPBar, entity.magicPointProportion);
            }
            else
            {
                UnBind += Binder.BindFillAmountImmediate(MPBar, entity.energyProportion);
                MPBar.color = Color.white;
            }
            
            return UnBind;
        }
    }
}
