using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulWarriorController : MonoBehaviour
{

    [SerializeField] float activeDistance;

    [Header("Attack")]
    [SerializeField] float attackDashRange;
    [SerializeField] float attackDashVelocity;

    [Header("Stomp")]
    [SerializeField] float stompHeight;
    [SerializeField] float stompFallVelocity;

    [Header("Evade")]
    [SerializeField] float evadeRange;
    [SerializeField] float evadeVelocity;

    [Header("Cast")]
    [SerializeField] Transform castPoint;
    [SerializeField] GameObject orb;

    private Transform playerTransform;
    private Transform soulWarriorTransform;
    private Rigidbody2D soulWarriorRigidbody;
    private Animator animator;
    private HealthController healthController;

    private float gravityScale;

    private bool isFlipped;
    private bool isSkill;
    private bool isSleep;
    private bool isDeath;

    private void Start()
    {
        soulWarriorTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        healthController = GetComponent<HealthController>();
        soulWarriorRigidbody = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        isFlipped = false;
        isSkill = false;
        isSleep = true;
        isDeath = false;

        gravityScale = soulWarriorRigidbody.gravityScale;
    }

    private void Update()
    {
        wake();
        DeathControl();
    }


    private void wake()
    {
        if(Vector2.Distance(playerTransform.position, soulWarriorTransform.position) <= activeDistance && isSleep)
        {
            animator.SetTrigger("Wake");
            isSleep = false;

            MusicController musicController = FindObjectOfType<MusicController>();
            StartCoroutine(musicController.fightPrepareCoroutine());
        }
    }

    public void lookAtPlayer()
    {
        Vector3 flipped = soulWarriorTransform.localScale;
        flipped.z *= -1f;

        if(soulWarriorTransform.position.x > playerTransform.position.x && isFlipped)
        {
            soulWarriorTransform.localScale = flipped;
            soulWarriorTransform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if(soulWarriorTransform.position.x < playerTransform.position.x && !isFlipped)
        {
            soulWarriorTransform.localScale = flipped;
            soulWarriorTransform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }


    public void teleportOutEvent()
    {
        int randSkill = Random.Range(0, 2);

        if(randSkill == 0)
        {
            stompControl();
        }
        else  
        {
            castControl();
        }
    }

    //attack slash section
    public void attackDashEvent()
    {
        int direction = isFlipped==true ? 1 : -1;
        Vector2 newPosition = Vector2.MoveTowards(soulWarriorRigidbody.position, soulWarriorRigidbody.position+Vector2.right*attackDashRange*direction, attackDashVelocity*Time.fixedDeltaTime);
        soulWarriorRigidbody.MovePosition(newPosition);
    }

    // stomp section
    private void stompControl()
    {
        animator.SetTrigger("Stomp");
        soulWarriorRigidbody.position = playerTransform.position + Vector3.up*stompHeight;
    }

    public void stompFallEvent()
    {
        soulWarriorRigidbody.velocity = Vector2.down*stompFallVelocity;
    }


    // Evade section
    public void evadeControl()
    {
        int direction = isFlipped==true ? -1 : 1;
        Vector2 newPosition = Vector2.MoveTowards(soulWarriorRigidbody.position, soulWarriorRigidbody.position+Vector2.right*evadeRange*direction, evadeVelocity*Time.fixedDeltaTime);
        soulWarriorRigidbody.MovePosition(newPosition);

        int randSkill = Random.Range(0, 2);
        if(randSkill == 0)
        {
            animator.SetTrigger("DashTeleport");
        }
        else
        {
            animator.SetTrigger("DashRecover");
        }
    }


    // cast section
    private void castControl()
    {
        animator.SetTrigger("Cast");
    }

    public void castOrbEvent()
    {
        // create an instance of orb
        Instantiate(orb, castPoint.position, castPoint.rotation);
    }

    public void freezeGravityEvent()
    {
        soulWarriorRigidbody.velocity = Vector2.zero;
        soulWarriorRigidbody.gravityScale = 0;
    }

    public void unFreezeGravityEvent()
    {
        soulWarriorRigidbody.gravityScale = gravityScale;
    }


    private void DeathControl()
    {
        if(healthController.getHealthPoint() <= 0 && !isDeath)
        {
            animator.SetTrigger("Death");

            MusicController musicController = FindObjectOfType<MusicController>();
            musicController.fightEndCoroutine();
            isDeath = true;
        }
    }

    public void destroyGameObject()
    {
        Destroy(gameObject);
    }



    // setter, getter section
    public void setIsSkill()
    {
        this.isSkill = true;
    }

    public void resetIsSkill()
    {
        this.isSkill = false;
    }

    public bool getIsSkill()
    {
        return isSkill;
    }
}
