using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] int maxHealthPoint;

    private int healthPoint;

    private void Start()
    {
        healthPoint = maxHealthPoint;
    }

    public void takeDamage(int damage)
    {
        healthPoint = Mathf.Max(0, healthPoint-damage);
    }

    public void healing(int healthRecover)
    {
        healthPoint = Mathf.Min(healthRecover+healthPoint, maxHealthPoint);
    }

    public int getHealthPoint()
    {
        return healthPoint;
    }
}
