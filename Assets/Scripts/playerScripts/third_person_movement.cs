using System.Collections;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController character_controller;
    public ComboSystem comboSystem;
    public Transform camera_transform;
    public Rigidbody rigid_body;
    public Animator animator;
    private float current_rotation_velocity;

    [Header("Combat Settings")]
    [Header("light attack")]
    public int light_attack_damage = 5;
    public float light_attack_range = 2f;
    public float light_attack_cooldown = 0.5f;

    [Header("heavy attack")]
    public int heavy_attack_damage = 10;

    public float heavy_attack_range = 3f;
   
    public float heavy_attack_cooldown = 1.0f;

    public LayerMask enemy_layers;

    [Header("Player Settings")]
    public int max_health = 100;
    public float movement_speed = 12f;
    public float rotation_smooth_time = 0.1f;

    private int current_health;
    private float last_light_attack_time;
    private float last_heavy_attack_time;

    void Start()
    {
        if (rigid_body == null)
        {
            rigid_body = GetComponent<Rigidbody>();
        }
        current_health = max_health;

        // import combo component
        comboSystem = GetComponent<ComboSystem>();
        comboSystem.OnComboExecuted += ExecuteComboEffect;
    }

    void Update()
    {
        process_player_movement();
        process_player_input();
    }

    // combo logic
    private void ExecuteComboEffect(ComboSystem.DamageType damageType, int totalDamage)
    {
        Debug.Log($"combo executed: {damageType}, total damage: {totalDamage}");
        apply_attack_damage(totalDamage, damageType == ComboSystem.DamageType.Slash ? light_attack_range : heavy_attack_range);
    }
    // ------------------------------------------------------

    // player movement
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
    // ------------------------------------------------------
    
    //  executing attacks
    private void process_player_input()
    {
        if (Input.GetButtonDown("LightAttack") || Input.GetKeyDown(KeyCode.Q))
        {
            if (Time.time >= last_light_attack_time + light_attack_cooldown)
            {
                execute_light_attack();
                last_light_attack_time = Time.time;
            }
        }
        else if (Input.GetButtonDown("HeavyAttack") || Input.GetKeyDown(KeyCode.E))
        {
            if (Time.time >= last_heavy_attack_time + heavy_attack_cooldown)
            {
                execute_heavy_attack();
                last_heavy_attack_time = Time.time;
            }
        }
    }
    private void execute_light_attack()
    {
        comboSystem.RegisterAttack(ComboSystem.AttackType.Light);
        bool hit_enemy = apply_attack_damage(light_attack_damage, light_attack_range);
        // Debug.Log($"Light Attack: Damage={light_attack_damage}, Cooldown={light_attack_cooldown}s, Hit={(hit_enemy ? "enemy" : "nothing")}");
        animator.SetTrigger("fast attack");
    }

    private void execute_heavy_attack()
    {
        comboSystem.RegisterAttack(ComboSystem.AttackType.Heavy);
        bool hit_enemy = apply_attack_damage(heavy_attack_damage, heavy_attack_range);
        // Debug.Log($"Heavy Attack: Damage={heavy_attack_damage}, Cooldown={heavy_attack_cooldown}s, Hit={(hit_enemy ? "enemy" : "nothing")}");
        animator.SetTrigger("heavy attack");
    }
    // ------------------------------------------------------

    // Apply damage
    private bool apply_attack_damage(int damage, float range)
    {
        Collider[] hit_enemies = Physics.OverlapSphere(transform.position + transform.forward, range, enemy_layers);

        if (hit_enemies.Length > 0)
        {
            foreach (Collider enemy in hit_enemies)
            {
                EnemyScript enemy_script = enemy.GetComponent<EnemyScript>();
                if (enemy_script != null)
                {
                    enemy_script.TakeDamage(damage);
                }
            }
            return true; 
        }

        return false;
    }
    // ------------------------------------------------------

    // taking damage and dying 
    private bool has_died = false;

    public void TakeDamage(int damage_amount)
    {
        current_health -= damage_amount;

        if (current_health > 0)
        {
            Debug.Log("player took " + damage_amount + " damage. Current health: " + current_health);
        }
        else
        {
            if (!has_died)
            {
                has_died = true;
                Debug.Log("player died!");
            }
        }
    }
    // ------------------------------------------------------

    // show range of attacks
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward, light_attack_range);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, heavy_attack_range);
    }
    // ------------------------------------------------------
}
