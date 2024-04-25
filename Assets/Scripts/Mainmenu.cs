using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class Mainmenu : MonoBehaviour
{
    public GameObject[] screenUi;
    public GameObject setsUi;
    public ToggleGroup qualityGroup;
    public GameObject loading;
    public Text percentageText;
    public Slider loadingSlider;
    AsyncOperation loadingOperation;

    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerData.Instance)
        PlayerData.Create("Data");
        Screen.orientation = ScreenOrientation.Portrait;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
            Application.Quit();

        //if (loadingOperation != null) {
        //    print(loadingOperation.progress);
        //    float progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f);
        //    percentageText.text = Mathf.Round(progressValue * 100) + "%";
        //}
    }

    public void MenuOtionClick(bool isactive)
    {
        string newStr = qualityGroup.ActiveToggles().FirstOrDefault().name;
        newStr = newStr.Remove(newStr.Length - 3);
        foreach (var obj in screenUi)
        {
            if (newStr == obj.name)
            {
                obj.SetActive(true);
                if (newStr == "SetCollection")
                    setsUi.SetActive(false);
            }
            else
            {
                obj.SetActive(false);
            }
        }
        
    }


    public void PlayMatachClick()
    {
        loading.SetActive(true);
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        StartCoroutine(LoadSceneAsys());
        //SceneManager.LoadScene(2);
        //loadingOperation = SceneManager.LoadSceneAsync(1);
    }

    IEnumerator LoadSceneAsys()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress);
            print(progress);
            percentageText.text = Mathf.Round(progress * 100) + " %";
            loadingSlider.value = Mathf.Round(progress * 100);
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
   
    public void SetsClick()
    {
        setsUi.SetActive(true);
    }
}

