using System;
using DataManagement;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class HeroChooseManager : MonoBehaviour
    {
        private GameObject heroChoosePrefab;
        private GameObject heroSplash;
        private GameObject title;
        private AudioSource audioS;
        private GameObject startButton;

        private void Awake()
        {
            startButton = GameObject.Find("Canvas/StartButton");
            heroSplash = GameObject.Find("Canvas/HeroSplash");
            title = GameObject.Find("Canvas/Title");
            heroChoosePrefab = transform.Find("HeroChoosePrefab").gameObject;
            heroChoosePrefab.SetActive(false);
        }

        private void Start()
        {
            var heros = ResourceReader.GetAllHeros();
            audioS = AudioManager.Instance.GetAudioSource();
            
            foreach (var hero in heros)
            {
                void OnChosen()
                {
                    var clip = ResourceReader.LoadAudio($"Hero/{hero.heroName}/{hero.heroName}_Chosen");
                    var splash = heroSplash.GetComponent<Image>();
                    
                    audioS.Stop();
                    audioS.clip = clip;
                    audioS.Play();

                    splash.sprite =
                        ResourceReader.LoadImage("Sprites/HeroImage/" + hero.heroName + "_Splash");

                    splash.DOFade(1, 4f);
                }
                
                var newChoice = Instantiate(heroChoosePrefab, transform);
                
                newChoice.transform.Find("HeroLoadingImage").GetComponent<Image>().sprite =
                    ResourceReader.LoadImage("Sprites/HeroImage/" + hero.heroName + "_Loading");
                
                newChoice.transform.Find("HeroLoadingImage").GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (hero.heroName == PlayerData.Instance.heroName) return;
                    title.GetComponent<TMP_Text>().DOFade(0, 0.4f);
                    
                    var splash = heroSplash.GetComponent<Image>();
                    PlayerData.Instance.heroName = hero.heroName;
                    splash.DOKill(true);
                    
                    if (splash.color.a < 0.1f)
                    {
                        OnChosen();
                    }
                    else
                    {
                        splash.DOFade(0, 0.4f).OnComplete(OnChosen);
                    }
                });
                
                newChoice.SetActive(true);
            }
            
            startButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioManager.Instance.Play("Lock", "Lock");
                DOTween.KillAll(true);
                SceneManager.LoadScene("LoadScene");
            });
            
            SceneManager.activeSceneChanged += (_, _) =>
            {
                AudioManager.Instance.DestroyAudioSource(audioS);
            };
        }

        private void Update()
        {
            // startButton.SetActive(!string.IsNullOrEmpty(PlayerData.Instance.heroName));
            startButton.SetActive(PlayerData.Instance.heroName == "Yasuo");
        }
    }
}