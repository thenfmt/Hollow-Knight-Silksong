using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossKnightController : MonoBehaviour
{

    [SerializeField] float activeDistance;
    [SerializeField] float evadeRange;
    [SerializeField] float takeDamageForce;
    [SerializeField] float airDeathInterval;
    [SerializeField] float landDeathInterval;

    [Header("Shoot Section")]
    [SerializeField] Transform shotPoint;
    [SerializeField] GameObject projectile;
    

    private Transform playerTransform;
    private Transform mossKnightTransform;
    private Rigidbody2D mossKnightRigidbody;
    private HealthController healthController;
    private Animator animator;
    private AudioManager audioManager;

    private bool isFlipped;
    private bool isSleep;
    private bool isSkill;
    private bool isDeath;

    // Start is called before the first frame update
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("PlayerCentreOfGravity").transform;
        mossKnightTransform = GetComponent<Transform>();
        healthController = GetComponent<HealthController>();
        animator = GetComponent<Animator>();
        mossKnightRigidbody = GetComponent<Rigidbody2D>();
        audioManager = GetComponent<AudioManager>();

        isFlipped = false;
        isSleep = true;
        isSkill = false;
        isDeath = false;
    }

    // Update is called once per frame
    private void Update()
    {
        wake();
        DeathControl();
    }

    private void wake()
    {
        if(Vector2.Distance(playerTransform.position, mossKnightTransform.position) <= activeDistance && isSleep)
        {
            animator.SetTrigger("Wake");
            isSleep = false;
            MusicController musicController = FindObjectOfType<MusicController>();
            StartCoroutine(musicController.fightPrepareCoroutine());
        }
    }


    public void lookAtPlayer()
    {
        Vector3 flipped = mossKnightTransform.localScale;
        flipped.z *= -1f;

        if(mossKnightTransform.position.x > playerTransform.position.x && isFlipped)
        {
            mossKnightTransform.localScale = flipped;
            mossKnightTransform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if(mossKnightTransform.position.x < playerTransform.position.x && !isFlipped)
        {
            mossKnightTransform.localScale = flipped;
            mossKnightTransform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    public void evadeEvent()
    {
        int randSound = Random.Range(1, 5);
        int direction = isFlipped==true ? -1 : 1;
        mossKnightRigidbody.AddForce(new Vector2(direction*evadeRange, 0));

        audioManager.playSound("AttackYell"+randSound);
    }

    public void shootEvent()
    {
        // create an instance of projectile
        GameObject instance = (GameObject)Instantiate(projectile, shotPoint.position, shotPoint.rotation);

        int direction = isFlipped==true ? 1 : -1;
        Vector2 velocity = new Vector2(15*direction, 15);
        
        instance.GetComponent<Rigidbody2D>().velocity = velocity;
    }

    public void slashEvent()
    {
        int randSound = Random.Range(1, 5);
        audioManager.playSound("AttackYell"+randSound);
        audioManager.playSound("Slash");
    }

    private void DeathControl()
    {
        if(healthController.getHealthPoint() <= 0 && !isDeath)
        {
            animator.SetTrigger("Death");
            StartCoroutine(DeathCoroutine());

            MusicController musicController = FindObjectOfType<MusicController>();
            musicController.fightEndCoroutine();

            isDeath = true;
        }
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(airDeathInterval);
        animator.ResetTrigger("Death");

        yield return new WaitForSeconds(landDeathInterval);
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

    public void setIsShield()
    {
        this.isSkill = true;
    }

    public void resetIsShield()
    {
        this.isSkill = false;
    }

    public bool getIsShield()
    {
        return isSkill;
    }

}
