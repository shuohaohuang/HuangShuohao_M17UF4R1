using Unity.Cinemachine;
using UnityEngine;
public class PC : MonoBehaviour
{

    [SerializeField] private bool canRun = false;
    [SerializeField] private bool isOnFloor;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private float currentSpeed;
    [SerializeField] private Vector2 moveDirection = Vector2.zero;
    public Animator animator;
    public bool falling;
    public float hp = 100000;
    public float jumpForce;
    public float runSpeed;
    public float walkSpeed;
    public Rigidbody rb;
    public CinemachineFreeLook cinemachineCamera;
    public float verticalRotation = 0f;
    public float CurrentSpeed
    {
        get => currentSpeed;
        set
        {
            currentSpeed = value;
        }
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
            CurrentSpeed = hasMoved ? walkSpeed : 0;

            animator.SetBool("walk", hasMoved);

            if (hasMoved)
            {
                // Vector3 direccion = new(MoveDirection.x, 0, MoveDirection.y);
                // Quaternion rotacion = Quaternion.LookRotation(direccion);
                // transform.rotation = Quaternion.Euler(0, rotacion.eulerAngles.y, 0);
            }
            else
            {
                currentSpeed = 0;
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
    public void Dance()
    {
        animator.SetTrigger("dance");
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
    public void Fall()
    {
        animator.SetTrigger("fall");

    }
    public void MoveCam(Vector2 mouse)
    {

        float mouseX = mouse.x * 1 * Time.deltaTime;
        float mouseY = mouse.y * 1 * Time.deltaTime;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(verticalRotation, 0f, mouseX);
        cinemachineCamera.m_XAxis.Value += mouse.x * 1 * Time.deltaTime;
        // cuerpoJugador.Rotate(Vector3.up * mouseX);
    }
    public void FixedUpdate()
    {
        rb.MovePosition(transform.position + new Vector3(moveDirection.x, 0, moveDirection.y).normalized * currentSpeed);

    }
    void Update()
    {
        if (rb.linearVelocity.y < 1)
        {

            if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out RaycastHit controlRay2, 1.3f))
            {


                if (!IsOnFloor && controlRay2.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    animator.SetTrigger("getGround");
                    isOnFloor = true;
                }
                else
                {
                    Fall();

                    animator.SetBool("grounded", false);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            animator.SetBool("grounded", true);
        }
    }

}