using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(WeaponController))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionReference fireAction;
    [SerializeField] private InputActionReference reloadAction;

    private WeaponController weaponController;

    private void Awake()
    {
        weaponController = GetComponent<WeaponController>();
    }

    private void OnEnable()
    {
        // Habilitamos las acciones
        fireAction.action.Enable();
        reloadAction.action.Enable();

        // Nos suscribimos al evento de recarga (se ejecuta una sola vez al presionar)
        reloadAction.action.performed += OnReload;
    }

    private void OnDisable()
    {
        // Deshabilitamos y desuscribimos para evitar memory leaks
        fireAction.action.Disable();
        reloadAction.action.Disable();
        reloadAction.action.performed -= OnReload;
    }

    private void Update()
    {
        // IsPressed() devuelve true mientras el botón se mantenga presionado.
        // Esto permite fuego automático (ametralladora). El WeaponController 
        // se encarga de limitar la cadencia usando el 'fireRate'.
        if (fireAction.action.IsPressed())
        {
            weaponController.TryShoot();
        }
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        weaponController.TriggerManualReload();
    }
}