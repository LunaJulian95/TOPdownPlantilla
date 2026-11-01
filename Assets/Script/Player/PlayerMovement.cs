using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private const string FOOTSTEP_SOUND_NAME = "FootStep";

    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput, LastMoveInput;
    private Rigidbody2D rb;
    private Animator animator;

    private bool playFootStep = false;
    private bool canMove = true;

    public float footStepSpeed = 0.5f;
    public Player_Combat_anim player_combat;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        if (PauseController.IsGamePaused || !canMove)
        {
            StopMovement();
            return;
        }

        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);

        if (moveInput != Vector2.zero)
        {
            LastMoveInput = moveInput.normalized;
            animator.SetFloat("LastX", LastMoveInput.x);
            animator.SetFloat("LastY", LastMoveInput.y);
        }
    }

    private void FixedUpdate()
    {
        if (PauseController.IsGamePaused || !canMove)
        {
            StopMovement();
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
        animator.SetBool("isMoving", rb.linearVelocity.magnitude > 0);

        if (rb.linearVelocity.magnitude > 0 && !playFootStep)
        {
            StartFootStep();
        }

        if (rb.linearVelocity.magnitude == 0f)
        {
            StopFootStep();
        }
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        StopFootStep();
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (!canMove)
        {
            moveInput = Vector2.zero;
            StopMovement();
        }
    }

    private void StartFootStep()
    {
        playFootStep = true;
        InvokeRepeating(nameof(PlayFootSteps), 0f, footStepSpeed);
    }

    private void StopFootStep()
    {
        playFootStep = false;
        CancelInvoke(nameof(PlayFootSteps));
    }

    private void PlayFootSteps()
    {
        SoundEffectManager.Instance.Play(FOOTSTEP_SOUND_NAME, true);
    }

    public Vector2 GetLastMoveDirection()
    {
        if (LastMoveInput == Vector2.zero)
            return Vector2.down;

        return LastMoveInput;
    }
}