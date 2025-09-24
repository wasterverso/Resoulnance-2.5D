using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class LoginUsernamePassword : MonoBehaviour
{

    [SerializeField] InputField username_input;
    [SerializeField] InputField password_input;

    public async void FazerInscricao()
    {
        await Inscrever();
    }

    public async Task Inscrever()
    {
        string user = username_input.text;
        string senha = password_input.text;

        bool valida = SenhaValida(senha);

        if (!valida) return;

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(user, senha);
            Debug.Log("Login com senha concluido");
        }
        catch (AuthenticationException exception)
        {
            Debug.LogError(exception.Message);
        }
        catch (RequestFailedException exception)
        {
            Debug.LogError(exception.Message);
        }
    }

    public async void FazerLogin()
    {
        await EntrarComUserSenha();
    }

    public async Task EntrarComUserSenha()
    {
        string user = username_input.text;
        string senha = password_input.text;

        //bool valida = SenhaValida(senha);
        //Debug.Log($"Senha valida: {valida}");
        //if (!valida) return;

        try
        {
            await AuthenticationService.Instance.AddUsernamePasswordAsync(user, senha);
            Debug.Log("Login com senha concluido");
        }
        catch (AuthenticationException exception)
        {
            Debug.LogError(exception.Message);
        }
        catch (RequestFailedException exception)
        {
            Debug.LogError(exception.Message);
        }
    }
        
    bool SenhaValida(string senha)
    {
        if (senha.Length < 8 || senha.Length > 30) return false;

        bool hasUppercase = false;
        bool hasLowercase = false;
        bool hasDigit = false;
        bool hasSimbol = false;

        foreach (char c in senha)
        {
            if (char.IsUpper(c))
                hasUppercase = true;
            else if (char.IsLower(c))
                hasLowercase = true;
            else if (char.IsDigit(c))
                hasDigit = true;
            else if (char.IsLetterOrDigit(c))
                hasSimbol = true;
        }

        return hasUppercase && hasLowercase && hasDigit && hasSimbol;
    }
}
