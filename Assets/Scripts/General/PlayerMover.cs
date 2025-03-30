using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        _rb.linearVelocity = 5f * new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }


}
