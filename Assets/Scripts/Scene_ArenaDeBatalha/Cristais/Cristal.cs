using PurrNet.Prediction;
using Resoulnance.Scene_Arena.Player;
using UnityEngine;

namespace Resoulnance.Scene_Arena.Cristal
{
    public class Cristal : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] PredictedRigidbody _rb;
        [SerializeField] GameObject minimap_Img;

        [Header("Config")]
        public int idCristal;

        ListCristaisController cristaisController;

        void Start()
        {
            minimap_Img.SetActive(true);

            cristaisController = ListCristaisController.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 3)
            {
                if (!other.TryGetComponent(out PlayerReferences playerReferences)) return;

                ulong idPlayer = playerReferences.currentState.playerId;
                cristaisController.PegouCristal(idCristal, idPlayer, playerReferences.currentState.team);
            }
        }
    }
}


