using Managers;
using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace MVVM.ViewModels
{
    public class PauseViewModel : MonoBehaviour
    {
        private void Start()
        {
            var volumeScrollBar = transform.Find("VolumeSetting/VolumeScrollBar").GetComponent<Scrollbar>();
            var continueButton = transform.Find("ContinueButton").GetComponent<Button>();
            var exitButton = transform.Find("ExitButton").GetComponent<Button>();

            Binder.BindScrollBar(volumeScrollBar, AudioManager.Instance.volume);
            Binder.BindButton(continueButton, () => PauseSystem.Instance.ContinueGame());
            Binder.BindButton(exitButton, () => PauseSystem.Instance.ExitGame());
        }
    }
}