using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    public State state;
    public enum State { 
        Normal,
        HookShot,
        Climbing,
    }

    [SerializeField] GameObject onGroundChecker;
    
    [SerializeField] Rigidbody playerRB;
    float normalSpeed = 4.5f;
    float fastSpeed = 6.3f;
    float normalAnimSpeed = 0.9f;
    float fastAnimSpeed = 1.26f;
    public float smoothRotationTime = 0.25f;
    public bool enableMobileInputs = false;
    bool hasPistol;
    Vector2 input = Vector2.zero;

    public float MoveSpeed;
    float currentVelocity;
    float currentSpeed;
    float speedVelocity;
    Vector3 mouseWorldPosition;

    Transform cameraTransform;
    public FixedJoystick joystick;
    public FixedButton jumpButton;
    public FixedButton runFastButton;
    public FixedButton hitButton;
    public FixedButton kickButton;
    public FixedButton hookButton;
    public Canvas canvas;
    Animator animator;
    AnimationScriptController animationScriptController;

    // fire bullet
    public Transform bullet; // get the transform of the bullet that will be spawned
    public Transform spawnBulletPosition; // the position of where it is supposed to spawn
    public int multiplier; // the amount of force applied

    // aiming
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;

    // hookshot
    [SerializeField] private GameObject cameraDir;
    [SerializeField] private Transform debugHitPointTransform;
    private Vector3 hookshotPosition;
    private float oldPosition;
    [SerializeField] private Transform hookshotTransform;
    private CameraFov cameraFov;
    private const float NORMAL_FOV = 60f;
    private const float HOOKSHOT_FOV = 80f;
    [SerializeField] private LayerMask LayersToHook;
    private RaycastHit raycastHitHook;

    [Header("Climbing")]
    public Transform orientation;
    public Rigidbody rb;
    public float detectionLength;
    private bool wallFront;

    [SerializeField] float dur1;
    [SerializeField] float dur2;

    // Start is called before the first frame update
    void Start()
    {
        animationScriptController = GetComponent<AnimationScriptController>();
        Application.targetFrameRate = 60;
        state = State.Normal;
        cameraTransform = Camera.main.transform;
        animator = GetComponent<Animator>();
        hookshotTransform.gameObject.SetActive(false);
        cameraFov = cameraDir.GetComponent<CameraFov>();
    }

    // Update is called once per frame
    void Update()
    {
        hasPistol = animationScriptController.hasPistol;
        // if no input stand still
        input = Vector2.zero;
        if (state == State.Normal) {
            // get input from buttons or joystick
            GetInputNormal();

            RotateMovePlayer();

            // crosshair / aim position
            AimPoint();
        }

        if (state == State.HookShot) 
        {
            LookAtAim(10);
        }

        if (state == State.Climbing)
        {
            WallCheck();
            GetInputClimbing();
        }
    }

    void GetInputNormal()
    {
        // mobile input
        if (enableMobileInputs)
        {
            canvas.gameObject.SetActive(true);

            // run
            MoveSpeed = runFastButton.Pressed ? fastSpeed : normalSpeed;
            animator.speed = runFastButton.Pressed ? fastAnimSpeed : normalAnimSpeed;
            input = runFastButton.Pressed ? new Vector2(0, 1) : new Vector2(joystick.input.x, joystick.input.y);

            // jump
            if (jumpButton.Pressed)
            {
                Jump();
            }

            // hit or shoot
            if (!hasPistol)
            {
                if (hitButton.Pressed && input.magnitude == 0)
                {
                    animationScriptController.Hit();
                }
            }
            else
            {
                if (hitButton.Pressed && input.magnitude == 0)
                {
                    animationScriptController.Aim();
                }
            }

            // kick
            if (kickButton.Pressed && input.magnitude == 0)
            {
                animationScriptController.Kick();

            }

            // hookshot
            if (hookButton.Pressed && input.magnitude == 0)
            {
                ThrowHook();
            }
        }
        // keyboard input
        else
        {
            canvas.gameObject.SetActive(false);
            // run
            MoveSpeed = Input.GetKey(KeyCode.RightShift) ? fastSpeed : normalSpeed;
            animator.speed = Input.GetKey(KeyCode.RightShift) ? fastAnimSpeed : normalAnimSpeed;
            input = Input.GetKey(KeyCode.RightShift) ? new Vector2(0, 1) : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            // hit or shoot
            if (!hasPistol)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) && input.magnitude == 0)
                {
                    animationScriptController.Hit();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) && input.magnitude == 0)
                {
                    animationScriptController.Aim();
                }
            }

            // kick
            if (Input.GetKeyDown(KeyCode.Mouse1) && input.magnitude == 0)
            {
                animationScriptController.Kick();
            }

            // hookshot
            if (Input.GetKeyDown(KeyCode.Z) && input.magnitude == 0)
            {
                ThrowHook();
            }
        }
    }

    void GetInputClimbing()
    {
        if (wallFront)
        {
            if (enableMobileInputs)
            {
                MoveSpeed = normalSpeed;
                animator.speed = normalAnimSpeed;
                input = new Vector2(0, joystick.input.y);
                // return magnitude of 1/-1
                Vector2 inputDir = input.normalized;

                // move player's position
                float targetSpeed = MoveSpeed * inputDir.magnitude;
                currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, 0.1f);
                transform.Translate(input * currentSpeed * Time.deltaTime / 5, Space.World);

                // Climbing Animations
                if (transform.position.y > oldPosition)
                {
                    animationScriptController.ClimbUp();
                    oldPosition = transform.position.y;
                }
                else if (transform.position.y < oldPosition)
                {
                    animationScriptController.ClimbDown();
                    oldPosition = transform.position.y;
                }
                else
                {
                    animationScriptController.DontClimb();
                    oldPosition = transform.position.y;
                }

                // Jump
                if (jumpButton.Pressed)
                {
                    animationScriptController.JumpFromWall();
                }
            }
            else
            {
                MoveSpeed = normalSpeed;
                animator.speed = normalAnimSpeed;
                input = new Vector2(0, Input.GetAxisRaw("Vertical"));
                // return magnitude of 1/-1
                Vector2 inputDir = input.normalized;

                // move player's position
                float targetSpeed = MoveSpeed * inputDir.magnitude;
                currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, 0.1f);
                transform.Translate(input * currentSpeed * Time.deltaTime / 5, Space.World);

                // Climbing animations
                if (transform.position.y > oldPosition)
                {
                    animationScriptController.ClimbUp();
                    oldPosition = transform.position.y;
                }
                else if (transform.position.y < oldPosition)
                {
                    animationScriptController.ClimbDown();
                    oldPosition = transform.position.y;
                }
                else
                {
                    animationScriptController.DontClimb();
                    oldPosition = transform.position.y;
                }

                // Jump
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    animationScriptController.JumpFromWall();
                }
            }
        }
        else {
            animationScriptController.ClimbOnTop();
        }
        
    }

    public void ThrowHook()
    {
        mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out raycastHitHook, Mathf.Infinity, LayersToHook))
        {
            debugHitPointTransform.position = raycastHitHook.point;
            hookshotPosition = raycastHitHook.point;

            // change state
            state = State.HookShot;

            // adjust hook-throw
            StartCoroutine(Throw2Hook());
        }
    }

    IEnumerator Throw2Hook()
    {
        yield return new WaitForSeconds(0.7f);
        animationScriptController.Fly();
        hookshotTransform.gameObject.SetActive(true);
        hookshotTransform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        hookshotTransform.LookAt(hookshotPosition); // direction from player to target

        hookshotTransform.DOScaleZ(Vector3.Distance(transform.position, hookshotPosition), 0.1f).OnComplete(() => {
            //start fly animation

            // rescale hook
            hookshotTransform.DOScaleZ(0.1f, 2f);

            // change camera fov
            cameraFov.SetCameraFov(HOOKSHOT_FOV);

            // hook pulls the player
            float hookshotVelocity = 2f;

            // remove gravity in order to stay on wall
            playerRB.useGravity = false;
            transform.DOMove(hookshotPosition, hookshotVelocity).OnComplete(() =>
            {
                hookshotTransform.gameObject.SetActive(false);
                cameraFov.SetCameraFov(NORMAL_FOV);
                animationScriptController.DontFly();

                // organize position of player against the wall
                transform.forward = - raycastHitHook.normal;
                transform.Translate(new Vector3(0, 0, -0.2f));

                playerRB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                
                state = State.Climbing;
            });

        });
    }

    void Jump()
    {
        if (input.magnitude > 0)
        {
            animationScriptController.RunningJump();
        }
        else
        {
            animationScriptController.Jump();
        }
    }

    void RotateMovePlayer()
    {
        // return magnitude of 1/-1
        Vector2 inputDir = input.normalized;

        // when input, rotate player smoothly
        if (inputDir != Vector2.zero)
        {
            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref currentVelocity, smoothRotationTime);
        }

        // move player's position
        float targetSpeed = MoveSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, 0.1f);
        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        // play idle and walk animation according to input magnitude (0/1)
        if (targetSpeed > 0)
        {
            animationScriptController.Run();
        }
        else
        {
            animationScriptController.Idle();
        }
    }
    

    public void AimPoint()
    {
        mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;

            mouseWorldPosition = raycastHit.point;
        }
        if (animator.GetBool("aim"))
        {
            LookAtAim(50);
        }
    }
    
    // rotate player in order to look at aimpoint
    public void LookAtAim(float rate) 
    {
        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rate);
    }

    void WallCheck()
    {
        wallFront = Physics.Raycast(orientation.position, orientation.forward, out RaycastHit frontWallHit, detectionLength, LayersToHook);
    }

    public void SetActiveGO() {
        onGroundChecker.gameObject.SetActive(true);
    }

    // Functions called by animations
    public void FireWeapon()
    {
        // spawn bullet and shoot it forward
        Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
        Instantiate(bullet, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
    }

    public void ReturnToNormal()
    {
        playerRB.useGravity = true;
        state = State.Normal;
        animationScriptController.GoIdle();
        playerRB.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;

    }

    public void MoveForwardAfterClimb()
    {
        transform.DOMoveY(transform.position.y + 2.4f, dur1).SetEase(Ease.InCubic).OnComplete(() =>
        {
            transform.Translate(transform.forward, Space.World);
        }
        );
    }

    public void FixAnimPos() {

        float force = 2000f;
        playerRB.AddForce(-transform.forward * force / 7, ForceMode.Impulse);
        playerRB.AddTorque(transform.up * force / 300, ForceMode.Impulse);
    }
}
