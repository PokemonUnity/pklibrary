using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials.Data
{
	/// <summary>
	/// This interface defines the functionality for managing battle targets,
	/// including target selection, validation, and state management.
	/// </summary>
	/// <remarks>
	/// Represents the target system for Pokémon battles.
	/// NOTE: If adding a new target, you will need to add code in several places to
	///       make them work properly:
	///         - <see cref="FindTargets"/>
	///         - <see cref="MoveCanTarget"/>
	///         - <see cref="CreateTargetTexts"/>
	///         - <see cref="FirstTarget"/>
	///         - <see cref="TargetsMultiple"/>
	/// </remarks>
	public interface ITarget : IEnumeration<ITarget>
	{
		/// <summary>
		/// Gets the unique identifier for this target.
		/// </summary>
		int id					{ get; }

		/// <summary>
		/// Gets the untranslated name of this target.
		/// </summary>
		string real_name		{ get; }
		/// <summary>0, 1 or 2 (meaning 2+)</summary>
		int num_targets			{ get; }
		/// <summary>Is able to target one or more foes</summary>
		bool targets_foe		{ get; }
		/// <summary>Crafty Shield can't protect from these moves</summary>
		bool targets_all		{ get; }
		/// <summary>Pressure also affects these moves</summary>
		bool affects_foe_side	{ get; }
		/// <summary>Hits non-adjacent targets</summary>
		bool long_range			{ get; }

		/// <summary>
		/// Gets the data collection for all registered egg groups.
		/// </summary>
		//IDictionary DATA { get; }

		/// <summary>
		/// Initializes the target system.
		/// </summary>
		ITarget Initialize(int id, string name = null, int? num_targets = null, bool? targets_foe = false, bool? targets_all = false, bool? affects_foe_side = false, bool? long_range = false);

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

		bool can_choose_distant_target { get; }

		bool can_target_one_foe { get; }
	}
}