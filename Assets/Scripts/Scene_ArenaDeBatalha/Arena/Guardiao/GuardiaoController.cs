using PurrNet;
using PurrNet.Prediction;
using Resoulnance.Scene_Arena.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena.Guardiao
{
    public class GuardiaoController : PredictedIdentity<GuardiaoController.State>, IGuardiao
    {
        public struct State : IPredictedData<State>
        {
            public ulong idPlayerAlvo;
            public Vector3 direcaoAlvo;
            public float tempoEntreTiros;

            public void Dispose() { }
        }

        [Header("refs Script")]
        ListCristaisController listCristaisController;

        [Header("Controles")]
        [SerializeField] Team team;
        [SerializeField] float tempoEntreTiros;
        [SerializeField] float rangeVerificar = 5;
        [SerializeField] bool mostrarGizmo = false;

        [Header("Contagem")]
        [SerializeField] GameObject contagemCanvas;
        [SerializeField] Image contagemFrente;
        [SerializeField] Text contagemText;

        [Header("Refs")]
        [SerializeField] Transform rotationTransform;
        [SerializeField] Transform spawnProjetilTransform;
        [SerializeField] GameObject projetilPrefab;

        float rotationSpeed = 5f;
        int quantCristaisAtual = 0;
        int playerMask;
        GameObject alvoObj;

        protected override void LateAwake()
        {
            listCristaisController = ListCristaisController.Instance;
            listCristaisController.OnListChanged += AtualizarCristais;
            playerMask = 1 << LayerMask.NameToLayer("Player");
        }

        protected override State GetInitialState()
        {
            return new State()
            {
                idPlayerAlvo = 0,
            };
        }

        public void ReceberDano(int dano)
        {
            if (dano > 0 && quantCristaisAtual > 0)
            {
                int cristaisQuantidade = Mathf.Min(dano, quantCristaisAtual);

                List<int> cristaisParaRetirarList = new List<int>();
                int contagem = 0;

                foreach (var cris in listCristaisController.currentState.cristaisList)
                {
                    if (cris.estaNoGuardiao && cris.team == team)
                    {
                        cristaisParaRetirarList.Add(cris.id);
                        contagem++;

                        if (contagem >= cristaisQuantidade)
                        {
                            break;
                        }
                    }
                }

                listCristaisController.RetirarCristaisDoGuardiao(cristaisParaRetirarList);

                //SpawnNotification_Rpc((cristaisQuantidade * -1), 3);
            }
        }

        private void AtualizarCristais(CristalInfo info)
        {
            quantCristaisAtual = listCristaisController.currentState.cristaisList.Count(c => c.team == team && c.estaNoGuardiao);

            contagemText.text = quantCristaisAtual.ToString();
        }

        protected override void Simulate(ref State state, float delta)
        {
            if (quantCristaisAtual <= 0)
            {
                if (state.idPlayerAlvo != 0)
                    state.idPlayerAlvo = 0;
                return;
            }

            if (quantCristaisAtual > 0 && state.idPlayerAlvo == 0)
                VerificarPlayerNaArea(ref state);

            if (quantCristaisAtual > 0 && state.idPlayerAlvo != 0)
            {
                VerificarPosicao(ref state, delta);

                state.tempoEntreTiros += delta;
                if (state.tempoEntreTiros >= tempoEntreTiros && alvoObj != null)
                {
                    var status = alvoObj.GetComponent<IReceberDano>().ReceberStatusAlvo(); 

                    if (status != null && status.playerState != PlayerState.Dead)
                    {
                        state.tempoEntreTiros = 0f;
                        AtirarProjetil(alvoObj);
                    }
                }
            }
        }

        void VerificarPlayerNaArea(ref State state)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, rangeVerificar, playerMask);

            if (hits.Length == 0) return;

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out PlayerReferences playerReferences))
                {
                    if (playerReferences.currentState.team != team)
                    {
                        alvoObj = playerReferences.gameObject;
                        state.idPlayerAlvo = playerReferences.currentState.playerId;
                        break;
                    }
                }
            }
        }

        void VerificarPosicao(ref State state, float delta)
        {
            if (alvoObj != null)
            {
                float distancia = Vector3.Distance(transform.position, alvoObj.transform.position);

                // Se a distância for maior que 5, limpar alvo
                if (distancia > rangeVerificar)
                {
                    alvoObj = null;
                    state.idPlayerAlvo = 0;
                    return;
                }

                Vector3 direcao = alvoObj.transform.position - transform.position;
                direcao.y = 0f;

                state.direcaoAlvo = direcao;

                Quaternion rotacaoAlvo = Quaternion.LookRotation(direcao);
                rotationTransform.rotation = Quaternion.Slerp(rotationTransform.rotation, rotacaoAlvo, delta * rotationSpeed);
            }
        }

        void AtirarProjetil(GameObject alvoPrefs)
        {
            var createObject = predictionManager.hierarchy.Create(projetilPrefab.gameObject, spawnProjetilTransform.position, Quaternion.identity);
            if (!createObject.HasValue) return;

            if (!createObject.Value.TryGetComponent(predictionManager, out ProjetilGuardiao projetilScript)) return;
            projetilScript.SetInfoConfig(alvoPrefs, team);
        }

        public Team TeamGuardiao()
        {
            return team;
        }

        public int GetQuantidadeCristais()
        {
            return quantCristaisAtual;
        }

        private void OnDrawGizmos()
        {
            if (mostrarGizmo)
            {
                Gizmos.color = Color.red; // cor da esfera no editor
                Gizmos.DrawWireSphere(transform.position, rangeVerificar);
            }
        }

        protected override void Destroyed()
        {
            if (listCristaisController == null) return;

            listCristaisController.OnListChanged -= AtualizarCristais;
        }
    }
}

