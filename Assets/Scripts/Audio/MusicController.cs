using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] float fightPrepareInterval;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = GetComponent<AudioManager>();

        audioManager.playSound("Main");
    }


    public IEnumerator fightPrepareCoroutine()
    {
        audioManager.playSound("FightPrepare4");

        yield return new WaitForSeconds(fightPrepareInterval);

        audioManager.playSound("Action");
        audioManager.stopSound("Main");
    }

    public void fightEndCoroutine()
    {
        audioManager.playSound("FightPrepare3");
        audioManager.stopSound("Action");
        audioManager.playSound("Main");
    }
}
