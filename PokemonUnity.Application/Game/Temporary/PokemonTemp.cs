﻿using System;
using System.Collections;
using System.Collections.Generic;
using PokemonUnity;
using PokemonUnity.Character;
using PokemonUnity.Inventory;
using PokemonUnity.Interface;
using PokemonEssentials.Interface;
using PokemonEssentials.Interface.Field;
using PokemonEssentials.Interface.PokeBattle;

namespace PokemonUnity
{
	/// <summary>
	/// Temporary data which is not saved and which is erased when a game restarts.
	/// </summary>
	/// ToDo: Rename to `TempData` or `TempMetadata`
	public partial class PokemonTemp : PokemonEssentials.Interface.Field.ITempMetadata //PokemonEssentials.Interface.ITempMetadata,
	{
		public int menuLastChoice { get; set; }
		public int keyItemCalling { get; set; }
		public int hiddenMoveEventCalling { get; set; }
		public bool begunNewGame { get; set; }
		public int miniupdate { get; set; }
		public int waitingTrainer { get; set; }
		public ISprite darknessSprite { get; set; }
		public IList<string> pokemonDexData { get; }
		public IDictionary<int, IPokemonMetadata> pokemonMetadata { get; set; }
		public IList<IPhoneMessageData> pokemonPhoneData { get; }
		public IBattleRecordData lastbattle { get; set; }
		public int flydata { get; set; }

		public PokemonTemp() { initialize(); }
		public PokemonEssentials.Interface.Field.ITempMetadata initialize()
		{
			return this;
		}
	}
}