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

            notice.SetText("Hirohito Araki is disappointed");
            tip.SetText("This..this must be the work of an enemy stand!");

            return;
        }

        if (score <= length * 0.5f)
        {
            Debug.Log("You lose");
            notice.SetText("You missed a lot, Giorno is not amused");
            tip.SetText("Come on, you can do better!");
           
        }
        else if (score <= length * 0.75f)
        {
            notice.SetText("ORA ORA ORA ORA ORA ORA ORA");
            tip.SetText("YES! YES! YES! YES! YES!");
        }
        else if (score <= length * 0.99f)
        {
            notice.SetText("Greato Daze!");
            tip.SetText("Jotaro Kujo wants to send you on a mission");
        }
        else if (score == length )
        {
            notice.SetText("Niiiiiice!");
            tip.SetText("You should work at the Speedwagon Company");
        }
    }
}
