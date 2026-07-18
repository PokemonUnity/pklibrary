using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials.Data
{
	/// <summary>
	/// Represents the growth rate system for Pokémon.
	/// </summary>
	/// <remarks>
	/// This interface defines the functionality for managing Pokémon growth rates,
	/// including experience point calculations and level-up requirements.
	/// </remarks>
	public interface IGrowthRate
	{
		/// <summary>
		/// Gets the unique identifier for this target.
		/// </summary>
		int id					{ get; }

		/// <summary>
		/// Gets the untranslated name of this target.
		/// </summary>
		string real_name		{ get; }

		int[] exp_values		{ get; }

		Func<int,int> exp_formula		{ get; }

		/// <summary>
		/// Initializes the growth rate system.
		/// </summary>
		IGrowthRate initialize(int id, string name = null, int[] exp_values = null, Func<int,int> exp_formula = null);

		/// <summary>
		/// Loads target data from storage.
		/// </summary>
		void load();

		/// <summary>
		/// Saves target data to storage.
		/// </summary>
		void save();

		/// <summary>
		/// Gets or sets the growth rate name.
		/// </summary>
		string name { get; }

		/// <summary>
		/// </summary>
		/// <param name="level">a level number</param>
		/// <returns>the minimum Exp needed to be at the given level</returns>
		/// <exception cref="ArgumentOutOfRangeException">if less than or equal to zero</exception>
		int minimum_exp_for_level(int level);

		/// <summary>
		/// the maximum Exp a Pokémon with this growth rate can have
		/// </summary>
		int maximum_exp { get; }

		/// <summary>
		/// </summary>
		/// <param name="exp1">an Exp amount</param>
		/// <param name="exp2">an Exp amount</param>
		/// <returns>the sum of the two given Exp amounts</returns>
		int add_exp(int exp1, int exp2);

		/// <summary>
		/// </summary>
		/// <param name="exp">an Exp amount</param>
		/// <returns>the level of a Pokémon that has the given Exp amount</returns>
		/// <exception cref="ArgumentOutOfRangeException">if less than zero</exception>
		int level_from_exp(int exp);
	}
}