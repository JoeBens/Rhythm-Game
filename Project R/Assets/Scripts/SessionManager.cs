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

    public GameObject holdPanel;


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
        scoreText.SetText(score + " / " + length);

        holdPanel.SetActive(false);

        if (win == false)
        {
            Debug.Log("You lose");

            notice.SetText("Very Bad!");
            tip.SetText("Focus next time!");

            return;
        }
        if (score <= length * 0.5f)
        {
            notice.SetText("Well that was bad!");
            tip.SetText("You can do much better");
        }
        else if (score <= length * 0.75f)
        {
            notice.SetText("Hey that was good!");
            tip.SetText("..But you can do better");
        }
        else if (score <= length * 0.999999999999999999f)
        {
            notice.SetText("Almost perfect! Great job!");
            tip.SetText("You're very talented!");
        }
        else if (score == length )
        {
            notice.SetText("Perfect.");
            tip.SetText("You inspire perfection.");
        }
    }
}
