using PurrNet;
using PurrNet.Transports;
using Resoulnance.Scene_Preparation.Controles;
using UnityEngine;

namespace Resoulnance.Scene_Preparation.Inicialize
{
    public class PreparacaoStart_Inicial : MonoBehaviour
    {
        [Header("NetworkManager")]
        [SerializeField] NetworkManager networkManager;

        [Header("Refs Script")]
        [SerializeField] Cronometro_Preparacao cronometroScript;
        [SerializeField] PreparacaoController prepController;
        ListTeamController listTeamController;

        [Header("Refs Obj")]
        [SerializeField] GameObject listObjPrefab;

        bool isServidor = false;

        private void Awake() // Se for Servidor 
        {
            listTeamController = ListTeamController.Instance;

            if (Application.platform == RuntimePlatform.LinuxServer || listTeamController.networkMode == NetworkMode.Server)
            {
                if (ListTeamController.Instance == null)
                {
                    GameObject listaInstance = Instantiate(listObjPrefab);
                    listTeamController = listaInstance.GetComponent<ListTeamController>();
                }

                isServidor = true;
                networkManager.StartServer();
            }
        }

        void Start()
        {
            if (isServidor) return;

            if (listTeamController.networkMode == NetworkMode.Host && listTeamController.tipoSalaAtual == TiposDeSalas.Treinamento)
            {
                NetworkManager.main.StartHost();
                cronometroScript.IniciarCronometro();
            }

            if (listTeamController.networkMode == NetworkMode.Cliente)
            {
                if (listTeamController.port != 0 && !string.IsNullOrEmpty(listTeamController.address))
                {
                    UDPTransport udpTransport = NetworkManager.main.GetComponent<UDPTransport>();
                    if (udpTransport != null)
                    {
                        udpTransport.address = listTeamController.address;
                        udpTransport.serverPort = (ushort)listTeamController.port;
                        udpTransport.StartClient();
                    }
                }
            }

            TelaDeCarregamento.Instance?.DesativarAvatarPainel();
        }

        public void StartCliente()
        {
            NetworkManager.main.StartClient();
        }

        public void StartServer()
        {
            NetworkManager.main.StartServer();
        }
    }
}

