using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NextbotAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Search }
    public State currentState = State.Patrol;

    [Header("Referencias")]
    public NavMeshAgent agent;
    public Transform[] patrolPoints;
    public LayerMask playerMask;
    public AudioSource whiteNoise;
    public AudioSource voiceSource;

    [Header("Parámetros")]
    public float detectionRange = 15f;
    public float chaseSpeed = 6f;
    public float patrolSpeed = 2.5f;
    public float losePlayerDistance = 25f;
    public float searchTime = 5f;

    private Transform targetPlayer;
    private int currentPatrolIndex;
    private float searchTimer;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        GoToNextPatrolPoint();
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
        }
    }

    // 🔹 PATRULLA
    void Patrol()
    {
        agent.speed = patrolSpeed;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPatrolPoint();
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // 🔹 DETECCIÓN POR ÁREA
    void CheckForPlayersInArea()
    {
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, detectionRange, playerMask);

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

    // 🔹 PERSECUCIÓN
    void Chase()
    {
        if (targetPlayer == null)
        {
            currentState = State.Search;
            searchTimer = searchTime;
            whiteNoise.Stop();
            return;
        }

        agent.speed = chaseSpeed;
        agent.destination = targetPlayer.position;

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        // Si el jugador se aleja demasiado, pierde interés
        if (distance > losePlayerDistance)
        {
            targetPlayer = null;
            currentState = State.Search;
            searchTimer = searchTime;
            whiteNoise.Stop();
        }
    }

    // 🔹 BÚSQUEDA
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

    // 🔹 DETECCIÓN DE IMPACTO AL JUGADOR
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Nextbot atrapó al jugador!");
            // Aquí puedes manejar muerte o jumpscare
        }
    }

    // 🔹 DEBUG VISUAL
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, losePlayerDistance);
    }
}
