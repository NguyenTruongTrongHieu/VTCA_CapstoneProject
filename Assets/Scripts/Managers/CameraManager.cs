using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

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

        StartCoroutine(SetFollowOffset(0.5f, 'X', 0f)); // Reset follow offset Z to 0
        StartCoroutine(SetFollowOffset(0.5f, 'Z', -10f)); // Reset follow offset Y to 0

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

    public IEnumerator SetFollowOffset(float duration, char offsetPos, float target)
    {
        if (offsetPos == 'X' || offsetPos == 'x')
        { 
            float currentFollowOffsetX = cineCam.GetComponent<CinemachineFollow>().FollowOffset.x;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float newOffsetX = Mathf.Lerp(currentFollowOffsetX, target, elapsedTime / duration);
                cineCam.GetComponent<CinemachineFollow>().FollowOffset.x = newOffsetX;
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            cineCam.GetComponent<CinemachineFollow>().FollowOffset.x = target; // Ensure final value is set to target
        }
        else if (offsetPos == 'Y' || offsetPos == 'y')
        {
            float currentFollowOffsetY = cineCam.GetComponent<CinemachineFollow>().FollowOffset.y;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float newOffsetY = Mathf.Lerp(currentFollowOffsetY, target, elapsedTime / duration);
                cineCam.GetComponent<CinemachineFollow>().FollowOffset.y = newOffsetY;
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            cineCam.GetComponent<CinemachineFollow>().FollowOffset.y = target; // Ensure final value is set to target
        }
        else if (offsetPos == 'Z' || offsetPos == 'z')
        {
            float currentFollowOffsetZ = cineCam.GetComponent<CinemachineFollow>().FollowOffset.z;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float newOffsetZ = Mathf.Lerp(currentFollowOffsetZ, target, elapsedTime / duration);
                cineCam.GetComponent<CinemachineFollow>().FollowOffset.z = newOffsetZ;
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            cineCam.GetComponent<CinemachineFollow>().FollowOffset.z = target; // Ensure final value is set to target
        }
    }

    public IEnumerator ShakeCamera(float amplitude, float frequency, float time)
    { 
        // Start the camera shake effect
        if (cineCam != null)
        {
            cineCam.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = amplitude;
            cineCam.GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = frequency;

            float elapsedTime = 0f;
            float targetAmplitude = 0f;
            float targetFrequency = 0f;
            while (elapsedTime < time)
            {
                // Gradually reduce the amplitude and frequency to zero
                float t = elapsedTime / time;
                cineCam.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = Mathf.Lerp(amplitude, targetAmplitude, t);
                cineCam.GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = Mathf.Lerp(frequency, targetFrequency, t);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // Stop the camera shake effect
            cineCam.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0f;
            cineCam.GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = 0f;
        }
    }

    public IEnumerator SetCamWhenTargetDie(bool isPlayerDie, float followOffsetX, float followOffsetZ)
    {
        StartCoroutine(ShakeCamera(5f, 1f, 1f));
        float currentFollowOffsetX = cineCam.GetComponent<CinemachineFollow>().FollowOffset.x;
        float currentFollowOffsetZ = cineCam.GetComponent<CinemachineFollow>().FollowOffset.z;
        if (isPlayerDie)//Cam target to enemy
        {
            SetTargetForCam(LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform);
            StartCoroutine(SetFollowOffset(0.1f, 'Z', followOffsetZ));
            yield return StartCoroutine(SetFollowOffset(0.1f, 'X', followOffsetX));

            yield return new WaitForSeconds(0.35f);

            StartCoroutine(SetFollowOffset(0.6f, 'Z', currentFollowOffsetZ));
            yield return StartCoroutine(SetFollowOffset(0.6f, 'X', currentFollowOffsetX));

        }
        else//Cam target to plyer
        {
            StartCoroutine(SetFollowOffset(0.1f, 'Z', followOffsetZ));
            yield return StartCoroutine(SetFollowOffset(0.1f, 'X', -followOffsetX));

            yield return new WaitForSeconds(0.35f);

            StartCoroutine(SetFollowOffset(0.6f, 'Z', currentFollowOffsetZ));
            yield return StartCoroutine(SetFollowOffset(0.6f, 'X', currentFollowOffsetX));
        }
    }

    public IEnumerator SetCamForSpecialAttack(float targetLookAtOffsetZ, float targetFOV)
    {
        StartCoroutine(ShakeCamera(5f, 1f, 1f));
        float currentLookAtOffsetZ = cineCam.GetComponent<CinemachineHardLookAt>().LookAtOffset.z;
        float currentFOV = cineCam.Lens.FieldOfView;

        SetTargetForCam(LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform);
        StartCoroutine(SetHardLookAt(50f, 'z', targetLookAtOffsetZ));
        yield return StartCoroutine(SetVerticalFOV(targetFOV, 0.1f));

        float startSlowTime = Time.realtimeSinceStartup;
        float endSlowTime = startSlowTime + 1f; // Duration of the slow motion effect
        while (Time.realtimeSinceStartup < endSlowTime)
        {
            Time.timeScale = 0.1f;
            yield return null; // Wait for the next frame
        }
        Time.timeScale = 1f; // Reset time scale to normal

        SetTargetForCam(PlayerUltimate.instance.playerTransform);
        StartCoroutine(SetHardLookAt(1f, 'z',currentLookAtOffsetZ));
        yield return StartCoroutine(SetVerticalFOV(currentFOV, 0.5f));

        yield return null;    
    }
}
