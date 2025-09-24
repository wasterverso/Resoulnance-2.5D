public enum Team : byte { Nenhum = 0, Blue = 1, Red = 2, AutoAssign = byte.MaxValue }

public enum PlayerState { Nenhum, Idle, Move, Dash, Stun, Atk, Depositar, Dead }

public enum Mundo { Host, Server, Client }

public enum Atributo { Nenhum, Dano, DanoReal, Defesa, Cura, Movimento, Controle }

public enum Classes { Nenhum, Coletor, Guerreiro, Tanque, Suporte }

public enum Tipagem { Nenhum, Magia, Ataque, Defesa, Teletransporte, Movimento, Cura, Arma }

public enum ElosRanking { E, D, C, B, A, S, SS, RANKER }

public enum ProcurarAlvo { MaisPerto, MenorEscudo, MenorVida }

public enum TipoAlvoFinal { Nenhum, EuMesmo, Aliados, Adversarios }

public enum CartaAtivaPassiva { Ativa, Passiva }

public enum TiposDeSalas { Nenhum, Ranked, Solo, Normal1v1, Normal2v2, Normal4v4, Personalizada, Treinamento }

public enum NetworkMode { Nenhum, Server, Host, Cliente}