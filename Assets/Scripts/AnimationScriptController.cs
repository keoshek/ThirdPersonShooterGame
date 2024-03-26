using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimationScriptController : MonoBehaviour
{
    private bool enableMobileInputs;
    public bool hasPistol;
    public FixedButton gunButton;
    public GameObject pistol;
    MyCamera myCamera;

    Rigidbody playerRB;
    public Animator animator;
    int reachGroundHash;
    int isRunningHash;
    int jumpHash;
    int runningJumpHash;
    int hitHash;
    int kickHash;
    int aimHash;
    int shootHash;
    int flyHash;
    int climbUpHash;
    int climbDownHash;
    int goIdleHash;
    int climbOnTopHash;
    int jumpFromWallHash;

    [SerializeField] private Rig aimRig;
    private float aimRigWeight;
    // Start is called before the first frame update
    void Start()
    {
        enableMobileInputs = GetComponent<PlayerMovement>().enableMobileInputs;

        animator = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody>();

        // increases performance
        reachGroundHash = Animator.StringToHash("reachGround");
        isRunningHash = Animator.StringToHash("isRunning");
        jumpHash = Animator.StringToHash("jump");
        runningJumpHash = Animator.StringToHash("runningJump");
        hitHash = Animator.StringToHash("hit");
        kickHash = Animator.StringToHash("kick");
        aimHash = Animator.StringToHash("aim");
        shootHash = Animator.StringToHash("shoot");
        flyHash = Animator.StringToHash("fly");
        climbUpHash = Animator.StringToHash("climbUp");
        climbDownHash = Animator.StringToHash("climbDown");
        goIdleHash = Animator.StringToHash("goIdle");
        climbOnTopHash = Animator.StringToHash("climbOnTop");
        jumpFromWallHash = Animator.StringToHash("jumpFromWall");
    }

    // Update is called once per frame
    void Update()
    {
        ChangeGun();
        myCamera = GameObject.Find("Main Camera").GetComponent<MyCamera>();

        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);
    }

    void ChangeGun()
    {
        if (enableMobileInputs)
        {
            if (gunButton.Change && !hasPistol)
            {
                hasPistol = true;
            }
            else if (!gunButton.Change && hasPistol)
            {
                hasPistol = false;
            }

            if (hasPistol)
            {
                pistol.SetActive(true);
            }
            else
            {
                pistol.SetActive(false);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !hasPistol)
            {
                hasPistol = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift) && hasPistol)
            {
                hasPistol = false;
            }

            if (hasPistol)
            {
                pistol.SetActive(true);
            }
            else
            {
                pistol.SetActive(false);
            }
        }
    }

    public void ReachGround()
    {
        animator.SetTrigger(reachGroundHash);
        StartCoroutine(ReachGroundCountDown());
    }

    public void Idle()
    {
        animator.SetBool(isRunningHash, false);
    }

    public void Run()
    {
        animator.SetBool(isRunningHash, true);
        animator.SetBool(aimHash, false);
        aimRigWeight = 0f;
        myCamera.ChangePositionRate(2);
    }

    public void Jump()
    {
        animator.SetTrigger(jumpHash);
        StartCoroutine(JumpCountDown());
    }

    public void RunningJump()
    {
        animator.SetTrigger(runningJumpHash);
        StartCoroutine(JumpCountDown());
    }

    public void Hit()
    {
        animator.SetTrigger(hitHash);
        StartCoroutine(HitCountDown());
    }

    public void Kick()
    {
        animator.SetTrigger(kickHash);
        StartCoroutine(KickCountDown());
    }

    public void Aim()
    {
        animator.SetBool(aimHash, true);
        animator.SetBool(shootHash, true);
        myCamera.ChangePositionRate(0.3f);
        aimRigWeight = 1f;
        StopAllCoroutines();
        StartCoroutine(AimCountDown());
    }

    public void DontShoot()
    {
        animator.SetBool(shootHash, false);
    }

    public void Fly()
    {
        animator.SetBool(flyHash, true);
    }

    public void DontFly()
    {
        animator.SetBool(flyHash, false);
    }

    public void ClimbUp()
    {
        animator.SetBool(climbUpHash, true);
        animator.SetBool(climbDownHash, false);
    }

    public void ClimbDown()
    {
        animator.SetBool(climbDownHash, true);
        animator.SetBool(climbUpHash, false);
    }

    public void DontClimb()
    {
        animator.SetBool(climbDownHash, false);
        animator.SetBool(climbUpHash, false);
    }

    public void GoIdle()
    {
        animator.SetTrigger(goIdleHash);
        StartCoroutine(GoIdleCountDown());
    }

    public void ClimbOnTop()
    {
        animator.SetTrigger(climbOnTopHash);
        StartCoroutine(ClimbOnTopCountDown());
    }

    public void JumpFromWall()
    {
        animator.SetTrigger(jumpFromWallHash);
        StartCoroutine(JumpFromWallCountDown());
    }

    IEnumerator AimCountDown()
    {
        yield return new WaitForSeconds(5);
        animator.SetBool(aimHash, false);
        myCamera.ChangePositionRate(2);
        aimRigWeight = 0f;
    }

    IEnumerator ClimbOnTopCountDown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger(climbOnTopHash);
    }

    IEnumerator ReachGroundCountDown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger(reachGroundHash);
    }

    IEnumerator JumpCountDown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger(jumpHash);
        animator.ResetTrigger(runningJumpHash);
    }

    IEnumerator HitCountDown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger(hitHash);
    }

    IEnumerator KickCountDown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger(kickHash);
    }

    IEnumerator GoIdleCountDown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger(goIdleHash);
    }

    IEnumerator JumpFromWallCountDown()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger(jumpFromWallHash);
    }
}
