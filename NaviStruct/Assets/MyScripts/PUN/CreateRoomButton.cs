using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateRoomButton : MonoBehaviour
{
    [SerializeField]
    private Button loader;

    [SerializeField]
    private int customMatchSceneIndex;

    // Start is called before the first frame update
    public void CustomMatchButtonPressed()
    {           
        StartCoroutine(LoadAsynchronously(customMatchSceneIndex));        
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loader.image.fillAmount = progress;
            yield return null;
        }
    }
}

