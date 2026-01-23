using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    [Header("Variables for movement")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float jumpHeight = 5f;

    [Header("Variables for Dropkick")]
    [SerializeField] private float kickStrength = 7f;
    [SerializeField] private float kickRange = 5f;

    [Header("Variables for Camera")]
    [SerializeField] private float mouseSens = 0.1f;
    [SerializeField] private float lookRange = 80f;

    [Header("Input action references")]
    public InputActionReference move;
    public InputActionReference jump;
    public InputActionReference sprint;
    public InputActionReference look;
    public InputActionReference dropkick;

    [Header("Other References")]
    public Camera mainCamera;
    public CharacterController characterController;

    private Vector2 mousePos;
    private Vector2 moveDirection;
    private float vertRotation;

    private void LookControls()
    {
        mousePos = look.action.ReadValue<Vector2>();

        float mouseXRotation = mousePos.x * mouseSens;
        float mouseYRotation = mousePos.y * mouseSens;

        transform.Rotate(0, mouseXRotation, 0);

        vertRotation = Mathf.Clamp(vertRotation - mouseYRotation, -lookRange, lookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(vertRotation, 0, 0);
    }

    private void MovePlayer(Vector2 moveDirection)
    {
        float moveSpeed;

        Vector3 moveForce;
        Vector3 moveDirection3 = new Vector3(moveDirection.x, 0, moveDirection.y);

        Vector3 worldDirection = transform.TransformDirection(moveDirection3);

        worldDirection = worldDirection.normalized;

        if (sprint.action.IsPressed())
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        moveForce.x = worldDirection.x * moveSpeed;
        moveForce.y = 0;
        moveForce.z = worldDirection.z * moveSpeed;

        characterController.Move(moveForce * Time.deltaTime);

        Debug.Log("added force");
    }

    private void TriggerJump()
    {
        float jumpStep = 0;
        float previousStep = 0;
        float maxJumpHeight = jumpHeight;
        Vector3 moveForceUp = new Vector3(0, 0, 0);
        if (jump.action.IsPressed())
        {
            while (jumpStep <= maxJumpHeight)
            {
                previousStep = jumpStep;
                jumpStep += Time.deltaTime;

                float differenceStep = jumpStep - previousStep;
                moveForceUp.y = differenceStep;

                characterController.Move(moveForceUp);
            }
            Debug.Log("Jumping");
            Debug.Log(jumpStep);
        }
    }

    private void Gravity()
    {
        Vector3 moveForceUp = new Vector3(0, Physics.gravity.y, 0);

        characterController.Move(moveForceUp * Time.deltaTime);
    }

    private IEnumerator DropKick()
    {
        Debug.Log("Dropkick is running");
        if (characterController.isGrounded)
        {
            Vector3 shortHop = new Vector3 (0, 3, 0);
            characterController.Move(shortHop);
            Debug.Log("shorthop applied");
        }

        yield return new WaitForSeconds(0.2f);

        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, kickRange))
        {
            Debug.Log("Raycast hit something");
            characterController.Move(-mainCamera.transform.forward * Time.deltaTime * kickStrength);
        }
    }

    //yappa doo
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        LookControls();

        moveDirection = move.action.ReadValue<Vector2>();

        if (move.action.IsPressed() && moveDirection != new Vector2(0, 0))
        {
            MovePlayer(moveDirection);
        }

        if(characterController.isGrounded)
        {
            TriggerJump();
        }
        else
        {
            Gravity();
        }

        if (dropkick.action.IsPressed())
        {
            StartCoroutine(DropKick());
        }
    }
}
