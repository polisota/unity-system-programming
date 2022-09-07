using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mechanics;

public class PlanetSpawner : NetworkBehaviour
{
    [SerializeField] public PlanetOrbit _planetPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();

        GameObject planetInstance = Instantiate(_planetPrefab.gameObject);
        planetInstance.name = _planetPrefab.name;
        NetworkServer.Spawn(planetInstance);

        Destroy(gameObject);
    }
}