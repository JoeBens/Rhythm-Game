using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{

    public TextMeshProUGUI scoreText;

    [HideInInspector]
    public int score = 0;



    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreText.text = "0";
    }

    public void UpdateScore(int amount)
    {
        score = score + amount;
        scoreText.text = "" + score;
    }

    public int GetScore()
    {
        return score;
    }
    
}
