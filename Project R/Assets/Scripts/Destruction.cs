using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour
{


    Rigidbody2D rb;
    float dirX;
    float dirY;
    float torque;
    // Start is called before the first frame update
    void Start()
    {
        dirX = Random.Range(-10, 10);
        dirY = Random.Range(5, 8);
        torque = Random.Range(500, 1500);
        rb = this.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(dirX, dirY),ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Force);
    }

    // Update is called once per frame
  
}
