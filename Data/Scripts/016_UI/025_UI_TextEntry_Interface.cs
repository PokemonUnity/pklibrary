using System;
using System.Collections.Generic;

namespace PokemonEssentials
{
	public interface IWindow_CharacterEntry : IWindow_DrawableCommand
	{
		//XSIZE=13
		//YSIZE=4

		IWindow_CharacterEntry initialize(string charset, IViewport viewport = null);

		void setOtherCharset(string value);

		void setCharset(string value);

		char? character();

		int command();

		//int itemCount();

		//void drawItem(int index,int count,IRect rect);
	}

	/// <summary>
	/// Interface for the text entry scene that manages user text input functionality.
	/// Handles character input, text editing, and input validation for various text entry needs.
	/// </summary>
	/// <remarks>
	/// Scene used for entering text, such as naming a Pokémon or a box.
	/// Text entry screen - free typing.
	/// </remarks>
	public interface IScenePokemonTextEntry : IScene //,ITextEntryUIScene
	{
		/// <summary>
		/// Starts the text entry scene with specified parameters and constraints.
		/// Initializes keyboard interface, text display, and input validation.
		/// </summary>
		/// <param name="helptext">Help text to display for guidance.</param>
		/// <param name="minlength">Minimum required length of text input.</param>
		/// <param name="maxlength">Maximum allowed length of text input.</param>
		/// <param name="initialText">Initial text to pre-populate in the input field.</param>
		/// <param name="subject">Subject context for text entry (e.g., Pokemon, trainer).</param>
		/// <param name="pokemon">Pokemon object if text entry is Pokemon-related.</param>
		void StartScene(string helptext, int minlength, int maxlength, string initialText, int subject = 0, IPokemon pokemon = null);
		/// <summary>
		/// Ends the text entry scene and cleans up resources.
		/// Handles fade out transition and disposes of sprites and viewports.
		/// </summary>
		void EndScene();
		/// <summary>
		/// Handles the main scene interaction loop for text input.
		/// Processes character input, editing commands, and text submission.
		/// </summary>
		/// <returns>Final entered text string, or null if cancelled.</returns>
		string Entry();
		//string Entry1();
		//string Entry2();
	}

	/// <summary>
	/// Scene used for entering text, such as naming a Pokémon or a box.
	/// </summary>
	/// <remarks>
	/// Text entry screen - arrows to select letter.
	/// </remarks>
	public interface IScenePokemonTextEntry2 : IScene, IScenePokemonTextEntry, IHaveUpdate
	{
		//void StartScene(string helptext, int minlength, int maxlength, string initialText, int subject = 0, Pokemon pokemon = null);
		//void EndScene();
		//string Entry();
		void Update();
		void ChangeTab(int newtab = 0);
		bool ColumnEmpty(int m);
		void UpdateOverlay();
		void DoUpdateOverlay();
		void DoUpdateOverlay2();
		bool MoveCursor();
		int wrapmod(int x, int y);
	}

	/// <summary>
	/// Interface for the text entry screen that orchestrates user text input functionality.
	/// Coordinates between scenes and manages overall text input experience.
	/// </summary>
	public interface IScreenPokemonEntry : IScreen
	{
		/// <summary>
		/// Initializes the text entry screen with the specified scene.
		/// Sets up the scene instance for managing the text entry interface.
		/// </summary>
		/// <param name="scene">The text entry scene to use.</param>
		IScreenPokemonEntry initialize(IScenePokemonTextEntry scene);

		/// <summary>
		/// Starts the text entry screen for user input collection.
		/// Displays text entry interface and manages input validation and submission.
		/// </summary>
		/// <param name="helptext">Help text to guide user input.</param>
		/// <param name="minlength">Minimum required text length.</param>
		/// <param name="maxlength">Maximum allowed text length.</param>
		/// <param name="initialText">Initial text to pre-populate.</param>
		/// <param name="mode">Context subject for the text entry.</param>
		/// <param name="pokemon">Pokemon object if applicable to the input.</param>
		/// <returns>Final entered text, or null if cancelled.</returns>
		string StartScreen(string helptext, int minlength, int maxlength, string initialText = "", int mode = 0, int? pokemon = null);
	}

	/// <summary>
	/// Extension of <see cref="IMain"/>
	/// </summary>
	public interface IMainPokemonTextEntry : IMain
	{
		string EnterText(string helptext, int minlength, int maxlength, string initialText = "", int mode = 0, int? pokemon = null, bool nofadeout = false);

		string EnterPlayerName(string helptext, int minlength, int maxlength, string initialText = "", bool nofadeout = false);

		string EnterPokemonName(string helptext, int minlength, int maxlength, string initialText = "", int? pokemon = null, bool nofadeout = false);

		string EnterBoxName(string helptext, int minlength, int maxlength, string initialText = "", bool nofadeout = false);

		string EnterNPCName(string helptext, int minlength, int maxlength, string initialText = "", int id = 0, bool nofadeout = false);
	}
}