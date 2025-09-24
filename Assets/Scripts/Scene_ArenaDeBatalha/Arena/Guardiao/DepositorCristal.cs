using Resoulnance.Scene_Arena.Player;
using UnityEngine;

namespace Resoulnance.Scene_Arena.Guardiao
{
    public class DepositorCristal : MonoBehaviour
    {
        [SerializeField] GuardiaoController guardiaoController;
        ArenaReferences arenaReferences;

        GameObject target;

        private void Start()
        {
            arenaReferences = ArenaReferences.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<PlayerReferences>(out PlayerReferences playerReferences)) return;

            if (!playerReferences.isOwner) return;

            if (guardiaoController.TeamGuardiao() != playerReferences.currentState.team) return;

            arenaReferences.depositarBtn.SetActive(true);


            target = other.gameObject;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == target)
            {
                arenaReferences.depositarBtn.SetActive(false);
                //anim.SetBool("Guardando", false);
            }
        }
    }
}

