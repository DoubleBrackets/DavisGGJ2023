using System;
using UnityEditor;
using UnityEngine;

public class EntitySpawnPoint : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private BoolEventChannelSO askSpawnAllEntities;
    [SerializeField] private VoidEventChannelSO askClearAllEntities;
    
    [ColorHeader("Config", ColorHeaderColor.Config)]
    [SerializeField] private GameObject entityPrefab;
    [SerializeField] private bool isRequiredEnemy;
    [SerializeField] private bool respawnAfterCleared;

    private GameObject instance;


    private void OnEnable()
    {
        askSpawnAllEntities.OnRaised += SpawnEntity;
        askClearAllEntities.OnRaised += RemoveEntity;
    }
    
    private void OnDisable()
    {
        askSpawnAllEntities.OnRaised -= SpawnEntity;
        askClearAllEntities.OnRaised -= RemoveEntity;
    }

    // TODO: Pooling?
    public void SpawnEntity(bool levelCleared)
    {
        if (levelCleared && !respawnAfterCleared) return;
        
        instance = Instantiate(entityPrefab, transform);
        var enemy = instance.GetComponentInChildren<Enemy>();
        if (enemy)
        {
            enemy.isRequiredEnemy = isRequiredEnemy;
        }
        var heightBody = instance.GetComponentInChildren<HeightBody2D>();
        if (heightBody)
        {
            var position = transform.position;
            heightBody.horizontalPos = position - Vector3.up * position.z;
            heightBody.height = position.z;
        }
    }

    public void RemoveEntity()
    {
        if (instance == null) return;
        Destroy(instance);
        instance = null;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (entityPrefab == null) return;
        
        var position = transform.position;
        Handles.Label(
            position  + Vector3.up, 
            $"{entityPrefab.name}: {position - Vector3.up * position.z}");
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, 0.5f);
    }
    #endif
}
