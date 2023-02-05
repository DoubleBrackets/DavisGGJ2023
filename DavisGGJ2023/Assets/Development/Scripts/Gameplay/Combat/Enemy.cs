using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameStateSO gameState;


    private void Awake()
    {
        gameState.CurrentEnemyCount++;
    }
    
    private void OnDestroy()
    {
        gameState.CurrentEnemyCount--;
    }
    
}
