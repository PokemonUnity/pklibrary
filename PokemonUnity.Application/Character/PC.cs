﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonUnity;
using PokemonUnity.Monster;
using PokemonUnity.Attack;
using PokemonUnity.Inventory;
using PokemonUnity.Saving.SerializableClasses;
using PokemonUnity.Character;
using PokemonEssentials.Interface.Screen;
using PokemonEssentials.Interface.PokeBattle;

namespace PokemonUnity.Character
{
	//ToDo: Add Function to add more boxes to player pc
	// OR! A function to disable boxes until they're "unlocked"
	// Max # of boxes would be hard-capped if option 2
	//Add Game.GameData.Feature where player unlocks more boxes
	[System.Obsolete("Something i plan to transition to; not yet ready for full integration")]
	public class PC : PokemonEssentials.Interface.Screen.IPCPokemonStorage
	{
		#region Variables
		//private List<Mail> mails { get; set; } //ToDo: Add Mail to PC class
		/// <summary>
		/// </summary>
		/// <remarks>
		/// There's a limit to how many items can be stored in PC
		/// </remarks>
		//private List<Items> items { get; set; }
		private Dictionary<Items, int> items { get; set; }
		private IPokemon[,] pokemons { get; set; }
		//public Pokemon[,] AllBoxes { get { return pokemons; } }
		public IPokemon[][] AllBoxes
		{
			get
			{
				IPokemon[][] pkmns = new Pokemon[pokemons.GetLength(0)][];
				for (int i = 0; i < pkmns.GetLength(0); i++)
				{
					pkmns[i] = new Pokemon[pokemons.GetLength(1)];
					for (int j = 0; j < pkmns[i].Length; j++)
					{
						pkmns[i][j] = pokemons[i, j];
					}
				}
				return pkmns;
			}
		}
		public byte ActiveBox { get; private set; } //Rename to CurrentBox?
		public string[] BoxNames { get; private set; }
		public int[] BoxTextures { get; private set; }
		public string Name { get { return BoxNames[ActiveBox] ?? "Box " + (ActiveBox + 1).ToString(); } }
		public int Texture { get { return BoxTextures[ActiveBox]; } }
		public IPokemon[] Pokemons
		{
			get
			{
				IPokemon[] p = new Pokemon[pokemons.GetLength(1)];
				for (int t = 0; t < p.Length; t++)
				{
					p[t] = pokemons[ActiveBox, t];
				}
				return p;
			}
		}
		/// <summary>
		/// </summary>
		/// ToDo: Add filter to add/remove items...
		//public List<Items> Items { get { return Game.GameData.PC_Items; } set { Game.GameData.PC_Items = value; } }
		public KeyValuePair<Items, int>[] Items
		{
			get
			{
				//ToDo: Set PC Items Box Max Capicity as a public Const Variable
			//	KeyValuePair<Items, int>[] pairs = new KeyValuePair<Items, int>[50];
				List<KeyValuePair<Items, int>> l = new List<KeyValuePair<Items, int>>();
			//	int index = 0;
				//foreach (Items item in items.Distinct().Where(x => Game.GameData.ItemData[x].Pocket == ItemPockets.MISC))
				foreach (KeyValuePair<Items, int> item in items)
				{
					//Can only display the amount of items the PC can hold
			//		for (int n = index; n < pairs.Length; n++)
			//		{
						int total = 0;
						//int count = items.Where(x => x == item).Count();
						int count = item.Value;
						int groups = (int)Math.Floor(total / 99d);
						//Max number of items in an individual slot is capped
						for (int i = 0; i < (int)Math.Floor(count / 99d); i++)
						{
			//				pairs[index] = new KeyValuePair<Items, int>(item.Key, 99);
							l.Add(new KeyValuePair<Items, int>(item.Key, 99));
			//				index++;
						}
						int leftovers = total % 99;
			//			pairs[index] = new KeyValuePair<Items, int>(item.Key, leftovers);
						l.Add(new KeyValuePair<Items, int>(item.Key, leftovers));
						//total += (int)Math.Ceiling(total / 99d);
			//			index++;
			//		}
				}
			//	return pairs;
				return l.ToArray();
			}
		}

		#region Interface
		IPokemonBox[] IPCPokemonStorage.boxes
		{
			get
			{
				IPokemonBox[] boxes = new PokemonBox[pokemons.GetLength(0)];
				for (int i = 0; i < boxes.Length; i++)
				{
					//int ip1 = i + 1;
					@boxes[i] = new PokemonBox(BoxNames[i] ?? "Box " + (i + 1).ToString(), //string.Format("Box {0}", ip1),
						pokemons.GetLength(1));
					//int backid = i % 24;
					@boxes[i].background = BoxTextures[i].ToString(); //$"box{backid}";
				}
				return boxes;
			}
		}

