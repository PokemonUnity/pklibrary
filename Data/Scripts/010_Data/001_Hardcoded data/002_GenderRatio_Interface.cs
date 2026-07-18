using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials.Data
{
	/// <summary>
	/// Represents the gender ratio system for Pokémon.
	/// </summary>
	/// <remarks>
	/// This interface defines the functionality for managing Pokémon gender ratios,
	/// including gender determination and ratio calculations.
	/// </remarks>
	public interface IGenderRatio
	{
		/// <summary>
		/// Gets the unique identifier for this target.
		/// </summary>
		int id { get; }

		/// <summary>
		/// Gets the untranslated name of this target.
		/// </summary>
		string real_name { get; }

		int? female_chance { get; }

		/// <summary>
		/// Initializes the gender ratio system.
		/// </summary>
		IGenderRatio initialize(int id, string name = null, int? female_chance = null);

		/// <summary>
		/// Gets the gender ratio name.
		/// </summary>
		/// <returns>The gender ratio name.</returns>
		string name { get; }
	}
}