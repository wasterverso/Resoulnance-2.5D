using PlayFlow;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;

public class Services_PartidaController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] ServerConection serverConection;
    [SerializeField] Services_LobbyController servicesLobbyController;
    [SerializeField] UDPTransport purrnet_UdpTransport;

    public async void IniciarServidor_Host()
    {
        var (address, port) = await serverConection.StartNewServer();

        if (!string.IsNullOrEmpty(address) && port != 0)
        {
            //Debug.Log($"Conectando ao servidor em {address}:{port}");

            await servicesLobbyController.AtualizarCodigoServidorNoLobby(address, port);
        }
        else
        {
            Debug.LogError("Falha ao iniciar servidor.");
        }
    }

    public void ConectarNoServidor(string address, int port)
    {
        purrnet_UdpTransport.address = address;
        purrnet_UdpTransport.serverPort = (ushort)port;

        NetworkManager.main.StartClient();
    }
}
