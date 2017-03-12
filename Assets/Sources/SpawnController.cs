using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnController : MonoBehaviour {
    private const string PLAYER_RESOURCE = "PlayerCharacter";
    private const string SPAWN_TAG = "SpawnPoint";

    private GameObject _currentPlayer = null;

    public void Update () {
        if (_currentPlayer == null) {
            _currentPlayer = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(PLAYER_RESOURCE));
            var allSpawnPoints = GameObject.FindGameObjectsWithTag(SPAWN_TAG);
            var spawnAt = allSpawnPoints[Random.Range(0, allSpawnPoints.Length)];
            _currentPlayer.transform.position = spawnAt.transform.position;
            _currentPlayer.transform.rotation = spawnAt.transform.rotation;
            NetworkManager.Instance.SpawnAvatar(_currentPlayer.GetComponent<AvatarController>().GetState());
        }
    }
}
