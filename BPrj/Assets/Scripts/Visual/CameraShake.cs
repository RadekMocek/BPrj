using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineVirtualCamera VC;
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    private float shakeStartTime;

    private readonly float shakeDuration = .1f;

    private void Awake()
    {
        Instance = this;

        VC = GetComponent<CinemachineVirtualCamera>();
        cameraNoise = VC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity = 3)
    {
        cameraNoise.m_AmplitudeGain = intensity;
        shakeStartTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > shakeStartTime + shakeDuration) {
            cameraNoise.m_AmplitudeGain = 0;
        }
    }
}
