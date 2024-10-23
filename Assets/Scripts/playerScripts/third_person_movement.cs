using System.Collections;
using UnityEngine;

public class third_person_movement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Rigidbody rb; 
    public float speed = 12f;

    public Animator animator;

    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Combat Settings")]
    public float fastAttackRange = 2f;
    public float midSlowAttackRange = 3f;
    public int fastAttackDamage = 5;
    public int midSlowAttackDamage = 10;
    public LayerMask enemyLayers;
    private int attackCount = 0;
    private float lastAttackTime = 0f;
    private float attackCooldown = 0.5f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        currentHealth = maxHealth; 
    }

    void Update()
    {
        HandleMovement();

        
        HandleInput();         
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            PerformFastAttack();
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            PerformMidSlowAttack();
        }

    }

    private void PerformFastAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        attackCount = (attackCount % 3) + 1;
        animator.SetTrigger("FastAttack" + attackCount);
        lastAttackTime = Time.time;

        ApplyFastAttackDamage();
    }

    private void PerformMidSlowAttack()
    {
        animator.SetTrigger("MidSlowAttack");
        ApplyMidSlowAttackDamage();
    }

    private void ApplyFastAttackDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, fastAttackRange, enemyLayers);

        if (hitEnemies.Length == 0)
        {
            Debug.Log("Fast attack hit nothing.");
        }

        foreach (Collider enemy in hitEnemies)
        {
            EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(fastAttackDamage);
                Debug.Log("Enemy hit with fast attack. Damage: " + fastAttackDamage);
            }
            else
            {
                Debug.LogWarning("No EnemyScript found on hit enemy object: " + enemy.name);
            }
        }
    }


    private void ApplyMidSlowAttackDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, midSlowAttackRange, enemyLayers);

        if (hitEnemies.Length == 0)
        {
            Debug.Log("Mid/Slow attack hit nothing.");
        }

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyScript>().TakeDamage(midSlowAttackDamage);
            Debug.Log("Enemy hit with mid/slow attack. Damage: " + midSlowAttackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * fastAttackRange, fastAttackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * midSlowAttackRange, midSlowAttackRange);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth >= 0)
        {
            Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        if (currentHealth < 0)
        {
            Debug.Log("Player died!");
        }
    }
}
