using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulWarriorOrbController : MonoBehaviour
{
    [SerializeField] float velocity;
    [SerializeField] float lifeInterval;

    private Rigidbody2D orbRigidbody;
    private Transform orbTransform;
    private AudioSource audioSource;
    private Animator animator;
    private Transform playerTransform;

    private bool isHit;

    private void Start()
    {
        orbRigidbody = GetComponent<Rigidbody2D>();
        orbTransform = GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("PlayerCentreOfGravity").transform;
        isHit = false;
        StartCoroutine(destroyCoroutine());
    }

    private void Update()
    {
        if(!isHit)
        {
            followPlayer();
        }
    }

    private void followPlayer()
    {
        Vector2 target = new Vector2(playerTransform.position.x, playerTransform.position.y);
        Vector2 newPosition = Vector2.MoveTowards(orbRigidbody.position, target, velocity*Time.fixedDeltaTime);
        orbRigidbody.MovePosition(newPosition);
    }


    private IEnumerator destroyCoroutine()
    {
        yield return new WaitForSeconds(lifeInterval);
        animator.SetTrigger("Impact");
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            animator.SetTrigger("Impact");
            collider.gameObject.GetComponent<CharacterController2D>().takeDamage(1);
        }
        else if(collider.gameObject.layer == LayerMask.NameToLayer("Terrain")
        || collider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            animator.SetTrigger("Impact");
            orbRigidbody.velocity = Vector2.zero;
            isHit = true;
        }
    }

    public void destroyEvent()
    {
        Destroy(gameObject);
    }

    public void playSound()
    {
        audioSource.Play();
    }
}
