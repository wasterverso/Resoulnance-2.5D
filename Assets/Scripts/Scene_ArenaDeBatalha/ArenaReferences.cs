using PurrNet.Prediction;
using Resoulnance.Cartas;
using Resoulnance.Flyers;
using Resoulnance.Scene_Arena;
using Resoulnance.Scene_Arena.Camera;
using Resoulnance.Scene_Arena.Player;
using Resoulnance.Scene_Arena.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Resoulnance.Scene_Arena
{
    public class ArenaReferences : MonoBehaviour
    {
        public static ArenaReferences Instance;

        [Header("Camera")]
        public CameraFollow cameraFollow;

        [Header("Player")]
        public PlayerReferences playerReferences;
        public Team team;

        public event Action OnPlayerSpawned;

        [Header("Game")]
        public Joystick joystick;
        public GameManager gameManager;
        public GameStart_Inicial gameStartConfig;
        public CargasDashUI dashUI;
        public MorteController morteController;
        public PlayerHitView playerHitView;

        [Header("HUD")]
        public GameObject depositarBtn;

        [Header("Data")]
        public Cartas_Data cartasData;
        public Flyer_Data flyerData;
        public Icons_Data iconsData;
        public ItensAtivaveis_Data itensData;

        [Header("Network")]
        public PredictionManager predictionManager;

        [Header("Player Obj list")]
        public List<PlayersObjList> listaObjetosPlayers = new List<PlayersObjList>();

        [Header("Mapa")]
        public Collider paredeBlue;
        public Collider paredeRed;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void Set_PlayerObj(ulong playerId, GameObject gameObj)
        {
            if (listaObjetosPlayers.Exists(p => p.id == playerId)) return;

            var playerObj = new PlayersObjList();
            playerObj.player = $"PlayerId: {playerId}";
            playerObj.id = playerId;
            playerObj.obj = gameObj;
            
            listaObjetosPlayers.Add(playerObj);
        }

    }
}

[System.Serializable]
public struct PlayersObjList
{
    public string player;
    public ulong id;
    public GameObject obj;
}