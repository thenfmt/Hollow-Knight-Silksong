using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetector : MonoBehaviour
{
    [SerializeField] float hitEffectInterval;
    [SerializeField] bool isHitRecoil;
    [SerializeField] GameObject hitEnemyParticle;
    [SerializeField] Transform hitPoint;

    private CharacterController2D character;
    private Transform weaponDetectorTransform;
    private Effect effect;
    AudioManager audioManager;

    private void Start()
    {
        character = FindObjectOfType<CharacterController2D>();
        weaponDetectorTransform = GetComponent<Transform>();
        effect = GameObject.Find("Hit_Effect").GetComponent<Effect>();
        audioManager = character.GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // calculate the angle between weapon and enemy for hitParticle
        float deltaX = weaponDetectorTransform.position.x - collider.gameObject.transform.position.x;
        float deltaY = weaponDetectorTransform.position.y - collider.gameObject.transform.position.y;
        float angle = Mathf.Atan2(deltaX, deltaY)*Mathf.Rad2Deg;

        Vector2 hitDirection;
        hitDirection.x = deltaX>=0 ? -1 : 1;
        hitDirection.y = deltaY>=0 ? -1 : 1;

        if(collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyController enemyController = collider.gameObject.GetComponent<EnemyController>();
            HealthController enemyHealthController = collider.gameObject.GetComponent<HealthController>();

            // stop airDash when the needle hit something
            if(character.getIsAirDash())
            {
                StartCoroutine(character.airDashRecoverCoroutine(true));
            }

            if(enemyHealthController.getHealthPoint() > 1)
            {
                audioManager.playSound("HitEnemy");
            }
            else
            {
                audioManager.playSound("LastHitEnemy");
            }

            enemyController.takeDamage(1, hitDirection);

            // web leak out effect
            Instantiate(hitEnemyParticle, enemyController.getCentrePosition(), Quaternion.Euler(0f, 0f, angle));

            if(hitPoint != null)
            {
                effect.hitEffect("HitEnemy", hitEffectInterval, hitPoint.position, 15f);
            }
        }
        else if(collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            
            if(collider.gameObject.tag == "Trap")
            {
                // stop airDash when the needle hit something
                if(character.getIsAirDash())
                {
                    StartCoroutine(character.airDashRecoverCoroutine(true));
                }
                audioManager.playSound("HitReject");
            }
            else
            {
                // stop airDash
                if(character.getIsAirDash())
                {
                    character.StartCoroutine(character.airDashRecoverCoroutine(false));
                }
                audioManager.playSound("HitTerrain");
            }

            if(hitPoint != null)
            {
                effect.hitEffect("HitTerrain", hitEffectInterval, hitPoint.position, 15f);
            }
        }
        else if(collider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            
            if(collider.gameObject.tag == "Shield")
            {
                // stop airDash when the needle hit something
                if(character.getIsAirDash())
                {
                    StartCoroutine(character.airDashRecoverCoroutine(true));
                }
                audioManager.playSound("HitReject");
            }

            if(hitPoint != null)
            {
                effect.hitEffect("HitTerrain", hitEffectInterval, hitPoint.position, 15f);
            }
        }
    }
}
