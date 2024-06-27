using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
    [SerializeField] private float speed = 7;
    private Rigidbody2D body;

    private void Awake()
    {
       body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity =new Vector2(horizontalInput*speed, Input.GetAxis("Vertical") * speed);

        //Flip Player Facing Left Right
        if (horizontalInput > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1,1,1); 
        }
    }
}
