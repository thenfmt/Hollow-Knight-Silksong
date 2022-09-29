using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Transform centreGravity;
    [SerializeField] float recoilForce;

    private Rigidbody2D enemyRigidbody;
    private HealthController healthController;

    private void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        healthController = GetComponent<HealthController>();
    }

    public void takeDamage(int damage, Vector2 direction)
    {
        enemyRigidbody.AddForce(Vector2.right*direction*recoilForce);
        healthController.takeDamage(damage);
    }

    public Vector3 getCentrePosition()
    {
        return centreGravity.position;
    }
}
