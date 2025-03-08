using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerController : NetworkBehaviour
{
    //--------- MOVEMENT ---------
    private CharacterController characterController;
    internal float playerSpeed = 2.0f;

    //--------- SHOOTER ---------
    public GameObject bulletPrefab;
    internal int bulletVelocity = 6;
    public Transform bulletSpawnPoint;

    //--------- LIFE ---------
    internal NetworkVariable<float> Life = new NetworkVariable<float>();
    private TextMeshProUGUI lifeUI;
    internal NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>();



    // Evento que se lanza cuando se hace spawn de un objeto
    // Hay que pensar que se lanza en cada instancia de cada cliente/servidor
    public override void OnNetworkSpawn()
    {
        characterController = GetComponent<CharacterController>();
        lifeUI = GetComponentInChildren<TextMeshProUGUI>();

        if (IsOwner)
        {
            InitializeVariablesServerRpc();
        }
        
    }

    [ServerRpc] // Remote Procedure Call (llamada a procedimiento remoto)
    private void InitializeVariablesServerRpc(ServerRpcParams rpcParams = default)
    {
        Life.Value = 100; //Las Network variables deben ser modificadas/inicializadas en el servidor
        PlayerColor.Value = new Color(Random.Range(0, .5f), Random.Range(0, .5f), Random.Range(0, .5f), .6f);
    }

    void Update()
    {

        GetComponent<MeshRenderer>().material.color = PlayerColor.Value;

        // Life UI
        lifeUI.text = Life.Value.ToString();
        lifeUI.color = IsLocalPlayer ? Color.green : Color.red;
        lifeUI.transform.LookAt(GameObject.FindObjectOfType<Camera>().transform);

        if (IsLocalPlayer)
        {
            // MOVEMENT
            float ejeH = Input.GetAxis("Horizontal");
            float ejeV = Input.GetAxis("Vertical");
            PlayerMoveServerRpc(ejeH, ejeV);

            // SHOOT
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShootServerRpc();
            }
        }
    }

    [ServerRpc]
    private void PlayerMoveServerRpc(float ejeH, float ejeV, ServerRpcParams rpcParams = default)
    {
        Vector3 move = new Vector3(ejeH, 0, ejeV);
        characterController.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

    }

    [ServerRpc]
    private void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
        bullet.GetComponent<NetworkObject>().Spawn(); //Sincronizamos la instancia de la bala
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletVelocity;
        Destroy(bullet, 2f);
    }

    [ServerRpc]
    private void UpdatePlayerLifeServerRpc(float number, ServerRpcParams rpcParams = default)
    {
        if (Life.Value > 0)
        {
            Life.Value += number; //Actualizamos la networkVariable
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Bullet"))
        {
            other.gameObject.SetActive(false);
            if (IsLocalPlayer) UpdatePlayerLifeServerRpc(-1);
        }
        else if (other.transform.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            if (IsLocalPlayer) UpdatePlayerLifeServerRpc(+1);
        }
    }
}
