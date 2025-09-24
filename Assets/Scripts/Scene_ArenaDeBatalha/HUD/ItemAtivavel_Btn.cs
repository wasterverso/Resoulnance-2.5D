using Resoulnance.Scene_Arena;
using Resoulnance.Scene_Arena.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena.HUD
{
    public class ItemAtivavel_Btn : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] ArenaReferences arenaReferences;

        [Header("Item")]
        ItemAtivavel itemAtivavel;

        [Header("Button Referencias")]
        public Image circuloLimiteArrastar;
        public Image spriteSkill_Img;
        public Image barraContagem;
        public Text textCountdown;

        [Header("Button Controller")]
        public bool buttonAtivado = true;
        public bool arrastarHabilitado = false;
        public bool estaContando = false;
        public float tempoRestante;

        [Header("Cancelar Skill")]
        public Image cancelAreaImage;

        Vector3 posicaoInicial;
        float tempoPassado = 0f;
        float countDown;
        GameObject objGirar = null;
        Vector2 direcaoArrastar;
        Vector2 cancelarPosition;

        void Start()
        {
            posicaoInicial = transform.position;
        }

        void Update()
        {
            if (estaContando)
            {
                if (tempoPassado < countDown)
                {
                    barraContagem.fillAmount = tempoPassado / countDown;
                    tempoPassado += Time.deltaTime;

                    tempoRestante -= Time.deltaTime;
                    textCountdown.text = Mathf.Ceil(tempoRestante).ToString();
                }
                else
                {
                    ResetarBotao();
                    estaContando = false;
                }
            }
        }

        public void ReceberItem(int idItem)
        {
            itemAtivavel = arenaReferences.itensData.itensAtivaveis.Find(c => c.id == idItem);

            spriteSkill_Img.sprite = itemAtivavel.iconSprite;
            arrastarHabilitado = itemAtivavel.arrastar;

            countDown = itemAtivavel.tempoCowntDown;
            tempoRestante = countDown;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!buttonAtivado) return;

            if (objGirar == null)
            {
                objGirar = arenaReferences.playerReferences.skillDirectionPlayer.GetItemAtivavel(itemAtivavel.id);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!buttonAtivado) return;

            cancelAreaImage.gameObject.SetActive(true);

            if (!arrastarHabilitado) return;

            

            objGirar.SetActive(true);

            barraContagem.enabled = false;
            circuloLimiteArrastar.enabled = true;

            barraContagem.transform.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!buttonAtivado) return;

            if (EstaNaAreaDeCancelar(eventData.position))
            {
                cancelAreaImage.color = new Color(cancelAreaImage.color.r, cancelAreaImage.color.g, cancelAreaImage.color.b, 1f); // Opacidade 100%
                //skillDirection.AtualizarCor(true);
                //skillDirection.verificarPlayersInRange_Sprite.color = Color.red;
            }
            else
            {
                cancelAreaImage.color = new Color(cancelAreaImage.color.r, cancelAreaImage.color.g, cancelAreaImage.color.b, 0.5f); // Opacidade 30%
                //skillDirection.AtualizarCor(false);
                //skillDirection.verificarPlayersInRange_Sprite.color = Color.white;
            }

            if (!arrastarHabilitado) return;

            barraContagem.transform.position = eventData.position;

            // Verifica se o botão está dentro do círculo de limite de arrastar
            if (Vector3.Distance(barraContagem.transform.position, circuloLimiteArrastar.transform.position) > circuloLimiteArrastar.GetComponent<RectTransform>().sizeDelta.x)
            {
                // Se o botão estiver fora do círculo de limite de arrastar, move-o para a borda do círculo
                barraContagem.transform.position = circuloLimiteArrastar.transform.position + (barraContagem.transform.position - circuloLimiteArrastar.transform.position).normalized * circuloLimiteArrastar.GetComponent<RectTransform>().sizeDelta.x;
            }

            // Atualiza a direção em que o jogador está arrastando
            direcaoArrastar = eventData.position - (Vector2)circuloLimiteArrastar.transform.position;
            float angle = Mathf.Atan2(direcaoArrastar.y, direcaoArrastar.x) * Mathf.Rad2Deg;
            objGirar.transform.rotation = Quaternion.Euler(90, 0, angle);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            BotaoFoiSolto(eventData);
        }

        void BotaoFoiSolto(PointerEventData eventData)
        {
            if (!buttonAtivado) return;

            objGirar?.SetActive(false);

            circuloLimiteArrastar.enabled = false;
            barraContagem.transform.position = posicaoInicial;

            cancelAreaImage.gameObject.SetActive(false);
            cancelarPosition = eventData.position;
            if (!EstaNaAreaDeCancelar(cancelarPosition))
            {
                ChamarAcaoItem();
            }
        }

        void ChamarAcaoItem()
        {
            if (itemAtivavel.id == 0) //Flash
            {
                arenaReferences.playerReferences.playerItensAtivaveis.FlashDirection(direcaoArrastar);
            }
            else if (itemAtivavel.id == 1) //Curar
            {
                //playerController.itensAtivaveisScript.ChamarCurar_Rpc();
            }
            else if (itemAtivavel.id == 2) //Executar
            {
                //playerController.itensAtivaveisScript.ChamarExecutar_Rpc();
            }

            IniciarCountdown();
        }

        void IniciarCountdown()
        {
            spriteSkill_Img.color = Color.gray;
            buttonAtivado = false;
            textCountdown.gameObject.SetActive(true);
            barraContagem.enabled = true;
            estaContando = true;

            tempoPassado = 0f;
        }

        public void ResetarBotao()
        {
            estaContando = false;
            barraContagem.fillAmount = 1f;
            barraContagem.enabled = false;

            spriteSkill_Img.color = Color.white;
            textCountdown.gameObject.SetActive(false);
            estaContando = false;
            tempoRestante = countDown;
            buttonAtivado = true;
        }

        private bool EstaNaAreaDeCancelar(Vector2 position)
        {
            // Verifica se a posição está dentro da área onde a instanciação da habilidade é cancelada
            return RectTransformUtility.RectangleContainsScreenPoint(cancelAreaImage.rectTransform, position, null);
        }
    }
}

