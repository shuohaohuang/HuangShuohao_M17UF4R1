using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PCController : MonoBehaviour, InputSystem_Actions.IPlayerActions, IDamageable
{
    public PC PC;
    private InputSystem_Actions inputs;

    public void OnAttack(InputAction.CallbackContext context) { }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PC.MoveCam(context.ReadValue<Vector2>());
        }
        if (context.canceled)
        {
            PC.MoveCam(new(0, 0));
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PC.MoveDirection = context.ReadValue<Vector2>();
        }

        if (context.canceled)
        {
            PC.MoveDirection = context.ReadValue<Vector2>();
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

    public void OnHurt(float damage)
    {
        PC.Hp -= damage;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PC.IsRunning = true;
        }
        if (context.canceled)
        {
            PC.IsRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            PC.Jump();
    }

    public void OnDance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PC.Dance();
            enabled = false;
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            PC.Aim();
        }
    }
}
