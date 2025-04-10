using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] internal List<GameObject> spawnPointsList;

    private void Awake()
    {
        #region SINGLETON
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        #endregion
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Spawn Points
            spawnPointsList.AddRange(GameObject.FindGameObjectsWithTag("SpawnPoint"));
            // PowerUps
            SpawnPowerUps();
        }
    }

    public void SpawnPowerUps()
    {
        List<GameObject> availableSpawnPoints = new List<GameObject>(spawnPointsList); // Create a copy to avoid modifying the original list

        for (int i = 0; i < 5; i++)
        {
            int index = Random.Range(0, availableSpawnPoints.Count);
            GameObject spawnPoint = availableSpawnPoints[index];
            availableSpawnPoints.RemoveAt(index); // Remove used spawn point

            Vector3 spawnPosition = spawnPoint.transform.position;
            var powerUp = Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
            powerUp.GetComponent<PowerUp>().SetupPowerUp();
            powerUp.GetComponent<NetworkObject>().Spawn(); // Synchronize across clients
        }
    }
}