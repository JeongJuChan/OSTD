using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private CircleCollider2D circleCollider2D;
    private Vector2 rayOriginModVec;
    [SerializeField] private float climingPower = 3.5f;
    [SerializeField] private Monster monster;

    private float circleRadius;
    private float diameter;

    private Transform heroTransform;

    private float radius;

    [SerializeField] private bool isJumpable = true;

    private void FixedUpdate() 
    {
        if (isJumpable && monster.isForward)
        {
            // TryCliming();    
        }
    }
    #region Initialize
    public void Init()
    {
        circleRadius = circleCollider2D.radius;
        diameter = 2 * circleRadius;
        rayOriginModVec = new Vector2(circleRadius, circleRadius);
        heroTransform = HeroManager.instance.hero.transform;
        radius = circleCollider2D.radius;
    }
    #endregion

    #region Climing
    // public void TryCliming()
    // {
    //     if (rigid.velocity.y < 0)
    //     {
    //         return;
    //     }

    //     Vector2 backRayOriginPos = transform.position;
    //     backRayOriginPos += rayOriginModVec;

    //     RaycastHit2D backHit = Physics2D.Raycast(backRayOriginPos, Vector2.right, radius, 1 << gameObject.layer);
    //     Debug.DrawRay(backRayOriginPos, Vector2.right * 0.1f, Color.green);
    //     TryJump();

    // }

    // private bool TryJump()
    // {
    //     bool canJump = monster.CanJump();
    //     if (canJump)
    //     {
    //         AddForceY();
    //     }

    //     return canJump;
    // }


    private void AddForceY()
    {
        // isJumpable = false;
        // rigid.AddForce(Vector2.up * climingPower, ForceMode2D.Impulse);
        Vector2 velocity = rigid.velocity;
        velocity.y = climingPower;
        rigid.velocity = velocity;
    }
    #endregion
}
