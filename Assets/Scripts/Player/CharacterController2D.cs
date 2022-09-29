using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    private bool isGround;
    private bool isFalling;
    private bool isRunning;
    private bool isClimb;
    private bool isDeath;
    private bool isJumping;
    private bool isFacingLeft = true;
    private bool isFreezeInputMovement;
    private bool isInSkillInterval;
    private bool isInputable;
    private bool isGameIntro;

    private float gravityScale;

    [SerializeField] float gameIntroInterval;

    [Header("Movement Section")]
    [SerializeField] float runSpeed;


    [Header("Climb Jump Section")]
    [SerializeField] float climbJumpRange;
    [SerializeField] float climbJumpHeight;
    [SerializeField] float climbJumpInterval;
    private float xVelocity;


    [Header("Jump Section")]
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpAnticipateInterval;


    [Header("Evade Section")]
    [SerializeField] float evadeDistance;
    [SerializeField] float evadeAnticipateInterval;
    [SerializeField] float evadeInterval;
    [SerializeField] float evadeCooldownInterval;
    private bool isEvadeable;


    [Header("Sphere Section")]
    [SerializeField] float airSphereInterval;
    [SerializeField] float groundSphereInterval;
    [SerializeField] float sphereCooldownInterval;
    private bool isSphereable;


    [Header("Dash Section")]
    [SerializeField] float groundDashAnticipateInterval;
    [SerializeField] float groundDashInterval;
    [SerializeField] float groundDashRecoverInterval;
    [SerializeField] float groundDashDistance;
    [SerializeField] float groundDashEffectInterval;

    [SerializeField] float airDashAnticipateInterval;
    [SerializeField] float airDashVelocity;
    [SerializeField] float airDashEffectInterval;
    [SerializeField] float airDashRecoverInterval;

    [SerializeField] float dashCooldownInterval;
    private bool isDashable;
    private bool isAirDash;



    [Header("Recoil Section")]
    [SerializeField] float recoilHeight;
    [SerializeField] float recoilCooldownInterval;


    [Header("Javelin Throw Section")]
    [SerializeField] float javelinThrowAnticipateInterval;
    [SerializeField] float javelinThrowInterval;
    [SerializeField] float javelinThrowCooldownInterval;
    [SerializeField] GameObject javelin;
    [SerializeField] Transform javelinStartPoint;
    private bool isJavelinThrowable;



    [Header("Hurt Section")]
    [SerializeField] GameObject hitParticle;
    [SerializeField] Color invulnerableColor;
    [SerializeField] float invulnerableInterval;
    [SerializeField] float freezeSenceInterval;

    [Space]
    
    [SerializeField] CameraShake cameraShake;
    [SerializeField] float deathEffectInterval;
    private bool isInvincible;
    private Vector3 recoverPosition;


    [Header("Healing Section")]
    [SerializeField] GameObject healingParticle;
    [SerializeField] int healRecover;
    [SerializeField] float healingInterval;
    [SerializeField] float healingCooldownInterval;
    private bool isHealable;



    private Transform controllerTransform;
    private Rigidbody2D controllerRigidbody;
    private SpriteRenderer controllerSpriteRenderer;
    private Animator animator;
    private Effect effect;
    private HealthController healthController;
    private AudioManager audioManager;


    private void Start()
    {
        controllerTransform = GetComponent<Transform>();
        controllerRigidbody = GetComponent<Rigidbody2D>();
        controllerSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        effect = GameObject.Find("Dash_Effect").GetComponent<Effect>();
        healthController = GetComponent<HealthController>();
        //audioManager = FindObjectOfType<AudioManager>();
        audioManager = GetComponent<AudioManager>();
        
        isSphereable = true;
        isHealable = true;
        isDeath = false;
        isEvadeable = true;
        isDashable = true;
        isAirDash = false;
        isInSkillInterval = false;
        isJavelinThrowable = true;
        isInvincible = false;
        isFreezeInputMovement = false;
        isInputable = true;
        gravityScale = controllerRigidbody.gravityScale;
        xVelocity = 0;

        recoverPosition = controllerTransform.position;

        StartCoroutine(gameIntroCoroutine());
    }

    private void Update()
    {
        if(isGameIntro)
        {
            return;
        }

        updateCharacterState();
        jumpControl();
        evadeControl();
        sphereControl();
        dashControl();
        healControl();
        javelinThrowControl();
    }

    private void FixedUpdate()
    {
        if(isGameIntro)
        {
            return;
        }
        movement();
    }

    private IEnumerator gameIntroCoroutine()
    {

        isGameIntro = true;

        yield return new WaitForSeconds(gameIntroInterval);

        isGameIntro = false;
    }

    private void updateCharacterState()
    {
        if(isDeath)
        {
            controllerRigidbody.velocity = Vector2.zero;
        }


        isFalling = controllerRigidbody.velocity.y < 0 ? true : false;
        isFalling = isFalling && !isInSkillInterval && !isJumping;
        animator.SetBool("isFalling", isFalling);

        if(isGround)
        {
            isClimb = false;
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("ClimbJump");
            animator.SetBool("isFalling", false);
        }
    }



    // Movement section
    private void movement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");

        if(!isClimb && !isFreezeInputMovement)
        {
            // xVelocity of climb jump
            controllerRigidbody.velocity = new Vector2(runSpeed*horizontalMovement*Time.fixedDeltaTime + xVelocity, controllerRigidbody.velocity.y);

            // Flip character
            if(horizontalMovement > 0 && isFacingLeft)
            {
                Flip();
            }
            else if(horizontalMovement < 0 && !isFacingLeft)
            {
                Flip();
            }
        }

        if(horizontalMovement == 0 || !isGround)
        {
            animator.SetBool("isRunning", false);
            audioManager.stopSound("Run");
            isRunning = false;
        }

        if(isGround && horizontalMovement!=0 && !isRunning)
        {
            isRunning = true;
            animator.SetBool("isRunning", true);

            audioManager.playSound("Run");
        }
    }

    private void Flip()
    {
        isFacingLeft = !isFacingLeft;
        Vector3 theScale = controllerTransform.localScale;
        theScale.x *= -1;
        controllerTransform.localScale = theScale;

        javelinStartPoint.Rotate(0, 180f, 0);
        GameObject.Find("Air_Dash_Effect_Point").GetComponent<Transform>().Rotate(0, 180f, 0);
    }




    // Jump section
    private void jumpControl()
    {
        if(Input.GetKeyDown(KeyCode.W) && isInputable)
        {
            if(isClimb)
            {
                StartCoroutine(climbJumpCoroutine());
            }
            else if(isGround)
            {
                StartCoroutine(jumpCoroutine());
            }
        }

        if(isJumping)
        {
            if(Input.GetKeyUp(KeyCode.W))
            {
                isJumping = false;
            }
        }
    }

    private IEnumerator jumpCoroutine()
    {
        isJumping = true;
        animator.SetTrigger("Jump");
        audioManager.playSound("Jump");

        // wait for Jump_Anticipate animation to be finished before playing Jump animation
        yield return new WaitForSeconds(jumpAnticipateInterval);

        // Jumping a specific height using Velocity
        // Using time-independent acceleration formula to calculate velocity 
        Vector2 jumpVelocity = new Vector2(controllerRigidbody.velocity.x, Mathf.Sqrt(jumpHeight*(-2)*(Physics2D.gravity.y*controllerRigidbody.gravityScale)));
        
        controllerRigidbody.velocity = jumpVelocity;
    }

    private IEnumerator climbJumpCoroutine()
    {
        // Jumping a specific range and time interval
        // Using projectile motion equations to calculate velocity
        float yVelocity = climbJumpHeight / climbJumpInterval;

        xVelocity = climbJumpRange / climbJumpInterval;

        // Jump opposite direction
        xVelocity = isFacingLeft==true ? xVelocity : -xVelocity;

        animator.SetTrigger("ClimbJump");
        audioManager.playSound("ClimbJump");
        controllerRigidbody.velocity = new Vector2(xVelocity, yVelocity);

        yield return new WaitForSeconds(climbJumpInterval);

        xVelocity = 0f;
    }



    // Evade section
    private void evadeControl()
    {
        if(Input.GetKeyDown(KeyCode.S) && isGround && isEvadeable && isInputable)
        {
            StartCoroutine(evadeCoroutine());
        }
    }

    private IEnumerator evadeCoroutine()
    {
        freezeGravity();
        
        isEvadeable = false;
        audioManager.playSound("EvadeLaugh");
        audioManager.playSound("Dash");
        animator.SetTrigger("Evade");

        yield return new WaitForSeconds(evadeAnticipateInterval);

        int direction = isFacingLeft== true ? 1 : -1;
        controllerRigidbody.velocity = Vector2.right*direction*(evadeDistance/evadeInterval);

        yield return new WaitForSeconds(evadeInterval);
        unFreezeGravity();
        animator.ResetTrigger("Evade");

        yield return new WaitForSeconds(evadeCooldownInterval);
        isEvadeable = true;
    }



    // Sphere section
    private void sphereControl()
    {
        if(Input.GetKeyDown(KeyCode.E) && isSphereable && !isInSkillInterval && isInputable)
        {
            StartCoroutine(sphereCoroutine());
        }
    }

    private IEnumerator sphereCoroutine()
    {
        
        freezeGravity();
        isInSkillInterval = true;
        audioManager.playSound("Sphere");

        isSphereable = false;
        if(isGround)
        {
            audioManager.playSound("GroundSphereYell");
            animator.SetTrigger("GroundSphere");
            yield return new WaitForSeconds(groundSphereInterval);
        }
        else
        {
            audioManager.playSound("AirSphereYell");
            animator.SetTrigger("AirSphere");
            yield return new WaitForSeconds(airSphereInterval);
        }

        unFreezeGravity();
        
        animator.ResetTrigger("GroundSphere");
        animator.ResetTrigger("AirSphere");
        isInSkillInterval = false;

        yield return new WaitForSeconds(sphereCooldownInterval);
        isSphereable = true; 
    }



    // Dash section
    private void dashControl()
    {
        if(Input.GetKeyDown(KeyCode.Q) && isDashable && !isInSkillInterval && isInputable)
        {
            if(isClimb)
            {
                Flip();
            }

            StartCoroutine(dashCoroutine());
        }
    }


    private IEnumerator dashCoroutine()
    {
        isDashable = false;
        isInSkillInterval = true;
        freezeGravity();

        audioManager.playSound("Dash");
        if(isGround)
        {
            audioManager.playSound("GroundDashYell");
            animator.SetTrigger("GroundDash");
            yield return new WaitForSeconds(groundDashAnticipateInterval);

            effect.dashEffect("GroundDash", groundDashEffectInterval, isFacingLeft, 0);

            int direction = isFacingLeft==true ? -1 : 1;
            Vector2 dashVelocity = Vector2.right*direction*(groundDashDistance/groundDashInterval);
            controllerRigidbody.velocity = dashVelocity;
            yield return new WaitForSeconds(groundDashInterval);
            

            // add a force to stop the character
            // the force is calculated from the first kinematic formula and Newton's second law
            controllerRigidbody.AddForce(controllerRigidbody.mass*(-dashVelocity*2/groundDashRecoverInterval));
            animator.SetTrigger("DashRecover");
            yield return new WaitForSeconds(groundDashRecoverInterval);


            unFreezeGravity();
            animator.ResetTrigger("GroundDash");
            animator.ResetTrigger("DashRecover");
            isInSkillInterval = false;

            yield return new WaitForSeconds(dashCooldownInterval);

            isDashable = true;
        }
        else
        {
            audioManager.playSound("AirDashYell");
            isAirDash = true;
            animator.SetTrigger("AirDash");
            yield return new WaitForSeconds(airDashAnticipateInterval);

            int direction = isFacingLeft==true ? -1 : 1;
            controllerRigidbody.velocity = new Vector2(direction*airDashVelocity, -airDashVelocity);

            controllerTransform.rotation = Quaternion.Euler(0f, 0f, -direction*45);

            effect.dashEffect("AirDash", airDashEffectInterval, isFacingLeft, 45);
        }
    }

    public IEnumerator airDashRecoverCoroutine(bool isRecoil)
    {

        controllerTransform.rotation = Quaternion.Euler(0f, 0f, 0f);

        unFreezeGravity();

        isAirDash = false;
        animator.ResetTrigger("AirDash");
        

        if(isRecoil)
        {
            // recoil a specific height using Velocity
            // Using time-independent acceleration formula to calculate velocity 
            Vector2 recoilVelocity = new Vector2(controllerRigidbody.velocity.x, Mathf.Sqrt(recoilHeight*(-2)*(Physics2D.gravity.y*controllerRigidbody.gravityScale)));
            controllerRigidbody.velocity = recoilVelocity;

            // if air dash hit the enemy or trap will reset the skill
            animator.SetTrigger("AirRecoil");
            isDashable = true;
            isInSkillInterval = false;
            isJumping = true;

            yield return new WaitForSeconds(recoilCooldownInterval);
            animator.ResetTrigger("AirRecoil");
            isJumping = false;
        }
        else
        {
            // recoil a specific height using Velocity
            // Using time-independent acceleration formula to calculate velocity 
            Vector2 recoilVelocity = new Vector2(controllerRigidbody.velocity.x, Mathf.Sqrt((-2f)*(Physics2D.gravity.y*controllerRigidbody.gravityScale)));
            controllerRigidbody.velocity = recoilVelocity;

            animator.SetTrigger("DashRecover");
            isInSkillInterval = false;
            isJumping = true;

            yield return new WaitForSeconds(airDashRecoverInterval);
            animator.ResetTrigger("DashRecover");
            isJumping = false;
            
            yield return new WaitForSeconds(dashCooldownInterval);
            isDashable = true;
        }
    }



    // Javelin throw section
    private void javelinThrowControl()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isJavelinThrowable && !isInSkillInterval && isInputable && !isClimb)
        {
            StartCoroutine(javelinThowCoroutine());
        }
    }

    private IEnumerator javelinThowCoroutine()
    {
        isJavelinThrowable = false;
        animator.SetTrigger("JavelinThrow");
        audioManager.playSound("JavelinThrow");
        audioManager.playSound("JavelinThrowYell");

        yield return new WaitForSeconds(javelinThrowAnticipateInterval);

        // create an instance of javelin
        Instantiate(javelin, javelinStartPoint.position, javelinStartPoint.rotation);

        yield return new WaitForSeconds(javelinThrowInterval);
        animator.ResetTrigger("JavelinThrow");

        yield return new WaitForSeconds(javelinThrowCooldownInterval);
        isJavelinThrowable = true;
    }



    // Hurt section
    public void takeDamage(int damage)
    {
        if(isInvincible)
        {
            return;
        }

        // Health bar
        GameObject healthBar = GameObject.Find("hp_" + healthController.getHealthPoint());
        healthBar.GetComponent<Animator>().SetTrigger("Break");

        if(healthController.getHealthPoint() == 2)
        {
            GameObject.Find("Vintage_Low_Health").GetComponent<SpriteRenderer>().enabled = true;
        }

        audioManager.playSound("TakeDamage");
        healthController.takeDamage(damage);
        if(healthController.getHealthPoint() <= 0 && !isDeath)
        {
            StartCoroutine(dieCoroutine());
            StartCoroutine(cameraShake.startShake(deathEffectInterval));
        }
        else
        {
            StartCoroutine(stunCoroutine());
            StartCoroutine(invulnerableCoroutine());
        }
    }


    private IEnumerator stunCoroutine()
    {
        freezeGravity();
        animator.SetTrigger("Stun");


        // create an instance of particle
        Instantiate(hitParticle, controllerTransform.position, controllerTransform.rotation);

        isInputable = false;

        yield return new WaitForSeconds(0.01f*freezeSenceInterval);

        isInputable = true;
        unFreezeGravity();

        yield return new WaitForSeconds(1f);
        animator.ResetTrigger("Stun");
    }


    private IEnumerator dieCoroutine()
    {
        freezeGravity();
        isDeath = true;
        audioManager.playSound("DeathYell");
        yield return new WaitForSeconds(0.1f);
        audioManager.playSound("DeathSound");
        animator.SetTrigger("Death");
        isInvincible = true;
        isInputable = false;

        yield return new WaitForSeconds(deathEffectInterval);

        // health recover
        healthController.healing(5);
        GameObject.Find("Vintage_Low_Health").GetComponent<SpriteRenderer>().enabled = false;
        for(int i = 1; i <= 5; i++)
        {
            // Health bar
            GameObject healthBar = GameObject.Find("hp_" + i);
            healthBar.GetComponent<Animator>().SetTrigger("Health");
        }

        isInputable = true;
        controllerTransform.position = recoverPosition;
        animator.ResetTrigger("Death");
        unFreezeGravity();
        isDeath = false;
        isInvincible = false;
        StartCoroutine(gameIntroCoroutine());
    }

    private IEnumerator invulnerableCoroutine()
    {
        isInvincible = true;

        // collor flash color
        controllerSpriteRenderer.color = invulnerableColor;
        yield return new WaitForSeconds(invulnerableInterval);
        controllerSpriteRenderer.color = Color.white;

        isInvincible = false;
    }



    // healing section
    private void healControl()
    {
        if(Input.GetKeyDown(KeyCode.H) && !isInSkillInterval && isHealable && isInputable && !isClimb)
        {
            StartCoroutine(healCoroutine());
        }
    }

    private IEnumerator healCoroutine()
    {
        isHealable = false;
        isInSkillInterval = true;
        freezeGravity();
        animator.SetTrigger("Healing");

        yield return new WaitForSeconds(healingInterval);
        animator.ResetTrigger("Healing");
        isInSkillInterval = false;
        unFreezeGravity();
        healthController.healing(healRecover);

        


        // Health bar
        GameObject healthBar = GameObject.Find("hp_" + healthController.getHealthPoint());
        healthBar.GetComponent<Animator>().SetTrigger("Health");

        if(healthController.getHealthPoint() == 2)
        {
            GameObject.Find("Vintage_Low_Health").GetComponent<SpriteRenderer>().enabled = false;
        }

        yield return new WaitForSeconds(healingCooldownInterval);
        isHealable = true;
    }

    public void heallingEvent()
    {
        // create an instance of blood leak out
        Vector3 vectorTransform = Vector3.up*1;
        Instantiate(healingParticle, controllerTransform.position + vectorTransform, controllerTransform.rotation);
    }




    // other section
    private void freezeGravity()
    {
        controllerRigidbody.gravityScale = 0;
        isFreezeInputMovement = true;
        controllerRigidbody.velocity = Vector2.zero;
    }

    private void unFreezeGravity()
    {
        isFreezeInputMovement = false;
        controllerRigidbody.gravityScale = gravityScale;
    }

    public void blackScreen(int input)
    {
        bool isRender = input==1 ? true : false;
        GameObject.Find("Black_Solid").GetComponent<SpriteRenderer>().enabled = isRender;
    }


    // getter, setter region
    public void setIsGround(bool isGround)
    {
        this.isGround = isGround;
        animator.SetBool("isGround", isGround);
        //audioManager.playSound("Land");
    }

    public void setIsClimb(bool isClimb)
    {
        this.isClimb = isClimb && !isGround;
        animator.SetBool("isClimb", this.isClimb);
        if(isClimb)
        {
            audioManager.playSound("Climb");
            controllerRigidbody.velocity = Vector2.zero;
            controllerRigidbody.gravityScale = gravityScale/4;
        }
        else  
        {
            controllerRigidbody.gravityScale = gravityScale;
        }
    }

    public int getDirection()
    {
        return isFacingLeft==true ? -1 : 1;
    }

    public bool getIsAirDash()
    {
        return isAirDash;
    }

    public void setInvincible(int isInvincible)
    {
        this.isInvincible = isInvincible==0 ? false : true;
    }
}
