using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GhostController : MonoBehaviour
{
    public Transform[] spawnPoints;       // Array of spawn points
    public Transform player;             // Reference to the player
    public float jumpScareDistance = 2f; // Distance to trigger jump scare
    public Animator ghostAnimator;       // Animator for ghost animations
    public AudioSource jumpScareAudio;   // Audio source for jump scare sound
    public GameObject playerController;  // Reference to player movement controller
    public PlayerMovement playerMovement;
    public Camera mainCamera;            // Reference to the main camera
    public float cameraTurnSpeed = 5f;   // Speed at which camera turns to ghost

    private NavMeshAgent navMeshAgent;   // Reference to NavMeshAgent component
    private bool isActive = false;       // Ghost active state
    private bool isChasing = false;      // Whether the ghost is chasing the player
    private bool isJumpScareActive = false; // Whether a jump scare is active
    private Light ghostLight;            // Reference to the light component

    void Start()
    {
        ghostLight = GetComponentInChildren<Light>();
        if (ghostLight != null)
        {
            ghostLight.enabled = false;
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing on ghost!");
            return;
        }

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isActive && isChasing)
        {
            navMeshAgent.SetDestination(player.position);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= jumpScareDistance)
            {
                TriggerJumpScare();
            }
        }
    }

    public void ActivateGhost()
    {
        if (isJumpScareActive) return;

        TeleportToRandomSpawn();
        gameObject.SetActive(true);
        isActive = true;
        isChasing = true;

        navMeshAgent.enabled = true;
    }

    public void DeactivateGhost()
    {
        isActive = false;
        isChasing = false;
        gameObject.SetActive(false);

        navMeshAgent.enabled = false;
    }

    public void TeleportToRandomSpawn()
    {
        if (isJumpScareActive) return; // Prevent teleportation during jump scare

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[randomIndex].position;

        navMeshAgent.Warp(transform.position);
    }
    private void ClearUI()
    {
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            canvas.gameObject.SetActive(false);
        }
    }
    private IEnumerator CameraEffects()
    {
        float originalFOV = mainCamera.fieldOfView;
        float targetFOV = originalFOV + 5f; // Increase FOV for dramatic effect
        float duration = 2f;

        // Gradually change the FOV
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            mainCamera.fieldOfView = Mathf.Lerp(originalFOV, targetFOV, t / duration);
            yield return null;
        }

        // Screen shake effect
        float shakeDuration = 1f;
        float shakeMagnitude = 0.1f;

        Vector3 originalPosition = mainCamera.transform.localPosition;
        for (float t = 0; t < shakeDuration; t += Time.deltaTime)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            mainCamera.transform.localPosition = originalPosition + shakeOffset;
            yield return null;
        }

        // Reset camera to original FOV
        mainCamera.fieldOfView = originalFOV;
        mainCamera.transform.localPosition = originalPosition;
    }
    void TriggerJumpScare()
    {
        if (!isActive || isJumpScareActive) return;
        isJumpScareActive = true;
        isActive = false;
        isChasing = false;
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;

        // Play jumpscare audio
        if (jumpScareAudio != null) jumpScareAudio.Play();

        // Trigger ghost animation
        if (ghostAnimator != null)
        {
            ghostAnimator.applyRootMotion = false;
            ghostAnimator.SetTrigger("Jumpscare");
        }

        // Disable player movement
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Clear all UI elements
        ClearUI();

        // Enable ghost light
        if (ghostLight != null)
        {
            ghostLight.enabled = true;
        }

        // Move the ghost in front of the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.position = player.position - directionToPlayer * 1.5f;

        // Adjust the ghost's vertical position to align with the camera's height
        float cameraHeight = mainCamera.transform.position.y;
        transform.position = new Vector3(transform.position.x, cameraHeight - 3f, transform.position.z);

        // Make the ghost look at the camera
        Vector3 lookDirection = mainCamera.transform.position - transform.position;
        lookDirection.y = 0; // Horizontal alignment
        transform.rotation = Quaternion.LookRotation(lookDirection);

        // Start camera effects and end jumpscare sequence
        StartCoroutine(CameraEffects());
        StartCoroutine(RotateCameraToGhost());
        StartCoroutine(EndJumpScare());
    }

    IEnumerator RotateCameraToGhost()
    {
        Vector3 directionToGhost = (transform.position - mainCamera.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToGhost);

        // Apply tilt directly
        targetRotation *= Quaternion.Euler(-35f, 0f, 0f);

        while (Quaternion.Angle(mainCamera.transform.rotation, targetRotation) > 0.1f)
        {
            mainCamera.transform.rotation = Quaternion.Slerp(
                mainCamera.transform.rotation,
                targetRotation,
                Time.deltaTime * cameraTurnSpeed
            );
            yield return null;
        }

        // Snap to target rotation at the end
        mainCamera.transform.rotation = targetRotation;
    }

    IEnumerator EndJumpScare()
    {
        yield return new WaitForSeconds(3f);

        DeactivateGhost();
        SceneManager.LoadScene("Game Over");
        isJumpScareActive = false; // Reset jump scare flag
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerJumpScare();
        }
    }
}
