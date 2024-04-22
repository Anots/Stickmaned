using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 direction;
    private Rigidbody2D rb2D;
    private void Movement(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        rb2D.velocity = direction;
    }
}
