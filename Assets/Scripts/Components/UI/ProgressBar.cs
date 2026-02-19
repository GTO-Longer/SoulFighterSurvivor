using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Components
{
    public class ProgressBar : MonoBehaviour
    {
        private Image _progressBar;
        
        private void Awake()
        {
            _progressBar = transform.Find("ProgressBar").GetComponent<Image>();
        }
        
        // 使用协程
        private void Start()
        {
            StartCoroutine(LoadScene());
        }
 
        IEnumerator LoadScene()
        {
            var disableProgress = 0;
            var toProgress = 0;
 
            //异步场景切换
            var op = SceneManager.LoadSceneAsync("SampleScene");
            
            //不允许有场景切换功能
            op.allowSceneActivation = false;
            
            while (op.progress < 0.9f)
            {
                //获取真实的加载进度
                toProgress = (int)(op.progress * 100);
                while (disableProgress < toProgress)
                {
                    ++disableProgress;
                    _progressBar.fillAmount = disableProgress / 100.0f;
                    yield return new WaitForEndOfFrame();
                }
            }
            
            //因为op.progress 只能获取到90%，所以后面的值不是实际的场景加载值了
            toProgress = 100;
            while (disableProgress < toProgress)
            {
                ++disableProgress;
                _progressBar.fillAmount = disableProgress / 100.0f;
                yield return new WaitForEndOfFrame();
            }
            op.allowSceneActivation = true;
        }
    }
}