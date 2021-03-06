using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Scripts.UIController
{
    public class SplashScene : MonoBehaviour
    {
        [SerializeField] private Image loadingBar;
        [SerializeField] private float duration;
        private void Start()
        {
            
            GameDataManager.Instance.LoadUserData();
            StartCoroutine(Loading());
        }

        private IEnumerator Loading()
        {
            
            var count = 0f;
            while (count<duration)
            {
                count += Time.deltaTime;
                loadingBar.fillAmount = count / duration;
                yield return null;
            }
            loadingBar.fillAmount = 1f;
            var currentLevel = GameDataManager.Instance.GetCurrentLevel();
            if(currentLevel> GameDataManager.Instance.maxLevel)
                LoadSceneController.LoadMenu();
            else
            {
                LoadSceneController.LoadLevel(currentLevel);
            }

        }
    }
}