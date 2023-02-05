using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameStateSO gameState;
    public bool isRequiredEnemy;

    private bool wasRequiredEnemy = false;

    private void Start()
    {
        if (isRequiredEnemy)
        {
            wasRequiredEnemy = true;
            gameState.CurrentEnemyCount++;
        }
    }
    
    private void OnDestroy()
    {
        if(wasRequiredEnemy)
            gameState.CurrentEnemyCount--;
    }
    
}
