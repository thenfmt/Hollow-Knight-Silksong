using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavelinController : MonoBehaviour
{
    [SerializeField] float javelinThrowSpeed;
    [SerializeField] float javelinBreakInterval;
    [SerializeField] float javelinNeuralInterval;
    [SerializeField] float javelinLifeInterval;


    private Rigidbody2D javelinRigidbody;
    private Transform javelinTransform;
    private Animator javelinAnimator;
    private float javelinLifeTime;

    private void Awake()
    {
        javelinTransform = GetComponent<Transform>();
        javelinRigidbody = GetComponent<Rigidbody2D>();
        javelinAnimator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        javelinRigidbody.velocity = javelinTransform.right*javelinThrowSpeed;
        javelinLifeTime = 0;
    }

    private void FixedUpdate()
    {
        // auto destroy the gameobject when the object is out of life interval
        javelinLife();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        
        if(collider.gameObject.tag == "Ground" || collider.gameObject.tag == "Wall")
        {
            javelinRigidbody.velocity = Vector2.zero;
            javelinAnimator.SetTrigger("Stick");
            StartCoroutine(destroyCoroutine(javelinNeuralInterval));
        }
        else if(collider.gameObject.tag == "Trap" || collider.gameObject.tag == "Shield")
        {
            javelinRigidbody.velocity = Vector2.zero;
            StartCoroutine(destroyCoroutine(0f));
        }
        else if(collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            javelinRigidbody.velocity = Vector2.zero;
            collider.gameObject.GetComponent<HealthController>().takeDamage(1);
            StartCoroutine(destroyCoroutine(0f));
        }
    }

    public IEnumerator destroyCoroutine(float waitInterval)
    {   
        yield return new WaitForSeconds(waitInterval);

        javelinAnimator.SetTrigger("Break");

        yield return new WaitForSeconds(javelinBreakInterval);
        Destroy(gameObject);
    }

    private void javelinLife()
    {
        javelinLifeTime += Time.deltaTime;
        if(javelinLifeTime >= javelinLifeInterval)
        {
            Destroy(gameObject);
        }
    }
}
