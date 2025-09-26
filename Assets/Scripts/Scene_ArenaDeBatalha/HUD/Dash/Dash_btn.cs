using Resoulnance.Scene_Arena;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena.HUD
{
    public class Dash_btn : MonoBehaviour, IPointerClickHandler
    {
        ArenaReferences _arenaReferences;

        Button dash_btn;

        private void Start()
        {
            dash_btn = GetComponent<Button>();
            _arenaReferences = ArenaReferences.Instance;
            _arenaReferences.OnPlayerSpawned += StartSecundario;
        }

        private void StartSecundario()
        {
            _arenaReferences.playerReferences.playerMovement.OnVelocidadeEstaDiferente += PlayerMudouVelocidade;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ChamarDash();
        }

        void ChamarDash()
        {
            _arenaReferences.playerReferences.playerMovement.ChamarDash();
        }

        void PlayerMudouVelocidade(VelocidadeRefs velRefs)
        {
            dash_btn.interactable = (velRefs.velBasica == velRefs.velAtual);
        }

#if (UNITY_EDITOR || UNITY_STANDALONE) && !UNITY_SERVER //Rodar só no PC e Editor
        private void Update()
        {
            if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
            {
                ChamarDash();
            }
        }
#endif

        private void OnDestroy()
        {
            if (_arenaReferences == null) return;

            _arenaReferences.OnPlayerSpawned -= StartSecundario;

            if (_arenaReferences.playerReferences == null) return;
            _arenaReferences.playerReferences.playerMovement.OnVelocidadeEstaDiferente -= PlayerMudouVelocidade;
        }
    }

}
