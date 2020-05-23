using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LevelLoader : MonoBehaviour
{

    public GameObject loadingScreen;

    public TextMeshProUGUI progressText;
    private void Start()
    {

    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }
    public void RestartLevel(int sceneIndex)
    {
        /*if (checkGameManager == true)
        {
            FindObjectOfType<GameManager>().lastCheckpointPos = new Vector3(0, 0, 0);
        }*/

        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);


        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            progressText.text = Mathf.FloorToInt(progress * 100f) + "%";
            Debug.Log(progress);
            yield return null;
        }
    }
}
