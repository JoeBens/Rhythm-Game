using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector3 mousePos;
    public float moveSpeed;

    private Animator anim;

    public SpawnerBehaviour sb;

    private SpriteRenderer sr;
    private Color originalColor;

    private Vector3 originalScale;

    private ScoreManager sm;

    private PitchController pc;

    public GameObject winMenu;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        //anim = this.GetComponent<Animator>();
        sb = FindObjectOfType<SpawnerBehaviour>();
        sr = this.GetComponent<SpriteRenderer>();
        sm = FindObjectOfType<ScoreManager>();
        pc = FindObjectOfType<PitchController>();
        originalColor = sr.color;
        Time.timeScale = 0.5f;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sb.panelActivated == true)
            return;

        mousePos = Input.mousePosition;
        mousePos.z = 10;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = Vector2.Lerp(transform.position, new Vector2(mousePos.x, transform.position.y), moveSpeed);
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -2.5f, 2.5f), transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("note"))
        {
            //anim.SetTrigger("Attack");
            //this.GetComponent<AudioSource>().Play();
            PlayEffect();
            sb.hitNote = true;
            //Destroy(collision.gameObject);
            StartCoroutine(FlashHit());
            if (collision.gameObject.GetComponent<Note>().index == sb.arrayNote.Length-1)
            {
                Debug.Log("You won");
                //Win effect
                //Win music
                winMenu.SetActive(true);
                sb.enabled = false;
                AudioManager.instance.StopEverything();
            }


            collision.gameObject.SetActive(false);
            sm.UpdateScore(1);
            pc.Regulate();
        }
    }

    public void PlayEffect()
    {
        int chance = Random.Range(0, 101);
        if(chance <= 100)
        {
            int randNum = Random.Range(0, 3);
            if (randNum == 0)
            {
                AudioManager.instance.Play("EffectOne");
            }
            else if (randNum == 1)
            {
                AudioManager.instance.Play("EffectTwo");
            }
            else if (randNum == 2)
            {
                AudioManager.instance.Play("EffectThree");
            }
        }
    }
    IEnumerator FlashHit()
    {
        transform.localScale *= 1.15f;
        sr.color = Color.yellow;
        yield return new WaitForSeconds(0.04f);
        sr.color = originalColor;
        transform.localScale = originalScale;
    }


}
