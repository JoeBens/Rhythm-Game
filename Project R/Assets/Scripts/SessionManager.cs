using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class SessionManager : MonoBehaviour
{
    // Start is called before the first frame update


    public GameObject completionMenu;
    private SpawnerBehaviour sb;
    private ScoreManager sm;

    public TextMeshProUGUI notice;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI tip;

    void Start()
    {
        sb = FindObjectOfType<SpawnerBehaviour>();
        sm = FindObjectOfType<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        sm = FindObjectOfType<ScoreManager>();
    }

    public void Endgame(bool win)
    {
        completionMenu.SetActive(true);
        AudioManager.instance.StopEverything();
        int score = sm.GetScore();
        int length = sb.arrayNote.Length;
        sb.enabled = false;
        FindObjectOfType<PlayerBehaviour>().GetComponent<PlayerBehaviour>().enabled = false;

        if (win == false)
        {
            Debug.Log("You lose");

            return;
        }

        if (score <= length * 0.25f)
        {
            Debug.Log("You lose");
            notice.SetText("");
            scoreText.SetText("");
            tip.SetText("");
           
        }
        else if(score <= length * 0.5f)
        {
            Debug.Log("You won");
            notice.SetText("");
            scoreText.SetText("");
            tip.SetText("");

        }
        else if (score <= length * 0.75f)
        {
            notice.SetText("");
            scoreText.SetText("");
            tip.SetText("");
        }
        else if (score <= length * 0.99f)
        {
            notice.SetText("");
            scoreText.SetText("");
            tip.SetText("");
        }
        else if (score == length )
        {
            notice.SetText("");
            scoreText.SetText("");
            tip.SetText("");
        }
    }
}
