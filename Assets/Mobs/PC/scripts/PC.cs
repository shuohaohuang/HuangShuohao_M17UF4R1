using Unity.Cinemachine;
using UnityEngine;

public class PC : MonoBehaviour
{
    [SerializeField]
    private bool canRun = false;

    [SerializeField]
    private bool isOnFloor;

    [SerializeField]
    private bool isRunning = false;

    [SerializeField]
    private float currentSpeed;

    [SerializeField]
    private Vector2 moveDirection = Vector2.zero;
    public Animator animator;
    public bool falling;

    [SerializeField]
    private float hp = 100000;
    public float jumpForce;
    public float runSpeed;
    public float walkSpeed;
    public Rigidbody rb;
    public PcRotation pcRotation;
    public PCCamara pCCamara;
    public PCController pCController;
    public Collider _collider;
    public float verticalRotation = 0f;
    public float CurrentSpeed
    {
        get => currentSpeed;
        set { currentSpeed = value; }
    }

    public bool IsRunning
    {
        get => isRunning;
        set
        {
            isRunning = value;
            animator.SetBool("run", value);
            if (value)
                currentSpeed *= 2f;
            else
                currentSpeed /= 2f;
        }
    }
    public Vector2 MoveDirection
    {
        get => moveDirection;
        set
        {
            moveDirection = value;

            bool hasMoved = moveDirection != Vector2.zero;
            CurrentSpeed = hasMoved
                ? isRunning
                    ? CurrentSpeed
                    : walkSpeed
                : 0;

            animator.SetBool("walk", hasMoved);

            if (hasMoved)
            {
                // Vector3 direccion = new(MoveDirection.x, 0, MoveDirection.y);
                // Quaternion rotacion = Quaternion.LookRotation(direccion);
                // transform.rotation = Quaternion.Euler(0, rotacion.eulerAngles.y, 0);
            }
            else
            {
                CurrentSpeed = 0;
            }
        }
    }

    public bool IsOnFloor
    {
        get => isOnFloor;
        set
        {
            isOnFloor = value;
            if (value)
                animator.SetTrigger("getGround");
        }
    }

    public float Hp
    {
        get => hp;
        set
        {
            hp = value;
            if (hp <= 0 && enabled)
            {
                animator.SetTrigger("die");
                transform.Rotate(new(0, -90, 0));
                enabled = false;
                pCController.enabled = false;
            }
        }
    }

    public void Dance()
    {
        animator.SetTrigger("dance");
        pCCamara.Dance();
    }

    public void Jump()
    {
        if (isOnFloor)
        {
            animator.SetTrigger("jump");

            rb.AddForce(new(0, jumpForce * 10, 0));
            IsOnFloor = false;
        }
    }

    public void MoveCam(Vector2 mouse)
    {
        pcRotation.mouse = mouse;
    }

    public void Aim()
    {
        pCCamara.SwitchCam();
        animator.SetBool("aim", pCCamara.active == 0);
    }

    public void FixedUpdate()
    {
        Vector3 move = transform.forward * moveDirection.y + transform.right * moveDirection.x;

        rb.MovePosition(transform.position + move.normalized * currentSpeed);

        if (rb.linearVelocity.y < 1)
        {
            if (
                Physics.Raycast(
                    transform.position,
                    new Vector3(0, -1, 0),
                    out RaycastHit controlRay2,
                    1.3f
                )
            )
            {
                if (
                    !IsOnFloor
                    && controlRay2.collider.gameObject.layer == LayerMask.NameToLayer("Ground")
                )
                {
                    animator.SetBool("getGround", true);
                }
            }
            else
            {
                animator.SetTrigger("fall");
            }
        }
    }

    void Update() { }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            animator.SetBool("grounded", true);
            isOnFloor = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            animator.SetBool("getGround", false);
            animator.SetBool("grounded", false);
        }
    }
}
