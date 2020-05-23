using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    public Button[] lvlButtons;
    public Button[] lvlButtonsDisabled;
    // Start is called before the first frame update
    void Start()
    {
        int levelAt = PlayerPrefs.GetInt("levelAt", 1);

        for (int i = 0; i < lvlButtons.Length; i++)
        {
            if (i + 1 > levelAt)
                lvlButtons[i].interactable = false;
        }

        for (int i = 0; i < lvlButtonsDisabled.Length; i++)
        {
            if (levelAt-1 > i)
                lvlButtonsDisabled[i].gameObject.SetActive(false);
        }
    }

    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
    public void Quit()
    {
        Application.Quit();
    }

}
