using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NextbotAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Search, Investigate }
    public State currentState = State.Patrol;

    [Header("Referencias")]
    public NavMeshAgent agent;
    public Transform[] patrolPoints;
    public LayerMask playerMask;
    public AudioSource whiteNoise;
    public AudioSource voiceSource;

    [Header("Parámetros base")]
    public float baseDetectionRange = 15f;
    public float baseChaseSpeed = 6f;
    public float basePatrolSpeed = 2.5f;
    public float losePlayerDistance = 25f;
    public float searchTime = 5f;

    [Header("Agresividad")]
    public float aggressionMultiplier = 0.2f; // cuánto aumenta la agresividad por reliquia
    private float currentAggression = 0f; // entre 0 y 1
    private Vector3 lastInvestigatePoint;

    private Transform targetPlayer;
    private int currentPatrolIndex;
    private float searchTimer;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        GoToNextPatrolPoint();

        // Suscribirse al evento de GameManager
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
                break;
        }
    }

    // --- PATRULLA ---
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

    // --- DETECCIÓN ---
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
            if (!voiceSource.isPlaying) voiceSource.Play();
            if (!whiteNoise.isPlaying) whiteNoise.Play();
        }
    }

    // --- PERSECUCIÓN ---
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

    // --- BÚSQUEDA ---
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

    // --- INVESTIGAR ZONA DE RECOLECCIÓN ---
    void Investigate()
    {
        agent.speed = GetPatrolSpeed() * 1.2f; // va más rápido al investigar
        agent.destination = lastInvestigatePoint;

        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            CheckForPlayersInArea();
            searchTimer = searchTime;
            currentState = State.Search;
        }
    }

    // --- EVENTO: Recolección detectada ---
    void OnCollectibleCollected(Vector3 position)
    {
        // Aumentar agresividad progresivamente
        int collected = GameManager.Instance.GetCollectedCount();
        int total = GameManager.Instance.totalCollectibles;
        currentAggression = Mathf.Clamp01(collected / (float)total);

        // Actualizar comportamiento
        lastInvestigatePoint = position;
        currentState = State.Investigate;
        if (!voiceSource.isPlaying) voiceSource.Play();
    }

    // --- GETTERS de agresividad ---
    float GetDetectionRange() => baseDetectionRange * (1f + currentAggression * aggressionMultiplier * 5f);
    float GetChaseSpeed() => baseChaseSpeed * (1f + currentAggression * aggressionMultiplier * 4f);
    float GetPatrolSpeed() => basePatrolSpeed * (1f + currentAggression * aggressionMultiplier * 2f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Nextbot atrapó al jugador!");
            // Aquí puedes implementar un jumpscare o muerte
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.Lerp(Color.yellow, Color.red, currentAggression);
        Gizmos.DrawWireSphere(transform.position, GetDetectionRange());
    }
}
