using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class WayPointMover : MonoBehaviour
{
    

    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWayPoint = true;

    private Transform[] waypoint;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;


    private Animator animator;
    private float LastX,LastY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        animator = GetComponent<Animator>();
        waypoint = new Transform[waypointParent.childCount];

        for(int i = 0; i<waypoint.Length; i++)
        {
            waypoint[i] = waypointParent.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PauseController.IsGamePaused || isWaiting)
        {
            animator.SetBool("isMoving", false);
            animator.SetFloat("LastX", LastX);
            animator.SetFloat("LastY", LastY);
            return;
        }
        //movernos al siguiente waypoint

        MoveToWayPoint();
    }

    void MoveToWayPoint()
    {
        Transform target = waypoint[currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;

        if(direction.magnitude > 0)
        {
            LastX = direction.x;
            LastY = direction.y;
        }


        animator.SetFloat("moveX",direction.x);
        animator.SetFloat("moveY", direction.y);
        animator.SetBool("isMoving",direction.magnitude > 0);

        //mueve desde posicion player a posicion target
        transform.position = Vector3.MoveTowards(transform.position,
            target.position, moveSpeed* Time.deltaTime);

        if(Vector2.Distance(transform.position, target.position)<0.1f) 
        {
            //hemos llegado al objetivo del waypoint
            StartCoroutine(WaitAtWaypoint());   

        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        animator.SetBool("isMoving",false);
        animator.SetFloat("LastX", LastX);
        animator.SetFloat("LastY", LastY);
        yield return new WaitForSeconds(waitTime);

        if (loopWayPoint)
        {
            // si hay que ir en bucle, esto me permite volver
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoint.Length;
        }
        else
        {

            //si no vamo en bucle
            currentWaypointIndex = Mathf.Min(currentWaypointIndex + 1, waypoint.Length - 1);
        }

        isWaiting = false;

        
    }
}
