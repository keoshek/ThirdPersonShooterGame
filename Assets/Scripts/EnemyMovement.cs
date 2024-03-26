using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    State state;
    enum State
    {
        Walking,
        Attacking,
        Dead
    }

    [SerializeField] float movementSpeed;
    [SerializeField] Rigidbody enemyRB;
    [SerializeField] float lifeIndicator;
    GameObject player;
    GameManager gameManager;

    float currentSpeed = 0;
    float speedVelocity;

    // animations
    [SerializeField] Animator animator;
    int isDeadHash;
    int isHitHash;
    int attackHash;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Dead;

        player = GameObject.Find("Soldier");

        isDeadHash = Animator.StringToHash("isDead");
        isHitHash = Animator.StringToHash("isHit");
        attackHash = Animator.StringToHash("attack");

        lifeIndicator = 2f;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Walking)
        {
            MoveForward();
            Rotate();
        }
        if (state == State.Attacking)
        {
            Rotate();
        }

        AttackOrFollow();
    }

    // rotate towards player
    void Rotate()
    {
        Vector3 worldAimTarget = player.transform.position;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        transform.forward = Vector3.Slerp(transform.forward, aimDirection, Time.deltaTime * 5);
    }

    // move forward towards player
    void MoveForward()
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, movementSpeed, ref speedVelocity, 0.1f);
        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
    }

    // attack or follow
    void AttackOrFollow()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 1.2f)
        {
            state = State.Attacking;
            animator.SetBool(attackHash, true);
        }
        else if (animator.GetBool(isDeadHash) == false && Vector3.Distance(transform.position, player.transform.position) > 1f)
        {
            state = State.Walking;
            animator.SetBool(attackHash, false);
        }
    }

    public void IsHitByBullet() 
    {
        lifeIndicator = lifeIndicator - 1;
        animator.SetTrigger(isHitHash);
        StartCoroutine(IsHitCountDown());
        if (lifeIndicator < 1)
        {
            state = State.Dead;
            animator.SetBool(isDeadHash, true);
            gameManager.UpdateScore(5);
        }
    }

    IEnumerator IsHitCountDown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger(isHitHash);
    }

    public void WalkingState() 
    {
        animator.SetBool(isDeadHash, false);
        state = State.Walking;
    }

    // vanish after death
    public void Vanish()
    {
        Destroy(gameObject);
    }

    public void DecreasePlayerScore()
    {
        gameManager.UpdateLive(-1);
    }

}
