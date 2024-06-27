using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
    [SerializeField] private float speed = 7;
    private Rigidbody2D body;
    private Animator anim;



    private void Awake()
    {
       // Take properties from player 
       body = GetComponent<Rigidbody2D>();
       anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        body.velocity =new Vector2(horizontalInput*speed, verticalInput * speed);

        //Flip Player Facing Left Right
        if (horizontalInput > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1,1,1); 
        }


        anim.SetBool("Run",horizontalInput!=0 || verticalInput!=0);
    }
}
