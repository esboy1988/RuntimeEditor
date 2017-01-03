using System;
using UnityEngine;

public class SceneViewCameraMovement : MonoBehaviour
{
    public float LookSpeed = 2f;
    public float MoveSpeed = 0.1f;
	
    protected float m_RotationX = 0f;
    protected float m_RotationY = 0f;

    private bool m_Active = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            m_Active = !m_Active;

        if (!m_Active)
            return;

        float joyX = 0f;
        float joyY = 0f;

        try
        {
            joyX = Input.GetAxis("Joystick Mouse X");
            joyY = Input.GetAxis("Joystick Mouse Y");
        }
        catch
        {
            // ignored
        }

        float mx = Input.GetAxis("Mouse X") + joyX;
        float my = Input.GetAxis("Mouse Y") + joyY;
        m_RotationX += mx * LookSpeed;
        m_RotationY += my * LookSpeed;
        m_RotationY = Mathf.Clamp(m_RotationY, -90f, 90f);
 
        transform.localRotation = Quaternion.AngleAxis(m_RotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(m_RotationY, Vector3.left);

        float moveSpeed = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Fire1")
            ? MoveSpeed * 2f
            : MoveSpeed;
        transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
    }
}
