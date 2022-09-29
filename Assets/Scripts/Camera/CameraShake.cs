using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera vCamera;

    private void Start()
    {
        vCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public IEnumerator startShake(float shakeInterval)
    {
        vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2;
        vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 2;

        yield return new WaitForSeconds(shakeInterval);

        vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
    }
}
