using PurrNet;
using PurrNet.Prediction;
using Resoulnance.Cartas;
using Resoulnance.Scene_Arena;
using Resoulnance.Scene_Arena.Config;
using Resoulnance.Scene_Arena.Player;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena.HUD
{
    public class Skill_Btn : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler
    {
        ArenaReferences arenaReferences;

        [Header("Button Refs")]
        [SerializeField] Image rangeArrastar_img;
        [SerializeField] Image fundoCartaSkill_img;
        [SerializeField] Image cartaSplash_img;
        [SerializeField] Image BarraContagem_img;
        [SerializeField] Text textCountdown;

        [Header("Button Controller")]
        public bool ButtonAtivado = true;
        [SerializeField] int N_DessaSkill;
        [SerializeField] bool estaContando = false;
        [SerializeField] float TempoRestante;
        [SerializeField] float countDown;
        float rangeLimite = 110f;
        bool arrastarHabilitado = false;

        [Header("Cancelar Skill")]
        [SerializeField] Image cancelAreaImage;

        [Header("Carta")]
        [SerializeField] Carta carta;

        float tempoPassado = 0f;
        bool estaArrastando = false;
        Vector3 posicaoInicial;
        Vector2 direcaoDoArrasto;
        Vector2 cancelarPosition;

        Team team = Team.Nenhum;

        private void Start()
        {
            arenaReferences = ArenaReferences.Instance;

            posicaoInicial = transform.position;
        }

        public void ReceberCarta(Carta card)
        {
            carta = card;
            countDown = carta.tempoRecarga;
            TempoRestante = countDown;
            arrastarHabilitado = carta.arrastar;
            cartaSplash_img.sprite = carta.splashSprite;
        }

        void Update()
        {
            if (estaContando)
            {
                if (tempoPassado < countDown)
                {
                    BarraContagem_img.fillAmount = tempoPassado / countDown;
                    tempoPassado += Time.deltaTime;

                    TempoRestante -= Time.deltaTime;
                    textCountdown.text = Mathf.Ceil(TempoRestante).ToString();
                }
                else
                {
                    ResetarBotao();
                    estaContando = false;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!ButtonAtivado) return;

            direcaoDoArrasto = Vector2.zero;
            estaArrastando = false;

            if (team == Team.Nenhum)
            {
                team = arenaReferences.playerReferences.currentState.team;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!ButtonAtivado) return;

            cancelAreaImage.gameObject.SetActive(true);

            if (!arrastarHabilitado) return;

            estaArrastando = true;

            arenaReferences.playerReferences.skillDirectionPlayer.rangeSkillDiection_img.gameObject.SetActive(true);

            BarraContagem_img.enabled = false;
            rangeArrastar_img.enabled = true;

            BarraContagem_img.transform.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!ButtonAtivado) return;

            if (EstaNaAreaDeCancelar(eventData.position))
            {
                cancelAreaImage.color = new Color(cancelAreaImage.color.r, cancelAreaImage.color.g, cancelAreaImage.color.b, 1f); // Opacidade 100%
                arenaReferences.playerReferences.skillDirectionPlayer.rangeSkillDiection_img.color = Color.red;
            }
            else
            {
                cancelAreaImage.color = new Color(cancelAreaImage.color.r, cancelAreaImage.color.g, cancelAreaImage.color.b, 0.5f); // Opacidade 30%
                arenaReferences.playerReferences.skillDirectionPlayer.rangeSkillDiection_img.color = Color.white;
            }

            if (!arrastarHabilitado) return;

            BarraContagem_img.transform.position = eventData.position;

            // Verifica se o botão está dentro do círculo de limite de arrastar
            Vector3 centro = rangeArrastar_img.transform.position;
            Vector3 posAtual = BarraContagem_img.transform.position;

            float distancia = Vector3.Distance(posAtual, centro);

            if (distancia > rangeLimite)
            {
                Vector3 direcao = (posAtual - centro).normalized;
                BarraContagem_img.transform.position = centro + direcao * rangeLimite;
            }

            // Atualiza a direção em que o jogador está arrastando
            direcaoDoArrasto = eventData.position - (Vector2)rangeArrastar_img.transform.position;
            float angle = Mathf.Atan2(direcaoDoArrasto.y, direcaoDoArrasto.x) * Mathf.Rad2Deg;

            arenaReferences.playerReferences.skillDirectionPlayer.AtualizarDirecaoSkill(angle);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            BotaoFoiSolto(eventData);
        }

        void BotaoFoiSolto(PointerEventData eventData)
        {
            if (!ButtonAtivado) return;

            rangeArrastar_img.enabled = false;
            BarraContagem_img.transform.position = posicaoInicial;

            cancelAreaImage.gameObject.SetActive(false);
            cancelarPosition = eventData.position;
            if (!EstaNaAreaDeCancelar(cancelarPosition))
            {
                InstanciarSkill();
            }
            else
            {
                ResetarBotao();
            }
        }

        void InstanciarSkill()
        {
            PlayerSkills playerSkill = null;
            PlayerReferences playerReferences = arenaReferences.playerReferences;

            if (N_DessaSkill == 1)
            {
                playerSkill = arenaReferences.playerReferences.playerSkill_1;
            }
            else if (N_DessaSkill == 2)
            {
                playerSkill = arenaReferences.playerReferences.playerSkill_2;
            }
            else
            {
                playerSkill = arenaReferences.playerReferences.playerSkill_3;
            }

            if (carta.arrastar)
            {
                if (carta.in_Mapa)
                {
                    if (!estaArrastando)
                    {
                        float range = 5;
                        GameObject alvo = VerificarRange_System.GetAlvoAutomatico(
                                ArenaConfig.Instance.alvo, playerReferences.transform, range, team);

                        if (alvo != null)
                        {
                            Vector3 posJogador = playerReferences.transform.position;
                            Vector3 posAlvo = alvo.transform.position;

                            direcaoDoArrasto = new Vector2(posAlvo.x - posJogador.x, posAlvo.z - posJogador.z);
                        }
                        else
                        {
                            ResetarBotao();
                            return;
                        }
                    }

                    direcaoDoArrasto.Normalize();
                    Vector3 posicao = playerSkill.transform.position;
                    playerSkill.SetAtivarSkill(carta.id, countDown, direcaoDoArrasto, posicao);
                    playerReferences.playerAtk.EstaEmAtaque(true);
                    playerReferences.playerArma.DefinirDirecao(direcaoDoArrasto);
                }
            }
            else
            {
                playerSkill.SetAtivarSkill(carta.id, countDown, Vector2.zero, Vector3.zero);
            }

            arenaReferences.playerReferences.playerMostrarCartas.ChamarMostrarCarta(carta.id);

            IniciarCountdown();
        }

        void IniciarCountdown()
        {
            if (estaArrastando && arrastarHabilitado)
            {
                estaArrastando = false;
                arenaReferences.playerReferences.skillDirectionPlayer.rangeSkillDiection_img.gameObject.SetActive(false);
            }

            cartaSplash_img.color = Color.gray;
            fundoCartaSkill_img.color = Color.gray;
            ButtonAtivado = false;
            textCountdown.gameObject.SetActive(true);
            BarraContagem_img.enabled = true;
            estaContando = true;

            tempoPassado = 0f;
        }

        private void ResetarBotao()
        {
            estaContando = false;
            BarraContagem_img.fillAmount = 1f;

            cartaSplash_img.color = Color.white;
            fundoCartaSkill_img.color = Color.white;
            textCountdown.gameObject.SetActive(false);
            estaContando = false;
            BarraContagem_img.enabled = true;
            TempoRestante = countDown;
            ButtonAtivado = true;
        }

        private bool EstaNaAreaDeCancelar(Vector2 position)
        {
            // Verifica se a posição está dentro da área onde a instanciação da habilidade é cancelada
            return RectTransformUtility.RectangleContainsScreenPoint(cancelAreaImage.rectTransform, position, null);
        }
    }
}

