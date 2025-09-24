using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FPS_Display : MonoBehaviour
{
    [Header("Exibição")]
    [SerializeField] private Text _fpsText;
    [SerializeField] private float _updateInterval = 1f;

    [Header("Configurações de FPS na Inicialização")]
    [SerializeField][Range(30, 240)] private int _fpsAlvoInicial = 60;

    // Variáveis para o cálculo do contador de FPS
    private float _accum = 0.0f;
    private int _frames = 0;
    private float _timeLeft;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.LinuxServer)
        {
            Application.targetFrameRate = 30;
        }
        else
        {
            DefinirFpsAlvo(_fpsAlvoInicial);
        }

        _timeLeft = _updateInterval;
    }

    public void DefinirFpsAlvo(int valor)
    {
        // Para definir um FPS alvo, o VSync precisa ser desativado.
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = valor;
    }

    public void AtivarVSync(bool ativo)
    {
        if (ativo)
        {
            // Ativa o VSync. O valor 1 sincroniza com a taxa de atualização do monitor.
            QualitySettings.vSyncCount = 1;
            // Quando o VSync está ativo, o targetFrameRate deve ser -1 (padrão) para não haver conflito.
            Application.targetFrameRate = -1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            Debug.Log("VSync Desativado.");
        }
    }

    private void Update()
    {
        // Se o texto não foi atribuído, não faz nada
        if (_fpsText == null) return;

        _timeLeft -= Time.deltaTime;
        _accum += Time.timeScale / Time.deltaTime;
        ++_frames;

        if (_timeLeft <= 0.0)
        {
            float fps = _accum / _frames;
            _fpsText.text = $"{Mathf.RoundToInt(fps)} FPS";

            // Reseta para o próximo intervalo
            _timeLeft = _updateInterval;
            _accum = 0.0f;
            _frames = 0;
        }
    }
}
