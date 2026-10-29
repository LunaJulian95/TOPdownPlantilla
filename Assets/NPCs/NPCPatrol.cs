using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCPatrol : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public Rigidbody2D rb;
    public Transform[] patrolPoints;

    [Header("Movimiento")]
    public float moveSpeed = 1.5f;
    public float waitTime = 2f;
    public float arrivalDistance = 0.05f;

    private int currentPoint = 0;
    private Vector2 lastDirection = Vector2.down;
    private Coroutine patrolCoroutine;
    private string currentAnimation = "";

    private bool hablandoConPlayer = false;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (animator == null)
        {
            Debug.LogError("NPCPatrol: falta el Animator.", this);
            enabled = false;
            return;
        }

        if (patrolPoints == null || patrolPoints.Length < 2)
        {
            Debug.LogWarning(
                "NPCPatrol: asigná al menos 2 puntos de patrulla.",
                this
            );
            enabled = false;
            return;
        }

        foreach (Transform point in patrolPoints)
        {
            if (point == null)
            {
                Debug.LogError(
                    "NPCPatrol: hay un punto de patrulla sin asignar.",
                    this
                );
                enabled = false;
                return;
            }
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Empezar por el punto más cercano al NPC.
        currentPoint = GetClosestPointIndex();

        PlayIdle();
        patrolCoroutine = StartCoroutine(PatrolRoutine());
    }

    private int GetClosestPointIndex()
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float distance = Vector2.Distance(
                rb.position,
                patrolPoints[i].position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            // Esperar hasta que termine el diálogo o la pausa.
            while (hablandoConPlayer || PauseController.IsGamePaused)
            {
                rb.linearVelocity = Vector2.zero;
                yield return null;
            }

            Vector2 destination = patrolPoints[currentPoint].position;

           

            // Ir exactamente hacia el punto actual.
            while (Vector2.Distance(rb.position, destination) > arrivalDistance)
            {
                if (hablandoConPlayer || PauseController.IsGamePaused)
                {
                    rb.linearVelocity = Vector2.zero;
                    PlayIdle();
                    yield return null;
                    continue;
                }

                Vector2 difference = destination - rb.position;

                UpdateDirection(difference);
                PlayWalk();

                Vector2 nextPosition = Vector2.MoveTowards(
                    rb.position,
                    destination,
                    moveSpeed * Time.fixedDeltaTime
                );

                rb.MovePosition(nextPosition);

                yield return new WaitForFixedUpdate();
            }

            // Ajustar la posición para que coincida con el punto.
            rb.position = destination;
            rb.linearVelocity = Vector2.zero;
            PlayIdle();

           

            // Esperar antes de dirigirse al siguiente punto.
            float elapsed = 0f;

            while (elapsed < waitTime)
            {
                if (hablandoConPlayer || PauseController.IsGamePaused)
                {
                    yield return null;
                    continue;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Pasar al siguiente punto. Al llegar al último, volver al primero.
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }

    private void UpdateDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
            return;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            lastDirection = direction.x > 0f
                ? Vector2.right
                : Vector2.left;
        }
        else
        {
            lastDirection = direction.y > 0f
                ? Vector2.up
                : Vector2.down;
        }
    }

    private void PlayWalk()
    {
        string state;

        if (lastDirection == Vector2.up)
            state = "WalkUp";
        else if (lastDirection == Vector2.down)
            state = "WalkDown";
        else if (lastDirection == Vector2.left)
            state = "WalkLrft";
        else
            state = "WalkRigth";

        PlayAnimation(state);
    }

    private void PlayIdle()
    {
        string state;

        if (lastDirection == Vector2.up)
            state = "IdleUP";
        else if (lastDirection == Vector2.down)
            state = "IdleDown";
        else if (lastDirection == Vector2.left)
            state = "Idle";
        else
            state = "IdleRigth";

        PlayAnimation(state);
    }

    private void PlayAnimation(string state)
    {
        if (animator == null)
            return;

        int stateHash = Animator.StringToHash(state);

        if (!animator.HasState(0, stateHash))
        {
            Debug.LogWarning(
                $"No existe el estado '{state}' en el Animator.",
                this
            );
            return;
        }

        if (currentAnimation == state)
            return;

        currentAnimation = state;
        animator.Play(stateHash, 0, 0f);
    }

    public void IniciarDialogo()
    {
        hablandoConPlayer = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        PlayIdle();

        Debug.Log("NPC: diálogo iniciado.", this);
    }

    public void TerminarDialogo()
    {
        hablandoConPlayer = false;
        Debug.Log("NPC: diálogo terminado. Patrulla reanudada.", this);
    }

    private void OnDisable()
    {
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
    }
}