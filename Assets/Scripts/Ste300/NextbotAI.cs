using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NextbotAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Search, Investigate }
    public State currentState = State.Patrol;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform[] patrolPoints;
    public LayerMask playerMask;
    public AudioSource whiteNoise;
    public AudioSource voiceSource;

    [Header("Base data")]
    public float baseDetectionRange = 15f;
    public float baseChaseSpeed = 6f;
    public float basePatrolSpeed = 2.5f;
    public float losePlayerDistance = 25f;
    public float searchTime = 5f;

    [Header("Agression")]
    public float aggressionMultiplier = 0.2f; // Agression multiplier by relic amount
    private float currentAggression = 0f; // 0 or 1
    private Vector3 lastInvestigatePoint;
    private bool isInvestigatingActive = false;

    private Transform targetPlayer;
    private int currentPatrolIndex;
    private float searchTimer;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        GoToNextPatrolPoint();

        // Get game manager events
        if (GameManager.Instance != null)
            GameManager.Instance.OnCollectiblePickedUp += OnCollectibleCollected;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCollectiblePickedUp -= OnCollectibleCollected;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                CheckForPlayersInArea();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Search:
                Search();
                break;
            case State.Investigate:
                Investigate();
                CheckForPlayersInArea(); 
                break;
        }
    }

    // --- Patrol ---
    void Patrol()
    {
        agent.speed = GetPatrolSpeed();
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPatrolPoint();
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // --- Detection ---
    void CheckForPlayersInArea()
    {
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, GetDetectionRange(), playerMask);
        if (playersInRange.Length == 0) return;

        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (Collider playerCol in playersInRange)
        {
            float dist = Vector3.Distance(transform.position, playerCol.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = playerCol.transform;
            }
        }

        if (closest != null)
        {
            targetPlayer = closest;
            currentState = State.Chase;
            isInvestigatingActive = false; // If player found, chase him
            if (!voiceSource.isPlaying) voiceSource.Play();
            if (!whiteNoise.isPlaying) whiteNoise.Play();
        }
    }

    // --- Chase ---
    void Chase()
    {
        if (targetPlayer == null)
        {
            currentState = State.Search;
            searchTimer = searchTime;
            whiteNoise.Stop();
            return;
        }

        agent.speed = GetChaseSpeed();
        agent.destination = targetPlayer.position;

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        if (distance > losePlayerDistance)
        {
            targetPlayer = null;
            currentState = State.Search;
            searchTimer = searchTime;
            whiteNoise.Stop();
        }
    }

    // --- Search ---
    void Search()
    {
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0)
        {
            currentState = State.Patrol;
            GoToNextPatrolPoint();
        }
        else
        {
            CheckForPlayersInArea();
        }
    }

    // --- This one is used when a player picks up a bit, Albert goes there to search for the player ---
    void Investigate()
    {
        if (!isInvestigatingActive)
        {
            isInvestigatingActive = true;
            agent.speed = GetPatrolSpeed() * 1.5f; 
            agent.destination = lastInvestigatePoint;
        }

        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            isInvestigatingActive = false;
            currentState = State.Search;
            searchTimer = searchTime;
        }
    }

    // --- Event: On Collectible Collected ---
    void OnCollectibleCollected(Vector3 position)
    {
        // If chasing, ignore the new event
        if (currentState == State.Chase) return;

        // Multiply aggresion temporaly
        int collected = GameManager.Instance.GetCollectedCount();
        int total = GameManager.Instance.totalCollectibles;
        currentAggression = Mathf.Clamp01(collected / (float)total);

        // Update behaviour
        lastInvestigatePoint = position;
        currentState = State.Investigate;
        isInvestigatingActive = false; // reset movement to point
        if (!voiceSource.isPlaying) voiceSource.Play();
    }

    // --- GETTERS agression ---
    float GetDetectionRange() => baseDetectionRange * (1f + currentAggression * aggressionMultiplier * 5f);
    float GetChaseSpeed() => baseChaseSpeed * (1f + currentAggression * aggressionMultiplier * 4f);
    float GetPatrolSpeed() => basePatrolSpeed * (1f + currentAggression * aggressionMultiplier * 2f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Albert catched someone lol!");
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && JumpscareManager.Instance != null)
            {
                JumpscareManager.Instance.TriggerJumpscare(player);
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.Lerp(Color.yellow, Color.red, currentAggression);
        Gizmos.DrawWireSphere(transform.position, GetDetectionRange());
    }
}
