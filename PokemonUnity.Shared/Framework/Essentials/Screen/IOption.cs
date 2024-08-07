﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonUnity;
using PokemonUnity.Interface;
using PokemonUnity.Combat;
using PokemonUnity.Character;
using PokemonUnity.Inventory;
using PokemonEssentials.Interface;
using PokemonEssentials.Interface.Battle;
using PokemonEssentials.Interface.Field;
using PokemonEssentials.Interface.Item;
using PokemonEssentials.Interface.PokeBattle;
using PokemonEssentials.Interface.PokeBattle.Effects;
using PokemonEssentials.Interface.EventArg;

namespace PokemonEssentials.Interface.Screen
{
	public interface IWindow_PokemonOption //: Window_DrawableCommand
	{
		bool mustUpdateOptions				{ get; set; }

		IWindow_PokemonOption initialize(IOptionValue[] options, float x, float y, float width, float height);

		IOptionValue this[int i] { get; set; }

		int itemCount();

		void drawItem(int index, int count, IRect rect);

		void update();
	}

	public interface IPropertyMixin<T> {
		Func<int, T> get();

		void set(Action<int> value);
	}

	public interface IEnumOption : IOptionValue, IPropertyMixin<string>, IEnumOption<string>
	{
		//string values			{ get; }
		//string name			{ get; }

		//IEnumOption initialize(string name, string options, Func<int,string> getProc, Action<int> setProc);

		//int next(int current);

		//int prev(int current);
	}

	public interface IEnumOption<T> : IOptionValue, IPropertyMixin<T>
	{
		T values				{ get; }
		//string name				{ get; }

		IEnumOption<T> initialize(string name, T options, Func<int,T> getProc, Action<int> setProc);

		//int next(int current);

		//int prev(int current);
	}

	public interface INumberOption : IOptionValue, IPropertyMixin<int>
	{
		//string name				{ get; }
		int optstart			{ get; }
		int optend				{ get; }

		INumberOption initialize(string name, int optstart, int optend, Func<int,int> getProc, Action<int> setProc);

		//int next(int current);

		//int prev(int current);
	}

	public interface ISliderOption : IOptionValue, IPropertyMixin<int>
	{
		//string name				{ get; }
		int optstart			{ get; }
		int optend				{ get; }

		ISliderOption initialize(string name, int optstart, int optend, int optinterval, Func<int,int> getProc, Action<int> setProc);

		//int next(int current);

		//int prev(int current);
	}

	// ####################
	// Stores game options
	// Default options are at the top of script section SpriteWindow.
	/// <summary>
	/// Extension of <see cref="IGame"/>
	/// </summary>
	public interface IGameOption : IGame
	{

		//string[] SpeechFrames { get; }
		//string[] TextFrames { get; }
		//string[][] VersionStyles { get; }

		int SettingToTextSpeed(int speed);
	}

	//public interface IMessageConfig {
	//	public static IBitmap DefaultSystemFrame();
	//
	//	public static IBitmap DefaultSpeechFrame();
	//
	//	public static string DefaultSystemFontName();
	//
	//	public static int DefaultTextSpeed();
	//
	//	public int GetSystemTextSpeed();
	//}

	/// <summary>
	/// Stores game options
	/// </summary>
	public interface IPokemonSystemOption {
		int textspeed				{ get; set; }
		int battlescene				{ get; set; }
		int battlestyle				{ get; set; }
		int frame					{ get; set; }
		int textskin				{ get; }
		int font					{ get; set; }
		int screensize				{ get; set; }
		int language				{ get; }
		int border					{ get; }
		int runstyle				{ get; }
		int bgmvolume				{ get; }
		int sevolume				{ get; }

		int tilemap					{ get; }

		IPokemonSystemOption initialize();
	}

	/// <summary>
	/// Scene to change game settings.
	/// </summary>
	public interface IOptionScene : IScene {
		void Update();
		void StartScene(bool inloadscreen = false);
		object AddOnOptions(object options);
		void Options();
		void EndScene();
	}

	public interface IOptionScreen : IScreen {
		IOptionScreen initialize(IOptionScene scene);
		void StartScreen(bool inloadscreen = false);
	}

	public interface IOptionValue
	{
		string name { get; }
		int next(int current);
		int prev(int current);
	}
}