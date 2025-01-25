using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    public Animator Animator;
    
    public void SetSpeed(float speed)
    {
        Animator.SetFloat("MoveSpeed", speed);
    }
}
