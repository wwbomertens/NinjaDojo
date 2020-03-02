﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private IEnemyState currentState;

    public GameObject Target { get; set; }

    [SerializeField]
    private float MeleeRange;

    public bool InMeleeRange
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= MeleeRange;
            }
            else
            {
                return false;
            }


        }
    }

    public override void Death()
    {
        Destroy(gameObject);
    }


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        ChangeState(new IdleState());
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsDead)
        {
            if (!TakingDamage)
            {
                currentState.Execute();
            }
            LookAtTarget();
        }
    }
    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        currentState.Enter(this);

    }

    public void Move()
    {
        MyAnimator.SetFloat("speed", 1);
        if (this.Target != null && !InMeleeRange)
        {
            transform.Translate(getDirection() * ((movementSpeed + 2) * Time.deltaTime));
        }
        else
        {
            transform.Translate(getDirection() * ((movementSpeed) * Time.deltaTime));
        }

    }

    public Vector2 getDirection()
    {

        return facingRight ? Vector2.right : Vector2.left;
    }

    public  override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);

    }

    private void LookAtTarget()
    {
        if (this.Target != null)
        {
            float xDir = Target.transform.position.x - transform.position.x;

            if ((xDir < 0 && facingRight) || (xDir >0 && !facingRight))
            {
                ChangeDirection();
            }

        }
    }
    public override IEnumerator TakeDamage()
    {
        health -= 10;

        if (!IsDead)
        {
            MyAnimator.SetTrigger("damage");
        }
        else
        {
            MyAnimator.SetTrigger("die");
            yield return null;

        }


    }

    public override bool IsDead
    {
        get
        {
            return health <= 0;
        }
    }
}
