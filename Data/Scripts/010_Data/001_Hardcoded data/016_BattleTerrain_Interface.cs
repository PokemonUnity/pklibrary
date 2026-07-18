using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials.Data
{
	/// <summary>
	/// Represents the battle terrain system for Pokémon battles.
	/// </summary>
	/// <remarks>
	/// This interface defines the functionality for managing terrain conditions during battles,
	/// including terrain effects, transitions, and state management.
	/// </remarks>
	public interface IBattleTerrain : IEnumeration<IBattleTerrain>
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
		/// Initializes the battle terrain system.
		/// </summary>
		IBattleTerrain Initialize(int id, string name = null, string animation = null);
	}
}