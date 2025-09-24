using PurrNet;
using PurrNet.Packing;
using PurrNet.Pooling;
using PurrNet.Prediction;
using Resoulnance.Scene_Arena.Cristal;
using Resoulnance.Scene_Arena.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListCristaisController : PredictedIdentity<ListCristaisController.State>
{
    public static ListCristaisController Instance;

    public struct State : IPredictedData<State>
    {
        public DisposableList<CristalInfo> cristaisList;

        public void Dispose() { }
    }

    public DisposableList<CristalInfo> cristaisList;

    [Header("Lista de Cristais GO")]
    [SerializeField] GameObject[] cristaisObj;

    bool iniciou = false;

    private CristalInfo[] _lastCache;
    public event Action<CristalInfo> OnListChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    protected override void LateAwake()
    {
        OnListChanged += AtualizarCristais;
    }

    protected override State GetInitialState()
    {
        var state = new State
        {
            cristaisList = DisposableList<CristalInfo>.Create()
        };
        return state;
    }

    private void AtualizarCristais(CristalInfo change)
    {
        foreach (var crisObj in cristaisObj)
        {
            Cristal criScript = crisObj.GetComponent<Cristal>();
            int idCristal = criScript.idCristal;

            if (idCristal == change.id)
            {
                crisObj.SetActive(change.estaAtivado);
                break;
            }
        }
    }

    public void PegouCristal(int idCristal, ulong idPlayer, Team timePlayer)
    {
        var lista = currentState.cristaisList;
        for (int i = 0; i < lista.Count; i++)
        {
            if (lista[i].id == idCristal)
            {
                var cristal = lista[i];
                cristal.idPlayer = idPlayer;
                cristal.team = timePlayer;
                cristal.estaAtivado = false;
                cristal.estaNoGuardiao = false;

                lista[i] = cristal;
                break;
            }
        }
    }

    public void PlayerMorreu(ulong idPlayer)
    {
        var lista = currentState.cristaisList;
        for (int i = 0; i < lista.Count; i++)
        {
            if (lista[i].idPlayer == idPlayer)
            {
                var cristal = lista[i];
                cristal.idPlayer = default;
                cristal.team = Team.Nenhum;
                cristal.estaAtivado = true;
                cristal.estaNoGuardiao = false;

                lista[i] = cristal;
            }
        }
    }

    public void RetirarCristaisDoGuardiao(List<int> cristaisLista)
    {
        var idsSet = new HashSet<int>(cristaisLista);
        var lista = currentState.cristaisList;

        int encontrados = 0;
        int totalParaRetirar = idsSet.Count;

        for (int i = 0; i < lista.Count; i++)
        {
            if (idsSet.Contains(lista[i].id))
            {
                var cristal = lista[i];
                cristal.idPlayer = default;
                cristal.team = Team.Nenhum;
                cristal.estaAtivado = true;
                cristal.estaNoGuardiao = false;

                lista[i] = cristal;

                encontrados++;

                if (encontrados >= totalParaRetirar) break;
            }
        }
    }

    public void PlayerGuardouCristal(ulong playerId, Team team)
    {
        for (int i = 0; i < currentState.cristaisList.Count; i++)
        {
            if (currentState.cristaisList[i].idPlayer == playerId)
            {
                CristalInfo cristal = currentState.cristaisList[i];

                // modifica
                cristal.idPlayer = 0;
                cristal.team = team;
                cristal.estaAtivado = false;
                cristal.estaNoGuardiao = true;

                // devolve na lista
                currentState.cristaisList[i] = cristal;
                return; // sai, já que guardou
            }
        }
    }

    protected override void Simulate(ref State state, float delta)
    {
        CriarLista(state);

        DetectarMudancas(state); ;
    }

    void CriarLista(State state)
    {
        if (!iniciou && isServer)
        {
            iniciou = true;
            foreach (var cristal in cristaisObj)
            {
                Cristal criScript = cristal.GetComponent<Cristal>();

                CristalInfo newCris = new CristalInfo();
                newCris.id = criScript.idCristal;
                newCris.team = Team.Nenhum;
                newCris.estaAtivado = true;
                newCris.estaNoGuardiao = false;

                state.cristaisList.Add(newCris);
            }
        }
    }

    private void DetectarMudancas(State state)
    {
        if (_lastCache == null || _lastCache.Length != state.cristaisList.Count)
        {
            // força atualização inicial
            _lastCache = state.cristaisList.ToArray();
            return;
        }

        for (int i = 0; i < state.cristaisList.Count; i++)
        {
            var atual = state.cristaisList[i];
            var antigo = _lastCache[i];

            if (!Equals(atual, antigo))
            {
                // dispara evento de mudança
                OnListChanged?.Invoke(atual);

                // atualiza cache
                _lastCache[i] = atual;
            }
        }
    }

    public bool TemCristaisNaMao(ulong meuId)
    {
        foreach (var cris in currentState.cristaisList)
        {
            if (cris.idPlayer == meuId)
            {
                return true;
            }
        }
        return false;
    }

    public void Dispose()
    {
        cristaisList.Dispose();
        OnListChanged -= AtualizarCristais;
    }
}

[Serializable]
public struct CristalInfo : IPackedAuto
{
    public int id;
    public Team team;
    public ulong idPlayer;
    public bool estaAtivado;
    public bool estaNoGuardiao;

    public void Write(BitPacker packer)
    {
        Packer<int>.Write(packer, id);
        Packer<Team>.Write(packer, team);
        Packer<ulong>.Write(packer, idPlayer);
        Packer<bool>.Write(packer, estaAtivado);
        Packer<bool>.Write(packer, estaNoGuardiao);
    }

    public void Read(BitPacker packer)
    {
        Packer<int>.Read(packer, ref id);
        Packer<Team>.Read(packer, ref team);
        Packer<ulong>.Read(packer, ref idPlayer);
        Packer<bool>.Read(packer, ref estaAtivado);
        Packer<bool>.Read(packer, ref estaNoGuardiao);
    }
}