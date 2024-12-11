using System.Collections;
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

    void Start()
    {
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

    void TriggerJumpScare()
    {
        if (!isActive || isJumpScareActive) return;

        isJumpScareActive = true;
        isActive = false;
        isChasing = false;
        navMeshAgent.isStopped = true;

        if (jumpScareAudio != null) jumpScareAudio.Play();
        if (ghostAnimator != null) ghostAnimator.SetTrigger("JumpScare");

        if (playerController != null)
        {
            if (playerMovement != null)
                playerMovement.enabled = false;
        }

        if (!mainCamera.gameObject.activeInHierarchy || !mainCamera.enabled)
        {
            Debug.LogError("Main camera is not active or enabled during the jump scare!");
            return;
        }

        StartCoroutine(RotateCameraToGhost());
        StartCoroutine(EndJumpScare());
    }

    IEnumerator RotateCameraToGhost()
    {
        Vector3 directionToGhost = (transform.position - mainCamera.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToGhost);

        while (Quaternion.Angle(mainCamera.transform.rotation, lookRotation) > 0.1f)
        {
            mainCamera.transform.rotation = Quaternion.Slerp(
                mainCamera.transform.rotation,
                lookRotation,
                Time.deltaTime * cameraTurnSpeed
            );
            yield return null;
        }
    }

    IEnumerator EndJumpScare()
    {
        yield return new WaitForSeconds(3f);

        isJumpScareActive = false; // Reset jump scare flag
        DeactivateGhost();
        SceneManager.LoadScene("Game Over");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerJumpScare();
        }
    }
}
