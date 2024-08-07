﻿using System;
using System.Collections;
using System.Collections.Generic;
using PokemonUnity;
using PokemonUnity.Inventory;
using PokemonUnity.Overworld;
using PokemonEssentials.Interface.PokeBattle;

namespace PokemonEssentials.Interface.Item
{
	/// <summary>
	/// Data structure representing mail that the Pokémon can hold
	/// </summary>
	public interface IMail {
		Items item				{ get; set; }
		string message			{ get; set; }
		string sender			{ get; }
		int poke1				{ get; }
		int poke2				{ get; }
		int poke3				{ get; }

		/// <summary>
		/// </summary>
		/// <param name="item">Item represented by this mail</param>
		/// <param name="message">Message text</param>
		/// <param name="sender">Name of the message's sender</param>
		/// <param name="poke1">[species,gender,shininess,form,shadowness,is egg]</param>
		/// <param name="poke2"></param>
		/// <param name="poke3"></param>
		/// <returns></returns>
		IMail initialize(Items item, string message, string sender, IPokemon poke1 = null, IPokemon poke2 = null, IPokemon poke3 = null);
	}

	/// <summary>
	/// Extension of <seealso cref="IGame"/>
	/// </summary>
	public interface IGameMail : IGame
	{
		bool MoveToMailbox(IPokemon pokemon);

		void StoreMail(IPokemon pkmn, Items item, string message, IPokemon poke1 = null, IPokemon poke2 = null, IPokemon poke3 = null);

		void DisplayMail(IMail mail, string bearer = null);
	}
}