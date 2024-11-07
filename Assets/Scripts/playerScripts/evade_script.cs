using System.Collections;
using UnityEngine;

public class PlayerEvade : MonoBehaviour
{
    public CharacterController controller;
    public float evadeDistance = 10f;
    public float evadeDuration = 0.2f;
    public float evadeCooldown = 1f;
    private bool isEvading = false;
    private float evadeEndTime = 0f;

    private Vector3 evadeDirection;

    void Update()
    {
        HandleEvadeInput();
        CheckEvadeCooldown();
    }

    private void HandleEvadeInput()
    {
        if (Input.GetButtonDown("EvadeButton") || Input.GetKeyDown(KeyCode.Space))
        {
            if (!isEvading)
            {
                Debug.Log("Evade input detected");
                StartCoroutine(PerformEvade());
            }
        }
    }


    private IEnumerator PerformEvade()
    {
        Debug.Log("Evade initiated.");
        isEvading = true;

        evadeDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        if (evadeDirection == Vector3.zero)
        {
            evadeDirection = -transform.forward;
        }

        Debug.Log("Evade direction: " + evadeDirection);

        float evadeSpeed = evadeDistance / evadeDuration;
        float elapsedTime = 0f;

        while (elapsedTime < evadeDuration)
        {
            controller.Move(evadeDirection * evadeSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        evadeEndTime = Time.time + evadeCooldown;
        Debug.Log("Evade completed. Cooldown starts.");
        isEvading = false;
    }

    private void CheckEvadeCooldown()
    {
        if (isEvading && Time.time > evadeEndTime)
        {
            isEvading = false;
        }
    }
}
