using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CinemachineCamera cineCam;

    private void Awake()
    {
        if (instance == null)
        { 
            instance = this;
        }
    }

    public void SetTargetForCam(Transform target)
    {
        cineCam.Follow = target;
    }

    public void SetScreenPosCompositionWhenPlaying()
    { 
        cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.x = -0.25f;
    }

    public void SetScreenPosCompositionWhenNotPlaying()
    { 
        cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.x = 0f;
    }

    public IEnumerator SetScreenPosComposition(float speed, bool isSetScreenPosX, float screenPosTarget)
    {
        if (isSetScreenPosX)
        {
            float currentScreenPos = cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.x;
            if (currentScreenPos < screenPosTarget)
            {
                while (cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.x < screenPosTarget)
                {
                    cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.x += speed * Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else if (currentScreenPos > screenPosTarget)
            {
                while (cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.x > screenPosTarget)
                {
                    cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.x -= speed * Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else
                yield return null; // No change needed, exit coroutine
        }
        else
        {
            float currentScreenPos = cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.y;
            if (currentScreenPos < screenPosTarget)
            {
                while (cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.y < screenPosTarget)
                {
                    cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.y += speed * Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else if (currentScreenPos > screenPosTarget)
            {
                while (cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.y > screenPosTarget)
                {
                    cineCam.GetComponent<CinemachineRotationComposer>().Composition.ScreenPosition.y -= speed * Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else
                yield return null; // No change needed, exit coroutine
        }
    }
}
