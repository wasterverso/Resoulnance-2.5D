using PurrNet.Packing;
using PurrNet.Prediction;
using Resoulnance.Cartas;
using Resoulnance.Scene_Arena;
using Resoulnance.Scene_Arena.Player;
using System;
using System.Linq;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class PlayerSkills : PredictedIdentity<PlayerSkills.Input, PlayerSkills.State>
{
    public struct State : IPredictedData<State>
    {
        public float tempoCountdown;

        public void Dispose() { }
    }

    public struct Input : IPredictedData<Input>
    {
        public bool chamouSkill;

        public Vector2 direction;
        public Vector3 position;
        public float countdown;
        public int idCarta;

        public void Dispose() { }

        public override string ToString()
        {
            string result = $"IdCarta: {idCarta}";
            result += $"\n countdown: {countdown}";

            return result;
        }
    }

    [Header("Refs Script")]
    [SerializeField] PlayerReferences playerReferences;
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerHits playerHits;

    [Header("Prefabs")]
    [SerializeField] Transform skillTransform;

    [Header("Prefabs")]
    [SerializeField] int idSkill;

    private PlayerInput playerInput = new PlayerInput();

    PredictedEvent<EfeitoInterno> OnEfeitoVisual;

    protected override void LateAwake()
    {
        OnEfeitoVisual = new PredictedEvent<EfeitoInterno>(predictionManager, this);
        OnEfeitoVisual.AddListener(data =>
        {
            ChamarEfeitoVisual(data.cartaId, data.valor1, data.valor2, data.playerId);
        });
    }

    public void SetAtivarSkill(int idCart, float countdown, Vector2 direcao, Vector3 position)
    {
        var playerSkillInput = new PlayerInput();
        playerSkillInput.chamouSkill = true;
        playerSkillInput.position = position;
        playerSkillInput.direction = direcao;
        playerSkillInput.countdown = countdown;
        playerSkillInput.idCarta = idCart;

        playerInput = playerSkillInput;
    }

    protected override void UpdateInput(ref Input input)
    {
        if (playerInput.chamouSkill)
        {
            playerInput.chamouSkill = false;

            input.chamouSkill = true;
            input.position = playerInput.position;
            input.direction = playerInput.direction;
            input.countdown = playerInput.countdown;
            input.idCarta = playerInput.idCarta;
        }
    }

    protected override void Simulate(Input input, ref State state, float delta)
    {
        if (state.tempoCountdown > 0)
        {
            state.tempoCountdown -= delta;
        }

        if (input.chamouSkill)
        {
            input.chamouSkill = false;
            state.tempoCountdown = input.countdown;

            InstaciarSkill(input, state);
            playerReferences.playerAtk.PlayerUsouSkill();
        }
    }

    void InstaciarSkill(Input input, State state)
    {
        Carta carta = ArenaReferences.Instance.cartasData.cartas.Find(c => c.id == input.idCarta);

        if (carta != default)
        {
            if(carta.in_Mapa)
            {
                if (carta.arrastar)
                {
                    GameObject cartaObj = carta.prefab;
                    var createObject = predictionManager.hierarchy.Create(cartaObj, input.position, Quaternion.identity);
                    if (!createObject.HasValue) return;

                    if (!createObject.Value.TryGetComponent(predictionManager, out SkillControlePrefab skillControle)) return;

                    skillControle.SetInfoSkill(playerReferences, carta.valorAtributo1, 
                        carta.valorAtributo2, input.direction, playerReferences.currentState.team, owner.Value.id.value);
                }
            }            
            else if (carta.In_Player)
            {
                var efeitoInterno = new EfeitoInterno(input.idCarta, (int)carta.valorAtributo1, (int)carta.valorAtributo2, owner.Value.id.value);
                OnEfeitoVisual.Invoke(efeitoInterno);
            }
        }
        else
        {
            Debug.LogWarning($"Carta id: {input.idCarta} não encontrada. Nome: {carta?.NomeCarta}");
        }
    }

    void ChamarEfeitoVisual(int cartaID, int valor_1, int valor_2, ulong playerID)
    {
        Carta carta = ArenaReferences.Instance.cartasData.cartas.Find(c => c.id == cartaID);

        if (carta != null)
        {
            PredictedObjectID? createObject = predictionManager.hierarchy.Create(carta.prefab);
            if (!createObject.HasValue) return;

            if (!createObject.Value.TryGetComponent(predictionManager, out SkillControlePrefab skillControle)) return;

            skillControle.SetInfoSkill(playerReferences, valor_1, valor_2);

            playerHits.InstanciarHitsCartas(cartaID);
        }
        else
        {
            Debug.LogError($"[PlayerSkill{idSkill}] Carta nao encontrada");
        }
    }

    

    struct EfeitoInterno
    {
        public int cartaId;
        public int valor1;
        public int valor2;
        public ulong playerId;

        public EfeitoInterno(int carta, int valor_1, int valor_2, ulong playerID)
        {
            this.cartaId = carta;
            this.valor1 = valor_1;
            this.valor2 = valor_2;
            this.playerId = playerID;
        }
    }
}

public struct PlayerInput : IPackedAuto
{
    public bool chamouSkill;
    public Vector2 direction;
    public Vector3 position;
    public float countdown;
    public int idCarta;

    public void Write(BitPacker packer)
    {
        Packer<bool>.Write(packer, chamouSkill);
        Packer<Vector2>.Write(packer, direction);
        Packer<Vector3>.Write(packer, position);
        Packer<float>.Write(packer, countdown);
        Packer<int>.Write(packer, idCarta);
    }

    public void Read(BitPacker packer)
    {
        Packer<bool>.Read(packer, ref chamouSkill);
        Packer<Vector2>.Read(packer, ref direction);
        Packer<Vector3>.Read(packer, ref position);
        Packer<float>.Read(packer, ref countdown);
        Packer<int>.Read(packer, ref idCarta);
    }

    public override string ToString()
    {
        return $"Skill: {chamouSkill}, idCarta: {idCarta}, countdown: {countdown}, dir: {direction}, pos: {position}";
    }
}

