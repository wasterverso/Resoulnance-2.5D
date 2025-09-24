using PurrNet;
using Resoulnance.Scene_Arena.Player;
using System;
using UnityEngine;

namespace Resoulnance.Scene_Arena.Guardiao
{
    public class CircleRange_Guardiao : MonoBehaviour
    {
        [Header("Refs Scripts")]
        [SerializeField] GuardiaoController guardiaoScript;
        NetworkManager networkManager;

        [Header("Refs Int")]
        [SerializeField] SpriteRenderer spriteCircle;

        [Header("Sprite")]
        [SerializeField] Sprite rangePadrao_Sprite;
        [SerializeField] Sprite rangeRed_Sprite;

        GameObject target;
        float maxDistance = 15f;
        float minDistance = 9f;

        ulong meuId;
        bool focandoEmMim = false;

        private void Start()
        {
            networkManager = NetworkManager.main;
            networkManager.onLocalPlayerReceivedID += Set_MeuId;
        }

        private void Set_MeuId(PlayerID player)
        {
            meuId = player.id;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlayerReferences>(out PlayerReferences playerReferences))
            {
                if (playerReferences.isOwner && playerReferences.currentState.team != guardiaoScript.TeamGuardiao())
                {
                    Color spriteColor = spriteCircle.color;
                    spriteColor.a = 0;
                    spriteCircle.color = spriteColor;

                    spriteCircle.enabled = true;
                    target = other.gameObject;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == target)
            {
                spriteCircle.enabled = false;
            }
        }

        void Update()
        {
            if (target != null && guardiaoScript.GetQuantidadeCristais() != 0)
            {
                // Calcule a distância entre este objeto e o alvo (jogador)
                float distance = Vector2.Distance(transform.position, target.transform.position);

                // Interpole linearmente a opacidade com base na distância
                float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
                float alpha = Mathf.Lerp(1f, 0f, t); // 0 a 1

                // Aplique a opacidade ao sprite
                Color spriteColor = spriteCircle.color;
                spriteColor.a = alpha;
                spriteCircle.color = spriteColor;

                if (guardiaoScript.currentState.idPlayerAlvo == meuId)
                {
                    if (!focandoEmMim)
                    {
                        focandoEmMim = true;
                        spriteCircle.sprite = rangeRed_Sprite;
                    }
                }
                else
                {
                    if (focandoEmMim)
                    {
                        focandoEmMim = false;
                        spriteCircle.sprite = rangePadrao_Sprite;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            networkManager.onLocalPlayerReceivedID -= Set_MeuId;
        }
    }
}