		int IPCPokemonStorage.currentBox { get { return ActiveBox; } set { ActiveBox = (byte)value; } }

		int IPCPokemonStorage.maxBoxes { get { return pokemons.GetLength(0); } }

		IPokemon[] IPCPokemonStorage.party { get { return Game.GameData.Trainer.party; } }

		bool IPCPokemonStorage.full
		{
			get
			{
				for(int i = 0; i < pokemons.GetLength(1);i++)
				{
					if (!pokemons[ActiveBox, i].IsNotNullOrNone())
						return false;
				}
				return true; //pokemons.Count<IPokemon>(p => p.IsNotNullOrNone());
			}
		}

		IPokemon IPCPokemonStorage.this[int x, int y] { get { return pokemons[x,y]; } set { pokemons[x, y] = value; } }

		IPokemonBox IPCPokemonStorage.this[int x]
		{
			get
			{
				x = (byte)(x % pokemons.GetLength(0)); //Core.STORAGEBOXES);
				return this[(byte)x];
			}
		}

		//ToDo: return pokemon box, without changing activebox?
		public IPokemonBox this[byte i]
		{
			get
			{
				i = (byte)(i % pokemons.GetLength(0)); //Core.STORAGEBOXES);
				this.ActiveBox = i;
				IPokemonBox box = new PokemonBox(BoxNames[i] ?? "Box " + (i + 1).ToString(), //string.Format("Box {0}", ip1),
					pokemons.GetLength(1));
				box.background = BoxTextures[i].ToString(); //$"box{backid}";
				for (int t = 0; t < box.length; t++)
				{
					IPokemon p = pokemons[i, t];
					if (p.IsNotNullOrNone())
					{
						//box[t] = pokemons[x, t];
						box[t] = p;	//Add
					}
				}
				return box;
			}
		}
		#endregion
		#endregion

		#region Constructors
		public PC()
		{
			//PC_Poke = new Pokemon[Core.STORAGEBOXES, 30];
			pokemons = new Pokemon[Core.STORAGEBOXES, 30];
			//PC_boxNames = new string[Core.STORAGEBOXES];
			BoxNames = new string[Core.STORAGEBOXES];
			//PC_boxTexture = new int[Core.STORAGEBOXES];
			BoxTextures = new int[Core.STORAGEBOXES];
			for (int i = 0; i < Core.STORAGEBOXES; i++)
			{
				//Initialize the PC storage so pokemons arent null (in value)
				for (int j = 0; j < pokemons.GetLength(1); j++)
				{
					//All default values must be `NONE`
					pokemons[i, j] = new Pokemon(PokemonUnity.Pokemons.NONE);//pokemons[i, j];
				}
				//ToDo: Using string from translator here
				BoxNames[i] = string.Format("Box {0}", (i + 1).ToString());
				//PC_boxTexture[i] = i;
				BoxTextures[i] = i;
			}
			//PC_Items = new List<Items>();
			items = new Dictionary<Items, int>() { { Inventory.Items.POTION, 1 } };
		}

		public PC(IPokemon[][] pkmns = null, KeyValuePair<Items,int>[] items = null, byte? box = null, string[] names = null, int[] textures = null) : this()
		{
			if (names != null)
				BoxNames = names;
			if (textures != null)
				BoxTextures = textures;
			if (box.HasValue)
				ActiveBox = (byte)(box.Value % Core.STORAGEBOXES);
			if (items != null)
			{
				//Reset and clear space
				this.items = new Dictionary<Items, int>();
				foreach (KeyValuePair<Items, int> item in items)
					if(!this.items.ContainsKey(item.Key))
						this.items.Add(item.Key, item.Value);
			}
			if (pkmns != null)
				for (int x = 0; x < Core.STORAGEBOXES; x++)
					for (int y = 0; y < 30; y++)
						if (pkmns.Length - 1 < x || pkmns[x].Length - 1 < y)
							pokemons[x, y] = new Pokemon(PokemonUnity.Pokemons.NONE);
						else
							pokemons[x, y] = pkmns[x][y];
		}
		//public PC(Pokemon[] pokemons = null, Items[] items = null, byte? box = null, string[] names = null, int[] textures = null) : this()

		IPCPokemonStorage IPCPokemonStorage.initialize(int maxBoxes, int maxPokemon)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Methods
		public bool hasSpace()
		{
			if (getBoxCount() < 30) return true;
			else return false;
		}

