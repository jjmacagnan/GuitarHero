using Godot;

public partial class MainMenu : Control
{
	public override void _Ready()
	{
		var play = GetNodeOrNull<Button>("VBox/PlayButton");
		var quit = GetNodeOrNull<Button>("VBox/QuitButton");

		if (play != null) play.Pressed += OnPlayPressed;
		else GD.PushError("[MainMenu] PlayButton não encontrado!");

		if (quit != null) quit.Pressed += OnQuitPressed;
		else GD.PushError("[MainMenu] QuitButton não encontrado!");
	}

	private void OnPlayPressed() => GetTree().ChangeSceneToFile("res://Scenes/SongSelect.tscn");
	private void OnQuitPressed() => GetTree().Quit();
}
