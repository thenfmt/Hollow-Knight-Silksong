using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulWarriorBehaviour : StateMachineBehaviour
{
    [SerializeField] float runSpeed;

    private Transform soulWarriorTransform;
    private Transform playerTransform;
    private Rigidbody2D soulWarriorRigidbody;
    private Rigidbody2D playerRigidbody;
    private SoulWarriorController soulWarriorController;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();

        soulWarriorTransform = animator.GetComponent<Transform>();
        soulWarriorRigidbody = animator.GetComponent<Rigidbody2D>();
        soulWarriorController = animator.GetComponent<SoulWarriorController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!soulWarriorController.getIsSkill())
        {
            moveToPlayerPosition();
            skillControl(animator);
        }
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.ResetTrigger("Attack");
       animator.ResetTrigger("Teleport");
       animator.ResetTrigger("DashTeleport");
       animator.ResetTrigger("DashRecover");
       animator.ResetTrigger("Evade");
       animator.ResetTrigger("Stomp");
       animator.ResetTrigger("Cast");
    }


    private void moveToPlayerPosition()
    {
        soulWarriorController.lookAtPlayer();
        
        Vector2 target = new Vector2(playerTransform.position.x, soulWarriorTransform.position.y);
        Vector2 newPosition = Vector2.MoveTowards(soulWarriorRigidbody.position, target, runSpeed*Time.fixedDeltaTime);
        soulWarriorRigidbody.MovePosition(newPosition);
        
    }


    private void skillControl(Animator animator)
    {
        int randSkill = Random.Range(0, 3);

        if(randSkill == 0)
        {
            animator.SetTrigger("Attack");
        }
        else if(randSkill == 1)
        {
            animator.SetTrigger("Evade");
        }
        else 
        {
            animator.SetTrigger("Teleport");
        }
        
    }
}