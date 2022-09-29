using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    Animator animator;
    Renderer effectRenderer;
    Transform effectTransform;

    [SerializeField] Transform groundDetector;

    private void Start()
    {
        animator = GetComponent<Animator>();
        effectRenderer = GetComponent<Renderer>();
        effectTransform = GetComponent<Transform>();
    }


    // dash effect
    public void dashEffect(string effectTriggerName, float effectInterval, bool isFacingLeft, float angle)
    {
        int direction = isFacingLeft==true ? 1 : -1;
        Vector3 transformVector = new Vector3(3f*direction, -0.5f, 0);

        effectTransform.position = groundDetector.position + transformVector;
        effectTransform.localScale = new Vector3(direction, 1, 1);
        effectTransform.Rotate(0, 0, direction*angle);

        effectRenderer.enabled = true;
        animator.SetTrigger(effectTriggerName);

        StartCoroutine(finishEffectCorountine(effectTriggerName, effectInterval, direction*angle));
    }

    


    // hit effect
    public void hitEffect(string effectTriggerName, float effectInterval, Vector3 position, float angle)
    {
        effectTransform.position = position;
        effectTransform.Rotate(0, 0, angle);

        effectRenderer.enabled = true;
        animator.SetTrigger(effectTriggerName);

        StartCoroutine(finishEffectCorountine(effectTriggerName, effectInterval, angle));
    }


    // finish effect
    private IEnumerator finishEffectCorountine(string effectTriggerName, float effectInterval, float angle)
    {
        yield return new WaitForSeconds(effectInterval);
        effectRenderer.enabled = false;
        animator.ResetTrigger(effectTriggerName);
        effectTransform.Rotate(0, 0, 360-angle);
    }
}
