using System;
using UnityEditor;
using UnityEngine;

public class EntitySpawnPoint : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private VoidEventChannelSO askSpawnAllEntities;
    [SerializeField] private VoidEventChannelSO askClearAllEntities;
    
    [ColorHeader("Config", ColorHeaderColor.Config)]
    [SerializeField] private GameObject entityPrefab;

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
    public void SpawnEntity()
    {
        instance = Instantiate(entityPrefab, transform);
        var heightBody = instance.GetComponentInChildren<HeightBody2D>();
        if (heightBody)
        {
            var position = transform.position;
            heightBody.horizontalCoords = position - Vector3.up * position.z;
            heightBody.height = position.z;
        }
    }

    public void RemoveEntity()
    {
        if (instance == null) return;
        Destroy(instance);
        instance = null;
    }
    
    private void OnDrawGizmos()
    {
        if (entityPrefab == null) return;
        
        var position = transform.position;
        Handles.Label(
            position  + Vector3.up, 
            $"{entityPrefab.name}: {position}");
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, 0.5f);
    }
}
