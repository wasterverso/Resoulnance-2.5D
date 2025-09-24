using UnityEngine;

public class SkillDirectionPlayer : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] SpriteRenderer rangeAtkBasico_img;
    public SpriteRenderer rangeSkillDiection_img;
    [SerializeField] GameObject _iconMiniMap;

    [Header("Sprites")]
    [SerializeField] Sprite rangeSprite;
    [SerializeField] Sprite directionSprite;

    [Header("Circle range")]
    [SerializeField] Transform circlePosition;
    [SerializeField] bool mostrarCircle = false;
    [SerializeField] float range;

    [Header("Sprites Itens")]
    [SerializeField] GameObject flashDirection;
    [SerializeField] GameObject executarDirection;

    private void Start()
    {
        _iconMiniMap.SetActive(true);
    }

    public void AtivarRangeAtkBasico(bool ativar)
    {
        rangeAtkBasico_img.gameObject.SetActive(ativar);
    }

    public void MudarRangeAtkBasico(bool isDirection)
    {
        if (isDirection)
        {
            rangeAtkBasico_img.sprite = directionSprite;
        }
        else
        {
            rangeAtkBasico_img.sprite = rangeSprite;
        }
    }

    public void AtualizarDirecaoAtkBasico(float angle)
    {
        rangeAtkBasico_img.gameObject.transform.rotation = Quaternion.Euler(new Vector3(90, 0, angle)); ;
    }

    public void AtualizarDirecaoSkill(float angle)
    {
        rangeSkillDiection_img.gameObject.transform.rotation = Quaternion.Euler(new Vector3(90, 0, angle)); ;
    }

    public GameObject GetItemAtivavel(int idItem)
    {
        if (idItem == 0) //Flash
        {
            return flashDirection;
        }
        else if (idItem == 1) //Curar
        {
            return null;
        }
        else if (idItem == 2) //Executar
        {
            return executarDirection;
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        if (mostrarCircle)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(circlePosition.position, range);
        }
    }
}
