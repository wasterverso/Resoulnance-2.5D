using PurrNet.Prediction;
using UnityEngine;
using UnityEngine.AI;

namespace Resoulnance.Scene_Arena.Player
{
    public class PlayerItensAtivaveis : PredictedIdentity<PlayerItensAtivaveis.Input, PlayerItensAtivaveis.State>
    {
        public struct State : IPredictedData<State>
        {

            public void Dispose() { }
        }

        public struct Input : IPredictedData<Input>
        {
            public bool chamouFlash;
            public Vector2 direcaoFlash;

            public void Dispose() { }
        }

        float distanceFlash = 3f;
        float maxNavMeshDistance = 4f; // raio de tolerância

        bool chamouFlash = false;
        Vector2 direcaoFlash;

        public void FlashDirection(Vector2 direcao)
        {
            if (direcao == Vector2.zero) return;

            chamouFlash = true;
            direcaoFlash = direcao;
        }

        protected override void UpdateInput(ref Input input)
        {
            if (chamouFlash)
            {
                chamouFlash = false;
                input.chamouFlash = true;
                input.direcaoFlash = direcaoFlash;
            }
        }

        protected override void Simulate(Input input, ref State state, float delta)
        {
            if (input.chamouFlash)
            {
                input.chamouFlash = false;

                Vector3 moveDir = new Vector3(input.direcaoFlash.x, 0, input.direcaoFlash.y).normalized;
                Vector3 destinoTentado = transform.position + moveDir * distanceFlash;

                // Verifica se destino está no NavMesh
                if (NavMesh.SamplePosition(destinoTentado, out NavMeshHit hit, maxNavMeshDistance, NavMesh.AllAreas))
                {
                    Vector3 destinoValido = hit.position;

                    // Mantém o Y atual do jogador
                    destinoValido.y = transform.position.y;

                    // Teleporta
                    transform.position = destinoValido;
                }
                else
                {
                    Debug.Log("Destino inválido (fora do NavMesh)");
                }
            }
        }
    }
}

