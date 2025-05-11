using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PcRotation : MonoBehaviour
{
    public Vector3 mouse;
    public float sensivility;

    void Update()
    {
        transform.Rotate(0, mouse.x, 0);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
