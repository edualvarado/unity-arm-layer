using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPlayerController : MonoBehaviour
{
    [Header("Stats")]
    Vector3 moveDirection;

    [Header("Options")]
    public bool alternateCameraMode = true;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 280f;

    [Header("Objects")]
    //public Rigidbody _body;
    public CharacterController _controller;

    [Header("Animation")]
    public Animator _anim;
    public float inputMagnitude;

    float horizontal;
    float vertical;

    private void Update()
    {
        // Direction of the character
        moveDirection = Vector3.forward * vertical + Vector3.right * horizontal;

        // 1) Rotate with respect to the camera
        Vector3 projectedCameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Quaternion rotationToCamera = Quaternion.LookRotation(projectedCameraForward, Vector3.up);

        // 2) Rotate with respect to the movement
        moveDirection = rotationToCamera * moveDirection;
        Quaternion rotationToMoveDirection = Quaternion.LookRotation(moveDirection, Vector3.up);

        if (alternateCameraMode)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToCamera, rotationSpeed * Time.deltaTime);
        else
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToMoveDirection, rotationSpeed * Time.deltaTime);

        // TODO: Change by move RB
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        //_controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Animation
        /*
        _anim.SetFloat("InputX", moveDirection.x, 0.0f, Time.deltaTime);
        _anim.SetFloat("InputZ", moveDirection.z, 0.0f, Time.deltaTime);
        inputMagnitude = new Vector2(moveDirection.x, moveDirection.z).sqrMagnitude;
        inputMagnitude = Mathf.Clamp(inputMagnitude, 0, 1);
        _anim.SetFloat("InputMagnitude", inputMagnitude, 0.0f, Time.deltaTime);
        */
    }

    void FixedUpdate()
    {
        //Debug.Log("moveDirection * moveSpeed " + (moveDirection * moveSpeed));
        //_body.MovePosition(_body.position +  moveDirection * moveSpeed * Time.fixedDeltaTime);
    }


    /// <summary>
    /// Called when Input is given. Store input in both floats.
    /// </summary>
    /// <param name="horizontal"></param>
    /// <param name="vertical"></param>
    public void OnMoveInput(float horizontal, float vertical)
    {
        this.vertical = vertical;
        this.horizontal = horizontal;
        Debug.Log($"Player Controller : Move Input: {vertical}, {horizontal}");
    }

}
