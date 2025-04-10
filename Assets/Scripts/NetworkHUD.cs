using UnityEngine;
using Unity.Netcode;

public class NetworkHUD : MonoBehaviour
{
    void OnGUI()
    {
        // Creamos una interfaz en pantalla
        GUILayout.BeginArea(new Rect(10, 20, 300, 300));

        // Si no somo un cliente o un servidor
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons(); // Mostramos los botones para iniciar una conexion
        }
        else
        {
            StatusLabels(); // Mostramos el estado del MULTIJUGADOR
        }

        GUILayout.EndArea();
    }

    // Cada boton inicia un tipo de servicio
    static void StartButtons()
    {
        var buttonStyle = new GUIStyle(GUI.skin.button) { normal = { textColor = Color.blue }, hover = { textColor = Color.green } };

        if (GUILayout.Button("HOST", buttonStyle)) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("CLIENT")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("SERVER")) NetworkManager.Singleton.StartServer();
    }

    // Mostramos el estado del servicio multijugador
    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
}