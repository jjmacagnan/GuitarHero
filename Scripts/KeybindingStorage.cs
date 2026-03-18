using Godot;

/// <summary>
/// Persiste bindings de teclado e gamepad para as 5 lanes em user://keybindings.cfg
/// e aplica ao InputMap em tempo de execução.
///
/// Seções do arquivo:
///   [keyboard]  lane_0..4 = Key enum (int)
///   [gamepad]   lane_N_is_axis = bool
///               lane_N_axis    = JoyAxis enum (int)  — quando is_axis = true
///               lane_N_button  = JoyButton enum (int) — quando is_axis = false
/// </summary>
public static class KeybindingStorage
{
    private const string SavePath = "user://keybindings.cfg";

    // ── Defaults (espelham GameManager.SetupInputMap exatamente) ──────────
    public static readonly Key[] DefaultKeys =
        { Key.A, Key.S, Key.J, Key.K, Key.L };

    // Lanes 0 e 3 usam eixo (L2/R2); demais usam botão
    public static readonly bool[]      DefaultIsAxis  = { true,  false,                    false,                     true,  false        };
    public static readonly JoyAxis[]   DefaultAxis    = { JoyAxis.TriggerLeft, JoyAxis.Invalid, JoyAxis.Invalid, JoyAxis.TriggerRight, JoyAxis.Invalid };
    public static readonly JoyButton[] DefaultButtons = { JoyButton.Invalid, JoyButton.LeftShoulder, JoyButton.RightShoulder, JoyButton.Invalid, JoyButton.Y };

    // ── Estado em memória ─────────────────────────────────────────────────
    private static Key[]?      _keys;
    private static bool[]?     _isAxis;
    private static JoyAxis[]?  _axes;
    private static JoyButton[]? _buttons;

    // ── API pública ───────────────────────────────────────────────────────

    public static Key      GetKey(int lane)    { EnsureLoaded(); return _keys![lane]; }
    public static bool     GetIsAxis(int lane) { EnsureLoaded(); return _isAxis![lane]; }
    public static JoyAxis  GetAxis(int lane)   { EnsureLoaded(); return _axes![lane]; }
    public static JoyButton GetButton(int lane){ EnsureLoaded(); return _buttons![lane]; }

    public static void SetKey(int lane, Key key)
    {
        EnsureLoaded();
        _keys![lane] = key;
    }

    public static void SetAxis(int lane, JoyAxis axis)
    {
        EnsureLoaded();
        _isAxis![lane] = true;
        _axes![lane]   = axis;
    }

    public static void SetButton(int lane, JoyButton button)
    {
        EnsureLoaded();
        _isAxis![lane]  = false;
        _buttons![lane] = button;
    }

    /// <summary>Reseta tudo para os valores padrão em memória (não salva no disco).</summary>
    public static void ResetToDefaults()
    {
        _keys    = (Key[])DefaultKeys.Clone();
        _isAxis  = (bool[])DefaultIsAxis.Clone();
        _axes    = (JoyAxis[])DefaultAxis.Clone();
        _buttons = (JoyButton[])DefaultButtons.Clone();
    }

    /// <summary>Persiste os bindings atuais em user://keybindings.cfg.</summary>
    public static void Save()
    {
        EnsureLoaded();
        var cfg = new ConfigFile();

        for (int i = 0; i < 5; i++)
        {
            cfg.SetValue("keyboard", $"lane_{i}", (int)_keys![i]);
            cfg.SetValue("gamepad",  $"lane_{i}_is_axis", _isAxis![i]);
            if (_isAxis![i])
                cfg.SetValue("gamepad", $"lane_{i}_axis",   (int)_axes![i]);
            else
                cfg.SetValue("gamepad", $"lane_{i}_button", (int)_buttons![i]);
        }

        var err = cfg.Save(SavePath);
        if (err != Error.Ok)
            GD.PushError($"[KeybindingStorage] Erro ao salvar: {err}");
    }

    /// <summary>
    /// Aplica os bindings em memória ao InputMap do Godot,
    /// substituindo completamente os eventos das lanes 0–4.
    /// Seguro de chamar a qualquer momento.
    /// </summary>
    public static void ApplyToInputMap()
    {
        EnsureLoaded();

        for (int i = 0; i < 5; i++)
        {
            string action = GameManager.LaneActions[i];

            if (!InputMap.HasAction(action))
                InputMap.AddAction(action);

            InputMap.ActionEraseEvents(action);

            // Teclado
            var evKey = new InputEventKey { Keycode = _keys![i] };
            InputMap.ActionAddEvent(action, evKey);

            // Gamepad
            if (_isAxis![i])
            {
                var evAxis = new InputEventJoypadMotion
                {
                    Axis      = _axes![i],
                    AxisValue = 1f
                };
                InputMap.ActionAddEvent(action, evAxis);
            }
            else
            {
                var evBtn = new InputEventJoypadButton
                {
                    ButtonIndex = _buttons![i]
                };
                InputMap.ActionAddEvent(action, evBtn);
            }
        }
    }

    // ── Nomes dos lane names (ordem fixa: Verde, Vermelho, Amarelo, Azul, Laranja) ──
    private static readonly string[] LaneNameKeys =
        { "LANE_GREEN", "LANE_RED", "LANE_YELLOW", "LANE_BLUE", "LANE_ORANGE" };

    /// <summary>
    /// Gera a string de hint de teclas dinamicamente a partir dos bindings atuais.
    /// Ex: "[A] Verde   [S] Vermelho   [J] Amarelo   [K] Azul   [L] Laranja"
    /// Se <paramref name="includeEscHint"/> for true, acrescenta "  |   [ESC] Pausar".
    /// </summary>
    public static string BuildControlsHint(bool includeEscHint = false)
    {
        EnsureLoaded();
        var parts = new string[5];
        for (int i = 0; i < 5; i++)
        {
            string keyName  = OS.GetKeycodeString(_keys![i]);
            string laneName = Locale.Tr(LaneNameKeys[i]);
            parts[i] = Locale.Tr("LANE_HINT_FMT", keyName, laneName);
        }
        string hint = string.Join("   ", parts);
        if (includeEscHint)
            hint += "   |   " + Locale.Tr("PAUSE_HINT");
        return hint;
    }

    // ── Privado ───────────────────────────────────────────────────────────

    private static void EnsureLoaded()
    {
        if (_keys != null) return;

        ResetToDefaults(); // parte dos defaults

        if (!FileAccess.FileExists(SavePath)) return;

        var cfg = new ConfigFile();
        var err = cfg.Load(SavePath);
        if (err != Error.Ok)
        {
            GD.PushError($"[KeybindingStorage] Erro ao carregar: {err}");
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            if (cfg.HasSectionKey("keyboard", $"lane_{i}"))
                _keys![i] = (Key)(int)cfg.GetValue("keyboard", $"lane_{i}");

            if (cfg.HasSectionKey("gamepad", $"lane_{i}_is_axis"))
            {
                _isAxis![i] = (bool)cfg.GetValue("gamepad", $"lane_{i}_is_axis");
                if (_isAxis[i])
                {
                    if (cfg.HasSectionKey("gamepad", $"lane_{i}_axis"))
                        _axes![i] = (JoyAxis)(int)cfg.GetValue("gamepad", $"lane_{i}_axis");
                }
                else
                {
                    if (cfg.HasSectionKey("gamepad", $"lane_{i}_button"))
                        _buttons![i] = (JoyButton)(int)cfg.GetValue("gamepad", $"lane_{i}_button");
                }
            }
        }
    }
}
