﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonUnity;
using PokemonUnity.Inventory;
using PokemonUnity.Combat.Data;
using PokemonUnity.Character;
using PokemonUnity.Saving;
using PokemonUnity.Saving.SerializableClasses;
using PokemonUnity.Utility;
using PokemonUnity.Monster;
using PokemonEssentials.Interface;
using PokemonEssentials.Interface.PokeBattle;
using PokemonEssentials.Interface.EventArg;

namespace PokemonEssentials.Interface
{
	namespace PokeBattle
	{
		/// <summary>
		/// Extensions of <seealso cref="IPokemon"/>
		/// </summary>
		public interface IPokemonChatter : IPokemon
		{
			IWaveData chatter		{ get; set; }
		}
	}

	/// <summary>
	/// Extensions of <seealso cref="IGame"/>
	/// </summary>
	public interface IGameChatter : IGame
	{
		void Chatter(PokemonEssentials.Interface.PokeBattle.IPokemonChatter pokemon);

		//HiddenMoveHandlers.addCanUseMove(:CHATTER,proc {|item,pokemon|
		//   return true;
		//});

		//HiddenMoveHandlers.addUseMove(:CHATTER,proc {|item,pokemon|
		//   Chatter(pokemon);
		//   return true;
		//});
	}

	/// <summary>
	/// Extensions of <seealso cref="IPokeBattle_Scene"/>
	/// </summary>
	public interface IPokeBattle_SceneChatter //: IPokeBattle_Scene
	{
		void Chatter(IBattler attacker, IBattler opponent);
	}
}