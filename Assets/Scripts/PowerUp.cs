using Unity.Netcode;
using UnityEngine;

public class PowerUp : NetworkBehaviour
{
    private int powerUpType;
    internal NetworkVariable<Color> PowerupColor = new NetworkVariable<Color>();

    public override void OnNetworkSpawn()
    {
        GetComponent<MeshRenderer>().material.color = PowerupColor.Value;
        PowerupColor.OnValueChanged += (oldColor, newColor) =>
        {
            GetComponent<MeshRenderer>().material.color = newColor;
        };
    }

    public void SetupPowerUp()
    {
        powerUpType = Random.Range(0, 3);

        Color color = powerUpType switch
        {
            0 => Color.blue,
            1 => Color.red,
            2 => Color.green,
            _ => Color.yellow
        };
        PowerupColor.Value = color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            powerUpAction(other.gameObject);
            Destroy(this.gameObject);
        }
    }

    private void powerUpAction(GameObject player)
    {
        switch (powerUpType)
        {
            case 0: //movimiento
                player.GetComponent<PlayerController>().playerSpeed = player.GetComponent<PlayerController>().playerSpeed * 2;
                Debug.Log("Más movement");
                break;

            case 1: //disparo
                player.GetComponent<PlayerController>().bulletVelocity = player.GetComponent<PlayerController>().bulletVelocity * 2;
                Debug.Log("Más disparo");
                break;

            case 2: //vida
                player.GetComponent<PlayerController>().Life.Value = player.GetComponent<PlayerController>().Life.Value * 2;
                Debug.Log("Más vida");
                break;
        }
    }
}