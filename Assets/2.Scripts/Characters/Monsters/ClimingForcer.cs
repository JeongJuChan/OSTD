using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimingController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private CircleCollider2D circleCollider2D;
    private Vector2 rayOriginModVec;
    private float climingPower = 3.5f;

    private float circleRadius;
    private float diameter;

    private void Awake() 
    {
        Init();
    }

    private void FixedUpdate() 
    {
        TryCliming();    
    }

    #region Initialize
    public void Init()
    {
        circleRadius = circleCollider2D.radius;
        diameter = 2 * circleRadius;
        rayOriginModVec = new Vector2(circleRadius, circleRadius);
    }
    #endregion

    #region Climing
    public void TryCliming()
    {
        if (rigid.velocity.y < 0)
        {
            return;
        }

        Vector2 backRayOriginPos = transform.position;
        backRayOriginPos += rayOriginModVec;

        RaycastHit2D backHit = Physics2D.Raycast(backRayOriginPos, Vector2.right, 0.1f, 1 << gameObject.layer);
        Debug.DrawRay(backRayOriginPos, Vector2.right * 0.1f, Color.green);
        if (backHit.collider != null)
        {
            return;
        }

        Vector2 frontRayOriginPos = transform.position;
        frontRayOriginPos += new Vector2(-circleRadius, rayOriginModVec.y);
        RaycastHit2D[] hits = Physics2D.RaycastAll(frontRayOriginPos, Vector2.up, diameter * 1.5f, 1 << gameObject.layer);
        Debug.DrawRay(frontRayOriginPos, Vector2.up * diameter, Color.green);

        int count = 0;
        foreach (RaycastHit2D hit in hits)
        {
            Collider2D collider2D = hit.collider;
            if (collider2D != null && collider2D != circleCollider2D)
            {
                if (collider2D.transform.position.y > transform.position.y + circleRadius)
                {
                    return;
                }
                else
                {
                    count++;
                }
            }
        }

        if (count == 0)
        {
            return;
        }

        AddForceY();
    }

    private void AddForceY()
    {
        Vector2 velocity = rigid.velocity;
        velocity.y = climingPower;
        rigid.velocity = velocity;
    }
    #endregion
}
