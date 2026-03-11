# Guitar Hero 3D — Godot 4 + C#

Jogo de ritmo estilo Guitar Hero construído do zero com Godot 4.6 e C#. Suporta charts no formato Clone Hero (`.chart`), hold notes, seleção de dificuldade, controle gamepad e teclado simultâneos.

---

## Requisitos

- Godot 4.6 (com suporte a C# / .NET)
- .NET 8 SDK

> Os arquivos de áudio (`.ogg`, `.mp3`) e charts (`.chart`) **não estão incluídos** no repositório. Adicione-os na pasta `Audio/` localmente.

---

## Estrutura do Projeto

```
res://
├── Scripts/
│   ├── GameManager.cs       ← Controlador principal (spawn, score, HUD, pause)
│   ├── Lane.cs              ← Lógica de pista (input, visuals, hold tracking)
│   ├── Note.cs              ← Física e visual da nota (tap e hold)
│   ├── SongChart.cs         ← Estrutura de dados + geração procedural
│   ├── ChartImporter.cs     ← Parser de arquivos .chart (Clone Hero)
│   ├── GameData.cs          ← Dados estáticos entre cenas
│   ├── LoadingScreen.cs     ← State machine de carregamento
│   ├── SongSelectMenu.cs    ← Seleção de música (scan da pasta Audio/)
│   ├── DifficultySelect.cs  ← Seleção de dificuldade
│   ├── MainMenu.cs          ← Menu principal
│   └── ResultsScreen.cs     ← Tela de resultado
├── Scenes/
│   ├── MainMenu.tscn
│   ├── SongSelect.tscn
│   ├── DifficultySelect.tscn
│   ├── Loading.tscn
│   ├── Game.tscn
│   └── Results.tscn
├── Audio/               ← Coloque seus .ogg/.mp3 e .chart aqui (ignorados pelo git)
└── project.godot
```

---

## Fluxo do Jogo

```
MainMenu → SongSelect → [DifficultySelect] → Loading → Game → Results
```

---

## Controles

### Teclado

| Tecla | Lane | Cor      |
|-------|------|----------|
| A     | 0    | Verde    |
| S     | 1    | Vermelho |
| J     | 2    | Amarelo  |
| K     | 3    | Azul     |
| L     | 4    | Laranja  |
| ESC   | —    | Pause    |

### Gamepad (Switch Pro / Xbox)

| Botão       | Lane | Cor      |
|-------------|------|----------|
| ZL / LT     | 0    | Verde    |
| L / LB      | 1    | Vermelho |
| R / RB      | 2    | Amarelo  |
| ZR / RT     | 3    | Azul     |
| X (topo)    | 4    | Laranja  |
| Start / +   | —    | Pause    |

Teclado e gamepad funcionam simultaneamente.

---

## Pontuação

| Timing  | Janela   | Pontos base |
|---------|----------|-------------|
| PERFECT | < 0.48u  | 100         |
| GREAT   | < 1.20u  | 75          |
| GOOD    | >= 1.20u | 50          |
| HOLD    | completo | 150         |
| MISS    | —        | 0 + reset combo |

**Multiplicadores:**

| Combo | Multiplicador |
|-------|--------------|
| < 10  | 1x           |
| >= 10 | 2x           |
| >= 20 | 4x           |
| >= 30 | 8x           |

**Grades:** S >= 95% · A >= 85% · B >= 70% · C >= 55% · D < 55%

---

## Adicionando Musicas

1. Coloque o arquivo de audio (`.ogg` recomendado) na pasta `Audio/`
2. Coloque o `.chart` correspondente na mesma pasta com o mesmo nome base
3. Abra o jogo — a musica aparece automaticamente no menu de selecao

### Formato `.chart` suportado

Compativel com o formato Clone Hero. Dificuldades suportadas:
`ExpertSingle`, `HardSingle`, `MediumSingle`, `EasySingle`

### Fallback: chart procedural

Se nao houver `.chart`, o jogo gera um chart automatico baseado no BPM e duracao do audio.

---

## Licenca

MIT
