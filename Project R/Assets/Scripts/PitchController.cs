using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchController : MonoBehaviour
{

    public AudioLowPassFilter alpf;

    public GameObject gameOverMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        alpf.cutoffFrequency = Mathf.Clamp(alpf.cutoffFrequency, 400f, 22000f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("note"))
        {
            alpf.cutoffFrequency -= 4320f;
            collision.gameObject.SetActive(false);
            if (alpf.cutoffFrequency <= 400f)
            {
                Debug.Log("You lose");
                FindObjectOfType<SpawnerBehaviour>().gameObject.SetActive(false);
                FindObjectOfType<PlayerBehaviour>().GetComponent<PlayerBehaviour>().enabled = false;
                gameOverMenu.SetActive(true);
                AudioManager.instance.StopEverything();
            }
        }
    }
    public void Regulate()
    {
        alpf.cutoffFrequency += 1000f; 
    }
}
