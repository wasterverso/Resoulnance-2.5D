using PurrNet;
using Resoulnance.Flyers;
using Resoulnance.Scene_Arena;
using Resoulnance.Scene_Arena.Config;
using Resoulnance.Scene_Arena.HUD;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena
{
    public class GameStart_Treinamento : MonoBehaviour
    {
        [Header("Controle")]
        [SerializeField] GameStart_Inicial gameStart;
        [SerializeField] HudController hudController;
        [SerializeField] PlayerSpawn_Arena playerSpawnArena;
        ListTeamController listTeamController;

        [Header("Controle")]
        [SerializeField] int idFlyer_test;
        [SerializeField] int idCarta1_Test;
        [SerializeField] int idCarta2_Test;
        [SerializeField] int idCarta3_Test;
        [SerializeField] int idItem;

        [Header("UI")]
        [SerializeField] GameObject connectPainel;
        [SerializeField] Dropdown mundos_Dropdown;
        [SerializeField] Button conectar_btn;

        [Header("Refs Obj")]
        [SerializeField] GameObject listObjPrefab;

        Mundo mundoAtual;
        bool isTestarNaCena = false;
        Personagem_Data flyerEscolhido = default;

        private void Start()
        {
            if (Application.platform == RuntimePlatform.LinuxServer) return;

            listTeamController = ListTeamController.Instance;

            if (ListTeamController.Instance != null)
            {
                if (listTeamController.tipoSalaAtual == TiposDeSalas.Treinamento)
                {
                    IniciarTreinamento();
                }
            }
            else
            {
                IniciarPainelConexaoLocal();
            }
        }

        void IniciarPainelConexaoLocal()
        {
            mundos_Dropdown.ClearOptions();
            mundos_Dropdown.AddOptions(new List<string>(Enum.GetNames(typeof(Mundo))));

            conectar_btn.onClick.AddListener(IniciarMundoTeste);

            connectPainel.SetActive(true);

            if (ListTeamController.Instance == null)
            {
                GameObject listaInstance = Instantiate(listObjPrefab);
                listTeamController = listaInstance.GetComponent<ListTeamController>();
            }

            isTestarNaCena = true;
        }

        void IniciarMundoTeste()
        {
            int indexSelecionado = mundos_Dropdown.value;
            mundoAtual = (Mundo)indexSelecionado;

            //Debug.Log("Modo selecionado: " + mundoAtual);

            switch (mundoAtual)
            {
                case Mundo.Host:
                    NetworkManager.main.StartHost();
                    break;

                case Mundo.Server:
                    NetworkManager.main.StartServer();
                    break;

                case Mundo.Client:
                    NetworkManager.main.StartClient();
                    break;
            }

            IniciarTreinamento();

            mundos_Dropdown.gameObject.SetActive(false);
            conectar_btn.gameObject.SetActive(false);
            connectPainel.SetActive(false);
        }

        public void IniciarTreinamento()
        {
            var flyerData = ArenaReferences.Instance.flyerData;
            if (isTestarNaCena)
            {
                flyerEscolhido = flyerData.personagens.Find(c => c.id == idFlyer_test);
            }
            else
            {
                listTeamController = ListTeamController.Instance;

                idCarta1_Test = listTeamController.JogadoresConfig[0].idCarta1;
                idCarta2_Test = listTeamController.JogadoresConfig[0].idCarta2;
                idCarta3_Test = listTeamController.JogadoresConfig[0].idCarta3;

                flyerEscolhido = flyerData.personagens.Find(c => c.id == listTeamController.JogadoresConfig[0].idFlyer);
            }

            hudController.AtribuirCartas(idCarta1_Test, idCarta2_Test, idCarta3_Test, idItem);
            hudController.AtribuirSupremaFlyer(flyerEscolhido);
        }

        public Personagem_Data GetPersonagem()
        {
            return flyerEscolhido;
        }

        public bool EstaTestandoNaCena()
        {
            return isTestarNaCena;
        }
    }
}

