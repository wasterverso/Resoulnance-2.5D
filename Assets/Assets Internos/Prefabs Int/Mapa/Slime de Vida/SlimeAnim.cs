using System.Collections;
using UnityEngine;

public class SlimeAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject slimeObject;

    void Start()
    {
        StartCoroutine(SlimeAnimCorroutine());
    }

    IEnumerator SlimeAnimCorroutine()
    {
        while (true)
        {
            // Escolhe um tempo aleatório entre 3 e 7 segundos
            float tempoAleatorio = Random.Range(3f, 7f);
            yield return new WaitForSeconds(tempoAleatorio);

            if (slimeObject.activeSelf)
            {
                anim.Play("Slime_Jump");
            }
        }
    }
}
