using PurrNet;
using PurrNet.Transports;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena
{
    public class GameManager : MonoBehaviour
    {
        public void SairDoJogo()
        {
            ListTeamController listTeamController = ListTeamController.Instance;
            if (listTeamController != null)
            {
                if (listTeamController.tipoSalaAtual == TiposDeSalas.Treinamento)
                {
                    Destroy(NetworkManager.main.gameObject);

                    SceneManager.LoadScene("1_MainMenu", LoadSceneMode.Single);
                }                    
            }
        }
    }
}

