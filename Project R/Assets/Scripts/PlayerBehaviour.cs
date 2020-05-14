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
            //instantiate effect
            GameObject obj = ObjectPooler.SharedInstance.GetPooledObject(1);
            obj.transform.position = collision.gameObject.transform.position;
            var rotationVector = obj.transform.rotation.eulerAngles;
            rotationVector.z = 45;
            obj.transform.rotation = Quaternion.Euler(rotationVector);
            
            obj.SetActive(true);
            //if the song ended
            if (collision.gameObject.GetComponent<Note>().index == sb.arrayNote.Length-1)
            {
                FindObjectOfType<SessionManager>().Endgame(true);
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
