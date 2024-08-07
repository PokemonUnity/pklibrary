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
using PokemonEssentials.Interface.Field;
using PokemonEssentials.Interface.Item;
using PokemonEssentials.Interface.PokeBattle;
using PokemonEssentials.Interface.PokeBattle.Effects;
//using PokemonEssentials.Interface.PokeBattle.Rules;
using PokemonEssentials.Interface.EventArg;

namespace PokemonEssentials.Interface.Screen
{
	/// <summary>
	/// The PC item storage object, which actually contains all the items
	/// </summary>
	public interface IPCItemStorage : IList<Items>, ICollection<Items> {
		//int MAXSIZE       { get; } //= 50;    // Number of different slots in storage
		//int MAXPERSLOT    { get; } //= 999;   // Max. number of items per slot

		IPCItemStorage initialize();

		bool empty();

		int length();

		//Items this[int i] { get; }

		Items getItem(int index);

		int getCount(int index);

		int Quantity(Items item);

		bool DeleteItem(Items item, int qty = 1);

		bool CanStore(Items item, int qty = 1);

		bool StoreItem(Items item,int qty=1);
	}

	// ===============================================================================
	// PC item storage screen
	// ===============================================================================
	public interface IWindow_PokemonItemStorage //: IWindow_DrawableCommand
	{
		IBag bag				{ get; set; }
		int pocket				{ get; set; }
		int sortIndex			{ get; set; }
		//int sortIndex         { set; }


		IWindow_PokemonItemStorage initialize(IBag bag, float x, float y, float width, float height);

		Items item { get; }

		int itemCount();

		void drawItem(int index, int count, IRect rect);
	}

	public interface IItemStorageScene : IScene
	{
		// Configuration
		IColor ITEMLISTBASECOLOR        { get; } //= new Color(88,88,80);
		IColor ITEMLISTSHADOWCOLOR      { get; } //= new Color(168,184,184);
		IColor ITEMTEXTBASECOLOR        { get; } //= new Color(248,248,248);
		IColor ITEMTEXTSHADOWCOLOR      { get; } //= new Color(0,0,0);
		IColor TITLEBASECOLOR           { get; } //= new Color(248,248,248);
		IColor TITLESHADOWCOLOR         { get; } //= new Color(0,0,0);
		int ITEMSVISIBLE                { get; } // = 7;

		IItemStorageScene initialize(string title);

		void update();

		void StartScene(IBag bag);

		void EndScene();

		//void Refresh();

		//ToDo: Might need to split into two functions,
		//one for returning item selected
		//the other to handle selection mechanic
		//IEnumerator<Items> ChooseItem();
		Items ChooseItem();

		int ChooseNumber(string helptext, int maximum);

		void Display(string msg, bool brief = false);

		bool Confirm(string msg);

		int ShowCommands(string helptext, string[] commands);
	}

	public interface IWithdrawItemScene : IItemStorageScene {
		IWithdrawItemScene initialize();
	}

	public interface ITossItemScene : IItemStorageScene {
		ITossItemScene initialize();
	}
}