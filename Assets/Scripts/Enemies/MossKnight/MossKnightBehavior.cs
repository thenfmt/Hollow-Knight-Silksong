using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossKnightBehavior : StateMachineBehaviour
{
    
    [SerializeField] float runSpeed;
    [SerializeField] float attackActiveRange;
    [SerializeField] float shieldActiveRange;
    [SerializeField] float evadeActiveRange;


    private Rigidbody2D mossKnightRigidbody;
    private Transform mossKnightTransform;
    private MossKnightController mossKnightController;
    private Transform playerTransform;
    private Rigidbody2D playerRigidbody;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();

        mossKnightTransform = animator.GetComponent<Transform>();
        mossKnightRigidbody = animator.GetComponent<Rigidbody2D>();
        mossKnightController = animator.GetComponent<MossKnightController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Vector2.Distance(playerTransform.position, mossKnightTransform.position) < evadeActiveRange)
        {
            animator.Play("Evade");
        }
        else 
        {
            moveToPlayerPosition();
            attackControl(animator);
            shieldControl(animator);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.ResetTrigger("Slash");
       animator.ResetTrigger("ShieldTop");
       animator.ResetTrigger("ShieldFront");
       animator.ResetTrigger("Shoot");
       animator.ResetTrigger("Evade");
    }


    private void moveToPlayerPosition()
    {
        mossKnightController.lookAtPlayer();
        
        Vector2 target = new Vector2(playerTransform.position.x, mossKnightTransform.position.y);
        Vector2 newPosition = Vector2.MoveTowards(mossKnightRigidbody.position, target, runSpeed*Time.fixedDeltaTime);
        mossKnightRigidbody.MovePosition(newPosition);
    }


    private void attackControl(Animator animator)
    {
        if(Vector2.Distance(playerTransform.position, mossKnightTransform.position) < attackActiveRange
        && Vector2.Distance(playerTransform.position, mossKnightTransform.position) > shieldActiveRange
        && !mossKnightController.getIsSkill())
        {
            int skillRandom = Random.Range(0, 3);
            mossKnightController.setIsSkill();

            if(skillRandom == 0)
            {
                animator.SetTrigger("Slash");
            }
            else if(skillRandom == 1)
            {
                animator.SetTrigger("Evade");
            }
            else
            {
                animator.SetTrigger("Shoot");
            }
        }
    }


    private void shieldControl(Animator animator)
    {
        // detect character 
        if(Vector2.Distance(playerTransform.position, mossKnightTransform.position) < shieldActiveRange)
        {
            if(playerTransform.position.y > mossKnightTransform.position.y+3)
            {
                animator.SetTrigger("ShieldTop");
            }
            else
            {
                animator.SetTrigger("ShieldFront");
            }
        }


        // detect weapon
        GameObject javelin = GameObject.FindGameObjectWithTag("Javelin");
        if(javelin != null)
        {
            if(Vector2.Distance(javelin.transform.position, mossKnightTransform.position) < shieldActiveRange)
            {
                if(playerTransform.position.y > mossKnightTransform.position.y+3)
                {
                    animator.SetTrigger("ShieldTop");
                }
                else
                {
                    animator.SetTrigger("ShieldFront");
                }
            }
        }
    }
}
