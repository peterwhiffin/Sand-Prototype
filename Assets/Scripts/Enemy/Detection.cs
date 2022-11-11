using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    public Enemy enemyScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyScript.playerInProximity = true;
            enemyScript.detectedPlayer = other.gameObject.GetComponent<Player>();
        }
    }
}
