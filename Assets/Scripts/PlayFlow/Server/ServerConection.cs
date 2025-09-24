using PlayFlow.SDK.Servers;
using PurrNet;
using PurrNet.Transports;
using Resoulnance.Scene_Arena;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ServerConection : MonoBehaviour
{
    string playflowApiKey = "pfgvQTb8gfIOY31m0h2Xfrqrs9RtCRjMPI9MjBJS5Ho5A=";
    string TT = "[Server Connection]";

    [Header("Refs")]
    [SerializeField] GameManager gameManager;

    [Header("Refs")]
    [SerializeField] UDPTransport purrnet_UdpTransport;
    [SerializeField] InputField address_input;
    [SerializeField] InputField port_input;

    [Header("Config")]
    [SerializeField] string _address;
    [SerializeField] ushort _port;
    [SerializeField] string region = "south-america-brazil";
    [SerializeField] int tempoAtividade = 300;

    private PlayflowServerApiClient _apiClient;
    string novoServerId;

    void Start()
    {
        _apiClient = new PlayflowServerApiClient(playflowApiKey);
    }

    ServerCreateRequest DadosServidor()
    {
        var serverCreateRequest = new ServerCreateRequest
        {
            name = $"Resoulnance_Br_{System.DateTime.UtcNow:yyyyMMddHHmmss}",
            region = region, // Altere para uma região válida do seu projeto
            compute_size = ComputeSizes.Small, // Ou Medium, Large, etc.
            version_tag = "default", // Especifique uma tag de versão da build, se necessário
            ttl = tempoAtividade, // Opcional: tempo de vida (em segundos)
            custom_data = new Dictionary<string, object>
        {
            { "purpose", "teste_helloworld" },
            { "started_by_script", true }
        }
        };

        return serverCreateRequest;
    }

    [ContextMenu("Iniciar Novo Servidor")]
    public async void IniciarNovoServidor()
    {
        var (address, port) = await StartNewServer();
    }

    public async Task<(string address, int port)> StartNewServer()
    {
        Debug.Log($"{TT} Tentando iniciar um novo servidor...");

        try
        {
            var serverData = DadosServidor();

            ServerStartResponse response = await _apiClient.StartServerAsync(serverData);
            novoServerId = response.instance_id;
            Debug.Log($"{TT} Servidor iniciado com sucesso! ID da instância: {response.instance_id}, Status: {response.status}");
            Debug.Log($"{TT} Servidor iniciado na regiao: {response.region} | Processamento servidor: {response.compute_size}");

            if (response.network_ports != null && response.network_ports.Count > 0)
            {
                string address = response.network_ports[0].host;
                int port = response.network_ports[0].external_port;
                Debug.Log($"{TT} Dados Server: (Address: {address}) (Port: {port}) ");

                bool pronto = await EsperarServidorFicarPronto(response.instance_id);

                if (!pronto) return (null, 0);

                return (address, port);
            }
            else
            {
                Debug.LogWarning($"{TT} Servidor iniciado, mas não retornou nenhuma porta de rede.");
                return (null, 0);
            }
        }
        catch (PlayFlowApiException apiEx)
        {
            Debug.LogError($"{TT} Erro da API ao iniciar o servidor: {apiEx.Message}");
            return (null, 0);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"{TT} Erro inesperado ao iniciar o servidor: {ex.Message}\n{ex.StackTrace}");
            return (null, 0);
        }
    }


    [ContextMenu("Parar Servidor")]
    public async void StopServer()
    {
        if (string.IsNullOrEmpty(novoServerId))
        {
            Debug.LogWarning($"{TT} Nenhum ID de instância de servidor foi fornecido.");
            return;
        }

        Debug.Log($"{TT} Tentando parar o servidor com ID da instância: {novoServerId}...");

        try
        {
            ServerStopResponse response = await _apiClient.StopServerAsync(novoServerId);
            Debug.Log($"{TT} Servidor parado com sucesso! Status: {response.status}");
        }
        catch (PlayFlowApiException apiEx)
        {
            Debug.LogError($"{TT} Erro da API ao parar o servidor {novoServerId}: {apiEx.Message}");
        }
    }

    [ContextMenu("Listar Servidores")]
    public void ListarServidores()
    {
        ListarTodosServidores();
    }

    private async void ListarTodosServidores()
    {
        Debug.Log($"{TT} Tentando listar todos os servidores...");
        try
        {
            // Defina includeLaunching como true para também ver servidores que estão iniciando
            ServerList response = await _apiClient.ListServersAsync(includeLaunching: false);
            Debug.Log($"{TT} Total de servidores: {response.total_servers}");

            foreach (var server in response.servers)
            {
                Debug.Log($"{TT} - Servidor encontrado: {server.name} (ID: {server.instance_id}), Status: {server.status}, regiao: {server.region}");
                
                if (server.network_ports != null && server.network_ports.Count > 0)
                {
                    foreach (var port in server.network_ports)
                    {
                        Debug.Log($" {TT} Port Name: \"{port.name}\" - Connection: {port.host}:{port.external_port} ({port.protocol.ToUpper()})");
                    }
                }
                else
                {
                    Debug.Log($"{TT} Network Ports: None reported.");
                }

                Debug.Log($"----------------------");
            }
        }
        catch (PlayFlowApiException apiEx)
        {
            Debug.LogError($"{TT} Erro da API PlayFlow ao listar os servidores: {apiEx.Message}");
        }
    }

    async void ProcurarConectarNoServidor()
    {
        try
        {
            // Defina includeLaunching como true para também ver servidores que estão iniciando
            ServerList response = await _apiClient.ListServersAsync(includeLaunching: true);
            Debug.Log($"{TT} Total de servidores: {response.total_servers}");

            foreach (var server in response.servers)
            {
                Debug.Log($"{TT} - Servidor encontrado: {server.name} (ID: {server.instance_id}), Status: {server.status}, regiao: {server.region}");
                _port = (ushort)server.network_ports[0].external_port;
                _address = server.network_ports[0].host;

                break;
            }
        }
        catch (PlayFlowApiException apiEx)
        {
            Debug.LogError($"{TT} Erro da API PlayFlow ao listar os servidores: {apiEx.Message}");
        }
    }

    public async Task<bool> EsperarServidorFicarPronto(string instanceId, int timeoutSegundos = 60)
    {
        var tempoDecorrido = 0;

        while (tempoDecorrido < timeoutSegundos)
        {
            var response = await _apiClient.GetServerDetailsAsync(instanceId);

            Debug.Log($"{TT} Status atual do servidor: {response.status}");

            if (response.status == "running")
            {
                Debug.Log($"{TT} Servidor está pronto!");
                return true; // retorna verdadeiro se o servidor estiver pronto
            }

            await Task.Delay(2000); // espera 2 segundos antes de tentar de novo
            tempoDecorrido += 2;
        }

        Debug.LogError($"{TT} Tempo limite atingido antes do servidor ficar pronto.");
        return false; // retorna falso se o timeout estourar
    }
}
