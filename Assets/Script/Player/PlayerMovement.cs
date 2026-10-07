using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMovement : MonoBehaviour
{
    private const string FOOTSTEP_SOUND_NAME = "FootStep";
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput,LastMoveInput;
    private Rigidbody2D rb;
    private Animator animator;
    bool isAttacking;
    private bool playFootStep = false;
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

        
        
            
        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);
        if(moveInput != Vector2.zero) 
        {
            LastMoveInput = moveInput.normalized;
            animator.SetFloat("LastX",LastMoveInput.x);
            animator.SetFloat("LastY", LastMoveInput.y);
        }

       
        

       
    }

    private void FixedUpdate()
    {
        if (PauseController.IsGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            //parar la reproducion de pasos
            StopFootStep();
            return;

        }
        rb.linearVelocity = moveInput * moveSpeed;
        animator.SetBool("isMoving", rb.linearVelocity.magnitude>0);

        //si la velocidad del player es >0 -> reproducir pasos 
        if(rb.linearVelocity.magnitude > 0 && !playFootStep) 
        {
            StartFootStep();
        }

        //si el personaje no se mueve -> parar los pasos
        if(rb.linearVelocity.magnitude == 0f) 
        {
            StopFootStep();
        }
        
    }

    private void StartFootStep() 
    {
        playFootStep = true;
        InvokeRepeating(nameof(PlayFootSteps),0f,footStepSpeed);
        
    }

    private void StopFootStep() 
    {
        playFootStep= false;
        CancelInvoke(nameof(PlayFootSteps));
    }

    private void PlayFootSteps() 
    {
        SoundEffectManager.Instance.Play(FOOTSTEP_SOUND_NAME, true);
    }

}
