using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCombat : MonoBehaviour {
    public Animator animator;
    public float fastAttackRange = 2f;
    public float midSlowAttackRange = 3f;
    public int fastAttackDamage = 5;
    public int midSlowAttackDamage = 10;
    public LayerMask enemyLayers;
    public float evadeDistance = 2f; 

    private CharControler charControler;
    private int attackCount = 0;
    private float lastAttackTime = 0f;
    private float attackCooldown = 0.5f;

    private void Start() {
        charControler = GetComponent<CharControler>(); 
    }

    void Update() {
        HandleInput();
    }

    void HandleInput() {
        if (Input.GetButtonDown("Fire1")) {
            PerformFastAttack();
        } else if (Input.GetButtonDown("Fire2")) {
            PerformMidSlowAttack();
        } else if (Input.GetButtonDown("Evade")) {
            PerformEvade();
        }
    }

    void PerformFastAttack() {
        if (Time.time - lastAttackTime < attackCooldown) {
            return;
        }

        attackCount++;
        if (attackCount > 3) {
            attackCount = 1; 
        }

        animator.SetTrigger("FastAttack" + attackCount);
        Debug.Log("fast attack" + attackCount);

        lastAttackTime = Time.time;
    }

    void PerformMidSlowAttack() {
        animator.SetTrigger("MidSlowAttack");
        Debug.Log("mid slow attack");
    }

    void PerformEvade() {
        animator.SetTrigger("Evade");
        Debug.Log("evading");
        Vector3 isoInputDirection = charControler.GetIsoInputDirection();
        if (isoInputDirection != Vector3.zero) {
            Vector3 evadePosition = transform.position + isoInputDirection * evadeDistance;
            transform.position = evadePosition;
        }
    }

    void ApplyFastAttackDamage() {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, fastAttackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies) {
            enemy.GetComponent<enemyScript>().TakeDamage(fastAttackDamage);
        }
    }

    void ApplyMidSlowAttackDamage() {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, midSlowAttackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies) {
            enemy.GetComponent<enemyScript>().TakeDamage(midSlowAttackDamage);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * fastAttackRange, fastAttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * midSlowAttackRange, midSlowAttackRange);
    }
}
