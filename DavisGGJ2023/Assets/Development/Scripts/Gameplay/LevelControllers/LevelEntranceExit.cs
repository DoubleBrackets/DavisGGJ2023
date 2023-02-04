using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class LevelEntranceExit : MonoBehaviour
{
    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private LevelEntranceEventChannelSO askTravelLevels;

    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private LevelEntranceEventChannelSO askSpawnPlayer;
    [SerializeField] private VoidEventChannelSO askClearAllEntities;

    [ColorHeader("Dependencies")]
    [SerializeField] private LevelEntranceSO entrance;
    [SerializeField] private LevelEntranceSO exit;

    [ColorHeader("Config")]
    [SerializeField] private GameObject protagPrefab;
    [SerializeField] private LayerMask playerMask;
    
    private GameObject instance;

    private bool spawned = true;

    private void OnEnable()
    {
        askSpawnPlayer.OnRaised += SpawnPlayer;
        askClearAllEntities.OnRaised += RemoveEntity;
    }
    
    private void OnDisable()
    {
        askSpawnPlayer.OnRaised -= SpawnPlayer;
        askClearAllEntities.OnRaised -= RemoveEntity;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!spawned) return;
        if (((1 << other.gameObject.layer) | playerMask) == playerMask)
        {
            if(exit != null)
                askTravelLevels.RaiseEvent(exit);
        }
    }
    
    public void SpawnPlayer(LevelEntranceSO levelEntranceSo)
    {
        if (levelEntranceSo != entrance) return;

        spawned = false;
        instance = Instantiate(protagPrefab, transform);
        var heightBody = instance.GetComponentInChildren<HeightBody2D>();
        if (heightBody)
        {
            var position = transform.position;
            heightBody.horizontalCoords = position - Vector3.up * position.z;
            heightBody.height = position.z;
        }

        StartCoroutine(CoroutDebounceEnter());
    }

    private IEnumerator CoroutDebounceEnter()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        spawned = true;
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
            if (protagPrefab == null) return;
            
            var position = transform.position;
            string entranceName = entrance == null ? "" : entrance.name;
            string exitName = exit == null ? "" : exit.name;
            Handles.Label(
                position  + Vector3.up * 2f, 
                $"{position}\nEnter:{entranceName}\nExit:{exitName}");

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(position, 0.5f);
        }
    #endif
}
