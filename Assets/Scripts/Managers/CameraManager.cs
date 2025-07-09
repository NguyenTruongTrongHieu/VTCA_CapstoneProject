using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //Vertical FOV: 35 - 45
    //Hard Look At Offset Z: 0, 0.7, 1
    public static CameraManager instance;
    public CinemachineCamera cineCam;

    private void Awake()
    {
        if (instance == null)
        { 
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        { 
            Destroy(gameObject);
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

    public IEnumerator SetHardLookAt(float speed, char offsetPos, float target)
    {
        if (offsetPos == 'X' || offsetPos == 'x')
        { 
            float currentLookAtX = cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.x;
            if (currentLookAtX < target)
            {
                while (cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.x < target)
                {
                    cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.x += speed * Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else if (currentLookAtX > target)
            {
                while (cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.x > target)
                {
                    cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.x -= speed * Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else
            {
                yield return null; // Invalid offset position, exit coroutine
            }
        }
        else if(offsetPos == 'Y' || offsetPos == 'y')
        {
            float currentLookAtY = cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.y;
            if (currentLookAtY < target)
            {
                while (cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.y < target)
                {
                    cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.y += speed * Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else if (currentLookAtY > target)
            {
                while (cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.y > target)
                {
                    cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.y -= speed * Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else
            {
                yield return null; // Invalid offset position, exit coroutine
            }
        }
        else if (offsetPos == 'Z' || offsetPos == 'z')
        {
            float currentLookAtZ = cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.z;
            if (currentLookAtZ < target)
            {
                while (cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.z < target)
                {
                    cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.z += speed * Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else if (currentLookAtZ > target)
            {
                while (cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.z > target)
                {
                    cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.z -= speed * Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else
            {
                yield return null; // Invalid offset position, exit coroutine
            }
        }
        else
        {
            yield return null; // Invalid offset position, exit coroutine
        }
    }
}
