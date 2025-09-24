using UnityEngine;
using UnityEngine.UI;

public class DamageNotification : MonoBehaviour
{
    [SerializeField] Text valor_txt;
    public void StartarScript(int dano, int tipo)
    {
        switch (tipo)
        {
            case 1: // Normal
                valor_txt.color = Color.red;
                valor_txt.text = dano.ToString();
                break;

            case 2: // Real
                valor_txt.color = Color.white;
                valor_txt.text = dano.ToString();
                break;

            case 3: // Cristal
                valor_txt.color = Color.blue;
                valor_txt.text = "- " + dano.ToString();

                valor_txt.transform.localScale = dano < 3 ? new Vector3(0.015f, 0.015f, 0.015f) : new Vector3(0.03f, 0.03f, 0.03f);
                break;

            case 4: // Cura
                valor_txt.color = Color.green;
                valor_txt.text = "+ " + dano.ToString();
                break;

            case 5: // Escudo
                valor_txt.color = Color.yellow;
                valor_txt.text = "+ " + dano.ToString();
                break;

            case 6: // Azul Claro
                valor_txt.color = Color.cyan;
                valor_txt.text = "MAX";
                break;

            default:
                Debug.LogWarning($"Tipo de dano desconhecido: {tipo}");
                valor_txt.color = Color.gray; // Cor padrão para tipos desconhecidos
                valor_txt.text = dano.ToString();
                break;
        }

        Invoke("DevolverPool", 2f);
    }

    void DevolverPool()
    {
        PoolManager.Instance.Despawn("Damage", this.gameObject);
    }
}
