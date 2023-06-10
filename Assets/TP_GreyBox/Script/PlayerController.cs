using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed = 10.0f;

	Rigidbody _rigidbody = null;
	protected bool IsActive { get; private set; }

	public void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		Cursor.lockState = CursorLockMode.Locked;
	}

	void FixedUpdate()
    {
		Vector3 direction = Vector3.zero;

		Vector3 rightAxis = Camera.main.transform.right;
		rightAxis.y = 0;

        Vector3 forwardAxis = Camera.main.transform.forward;
		forwardAxis.y = 0;

        direction += Input.GetAxisRaw("Horizontal") * rightAxis;
		direction += Input.GetAxisRaw("Vertical") * forwardAxis;
		direction.Normalize();
		_rigidbody.velocity = direction * speed + Vector3.up * _rigidbody.velocity.y;
	}
}
