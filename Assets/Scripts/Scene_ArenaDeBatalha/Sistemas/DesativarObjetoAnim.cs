using UnityEngine;

public class DesativarObjetoAnim : MonoBehaviour
{
    [SerializeField] GameObject supremaAnim_Obj;

    public void DesativarAnimacaoSuprema()
    {
        supremaAnim_Obj.SetActive(false);
    }
}
