using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PitchController : MonoBehaviour
{

    public AudioLowPassFilter alpf;

    public GameObject gameOverMenu;

    private float[] cutOffValues = {22000f, 5000f, 700f, 600f, 450f, 300f, 100f};
    private float[] alphaValues = { 0f, 0.2f, 0.4f, 0.7f, 0.85f, 0.9f, 1f};

    public Image hurtPanel;

    Color originalColor;
    private int currentIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        originalColor = hurtPanel.color; 
    }

    // Update is called once per frame
    void Update()
    {
        
        alpf.cutoffFrequency = Mathf.Clamp(alpf.cutoffFrequency, 200f, 22000f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("note"))
        {
            AudioManager.instance.Play("Miss");


            if (currentIndex <= cutOffValues.Length-1)
            {
                currentIndex++;
                currentIndex = (int)Mathf.Clamp(currentIndex, 0f, cutOffValues.Length - 1);
            }

                
            alpf.cutoffFrequency = cutOffValues[currentIndex];

            
            Color tempColor = hurtPanel.color;
            tempColor.a = alphaValues[currentIndex];
            hurtPanel.color = tempColor;
            if (collision.gameObject.GetComponent<Note>().index == FindObjectOfType<SpawnerBehaviour>().arrayNote.Length - 1)
            {
                FindObjectOfType<SessionManager>().Endgame(true);
                return;
            }

            //instantiate effect
            GameObject obj = ObjectPooler.SharedInstance.GetPooledObject(1);
            obj.transform.position = collision.gameObject.transform.position;
            var rotationVector = obj.transform.rotation.eulerAngles;
            rotationVector.z = 45;
            obj.transform.rotation = Quaternion.Euler(rotationVector);
            obj.SetActive(true);
            collision.gameObject.SetActive(false);
            if (currentIndex >= cutOffValues.Length-1)
            {
                FindObjectOfType<SessionManager>().Endgame(false);
            }
        }
    }
    public void Regulate()
    {
        if(currentIndex > 0)
        {
            currentIndex--;
            currentIndex = (int)Mathf.Clamp(currentIndex, 0f, cutOffValues.Length - 1);
        }
        alpf.cutoffFrequency = cutOffValues[currentIndex];
        Color tempColor = hurtPanel.color;
        tempColor.a = alphaValues[currentIndex];
        hurtPanel.color = tempColor;
    }
}
