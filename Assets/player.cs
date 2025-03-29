using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    public Rigidbody rb;
    public float Speed;
    private InputSystem_Actions inputs;
    public void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("atacar");

    }

    public void OnLook(InputAction.CallbackContext context)
    {
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            rb.linearVelocity = new Vector3(direction.x, 0, direction.y) * Speed;
        }

        if (context.canceled)
        {
            rb.linearVelocity = new(0, 0, 0);
        }


    }
    void Awake()
    {
        inputs = new();
        inputs.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

}
