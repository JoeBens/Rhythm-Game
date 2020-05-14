using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Note : MonoBehaviour
{
    private string text;

    private SpawnerBehaviour sb;
    [SerializeField]
    private float startY;
    [SerializeField]
    private float endY;
    [SerializeField]
    private float removeLineY;
    [SerializeField]
    private decimal beat;
    [SerializeField]
    private float posX;
    [SerializeField]
    private Vector2 removePos;
    [SerializeField]
    private Vector2 spawnPos;

    public TextMeshPro number;

    [HideInInspector]
    public int index;

    public void Initialize(SpawnerBehaviour sb, float startY, float endY, float removeLineY, float posX, decimal beat, string text, int index)
    {
        this.sb = sb;
        this.startY = startY;
        this.endY = endY;
        spawnPos = new Vector2(posX, startY);
        this.removeLineY = removeLineY;
        this.beat = beat;
        this.posX = posX;
        //number.SetText(text);
        this.removePos = new Vector2(transform.position.x, removeLineY);
        this.index = index;
        
    }


    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(posX, startY + (endY - startY) * (1f - ((float)beat - sb.songPosition / sb.secPerBeat) / sb.beatsShownOnScreen));
        //if (transform.position.y < removeLineY)
        //{
        //    //Destroy(gameObject);
        //    gameObject.SetActive(false);
        //}
    }
}
