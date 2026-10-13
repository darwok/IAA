using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Input Configuration")]
    // Asigna aquí la acción de movimiento desde tu Input Action Asset
    [SerializeField] private InputActionReference moveAction;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void Update()
    {
        // Leer el valor del nuevo Input System
        Vector2 inputValues = moveAction.action.ReadValue<Vector2>();

        // Convertir el input 2D al plano XZ
        Vector3 move = new Vector3(inputValues.x, 0f, inputValues.y).normalized;

        if (move.magnitude >= 0.1f)
        {
            // Rotar el modelo hacia la dirección de movimiento
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            // Mover el personaje
            controller.Move(move * (moveSpeed * Time.deltaTime));
        }
    }
}