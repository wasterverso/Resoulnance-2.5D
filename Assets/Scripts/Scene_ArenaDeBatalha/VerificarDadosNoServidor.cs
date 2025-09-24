using PlayFlow;
using PurrNet;
using Resoulnance.Scene_Arena;
using System.Text;
using UnityEngine;

public class VerificarDadosNoServidor : NetworkBehaviour
{
    [Header("Server Debug")]
    [TextArea(5, 15)]
    [SerializeField] string dadosServer_Debug;

    [ContextMenu("Receber Dados Server")]
    public void ReceberDadosServer()
    {
        ServerData();
    }

    [ServerRpc(requireOwnership: false)]
    void ServerData()
    {
        var config = PlayFlowServerConfig.LoadConfig();
        if (config != null)
        {
            Debug.Log($"Server instance ID is: {config.instance_id}");

            var sb = new StringBuilder();

            // percorre todas as propriedades conhecidas
            sb.AppendLine($"instance_id: {config.instance_id}");
            sb.AppendLine($"region: {config.region}");

            // percorre todos os dados de custom_data
            if (config.custom_data != null)
            {
                sb.AppendLine("custom_data:");
                foreach (var kvp in config.custom_data)
                {
                    sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
                }
            }
            string dadosServer = $"[DadosServer] {sb}";

            ClientData(dadosServer);
        }
    }

    [ObserversRpc]
    void ClientData(string textData)
    {
        dadosServer_Debug = textData;
    }
}
