using UnityEngine;
using UnityEngine.Networking;

namespace Players
{
#pragma warning disable 618
    public class NetworkPlayer : NetworkBehaviour
#pragma warning restore 618
    {
        [SerializeField] private GameObject playerPrefab;
        private GameObject playerCharacter;

        private void Start()
        {
            SpawnCharacter();
        }

        private void SpawnCharacter()
        {
            if (!isServer)
            {
                return;
            }

            playerCharacter = Instantiate(playerPrefab, transform.position, transform.rotation);


            NetworkServer.Spawn(playerCharacter);
        }
    }
}