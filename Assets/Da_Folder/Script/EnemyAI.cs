using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using MaskTransitions;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public float detectRange = 5f;
    public float attackRange = 1.5f;

    [Header("Enemy Settings")]
    public float enemySpeed = 3.5f;
    public float attackDamage = 5f;
    public float attackCooldown = 1.5f;

    [Header("Fall Settings")]
    [Tooltip("Y position under which the enemy is destroyed")]
    public float fallYThreshold = -10f;

    [Header("Audio Clips")]
    public AudioClip attackClip;
    public AudioClip hitClip;
    public AudioClip deathClip;

    [Header("Visual Effects")]
    public GameObject attackEffect;
    public GameObject hitEffect;
    public GameObject deathEffect;

    private NavMeshAgent agent;
    private float lastAttackTime = -999f;

    // Tap to destroy
    private int tapCount = 0;
    public int tapThreshold = 5;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemySpeed;
    }

    void Update()
    {
        if (transform.position.y < fallYThreshold)
        {
            Destroy(gameObject);
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange) TryAttackPlayer();
        else if (dist <= detectRange) ChasePlayer();
        else Roam();
    }

    void Roam()
    {
        agent.isStopped = true;
        // No animation
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        // No animation
    }

    void TryAttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        agent.isStopped = true;
        transform.LookAt(player);
        // No animation

        if (attackClip != null)
            AudioSource.PlayClipAtPoint(attackClip, transform.position);

        if (attackEffect != null)
            Instantiate(attackEffect, player.position + Vector3.up * 0.5f, Quaternion.identity);

        CountdownTimer.Instance.TakeDamage(attackDamage);
        lastAttackTime = Time.time;
    }

    public void OnTapped(Vector3 tapPoint)
    {
        tapCount++;

        if (tapCount >= tapThreshold)
        {
            Die();
            return;
        }

        if (hitClip != null)
            AudioSource.PlayClipAtPoint(hitClip, transform.position);

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
    }

    void Die()
    {
        if (deathClip != null)
            AudioSource.PlayClipAtPoint(deathClip, transform.position);

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);

        Destroy(gameObject);
    }
}
