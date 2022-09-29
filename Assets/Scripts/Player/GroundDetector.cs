using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [SerializeField] GameObject landLeafParticle;

    CharacterController2D character;
    Transform detectorTransform;

    private void Awake()
    {
        character = FindObjectOfType<CharacterController2D>();
        detectorTransform = GetComponent<Transform>();
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            character.setIsGround(true);

            if(collision.gameObject.tag == "Grass")
            {
                // create an instance of particle
                Instantiate(landLeafParticle, detectorTransform.position, detectorTransform.rotation);
            }

            if(collision.gameObject.tag == "Trap" || collision.gameObject.tag == "Acid")
            {
                character.takeDamage(100);
            }
        }
    }

   private void OnTriggerStay2D(Collider2D collision)
    {   
        if(collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            character.setIsGround(true);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {   
        if(collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            character.setIsGround(false);
        }
    }
}
