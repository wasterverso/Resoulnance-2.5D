using Resoulnance.Scene_Login.Controles;
using Resoulnance.Scene_Login.Start;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class LoginUsernamePassword : MonoBehaviour
{
    [Header("Refs Script")]
    [SerializeField] AuthLogin authLogin;

    [Header("Login")]
    [SerializeField] InputField username_input;
    [SerializeField] InputField password_input;
    [SerializeField] Text erroUserlogin_txt;
    [SerializeField] Text erroSenhalogin_txt;

    [Header("Inscricao")]
    [SerializeField] InputField usernameInsc_input;
    [SerializeField] InputField passwordInsc_input;
    [SerializeField] InputField passwordInsc2_input;
    [SerializeField] Text erroUserInsc_txt;
    [SerializeField] Text erroSenhaInsc_txt;
    [SerializeField] Text erroSenhaInsc2_txt;

    [Header("Paineis")]
    [SerializeField] GameObject authPainel;
    [SerializeField] GameObject inscPainel;

    private bool mostrandoSenha = false;

    private void Start()
    {
        authPainel.SetActive(true);
        inscPainel.SetActive(false);
    }

    #region (------------------------------- Fazer Login -------------------------------)

    public async void FazerLogin()
    {
        string user = username_input.text;
        string senha = password_input.text;

        bool senhaValida = VerificarSenhaValida(senha);
        if (!senhaValida)
        {
            authLogin.MostrarAviso("Senha não esta no formato esperado ou é invalida.");
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(user, senha);
            Debug.Log("Logou com sucesso.");

            authLogin.FinalizarAuthenticacao();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);

            if (ex.Message.Contains("Username and/or Password are not in the correct format"))
            {
                authLogin.MostrarAviso("Usuário e/ou senha não esta no formato correto.");
            }
            else
            {
                authLogin.MostrarAviso(ex.Message);
            }
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes

            Debug.LogError($"Erro ao logar: {ex.Message}");

            // Se for usuário não existe ou senha errada, vai cair aqui
            if (ex.Message.Contains("Invalid username or password"))
            {
                authLogin.MostrarAviso("Usuário não existe ou senha incorreta.");
            }
            else
            {
                authLogin.MostrarAviso(ex.Message);
            }
        }
    }
    #endregion

    #region (------------------------------- Inscricao -------------------------------)
    public async void FazerInscricao()
    {
        string user = usernameInsc_input.text;
        string senha1 = passwordInsc_input.text;
        string senha2 = passwordInsc2_input.text;

        bool senhaValida = VerificarSenhaValida(senha1);
        if (!senhaValida)
        {
            authLogin.MostrarAviso("Senha não esta no formato esperado ou é invalida.");
            return;
        }

        if (senha2 != senha1)
        {
            authLogin.MostrarAviso("Senhas são diferentes.");
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(user, senha1);
            Debug.Log("Inscricao feita com sucesso.");

            authLogin.FinalizarAuthenticacao();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError(ex);

            if (ex.Message.Contains("Username and/or Password are not in the correct format"))
            {
                authLogin.MostrarAviso("Usuário e/ou senha não esta no formato correto.");
            }
            else if (ex.Message.Contains("username already exists"))
            {
                authLogin.MostrarAviso("Usuário invalido ou ja existe. Escolha outro...");
            }
            else
            {
                authLogin.MostrarAviso(ex.Message);
            }
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Erro ao se inscrever: {ex.Message}");
            authLogin.MostrarAviso(ex.Message);
        }
    }

    public void CancelarInscricao()
    {
        usernameInsc_input.text = string.Empty;
        passwordInsc_input.text = string.Empty;
        passwordInsc2_input.text = string.Empty;

        erroUserInsc_txt.text = string.Empty;
        erroSenhaInsc_txt.text = string.Empty;
        erroSenhaInsc2_txt.text = string.Empty;

        inscPainel.SetActive(false);
        authPainel.SetActive(true);
    }
    #endregion

    #region (------------------------------- Verificacoes -------------------------------)

    public void User_ForcarMinuscula()
    {
        int pos = username_input.caretPosition;
        username_input.text = username_input.text.ToLower();
        username_input.caretPosition = pos;

        int posOther = usernameInsc_input.caretPosition;
        usernameInsc_input.text = usernameInsc_input.text.ToLower();
        usernameInsc_input.caretPosition = posOther;
    }

    public void VerificarUser_Login()
    {
        string username = username_input.text;
        string pattern = @"^[A-Za-z0-9.\-@_]{5,20}$";

        if (!Regex.IsMatch(username, pattern))
        {
            erroUserlogin_txt.text = "<color=red>Entre 5 e 20 caracteres, e somente: letras, números, ., -, @ e _</color>";
        }
    }

    public void VerificarUser_inscricao()
    {
        string username = usernameInsc_input.text;
        string pattern = @"^[A-Za-z0-9.\-@_]{5,20}$";

        if (!Regex.IsMatch(username, pattern))
        {
            erroUserInsc_txt.text = "<color=red>Entre 5 e 20 caracteres, e somente: letras, números, ., -, @ e _</color>";
        }
    }

    public void AlternarVisualizacaoSenha()
    {
        mostrandoSenha = !mostrandoSenha;

        password_input.contentType = mostrandoSenha ? InputField.ContentType.Standard : InputField.ContentType.Password;
        password_input.ForceLabelUpdate();

        passwordInsc_input.contentType = mostrandoSenha ? InputField.ContentType.Standard : InputField.ContentType.Password;
        passwordInsc_input.ForceLabelUpdate();

        passwordInsc2_input.contentType = mostrandoSenha ? InputField.ContentType.Standard : InputField.ContentType.Password;
        passwordInsc2_input.ForceLabelUpdate();
    }

    public void VerificarSenha_Login()
    {
        SenhaValida(password_input, erroSenhalogin_txt);
    }

    public void VerificarSenha_Inscricao()
    {
        SenhaValida(passwordInsc_input, erroSenhaInsc_txt);
    }

    public void VerificarRepSenha_Inscricao()
    {
        string senha1 = passwordInsc_input.text;
        string senha2 = passwordInsc2_input.text;

        if (senha1 == senha2)
        {
            erroSenhaInsc2_txt.text = "";
        }
        else
        {
            erroSenhaInsc2_txt.text = "<color=red>Senhas são diferentes</color>";
        }
    }

    public void SenhaValida(InputField input, Text aviso)
    {
        string senha = input.text;

        bool temMinuscula = Regex.IsMatch(senha, "[a-z]");
        bool temMaiuscula = Regex.IsMatch(senha, "[A-Z]");
        bool temNumero = Regex.IsMatch(senha, "[0-9]");
        bool temSimbolo = Regex.IsMatch(senha, "[^a-zA-Z0-9]");
        bool tamanhoOk = senha.Length >= 8;

        aviso.text =
            (tamanhoOk ? "<color=green>Min 8</color>" : "<color=red>Min 8</color>") + " || " +
            (temMaiuscula ? "<color=green>1 Maiuscula</color>" : "<color=red>1 Maiuscula</color>") + " || " +
            (temMinuscula ? "<color=green>1 Minuscula</color>" : "<color=red>1 Minuscula</color>") + " \n " +
            (temNumero ? "<color=green>1 Numero</color>" : "<color=red>1 Numero</color>") + " || " +
            (temSimbolo ? "<color=green>1 Simbolo</color>" : "<color=red>1 Simbolo</color>");

        // opcional: se quiser mostrar "Senha válida " só quando tudo estiver certo
        if (tamanhoOk && temMaiuscula && temMinuscula && temNumero && temSimbolo)
        {
            aviso.text = "<color=green><b>SENHA VÁLIDA</b></color>";
        }
    }

    bool VerificarSenhaValida(string senha)
    {
        return senha.Length >= 8 &&
                Regex.IsMatch(senha, "[a-z]") &&
                Regex.IsMatch(senha, "[A-Z]") &&
                Regex.IsMatch(senha, "[0-9]") &&
                Regex.IsMatch(senha, "[^a-zA-Z0-9]");
    }
    #endregion
}
