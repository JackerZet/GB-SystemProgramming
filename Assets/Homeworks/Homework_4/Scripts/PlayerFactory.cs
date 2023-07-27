using UnityEngine;
using UnityEngine.Networking;

namespace Homeworks.homework_4
{
    public class PlayerFactory : NetworkBehaviour
    {
        [SerializeField]
        private GameObject playerPrefab;
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
            playerCharacter = Instantiate(playerPrefab, transform.position, transform.localRotation);
            NetworkServer.SpawnWithClientAuthority(playerCharacter, connectionToClient);
        }
    }
}