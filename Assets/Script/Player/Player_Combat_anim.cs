using UnityEngine;

public class Player_Combat_anim : MonoBehaviour
{
    public Animator anim;

    public void Attack()
    {
        anim.SetBool("isAttacking", true);
    }

    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }
}