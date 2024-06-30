using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
    [SerializeField] private float speed = 7;
    private Rigidbody2D body;
    private Animator anim;
    private HealthManager healthManager;
    private Vector2 movement;



    private void Awake()
    {
        // Take properties from player 
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthManager = GetComponent<HealthManager>();
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

        if(Input.GetKeyDown(KeyCode.Space)){
            Ulta();
        }
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

    public void Ulta(){
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3f);

        foreach (Collider2D collider in colliders)
        {
            // Проверяем, есть ли у объекта Rigidbody2D
            Rigidbody2D rb2d = collider.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                Vector2 direction = rb2d.transform.position - transform.position;
                direction.Normalize();

                rb2d.AddForce(direction * 100f, ForceMode2D.Impulse);
            }
        }
    }
}

