using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private MossKnightController mossKnightController;
    private Animator animator;

    private void Awake()
    {
        mossKnightController = FindObjectOfType<MossKnightController>();
        animator = mossKnightController.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if((collider.gameObject.tag == "Player" || collider.gameObject.tag == "Javelin") && mossKnightController.getIsShield())
        {
            animator.SetTrigger("hitShield");
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        animator.ResetTrigger("hitShield");
    }
}
