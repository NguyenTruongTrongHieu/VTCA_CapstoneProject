using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //Vertical FOV: 30, 40: Đòn đánh special; 35, 50: cam khi gặp enemy và boss; 35: cam bình thường
    //Hard Look At Offset Z: 0: cam bình thường; 0.7: cam khi gặp enemy; 1:  Đòn đánh special, cam khi gặp boss
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

    public void ResetCamForPlayer()
    {
        SetTargetForCam(PlayerUltimate.instance.playerTransform);
        StartCoroutine(SetHardLookAt(0.5f, 'Z', 0f));
        StartCoroutine(SetHardLookAt(0.5f, 'X', 0f));
        StartCoroutine(SetHardLookAt(0.5f, 'Y', 0f));
        StartCoroutine(SetVerticalFOV(35f, 0.5f)); // Reset vertical FOV to 35
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
            // Ensure the final value is set to the target
            cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.x = target;
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

            cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.y = target;
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

            cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.z = target;
        }
        else
        {
            yield return null; // Invalid offset position, exit coroutine
        }
    }

    public IEnumerator SetVerticalFOV(float target, float duration)
    {
        Debug.Log("Setting vertical FOV to: " + target + " over duration: " + duration);
        if (target < cineCam.Lens.FieldOfView)
        {
            // Decrease FOV to target with lerp and duration, don't use,deltaTime, use reality time
            float startTime = Time.realtimeSinceStartup;
            float endTime = startTime + duration;
            float initialFOV = cineCam.Lens.FieldOfView;

            while (Time.realtimeSinceStartup < endTime)
            {
                float t = (Time.realtimeSinceStartup - startTime) / duration;
                cineCam.Lens.FieldOfView = Mathf.Lerp(initialFOV, target, t);
                yield return null; // Wait for the next frame
            }

            cineCam.Lens.FieldOfView = target; // Ensure it ends exactly at target
        }
        else if (target > cineCam.Lens.FieldOfView)
        {
            // Increase FOV to target with lerp and duration, don't use,deltaTime, use reality time
            float startTime = Time.realtimeSinceStartup;
            float endTime = startTime + duration;
            float initialFOV = cineCam.Lens.FieldOfView;

            while (Time.realtimeSinceStartup < endTime)
            {
                float t = (Time.realtimeSinceStartup - startTime) / duration;
                cineCam.Lens.FieldOfView = Mathf.Lerp(initialFOV, target, t);
                yield return null; // Wait for the next frame
            }

            cineCam.Lens.FieldOfView = target; // Ensure it ends exactly at target
            //float duration = 0.5f; // Duration of the FOV change
            //float elapsedTime = 0f;
            //float initialFOV = cineCam.Lens.FieldOfView;
            //while (elapsedTime < duration)
            //{
            //    cineCam.Lens.FieldOfView = Mathf.Lerp(initialFOV, target, elapsedTime / duration);
            //    elapsedTime += Time.deltaTime;
            //    yield return null; // Wait for the next frame
            //}
        }
        else
        {
            yield return null; // No change needed, exit coroutine
        }
    }
}
