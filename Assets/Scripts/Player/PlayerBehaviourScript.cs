using UnityEngine;
using UnityEngine.UI;   
using TMPro;

public class PlayerBehaviourScript : MonoBehaviour
{
    [Header("Player Settings")]
    public float maxHP;
    public float currentHP;
    [Space]
    public float maxMana;
    public float currentMana;
    [Space]
    public float damage;
    [Space]
    public float speed;

    [Header("Components")]
    public GunBehaviour gun;
    public Image healthBar;
    public TMP_Text healthText;
    public Image manaBar;
    public TMP_Text manaText;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Private variables")]
    private Vector2 movement;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHP = maxHP;
        currentMana = maxMana;

        UpdateHealthVisual();
        UpdateManaVisual();
    }

    private void FixedUpdate()
    {
        rb.velocity = movement.normalized * speed;

        RotatePlayerTowardsMouse();
        RotateWeapon();
    }

    void Update(){
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        anim.SetBool("Run", movement.magnitude != 0);

        if(Input.GetKeyDown(KeyCode.Space)){
            TakeDamage(10);
            SpendMana(5);
            Ulta();
        }

        if(Input.GetKeyDown(KeyCode.C)){
            Heal(4);
            RestoreMana(8);
        }

        if(Input.GetMouseButtonDown(0)){
            gun.Shoot(true);
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
            spriteRenderer.flipX = false;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void RotateWeapon(){
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 direction = mousePosition - gun.transform.position;
        direction.z = 0f;
        
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        gun.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        UpdateHealthVisual();
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        UpdateHealthVisual();
    }

    public void SpendMana(float amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);

        UpdateManaVisual();
    }

    public void RestoreMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);

        UpdateManaVisual();
    }

    private void UpdateHealthVisual(){
        healthBar.fillAmount = currentHP / maxHP;
        healthText.text = currentHP.ToString() + "/" + maxHP.ToString();
    }

    private void UpdateManaVisual(){
        manaBar.fillAmount = currentMana / maxMana;
        manaText.text = currentMana.ToString() + "/" + maxMana.ToString();
    }

    public void Ulta(){
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3f);

        foreach (Collider2D collider in colliders)
        {
            Rigidbody2D rb2d = collider.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                Vector2 direction = rb2d.transform.position - transform.position;
                direction.Normalize();

                rb2d.AddForce(direction * 5000f, ForceMode2D.Force);
            }
        }
    }
}

