using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials
{
	public interface IBattleChoice
	{
		int Action { get; }
		/// <summary>
		/// Index of Action being used
		/// </summary>
		int Index { get; }
		IBattleMove Move { get; } //ToDo: Rename to Value?
		int Target { get; }
	}
}