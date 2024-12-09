using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloodCount : MonoBehaviour
{
    [Header("Blood Counting")]
    public int playerBloodPoints = 0; // ile krwi ma na sobie gracz
    public int addBloodPoints = 1; //ile krwi dodane po wejsciu w plame krwi
    [Header("Fear Mode")]
    public int bloodPointsFearMode = 50; // jak dużo krwi gracz musi mieć do osiągnięcia fear mode
    public int fearModeTime = 10;
    [Header("DMG")]

    public float multiplier = 1.0f;
    public int pointsForMultiplier = 5; // po ilu pkt krwi zwieksza się dmg multiplikator

    public void addBlood()
    {
        if(playerBloodPoints < bloodPointsFearMode - addBloodPoints)
        {
            playerBloodPoints += addBloodPoints;
            Debug.Log($"Blood points: {playerBloodPoints}");
            dmgMultiply();
        }
        else
        {
            StartCoroutine(fearMode());
        }
    }

    public void dmgMultiply()
    {
        if(playerBloodPoints % pointsForMultiplier == 0)
        {
            multiplier += 0.1f;
            Debug.Log($"Curent multiplier: {multiplier}");

        }
    }

//fear mode czyli enemies boją sie mekka i wiecej dmg do ataków
    public IEnumerator fearMode()
    {
        Debug.Log("Fear Mode");
        yield return new WaitForSeconds(fearModeTime);
        playerBloodPoints = 0;
        Debug.Log($"Blood points: {playerBloodPoints}");
        Debug.Log("End of fear mode");
    }
}
