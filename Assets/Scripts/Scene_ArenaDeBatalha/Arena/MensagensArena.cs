using PurrNet;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MensagensArena : NetworkBehaviour
{
    [SerializeField] Text mensagem_txt;
    [SerializeField] InputField mensagem_input;
    [SerializeField] Transform gridMensagens;
    [SerializeField] GameObject mensagensPrefab;

    Team team = Team.Nenhum;

    Coroutine apagarMensagemCoroutine;

    public void EnviarMensagem()
    {
        string texto = mensagem_input.text.Trim();
        if (string.IsNullOrEmpty(texto)) return;

        mensagem_input.text = "";

        ReceberMensagem(texto, team);
    }

    [ObserversRpc]
    void ReceberMensagem(string msg, Team idTeam)
    {
        mensagem_txt.text = msg;
        GameObject newObj = Instantiate(mensagensPrefab, gridMensagens);
        Text txt = newObj.GetComponentInChildren<Text>();
        txt.text = msg;

        if (apagarMensagemCoroutine != null)
            StopCoroutine(apagarMensagemCoroutine);

        // Inicia/reset o timer
        apagarMensagemCoroutine = StartCoroutine(ApagarMensagemDepoisDeSegundos(10f));
    }

    IEnumerator ApagarMensagemDepoisDeSegundos(float tempo)
    {
        yield return new WaitForSeconds(tempo);

        mensagem_txt.text = "";
    }
}
