using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

//ce script gère les mouvements de base des personnages, donc déplacement et jump
public class Character : MonoBehaviour
{
    [SerializeField] private bool drawGizmos;
    [SerializeField] private LayerMask layerGround;
    private Vector2 direction;
    private float maxSpeedCharacter;
    private bool canUseAbility = true;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;
    private float puissanceJump;
    private float AccelerationSpeedCharacter;
    private float airControlSpeed;
    private float gravityPower;
    public float timeToRechargeAbility;
    private RaycastHit2D IsGrounded
    {
        get { return Physics2D.BoxCast(transform.position + Vector3.down, new Vector2(boxCollider2D.size.x, 0.1f), 0, Vector2.down, 0, layerGround); }
    }
    private bool[] IsOnWall
    {
        get
        {
            return new bool[]
            {
                Physics2D.Raycast(transform.position + Vector3.right/2f, Vector2.right, 0.2f, layerGround),
                Physics2D.Raycast(transform.position + Vector3.left/2f, Vector2.left, 0.2f, layerGround)
            };
        }
    }
    private bool IsOnWalls
    {
        get { return IsOnWall[0] || IsOnWall[1]; }
    }

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        puissanceJump = 30;
        AccelerationSpeedCharacter = 5;
        maxSpeedCharacter = 10;
        airControlSpeed = 0.2f;
        gravityPower = -1;
    }

    public void GetInputsDeplacement(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
        if (context.started)
            animator.SetBool("Move", true);
        else if (context.canceled)
        {
            animator.SetBool("Move", false);
            return;
        }
    }

    private void FixedUpdate()
    {
        Deplacements();
    }

    private void Deplacements()
    {
        animator.SetBool("Falling", rigidBody2D.velocity.y < -0.1f);
        animator.SetBool("Grounded", IsGrounded);
        animator.SetBool("OnWalls", IsOnWalls);

        if (IsOnWalls)
            spriteRenderer.flipX = IsOnWall[1];
        else if (direction.x < 0)
            spriteRenderer.flipX = true;
        else if (direction.x > 0)
            spriteRenderer.flipX = false;

        rigidBody2D.velocity += new Vector2((IsGrounded ? direction.x : direction.x * airControlSpeed) * AccelerationSpeedCharacter, IsOnWalls ? gravityPower / 2 : gravityPower);
        rigidBody2D.velocity = new Vector2(Mathf.Clamp(rigidBody2D.velocity.x, -maxSpeedCharacter, maxSpeedCharacter), rigidBody2D.velocity.y);
        if (IsGrounded && direction.x == 0)
            rigidBody2D.velocity = new Vector2(Mathf.Lerp(rigidBody2D.velocity.x, 0, 0.25f), rigidBody2D.velocity.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded)
        {
            rigidBody2D.AddForce(Vector2.up * puissanceJump, ForceMode2D.Impulse);
            
            animator.SetTrigger("Jump");
        }
        else if (context.started && IsOnWalls)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, rigidBody2D.velocity.y / 2);
            rigidBody2D.AddForce((IsOnWall[0] ? new Vector2(-1, 1).normalized : new Vector2(1, 1).normalized) * puissanceJump, ForceMode2D.Impulse);
            
            animator.SetTrigger("Jump");
        }
    }

    protected IEnumerator TimeToRechargeAbility()
    {
        canUseAbility = false;
        yield return new WaitForSeconds(timeToRechargeAbility);
        canUseAbility = true;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.down, new Vector2(GetComponent<BoxCollider2D>().size.x, 0.1f));
    }
}
