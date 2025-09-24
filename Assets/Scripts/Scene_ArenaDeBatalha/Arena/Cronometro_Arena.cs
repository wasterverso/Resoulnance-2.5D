using PurrNet;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena.Controles
{
    public class Cronometro_Arena : NetworkBehaviour
    {
        [Header("Refer�ncias")]
        [SerializeField] Text timer_txt;

        [Header("Configura��o")]
        [SerializeField] float tempo;

        private SyncTimer timer = new();

        private void Awake()
        {
            // onTimerSecondTick � chamado a cada 1 segundo
            timer.onTimerSecondTick += OnTimerSecondTick;
        }

        protected override void OnSpawned(bool asServer)
        {
            // Isso inicia o cron�metro com uma contagem regressiva
            if (isServer)
                timer.StartTimer(tempo);
        }

        private void OnTimerSecondTick()
        {
            // Voc� tamb�m pode acessar .remaining para obter o valor exato como float
            // Para exibir o cron�metro, o remainingInt facilita
            int totalSegundos = timer.remainingInt;
            int minutos = totalSegundos / 60;
            int segundos = totalSegundos % 60;

            if (totalSegundos < 60)
                timer_txt.text = $"{segundos:00}";
            else
                timer_txt.text = $"{minutos:00}:{segundos:00}";

            // Disparar eventos nos tempos espec�ficos
            if (totalSegundos == 300)
                IniciarJogo();

            if (totalSegundos == 60)
                FaltaUmMinuto();

            if (totalSegundos == 10)
                ContagemDeDezSegundos();
        }

        public void PausarCronometro()
        {
            timer.PauseTimer(true);
        }

        public void ContinuarContagem()
        {
            timer.ResumeTimer();
        }

        void IniciarJogo()
        {

        }

        void FaltaUmMinuto()
        {

        }

        void ContagemDeDezSegundos()
        {

        }
    }
}

