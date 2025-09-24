using Resoulnance.Cartas;
using Resoulnance.Flyers;
using Resoulnance.Scene_Arena;
using Resoulnance.Scene_Arena.Player;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Suprema_Btn : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler
{
    ArenaReferences arenaReferences;

    [Header("Player/Flyer")]
    [SerializeField] Team team;
    Personagem_Data flyer;

    [Header("Button Referencias")]
    public Image rangeArrastar_img;
    public Image BarraContagem_img;
    public Image SpriteSkill_Img;
    public Image fundoRotatorio_img;
    public Text textCountdown;

    [Header("Button Controller")]
    public bool ButtonAtivado = true;
    public bool arrastarHabilitado = false;
    public float TempoRestante;

    [Header("Cancelar Skill")]
    public Image cancelAreaImage;

    float rotationSpeed = 80f;
    float tempoPassado = 0f;
    bool estaContando = false;
    bool estaArrastando = false;
    float rangeLimite = 110f;
    float CountDown;

    Vector3 posicaoInicial;
    Vector2 direcaoDoArrasto;
    Vector2 cancelarPosition;

    private void Start()
    {
        arenaReferences = ArenaReferences.Instance;

        posicaoInicial = transform.position;
        cancelAreaImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (estaContando)
        {
            if (tempoPassado < CountDown)
            {
                BarraContagem_img.fillAmount = tempoPassado / CountDown;
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
        else
        {
            fundoRotatorio_img.rectTransform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
        }
    }

    public void ReceberFlyer(Personagem_Data flyerData)
    {
        flyer = flyerData;

        SpriteSkill_Img.sprite = flyer.supremaIcon;
        CountDown = flyer.recargaSuprema;
        TempoRestante = CountDown;
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

        estaArrastando = true;

        if (!flyer.arrastarSuprema) return;

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
            //skillDirection.AtualizarCor(true);
            //playerController.verificarPlayersInRange.spriteCircle.color = Color.red;
        }
        else
        {
            cancelAreaImage.color = new Color(cancelAreaImage.color.r, cancelAreaImage.color.g, cancelAreaImage.color.b, 0.5f); // Opacidade 30%
            //skillDirection.AtualizarCor(false);
            //playerController.verificarPlayersInRange.spriteCircle.color = Color.white;
        }

        if (!flyer.arrastarSuprema) return;

        BarraContagem_img.transform.position = eventData.position;

        Vector3 centro = rangeArrastar_img.transform.position;
        Vector3 posAtual = BarraContagem_img.transform.position;

        float distancia = Vector3.Distance(posAtual, centro);

        if (distancia > rangeLimite)
        {
            Vector3 direcao = (posAtual - centro).normalized;
            BarraContagem_img.transform.position = centro + direcao * rangeLimite;
        }

        direcaoDoArrasto = eventData.position - (Vector2)rangeArrastar_img.transform.position;
        float angle = Mathf.Atan2(direcaoDoArrasto.y, direcaoDoArrasto.x) * Mathf.Rad2Deg;

        //arenaReferences.playerReferences.skillDirectionPlayer.AtualizarDirecaoSkill(angle);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!ButtonAtivado) return;

        BotaoFoiSolto(eventData);
    }

    void BotaoFoiSolto(PointerEventData eventData)
    {
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
        bool esquerda = direcaoDoArrasto.x < 0;
        arenaReferences.playerReferences.playerAtk.ChamarSuprema(estaArrastando, direcaoDoArrasto);

        IniciarCountdown();
    }

    void IniciarCountdown()
    {
        if (estaArrastando && arrastarHabilitado)
        {
            estaArrastando = false;
            //arenaReferences.playerReferences.skillDirectionPlayer.rangeSkillDiection_img.gameObject.SetActive(false);
        }

        SpriteSkill_Img.color = Color.gray;
        ButtonAtivado = false;
        textCountdown.gameObject.SetActive(true);
        estaContando = true;

        tempoPassado = 0f;
        fundoRotatorio_img.gameObject.SetActive(false);
    }

    private void ResetarBotao()
    {
        estaContando = false;
        BarraContagem_img.fillAmount = 1f;

        SpriteSkill_Img.color = Color.white;
        textCountdown.gameObject.SetActive(false);
        estaContando = false;
        BarraContagem_img.enabled = true;
        TempoRestante = flyer.recargaSuprema;
        ButtonAtivado = true;

        fundoRotatorio_img.gameObject.SetActive(true);
    }

    private bool EstaNaAreaDeCancelar(Vector2 position)
    {
        // Verifica se a posição está dentro da área onde a instanciação da habilidade é cancelada
        return RectTransformUtility.RectangleContainsScreenPoint(cancelAreaImage.rectTransform, position, null);
    }
}
