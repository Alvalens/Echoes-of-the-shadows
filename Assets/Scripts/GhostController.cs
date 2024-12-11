using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GhostController : MonoBehaviour
{
    public Transform[] spawnPoints;       // Array of spawn points
    public Transform player;             // Reference to the player
    public float jumpScareDistance = 2f; // Distance to trigger jump scare
    public Animator ghostAnimator;       // Animator for ghost animations
    public AudioSource jumpScareAudio;   // Audio source for jump scare sound
    public GameObject playerController;  // Reference to player movement controller

    private NavMeshAgent navMeshAgent;   // Reference to NavMeshAgent component
    private bool isActive = false;       // Ghost active state
    private bool isChasing = false;      // Whether the ghost is chasing the player

    void Start()
    {
        // Get the NavMeshAgent component
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing on ghost!");
            return;
        }

        // Deactivate ghost at the start
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isActive && isChasing)
        {
            // Continuously set the player as the destination
            navMeshAgent.SetDestination(player.position);

            // Check for jump scare trigger
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= jumpScareDistance)
            {
                TriggerJumpScare();
            }
        }
    }

    public void ActivateGhost()
    {
        // Spawn the ghost at a random spawn point
        TeleportToRandomSpawn();
        gameObject.SetActive(true); // Enable ghost
        isActive = true;
        isChasing = true;

        // Enable NavMeshAgent
        navMeshAgent.enabled = true;
    }

    public void DeactivateGhost()
    {
        // Deactivate the ghost
        isActive = false;
        isChasing = false;
        gameObject.SetActive(false);

        // Disable NavMeshAgent
        navMeshAgent.enabled = false;
    }

    public void TeleportToRandomSpawn()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        // Pick a random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[randomIndex].position;

        // Ensure NavMeshAgent syncs with the new position
        navMeshAgent.Warp(transform.position);
    }

    void TriggerJumpScare()
    {
        if (!isActive) return;

        // Stop chasing and deactivate NavMeshAgent
        isActive = false;
        isChasing = false;
        navMeshAgent.isStopped = true;

        // Play jump scare sound
        if (jumpScareAudio != null)
        {
            jumpScareAudio.Play();
        }

        // Play jump scare animation
        if (ghostAnimator != null)
        {
            ghostAnimator.SetTrigger("JumpScare");
        }

        // Disable player controls
        if (playerController != null)
        {
            playerController.SetActive(false);
        }

        // Optional: Rotate the player to face the ghost
        Vector3 directionToGhost = (transform.position - player.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToGhost);
        player.rotation = Quaternion.Slerp(player.rotation, lookRotation, Time.deltaTime * 5f);

        // Debug message
        Debug.Log("Jump scare triggered! You lose!");

        // Reactivate player and ghost after delay
        StartCoroutine(EndJumpScare());
    }

    IEnumerator EndJumpScare()
    {
        // Wait for 3 seconds or the duration of the animation
        yield return new WaitForSeconds(3f);

        // Reactivate ghost or handle game-over logic
        DeactivateGhost();

        // Enable player controls
        if (playerController != null)
        {
            playerController.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerJumpScare();
        }
    }
}
