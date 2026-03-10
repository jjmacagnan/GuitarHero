# 🎸 Guitar Hero Clone — Godot 4 + C#

## Estrutura do Projeto

```
res://
├── Scripts/
│   ├── Note.cs          ← Nota individual que desce
│   ├── Lane.cs          ← Pista com detecção de input
│   ├── SongChart.cs     ← Dados da música/chart
│   └── GameManager.cs   ← Controlador principal
├── Scenes/
│   ├── Note.tscn        ← Cena da nota
│   ├── Lane.tscn        ← Cena de uma pista
│   └── Game.tscn        ← Cena principal
├── Audio/
│   └── song.ogg         ← Sua música aqui
└── project.godot
```

---

## 🔧 Passo a Passo para Montar as Cenas

### 1. Note.tscn
```
Node3D (Note.cs)
└── Mesh (MeshInstance3D)
      └── BoxMesh: size (1.5, 0.4, 1.5)
      └── (material será aplicado via código)
```

### 2. Lane.tscn
```
Node3D (Lane.cs)
├── Track (MeshInstance3D)  ← Trilha visual
│     └── BoxMesh: size (1.8, 0.05, 60)  pos Y=10 Z=0
│     └── StandardMaterial3D: albedo cinza escuro, transparente 50%
└── Button (MeshInstance3D)  ← Botão na hit line
      └── CylinderMesh: radius 0.7, height 0.2
      └── pos Y=0 (hit line)
```

### 3. Game.tscn (cena principal)
```
Node3D (GameManager.cs)
├── Camera3D
│     └── pos: (0, 5, 18), rot X: -15°
├── DirectionalLight3D
│     └── rot X: -45°, Y: -30°
├── NoteSpawnPoint (Node3D)
│     └── pos Y: 20
├── AudioPlayer (AudioStreamPlayer)
├── Lanes (Node3D)
│     ├── Lane0 (Lane.tscn) pos X:-4  ← Verde  | Tecla: A
│     ├── Lane1 (Lane.tscn) pos X:-2  ← Vermelho| Tecla: S
│     ├── Lane2 (Lane.tscn) pos X: 0  ← Amarelo | Tecla: D
│     ├── Lane3 (Lane.tscn) pos X: 2  ← Azul    | Tecla: F
│     └── Lane4 (Lane.tscn) pos X: 4  ← Laranja | Tecla: Space
└── HUD (CanvasLayer)
      ├── ScoreLabel (Label)  — Ancora: Top-Left
      ├── ComboLabel (Label)  — Ancora: Center
      ├── MultLabel (Label)   — Ancora: Top-Right
      └── FeedbackLabel (Label) — Ancora: Center, pos Y:-80
```

---

## 🎮 Controles

| Tecla   | Pista | Cor      |
|---------|-------|----------|
| A       | 0     | Verde    |
| S       | 1     | Vermelho |
| D       | 2     | Amarelo  |
| F       | 3     | Azul     |
| Space   | 4     | Laranja  |

---

## 📊 Sistema de Pontuação

| Timing      | Janela     | Pontos base |
|-------------|------------|-------------|
| PERFECT     | < 0.3u     | 100         |
| GREAT       | < 0.8u     | 75          |
| GOOD        | < 1.5u     | 50          |
| MISS        | fora       | 0 + -combo  |

**Multiplicadores de Combo:**
- 10+ notas = 2x
- 20+ notas = 4x
- 30+ notas = 8x

---

## 🎵 Adicionando Músicas

### Opção A: Chart automático (já incluído)
O `SongChart.GenerateDemoChart()` gera padrões automaticamente.
Ajuste o BPM no inspetor.

### Opção B: Chart manual via código
```csharp
var chart = new SongChart { BPM = 128f };
chart.Notes.Add(new NoteData { Time = 1.0, Lane = 0 });
chart.Notes.Add(new NoteData { Time = 1.5, Lane = 2 });
// ...
```

### Opção C: Importar de JSON
Formato sugerido:
```json
{
  "songName": "Minha Música",
  "bpm": 128,
  "startOffset": 2.0,
  "notes": [
    { "time": 1.000, "lane": 0 },
    { "time": 1.500, "lane": 2 },
    { "time": 2.000, "lane": 4 }
  ]
}
```

---

## 🚀 Próximas Funcionalidades (para expandir)

- [ ] **Hold Notes** — Notas longas (já tem `IsLong` no NoteData)
- [ ] **Star Power** — Modo bônus com multiplicador extra
- [ ] **Medidor de Energia** — Falha se zerar
- [ ] **Telas de Menu/GameOver** — Fluxo completo de jogo
- [ ] **Importador de .chart/.mid** — Compatível com Clone Hero
- [ ] **Efeitos de partícula** — Explosão ao acertar notas
- [ ] **Whammy bar** — Deforma pitch nas hold notes
- [ ] **Multiplayer local** — Co-op / duelo

---

## ⚙️ Configurações no GameManager (Inspetor)

| Propriedade   | Padrão | Descrição                        |
|---------------|--------|----------------------------------|
| NoteScene     | —      | Arraste Note.tscn aqui           |
| Chart         | —      | Opcional; gera demo se vazio     |
| NoteSpeed     | 12.0   | Velocidade de queda (u/s)        |

---

## 💡 Dica de Camera

Para um look mais cinematográfico estilo Guitar Hero/Clone Hero:
- Camera pos: `(0, 3, 14)`
- Camera rot: `(-10°, 0°, 0°)`
- FOV: `75°`
- Use `Environment` com bloom habilitado para o glow neon das notas!
