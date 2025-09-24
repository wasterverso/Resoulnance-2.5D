using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class UnityAuthentication : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log(UnityServices.State);

        ConfigurarEventos();

        await EntrarAnonimamenteAsync();
    }

    // Configura os manipuladores de eventos de autenticação, se desejado
    void ConfigurarEventos()
    {
        AuthenticationService.Instance.SignedIn += () => {
            //Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Mostra como obter um token de acesso
            Debug.Log($"Token de Acesso: {AuthenticationService.Instance.AccessToken}");
        };

        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Jogador desconectado.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("A sessão do jogador não pôde ser renovada e expirou.");
        };
    }

    async Task EntrarAnonimamenteAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Login anônimo realizado com sucesso");
            Debug.Log($"PlayerId: {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}
