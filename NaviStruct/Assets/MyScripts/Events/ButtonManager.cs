using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    //public GameObject loadingScreen;    
    public Button loader;
    //public Text progressText;    

    public void QuickStartButton(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        print(sceneIndex);
        //loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);            
            loader.image.fillAmount = progress;
            //progressText.text = progress * 100f + "%";

            yield return null;
        }
    }
    public void CancelButton()
    {
        Application.Quit();
    }
}
