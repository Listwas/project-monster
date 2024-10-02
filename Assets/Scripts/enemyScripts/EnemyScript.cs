using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
     
    public int maxHealth = 50;
    private int currentHealth;
    public float shootInterval = 2f; 
    public float bulletSpeed = 10f; 
    public float bulletLifetime = 5f; 
    public float fireRange = 10f;
    public GameObject bulletPrefab;  
    public Transform firePoint;      


    private Transform player;        
    private float nextShootTime;     

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null && IsPlayerInRange()) {
            if (Time.time >= nextShootTime)
            {
                Shoot();
                nextShootTime = Time.time + shootInterval; 
            }
        }
    }


    bool IsPlayerInRange()
    {
        Vector3 enemyPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPosition = new Vector3(player.position.x, 0, player.position.z);
        float distance = Vector3.Distance(enemyPosition, playerPosition);

        return distance <= fireRange;
    }

    void Shoot()
    {
        if (player != null) {
            Vector3 direction = (player.position - firePoint.position).normalized;

            direction.y = 0;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;

            Destroy(bullet, bulletLifetime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fireRange);
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        Debug.Log("Enemy took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

}
