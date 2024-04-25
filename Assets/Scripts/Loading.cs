using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public Text percentageText;
    public Slider loadingSlider;
    AsyncOperation loadingOperation;
    void Start()
    {
        //loadingOperation = SceneManager.LoadSceneAsync(1);
        StartCoroutine(LoadSceneAsys());
    }

    void Update()
    {
        //print(loadingOperation.progress);
        //float progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f);
        //percentageText.text = Mathf.Round(progressValue * 100) + "%";
    }

    IEnumerator LoadSceneAsys()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress);
            print(progress);

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }             
    }
}
