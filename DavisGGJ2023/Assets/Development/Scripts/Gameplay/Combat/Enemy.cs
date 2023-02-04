using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameStateSO gameState;


    private void OnEnable()
    {
        gameState.CurrentEnemyCount++;
    }
    
    private void OnDisable()
    {
        gameState.CurrentEnemyCount--;
    }
    
}
