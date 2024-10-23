using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int bulletDamage = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            third_person_movement playerMovement = other.GetComponent<third_person_movement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(bulletDamage); 
            }

            Destroy(gameObject);  
        }
        else if (other.gameObject.tag == "Obstacle")
        {
            Destroy(gameObject);  
        }
    }
}
