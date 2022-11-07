using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionArea : MonoBehaviour
{
    public GameObject enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.GetComponent<Enemy>().playerInProximity = true;
            enemy.GetComponent<Enemy>().detectedPlayer = other.gameObject.GetComponent<Player>();
        }
    }
}
