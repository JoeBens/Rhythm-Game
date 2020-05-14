using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disabler : MonoBehaviour
{

    public float time;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Disable", time);
    }

    // Update is called once per frame
    public void Disable()
    {
        this.gameObject.SetActive(false);
    }
}
