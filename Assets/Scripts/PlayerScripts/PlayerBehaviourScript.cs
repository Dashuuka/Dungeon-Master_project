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

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        body.velocity = movement.normalized * speed;

        //Flip Player Facing Left Right
        if (movement.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-1,1,1); 
        }


        anim.SetBool("Run", movement.magnitude != 0);
    }
}
