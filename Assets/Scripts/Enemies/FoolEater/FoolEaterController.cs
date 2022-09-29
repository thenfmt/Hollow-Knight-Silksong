using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoolEaterController : MonoBehaviour
{
    [SerializeField] float snapAnticipateInterval;
    [SerializeField] float deathInterval;
    [SerializeField] float retractInterval;
    [SerializeField] GameObject landGrassParticle;
    

    private Animator animator;
    private Transform foolEaterTransform;
    private CharacterController2D character;
    private HealthController healthController;
    private AudioSource trapBiteSound;
    

    private bool isInSnapArea;
    private bool isSnapable;

    private void Start()
    {
        isInSnapArea = false;
        isSnapable = true;

        character = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();
        healthController = GetComponent<HealthController>();
        animator = GetComponent<Animator>();
        foolEaterTransform = GetComponent<Transform>();
        trapBiteSound = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if(isInSnapArea && isSnapable)
        {
            StartCoroutine(snapCoroutine());
        }

        if(healthController.getHealthPoint() <= 0)
        {
            StartCoroutine(dieCorountine());
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.layer==LayerMask.NameToLayer("Player") )
        {
            isInSnapArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.layer==LayerMask.NameToLayer("Player") )
        {
            isInSnapArea = false;
        }
    }

    private IEnumerator snapCoroutine()
    {
        animator.SetTrigger("Snap");
        trapBiteSound.Play();
        isSnapable = false;

        yield return new WaitForSeconds(snapAnticipateInterval);

        Instantiate(landGrassParticle, foolEaterTransform.position, foolEaterTransform.rotation);
        
        if(isInSnapArea)
        {
            character.takeDamage(1);
        }
        
        yield return new WaitForSeconds(retractInterval);
        animator.ResetTrigger("Snap");
        isSnapable = true;
    }

    

    private IEnumerator dieCorountine()
    {
        animator.SetTrigger("Death");

        yield return new WaitForSeconds(deathInterval);

        Destroy(gameObject);
    }

}
