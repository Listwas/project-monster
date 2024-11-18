using System.Collections;
using UnityEngine;

public class stopPlayerMovement : MonoBehaviour
{
    public PlayerEvade evade;
    public int moveBlockTime = 5;

    void Start()
    {
        evade = FindObjectOfType<PlayerEvade>();
    }

    public void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player in blood");
            StartCoroutine(moveBlock(moveBlockTime));
        }
            
    }

    IEnumerator moveBlock(int moveBlockTime)
    {
        Debug.Log("IE");
        if(evade != null)
        {
            stopMove();
            yield return new WaitForSeconds(moveBlockTime);
            startMoveAgain();
        }

    }
    void stopMove()
    {
        Debug.Log("Stop Move");
        evade.enabled = false;
    }

    void startMoveAgain()
    {
        Debug.Log("Start Move");
        evade.enabled = true;
    }
}
