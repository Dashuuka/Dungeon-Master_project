using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
    [SerializeField] private float speed = 7;
    private Rigidbody2D body;
    private Animator anim;

    private Vector2 movement;



    private void Awake()
    {
        // Take properties from player 
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        body.velocity = movement.normalized * speed;

        //Flip Player Facing Left Right
        /*if (movement.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-1,1,1); 
        }*/

        RotatePlayerTowardsMouse();


        anim.SetBool("Run", movement.magnitude != 0);
    }

    private void RotatePlayerTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        direction.Normalize();

        if (direction.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public bool canAttack()
    {
        return body.velocity.magnitude > 0;
    }
}

