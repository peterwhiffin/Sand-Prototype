using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : MonoBehaviour
{

    private Enemy enemyScript;
    public GameObject enemy;

    void Start()
    {
        enemyScript = enemy.GetComponent<Enemy>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyScript.playerInProximity = true;
            enemyScript.detectedPlayer = other.gameObject.GetComponent<Player>();
        }
    }
}
