using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials.Data
{
	/// <summary>
	/// Represents the battle weather system for Pokémon battles.
	/// </summary>
	/// <remarks>
	/// This interface defines the functionality for managing weather conditions during battles,
	/// including weather effects, transitions, and state management.
	/// </remarks>
	public interface IBattleWeather : IEnumeration<IBattleWeather>
	{
		/// <summary>
		/// Gets the unique identifier for this target.
		/// </summary>
		int id					{ get; }

		/// <summary>
		/// Gets the untranslated name of this target.
		/// </summary>
		string real_name		{ get; }
		string animation		{ get; }

		/// <summary>
		/// Initializes the battle weather system.
		/// </summary>
		IBattleWeather Initialize(int id, string name = null, string animation = null);

		/// <summary>
		/// Loads target data from storage.
		/// </summary>
		void load();

		/// <summary>
		/// Saves target data to storage.
		/// </summary>
		void save();

		/// <summary>
		/// Gets the translated name of this target.
		/// </summary>
		/// <returns>The localized name of the target.</returns>
		//string name();
		string name { get; }
	}
}