		public int getBoxCount()
		{
			int result = 0;
			for (int i = 0; i < Pokemons.Length; i++)
			{
				if (Pokemons[i].IsNotNullOrNone())// != null || Pokemons[i].Species != PokemonUnity.Pokemons.NONE)
				{
					result += 1;
				}
			}
			return result;
		}

		public int? getIndexOfFirstEmpty()
		{
			//int result = 0;
			for (int i = 0; i < Pokemons.Length; i++)
			{
				if (Pokemons[i] == null || Pokemons[i].Species == PokemonUnity.Pokemons.NONE)
				{
					return i;
				}
			}
			return null;
		}

		public int getBoxCount(int box)
		{
			int result = 0;
			for (int i = 0; i < Core.STORAGEBOXES; i++) //pokemons.GetLength(1)
			{
				if (pokemons[box,i].IsNotNullOrNone())// != null || Party[i].Species != Pokemons.NONE)
				{
					result += 1;
				}
			}
			return result;
		}

		public bool removePokemon(int boxID, int pkmnID)
		{
			byte x = (byte)(boxID % pokemons.GetLength(0));		//Core.STORAGEBOXES);
			byte y = (byte)(pkmnID % pokemons.GetLength(1));
			if (pokemons[x,y].IsNotNullOrNone())
			{
				Pokemons[pkmnID] = new Pokemon();
				pokemons[x, y] = new Pokemon();
				return true;
			}
			return false;
		}
		//public bool Switch_PC_And_Party_Pokemon(PokemonEssentials.Interface.PokeBattle.ITrainer player, int PartyID, int PCBoxID)
		//{
		//	try
		//	{
		//      PokemonEssentials.Interface.PokeBattle.IPokemon PartyHolder = player.party[PartyID];
		//		player.party[PartyID] = player.PC.Pokemons[PCBoxID];
		//		Pokemons[PCBoxID] = PartyHolder;
		//		pokemons[ActiveBox, PCBoxID] = PartyHolder;
		//		return true;
		//	}
		//	catch
		//	{
		//		//If could not switch pokemons
		//		return false;
		//	}
		//}
		public bool addPokemon(int box, int position, IPokemon pokemon)
		{
			try
			{
				pokemons[box, position] = pokemon;
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Add a new pokemon directly to active box.
		/// </summary>
		/// <param name="acquiredPokemon"></param>
		/// <returns>
		/// Returns position of stored pokemon.
		/// If pokemon could not be added return null.
		/// </returns>
		public KeyValuePair<int,int>? addPokemon(IPokemon acquiredPokemon)
		{
			//attempt to add to the earliest available opening in active box. no array packing needed.
			if (hasSpace())
			{
				KeyValuePair<int, int> kv = new KeyValuePair<int, int>(ActiveBox, getIndexOfFirstEmpty().Value);
				//Pokemons[getIndexOfFirstEmpty().Value] = acquiredPokemon;
				pokemons[kv.Key, kv.Value] = acquiredPokemon;
				return kv;
			}
			//if could not add a pokemon, return false. Party and PC are both full.
			return null;
		}

		public void swapPokemon(int box1, int pos1, int box2, int pos2)
		{
			IPokemon temp = pokemons[box1, pos1];
			pokemons[box1, pos1] = pokemons[box2, pos2];
			pokemons[box2, pos2] = temp;
		}

		int IPCPokemonStorage.maxPokemon(int box)
		{
			throw new NotImplementedException();
		}

		int IPCPokemonStorage.FirstFreePos(int box)
		{
			throw new NotImplementedException();
		}

		bool IPCPokemonStorage.Copy(int boxDst, int indexDst, int boxSrc, int indexSrc)
		{
			throw new NotImplementedException();
		}

		bool IPCPokemonStorage.Move(int boxDst, int indexDst, int boxSrc, int indexSrc)
		{
			throw new NotImplementedException();
		}

		void IPCPokemonStorage.MoveCaughtToParty(IPokemon pkmn)
		{
			throw new NotImplementedException();
		}

		bool IPCPokemonStorage.MoveCaughtToBox(IPokemon pkmn, int box)
		{
			throw new NotImplementedException();
		}

		int IPCPokemonStorage.StoreCaught(IPokemon pkmn)
		{
			KeyValuePair<int, int>? kv = addPokemon(pkmn);
			return kv == null ? -1 : kv.Value.Key; //return the box
		}

		void IPCPokemonStorage.Delete(int box, int index)
		{
			removePokemon(box, index);
		}
		#endregion
	}
}

/*namespace PokemonUnity
{
	public partial class Game
	{
		public class TrainerPC
		{
			//public static PC
			private Player trainer { get; set; }
			private int activeBox { get; set; }
			//public string Name { get { return Game.GameData.PC_boxNames[activeBox] ?? "Box " + (activeBox + 1).ToString(); } }
			public string Name { get { return Game.GameData.PC.BoxNames[activeBox] ?? "Box " + (activeBox + 1).ToString(); } }
			//public int Texture { get { return Game.GameData.PC_boxTexture[activeBox]; } }
			public int Texture { get { return Game.GameData.PC.BoxTextures[activeBox]; } }
			public Pokemon[] Pokemons
			{
				get
				{
					Pokemon[] p = new Pokemon[30];
					for (int t = 0; t < 30; t++)
					{
						//p[t] = Game.GameData.PC_Poke[activeBox, t];
						p[t] = Game.GameData.PC.AllBoxes[activeBox][t];
					}
					return p;
				}
			}
			/// <summary>
			/// </summary>
			/// ToDo: Add filter to add/remove items...
			//public List<Items> Items { get { return Game.GameData.PC_Items; } set { Game.GameData.PC_Items = value; } }
			public List<Items> Items { get { return Game.GameData.PC.Items; } set { Game.GameData.PC.Items = value; } }

			public TrainerPC this[int i]
			{
				get
				{
					i = i % Core.STORAGEBOXES;
					this.activeBox = i;
					//Pokemon[] p = new Pokemon[30];
					//for (int t = 0; t < 30; t++)
					//{
					//	p[t] = Game.GameData.PC_Poke[i, t];
					//}
					//this.Pokemons = p;
					//this.Texture = Game.GameData.PC_boxTexture[i];
					//this.Name = Game.GameData.PC_boxNames[i] ?? "Box " + (i + 1).ToString();
					return this;
				}
			}

			public TrainerPC()
			{
			}

			public TrainerPC(Player t, int? box = null) : this()
			{
				trainer = t;
				if (box.HasValue)
					activeBox = box.Value % Core.STORAGEBOXES;
			}

			public bool hasSpace()
			{
				if (getBoxCount().HasValue && getBoxCount().Value < 30) return true;
				else return false;
			}

			public int? getBoxCount()
			{
				int result = 0;
				for (int i = 0; i < Pokemons.Length; i++)
				{
					if (Pokemons[i] != null || Pokemons[i].Species != PokemonUnity.Pokemons.NONE)
					{
						result += 1;
					}
				}
				return result;
			}

			public int? getIndexOfFirstEmpty()
			{
				//int result = 0;
				for (int i = 0; i < Pokemons.Length; i++)
				{
					if (Pokemons[i] == null || Pokemons[i].Species == PokemonUnity.Pokemons.NONE)
					{
						return i;
					}
				}
				return null;
			}

			/*public int getBoxCount(int box)
			{
				int result = 0;
				for (int i = 0; i < Pokemons[box].Length; i++)
				{
					if (Pokemons[box,i] != null || Party[i].Species != Pokemons.NONE)
					{
						result += 1;
					}
				}
				return result;
			}* /

			/// <summary>
			/// Add a new pokemon directly to active box.
			/// If pokemon could not be added return false.
			/// </summary>
			/// <param name="acquiredPokemon"></param>
			/// <returns></returns>
			public bool addPokemon(Pokemon acquiredPokemon)
			{
				//attempt to add to the earliest available opening in active box. no array packing needed.
				if (hasSpace())
				{
					//Pokemons[getIndexOfFirstEmpty().Value] = acquiredPokemon;
					//Game.GameData.PC_Poke[activeBox, getIndexOfFirstEmpty().Value] = acquiredPokemon;
					Game.GameData.PC.AllBoxes[activeBox][getIndexOfFirstEmpty().Value] = acquiredPokemon;
					return true;
				}
				//if could not add a pokemon, return false. Party and PC are both full.
				return false;
			}

			public void swapPokemon(int box1, int pos1, int box2, int pos2)
			{
				//Pokemon temp = Game.GameData.PC_Poke[box1, pos1];
				//Game.GameData.PC_Poke[box1, pos1] = Game.GameData.PC_Poke[box2, pos2];
				//Game.GameData.PC_Poke[box2, pos2] = temp;
				Pokemon temp = Game.GameData.PC.AllBoxes[box1][pos1];
				Game.GameData.PC.AllBoxes[box1][pos1] = Game.GameData.PC.AllBoxes[box2][pos2];
				Game.GameData.PC.AllBoxes[box2][pos2] = temp;
			}
		}
	}
}*/