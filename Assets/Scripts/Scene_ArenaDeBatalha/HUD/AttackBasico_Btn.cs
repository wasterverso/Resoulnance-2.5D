using Resoulnance.Scene_Arena;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackBasico_Btn : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler
{
    [Header("Ref Script")]
    ArenaReferences arenaReferences;

    [Header("Controle")]
    public bool botaoAtivado = true;

    [Header("Controle Int")]
    Vector2 direction;
    bool arrastou = false;
    float angle = 0f;

    private void Start()
    {
        arenaReferences = ArenaReferences.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        direction = Vector2.zero;
        arrastou = false;
        angle = 0f;

        arenaReferences.playerReferences.skillDirectionPlayer.MudarRangeAtkBasico(false);
        arenaReferences.playerReferences.skillDirectionPlayer.AtivarRangeAtkBasico(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        arrastou = true;
        arenaReferences.playerReferences.skillDirectionPlayer.MudarRangeAtkBasico(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!botaoAtivado) return;

        direction = eventData.position - (Vector2)this.transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arenaReferences.playerReferences.skillDirectionPlayer.AtualizarDirecaoAtkBasico(angle);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!botaoAtivado) return;

        arenaReferences.playerReferences.skillDirectionPlayer.AtivarRangeAtkBasico(false);
        arenaReferences.playerReferences.playerAtk.AtkBasico(arrastou, direction);        
    }
}
