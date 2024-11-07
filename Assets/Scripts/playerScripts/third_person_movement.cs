using System.Collections;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController character_controller;
    public Transform camera_transform;
    public Rigidbody rigid_body;
    public float movement_speed = 12f;

    public Animator animator;

    public float rotation_smooth_time = 0.1f;
    private float current_rotation_velocity;

    [Header("Combat Settings")]
    public float light_attack_range = 2f;
    public float heavy_attack_range = 3f;
    public int light_attack_damage = 5;
    public int heavy_attack_damage = 10;
    public LayerMask enemy_layers;
    private int consecutive_attack_count = 0;
    private float previous_attack_timestamp = 0f;
    private float attack_rest_period = 0.5f;

    [Header("Health Settings")]
    public int max_health = 100;
    private int current_health;

    void Start()
    {
        if (rigid_body == null)
        {
            rigid_body = GetComponent<Rigidbody>();
        }
        current_health = max_health;
    }

    void Update()
    {
        process_player_movement();
        process_player_input();
    }

    private void process_player_movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float target_angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera_transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target_angle, ref current_rotation_velocity, rotation_smooth_time);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 move_dir = Quaternion.Euler(0f, target_angle, 0f) * Vector3.forward;
            character_controller.Move(move_dir.normalized * movement_speed * Time.deltaTime);
        }
        else
        {
            character_controller.Move(Vector3.zero);
        }
    }

    private void process_player_input()
    {
        if (Input.GetButtonDown("FastAttackButton") || Input.GetKeyDown(KeyCode.Q))
        {
            execute_light_attack();
        }
        else if (Input.GetButtonDown("MidSlowAttackButton") || Input.GetKeyDown(KeyCode.E))
        {
            execute_heavy_attack();
        }
    }

    private void execute_light_attack()
    {
        if (Time.time - previous_attack_timestamp < attack_rest_period) return;

        consecutive_attack_count = (consecutive_attack_count % 3) + 1;
        animator.SetTrigger("FastAttack" + consecutive_attack_count);
        previous_attack_timestamp = Time.time;

        apply_light_attack_damage();
    }

    private void execute_heavy_attack()
    {
        animator.SetTrigger("MidSlowAttack");
        apply_heavy_attack_damage();
    }

    private void apply_light_attack_damage()
    {
        Collider[] hit_enemies = Physics.OverlapSphere(transform.position + transform.forward, light_attack_range, enemy_layers);

        if (hit_enemies.Length == 0)
        {
            Debug.Log("Fast attack hit nothing.");
        }

        foreach (Collider enemy in hit_enemies)
        {
            EnemyScript enemy_script = enemy.GetComponent<EnemyScript>();
            if (enemy_script != null)
            {
                enemy_script.TakeDamage(light_attack_damage);
                Debug.Log("Enemy hit with fast attack. Damage: " + light_attack_damage);
            }
            else
            {
                Debug.LogWarning("No EnemyScript found on hit enemy object: " + enemy.name);
            }
        }
    }

    private void apply_heavy_attack_damage()
    {
        Collider[] hit_enemies = Physics.OverlapSphere(transform.position + transform.forward, heavy_attack_range, enemy_layers);

        if (hit_enemies.Length == 0)
        {
            Debug.Log("Mid/Slow attack hit nothing.");
        }

        foreach (Collider enemy in hit_enemies)
        {
            enemy.GetComponent<EnemyScript>().TakeDamage(heavy_attack_damage);
            Debug.Log("Enemy hit with mid/slow attack. Damage: " + heavy_attack_damage);
        }
    }

    public void TakeDamage(int damage_amount)
    {
        current_health -= damage_amount;
        if (current_health >= 0)
        {
            Debug.Log("Player took " + damage_amount + " damage. Current health: " + current_health);
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        if (current_health < 0)
        {
            Debug.Log("Player died!");
        }
    }
}
