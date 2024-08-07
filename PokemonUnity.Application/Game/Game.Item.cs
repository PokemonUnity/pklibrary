﻿using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUnity;
using PokemonUnity.Inventory;
using PokemonUnity.Character;
using PokemonUnity.Overworld;
using PokemonUnity.Monster;
using PokemonUnity.Monster.Data;
using PokemonEssentials.Interface;
using PokemonEssentials.Interface.Item;
using PokemonEssentials.Interface.Screen;
using PokemonEssentials.Interface.EventArg;
using PokemonEssentials.Interface.PokeBattle;
using PokemonEssentials.Interface.Battle;

namespace PokemonUnity//.Inventory
{
	public partial class Game : IGameItem, IItemCheck
	{
		#region Item Check
		public const int ITEMID        = 0;
		public const int ITEMNAME      = 1;
		public const int ITEMPLURAL    = 2;
		public const int ITEMPOCKET    = 3;
		public const int ITEMPRICE     = 4;
		public const int ITEMDESC      = 5;
		public const int ITEMUSE       = 6;
		public const int ITEMBATTLEUSE = 7;
		public const int ITEMTYPE      = 8;
		public const int ITEMMACHINE   = 9;

		/// <summary>
		/// </summary>
		/// <param name="move"></param>
		/// <returns></returns>
		/// <seealso cref="HiddenMoves"/>
		public bool IsHiddenMove (Moves move) {
			//if (Kernal.ItemData == null) return false;
			////for (int i = 0; i < Kernal.ItemData.Count; i++) {
			//for (int i = 0; i < Kernal.MachineData.Count; i++) {
			//  //if (!IsHiddenMachine(i)) continue;
			//  //if(Kernal.ItemData[i].Pocket == ItemPockets.MACHINE)
			//  //atk=Kernal.ItemData[i][ITEMMACHINE];
			//  MachineData atk = Kernal.MachineData[i]; //HiddenMachine is not HiddenMove
			//  if (atk.Type != MachineData.MachineType.HiddenMachine && move==atk.Move) return true;
			//}
			Moves[] hidden = new Moves[] {
				Moves.SURF,
				Moves.CUT,
				Moves.STRENGTH,
				Moves.FLASH,
				Moves.FLY,
				Moves.WHIRLPOOL,
				Moves.WATERFALL,
				//Moves.RIDE,
				Moves.DIVE,
				Moves.ROCK_CLIMB,
				Moves.ROCK_SMASH,
				Moves.HEADBUTT,
				Moves.DEFOG };
			//return false;
			return hidden.Contains(move);
		}

		public int GetPrice(Items item) {
			return Kernal.ItemData[item].Price; //[ITEMPRICE];
		}

		public ItemPockets? GetPocket(Items item) {
			return Kernal.ItemData[item].Pocket; //[ITEMPOCKET];
		}

		/// <summary>
		/// Important items can't be sold, given to hold, or tossed.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool IsImportantItem (Items item) {
			return Kernal.ItemData.ContainsKey(item) && (IsKeyItem(item) ||
				IsHiddenMachine(item) ||
				(Core.INFINITETMS && IsTechnicalMachine(item)));
		}

		public bool IsMachine (Items item) {
			return Kernal.ItemData[item].Category == ItemCategory.ALL_MACHINES || (IsTechnicalMachine(item) || IsHiddenMachine(item));
		}

		public bool IsTechnicalMachine (Items item) {
			//return Kernal.ItemData.ContainsKey(item) && (Kernal.ItemData[item][ITEMUSE]==3);
			Items[] TMs = new Items[] { Items.TM_ALL }; //Items.TM01, Items.TM02, Items.TM03, Items.TM04, Items.TM05, Items.TM06, Items.TM07, Items.TM08, Items.TM09, Items.TM10, Items.TM11, Items.TM12, Items.TM13, Items.TM14, Items.TM15, Items.TM16, Items.TM17, Items.TM18, Items.TM19, Items.TM20, Items.TM21, Items.TM22, Items.TM23, Items.TM24, Items.TM25, Items.TM26, Items.TM27, Items.TM28, Items.TM29, Items.TM30, Items.TM31, Items.TM32, Items.TM33, Items.TM34, Items.TM35, Items.TM36, Items.TM37, Items.TM38, Items.TM39, Items.TM40, Items.TM41, Items.TM42, Items.TM43, Items.TM44, Items.TM45, Items.TM46, Items.TM47, Items.TM48, Items.TM49, Items.TM50, Items.TM51, Items.TM52, Items.TM53, Items.TM54, Items.TM55, Items.TM56, Items.TM57, Items.TM58, Items.TM59, Items.TM60, Items.TM61, Items.TM62, Items.TM63, Items.TM64, Items.TM65, Items.TM66, Items.TM67, Items.TM68, Items.TM69, Items.TM70, Items.TM71, Items.TM72, Items.TM73, Items.TM74, Items.TM75, Items.TM76, Items.TM77, Items.TM78, Items.TM79, Items.TM80, Items.TM81, Items.TM82, Items.TM83, Items.TM84, Items.TM85, Items.TM86, Items.TM87, Items.TM88, Items.TM89, Items.TM90, Items.TM91, Items.TM92, Items.TM93, Items.TM94, Items.TM95, Items.TM96, Items.TM97, Items.TM98, Items.TM99, Items.TM100 };
			return TMs.Contains(item);
		}

		public bool IsHiddenMachine (Items item) {
			//return Kernal.ItemData.ContainsKey(item) && (Kernal.ItemData[item][ITEMUSE]==4);
			Items[] HMs = new Items[] { Items.HM01, Items.HM02, Items.HM03, Items.HM04, Items.HM05, Items.HM06, Items.HM07, Items.HM08 };
			return HMs.Contains(item);
		}

		public bool IsMail (Items item) {
			return Kernal.ItemData.ContainsKey(item) && (ItemData.IsLetter(item)); //[ITEMTYPE]==1 || Kernal.ItemData[item][ITEMTYPE]==2
		}

		public bool IsSnagBall (Items item) {
			//return Kernal.ItemData.ContainsKey(item) && (Kernal.ItemData[item].IsPokeBall || Kernal.ItemData[item].Pocket == ItemPockets.POKEBALL) && //[ITEMTYPE]==3
			//	Global.snagMachine; //Kernal.ItemData[item][ITEMTYPE]==4 && 4: SnagBall Item
			return IsPokeBall(item) && Global.snagMachine;
		}

		public bool IsPokeBall (Items item) {
			return Kernal.ItemData.ContainsKey(item) && (ItemData.IsPokeBall(item) || Kernal.ItemData[item].Pocket == ItemPockets.POKEBALL);//[ITEMTYPE]==4
		}

		public bool IsBerry (Items item) {
			return Kernal.ItemData.ContainsKey(item) && ItemData.IsBerry(item); //[ITEMTYPE]==5
		}

		public bool IsKeyItem (Items item) {
			return Kernal.ItemData.ContainsKey(item) && (Kernal.ItemData[item].Pocket == ItemPockets.KEY);//[ITEMTYPE]==6
		}

		public bool IsGem (Items item) {
			Items[] gems=new Items[] {Items.FIRE_GEM,Items.WATER_GEM,Items.ELECTRIC_GEM,Items.GRASS_GEM,Items.ICE_GEM,
				Items.FIGHTING_GEM,Items.POISON_GEM,Items.GROUND_GEM,Items.FLYING_GEM,Items.PSYCHIC_GEM,
				Items.BUG_GEM,Items.ROCK_GEM,Items.GHOST_GEM,Items.DRAGON_GEM,Items.DARK_GEM,
				Items.STEEL_GEM,Items.NORMAL_GEM,Items.FAIRY_GEM};
			//foreach (Items i in gems) {
			//  if (item == i) return true;
			//}
			//return false;
			return gems.Contains(item) || Kernal.ItemData[item].Category == ItemCategory.JEWELS;
		}

		public bool IsEvolutionStone (Items item) {
			Items[] stones=new Items[] {Items.FIRE_STONE,Items.THUNDER_STONE,Items.WATER_STONE,Items.LEAF_STONE,Items.MOON_STONE,
					Items.SUN_STONE,Items.DUSK_STONE,Items.DAWN_STONE,Items.SHINY_STONE};
			//foreach (Items i in stones) {
			//  if (item == i) return true;
			//}
			//return false;
			return stones.Contains(item) || Kernal.ItemData[item].Category == ItemCategory.EVOLUTION;
		}

		public bool IsMegaStone (Items item) {   // Does NOT include Red Orb/Blue Orb
			Items[] stones=new Items[] { Items.ABOMASITE,Items.ABSOLITE,Items.AERODACTYLITE,Items.AGGRONITE,Items.ALAKAZITE,
					Items.ALTARIANITE,Items.AMPHAROSITE,Items.AUDINITE,Items.BANETTITE,Items.BEEDRILLITE,
					Items.BLASTOISINITE,Items.BLAZIKENITE,Items.CAMERUPTITE,Items.CHARIZARDITE_X,Items.CHARIZARDITE_Y,
					Items.DIANCITE,Items.GALLADITE,Items.GARCHOMPITE,Items.GARDEVOIRITE,Items.GENGARITE,
					Items.GLALITITE,Items.GYARADOSITE,Items.HERACRONITE,Items.HOUNDOOMINITE,Items.KANGASKHANITE,
					Items.LATIASITE,Items.LATIOSITE,Items.LOPUNNITE,Items.LUCARIONITE,Items.MANECTITE,
					Items.MAWILITE,Items.MEDICHAMITE,Items.METAGROSSITE,Items.MEWTWONITE_X,Items.MEWTWONITE_Y,
					Items.PIDGEOTITE,Items.PINSIRITE,Items.SABLENITE,Items.SALAMENCITE,Items.SCEPTILITE,
					Items.SCIZORITE,Items.SHARPEDONITE,Items.SLOWBRONITE,Items.STEELIXITE,Items.SWAMPERTITE,
					Items.TYRANITARITE,Items.VENUSAURITE};
			//foreach (Items i in stones) {
			//  if (item == i) return true;
			//}
			//return false;
			return stones.Contains(item) || Kernal.ItemData[item].Category == ItemCategory.MEGA_STONES;
		}

		public bool IsMulch (Items item) {
			Items[] mulches= new Items[] { Items.GROWTH_MULCH,Items.DAMP_MULCH,Items.STABLE_MULCH,Items.GOOEY_MULCH };
			//foreach (Items i in mulches) {
			//  if (item == i) return true;
			//}
			//return false;
			return mulches.Contains(item) || Kernal.ItemData[item].Category == ItemCategory.MULCH;
		}
		#endregion Item Check

		#region Game Item Interface
		public void ChangeLevel(IPokemon pokemon,int newlevel,IScene scene) {
			if (newlevel<1) newlevel=1;
			if (newlevel>Core.MAXIMUMLEVEL) newlevel=Core.MAXIMUMLEVEL;
			if (pokemon.Level>newlevel) {
				int attackdiff=pokemon.ATK;
				int defensediff=pokemon.DEF;
				int speeddiff=pokemon.SPE;
				int spatkdiff=pokemon.SPA;
				int spdefdiff=pokemon.SPD;
				int totalhpdiff=pokemon.TotalHP;
				//pokemon.Level=newlevel;
				(pokemon as Pokemon).SetLevel((byte)newlevel);
				//pokemon.Exp=Experience.GetStartExperience(pokemon.GrowthRate, newlevel);
				pokemon.calcStats();
				scene.Refresh();
				GameMessage.Message(Game._INTL("{1} was downgraded to Level {2}!",pokemon.Name,pokemon.Level));
				attackdiff=pokemon.ATK-attackdiff;
				defensediff=pokemon.DEF-defensediff;
				speeddiff=pokemon.SPE-speeddiff;
				spatkdiff=pokemon.SPA-spatkdiff;
				spdefdiff=pokemon.SPD-spdefdiff;
				totalhpdiff=pokemon.TotalHP-totalhpdiff;
				TopRightWindow(Game._INTL("Max. HP<r>{1}\r\nAttack<r>{2}\r\nDefense<r>{3}\r\nSp. Atk<r>{4}\r\nSp. Def<r>{5}\r\nSpeed<r>{6}",
					totalhpdiff,attackdiff,defensediff,spatkdiff,spdefdiff,speeddiff));
				TopRightWindow(Game._INTL("Max. HP<r>{1}\r\nAttack<r>{2}\r\nDefense<r>{3}\r\nSp. Atk<r>{4}\r\nSp. Def<r>{5}\r\nSpeed<r>{6}",
					pokemon.TotalHP,pokemon.ATK,pokemon.DEF,pokemon.SPA,pokemon.SPD,pokemon.SPE));
			} else if (pokemon.Level==newlevel) {
				GameMessage.Message(Game._INTL("{1}'s level remained unchanged.",pokemon.Name));
			} else {
				int attackdiff=pokemon.ATK;
				int defensediff=pokemon.DEF;
				int speeddiff=pokemon.SPE;
				int spatkdiff=pokemon.SPA;
				int spdefdiff=pokemon.SPD;
				int totalhpdiff=pokemon.TotalHP;
				int oldlevel=pokemon.Level;
				//pokemon.Level=newlevel;
				(pokemon as Pokemon).SetLevel((byte)newlevel);
				//pokemon.Exp = Experience.GetStartExperience(pokemon.GrowthRate, newlevel);
				pokemon.ChangeHappiness(HappinessMethods.LEVELUP);
				pokemon.calcStats();
				scene.Refresh();
				GameMessage.Message(Game._INTL("{1} was elevated to Level {2}!",pokemon.Name,pokemon.Level));
				attackdiff=pokemon.ATK-attackdiff;
				defensediff=pokemon.DEF-defensediff;
				speeddiff=pokemon.SPE-speeddiff;
				spatkdiff=pokemon.SPA-spatkdiff;
				spdefdiff=pokemon.SPD-spdefdiff;
				totalhpdiff=pokemon.TotalHP-totalhpdiff;
				TopRightWindow(Game._INTL("Max. HP<r>+{1}\r\nAttack<r>+{2}\r\nDefense<r>+{3}\r\nSp. Atk<r>+{4}\r\nSp. Def<r>+{5}\r\nSpeed<r>+{6}",
					totalhpdiff,attackdiff,defensediff,spatkdiff,spdefdiff,speeddiff));
				TopRightWindow(Game._INTL("Max. HP<r>{1}\r\nAttack<r>{2}\r\nDefense<r>{3}\r\nSp. Atk<r>{4}\r\nSp. Def<r>{5}\r\nSpeed<r>{6}",
					pokemon.TotalHP,pokemon.ATK,pokemon.DEF,pokemon.SPA,pokemon.SPD,pokemon.SPE));
				//Moves[] movelist=pokemon.getMoveList();
				//foreach (Moves i in pokemon.getMoveList(LearnMethod.levelup)) { //movelist
				foreach (KeyValuePair<Moves,int> i in Kernal.PokemonMovesData[pokemon.Species].LevelUp) {
					if (i.Value==pokemon.Level) {		// Learned a new move
						LearnMove(pokemon,i.Key,true);
					}
				}
				Pokemons newspecies=CheckEvolution(pokemon)[0];
				if (newspecies>0) {
					FadeOutInWithMusic(99999, block: () => {
						IPokemonEvolutionScene evo=Scenes.EvolvingScene.initialize(); //new PokemonEvolutionScene();
						evo.StartScreen(pokemon,newspecies);
						evo.Evolution();
						evo.EndScreen();
					});
				}
			}
		}

		public int ItemRestoreHP(IPokemon pokemon,int restorehp) {
			int newhp=pokemon.HP+restorehp;
			if (newhp>pokemon.TotalHP) newhp=pokemon.TotalHP;
			int hpgain=newhp-pokemon.HP;
			pokemon.HP=newhp;
			return hpgain;
		}

		public bool HPItem(IPokemon pokemon,int restorehp,PokemonEssentials.Interface.Screen.IHasDisplayMessage scene) {
			if (pokemon.HP<=0 || pokemon.HP==pokemon.TotalHP || pokemon.isEgg) {
				scene.Display(Game._INTL("It won't have any effect."));
				return false;
			} else {
				int hpgain=ItemRestoreHP(pokemon,restorehp);
				scene.Refresh();
				scene.Display(Game._INTL("{1}'s HP was restored by {2} points.",pokemon.Name,hpgain));
				return true;
			}
		}

		public bool BattleHPItem(IPokemon pokemon,IBattler battler,int restorehp, PokemonEssentials.Interface.Screen.IHasDisplayMessage scene) {
			if (pokemon.HP<=0 || pokemon.HP==pokemon.TotalHP || pokemon.isEgg) {
				scene.Display(Game._INTL("But it had no effect!"));
				return false;
			} else {
				int hpgain=ItemRestoreHP(pokemon,restorehp);
				if (battler.IsNotNullOrNone()) battler.HP=pokemon.HP;
				scene.Refresh();
				scene.Display(Game._INTL("{1}'s HP was restored.",pokemon.Name,hpgain));
				return true;
			}
		}

		public int JustRaiseEffortValues(IPokemon pokemon,Stats ev,int evgain) {
			int totalev=0;
			for (int i = 0; i < 6; i++) {
				totalev+=pokemon.EV[i];
			}
			if (totalev+evgain>Pokemon.EVLIMIT) {
				//  Bug Fix: must use "-=" instead of "="
				evgain-=totalev+evgain-Pokemon.EVLIMIT;
			}
			if (pokemon.EV[(int)ev]+evgain>Pokemon.EVSTATLIMIT) {
				//  Bug Fix: must use "-=" instead of "="
				evgain-=pokemon.EV[(int)ev]+evgain-Pokemon.EVSTATLIMIT;
			}
			if (evgain>0) {
				//pokemon.EV[ev]+=evgain;
				pokemon.EV[(int)ev]=(byte)(pokemon.EV[(int)ev]+evgain);
				pokemon.calcStats();
			}
			return evgain;
		}

		public int RaiseEffortValues(IPokemon pokemon,Stats ev,int evgain=10,bool evlimit=true) {
			if (pokemon.EV[(int)ev]>=100 && evlimit) {
				return 0;
			}
			int totalev=0;
			for (int i = 0; i < 6; i++) {
				totalev+=pokemon.EV[i];
			}
			if (totalev+evgain>Pokemon.EVLIMIT) {
				evgain=Pokemon.EVLIMIT-totalev;
			}
			if (pokemon.EV[(int)ev]+evgain>Pokemon.EVSTATLIMIT) {
				evgain=Pokemon.EVSTATLIMIT-pokemon.EV[(int)ev];
			}
			if (evlimit && pokemon.EV[(int)ev]+evgain>100) {
				evgain=100-pokemon.EV[(int)ev];
			}
			if (evgain>0) {
				//pokemon.EV[ev]+=evgain;
				pokemon.EV[(int)ev]=(byte)(pokemon.EV[(int)ev]+evgain);
				pokemon.calcStats();
			}
			return evgain;
		}

		public bool RaiseHappinessAndLowerEV(IPokemon pokemon, PokemonEssentials.Interface.Screen.IHasDisplayMessage scene,Stats ev,string[] messages) {
			bool h=(pokemon.Happiness<255);
			bool e=(pokemon.EV[(int)ev]>0);
			if (!h && !e) {
				scene.Display(Game._INTL("It won't have any effect."));
				return false;
			}
			if (h) {
				pokemon.ChangeHappiness(HappinessMethods.EVBERRY);
			}
			if (e) {
				pokemon.EV[(int)ev]-=10;
				if (pokemon.EV[(int)ev]<0) pokemon.EV[(int)ev]=0;
				pokemon.calcStats();
			}
			scene.Refresh();
			scene.Display(messages[2-(h ? 0 : 1)-(e ? 0 : 2)]);
			return true;
		}

		public int RestorePP(IPokemon pokemon,int move,int pp) {
			if (pokemon.moves[move].id==0) return 0;
			if (pokemon.moves[move].TotalPP==0) return 0;
			int newpp=pokemon.moves[move].PP+pp;
			if (newpp>pokemon.moves[move].TotalPP) {
				newpp=pokemon.moves[move].TotalPP;
			}
			int oldpp=pokemon.moves[move].PP;
			pokemon.moves[move].PP=(byte)newpp;
			return newpp-oldpp;
		}

		public int BattleRestorePP(IPokemon pokemon,IBattler battler,int move,int pp) {
			int ret=RestorePP(pokemon,move,pp);
			if (ret>0) {
				if (battler.IsNotNullOrNone()) battler.SetPP(battler.moves[move],pokemon.moves[move].PP);
			}
			return ret;
		}

		public bool BikeCheck() {
			if (Global.surfing ||
				(!Global.bicycle && Terrain.onlyWalk((this is PokemonEssentials.Interface.Field.IGameField f ? f.GetTerrainTag() : (Terrains?)null)))) {
				GameMessage.Message(Game._INTL("Can't use that here."));
				return false;
			}
			if (GamePlayer.HasDependentEvents()) {
				GameMessage.Message(Game._INTL("It can't be used when you have someone with you."));
				return false;
			}
			if (Global.bicycle) {
				//if (GetMetadata(GameMap.map_id,MetadataBicycleAlways)) {
				if (GetMetadata(GameMap is IGameMapOrgBattle gmo ? gmo.map_id : 0).Map.BicycleAlways) {
					GameMessage.Message(Game._INTL("You can't dismount your Bike here."));
					return false;
				}
				return true;
			} else {
				//bool? val=GetMetadata(GameMap.map_id,MetadataBicycle);
				bool? val=GetMetadata(GameMap is IGameMapOrgBattle gmo0 ? gmo0.map_id : 0).Map.Bicycle;
				//if (val == null) val=GetMetadata(GameMap.map_id,MetadataOutdoor);
				if (val == null) val=GetMetadata(GameMap is IGameMapOrgBattle gmo1 ? gmo1.map_id : 0).Map.Outdoor;
				if (val == null) {
					GameMessage.Message(Game._INTL("Can't use that here."));
					return false;
				}
				return true;
			}
		}

		public IGameCharacter ClosestHiddenItem() {
			IList<IGameCharacter> result = new List<IGameCharacter>();
			float playerX=GamePlayer.x;
			float playerY=GamePlayer.y;
			foreach (IGameEvent @event in GameMap.events.Values) {
				if (@event.name!="HiddenItem") continue;
				if (Math.Abs(playerX-@event.x)>=8) continue;
				if (Math.Abs(playerY-@event.y)>=6) continue;
				if (GameSelfSwitches[(ISelfSwitchVariable)new SelfSwitchVariable(GameMap is IGameMapOrgBattle gmo ? gmo.map_id : 0,@event.id,"A")]) continue;
				result.Add(@event);
			}
			if (result.Count==0) return null;
			IGameCharacter ret=null;
			float retmin=0;
			foreach (IGameCharacter @event in result) {
				float dist=Math.Abs(playerX-@event.x)+Math.Abs(playerY-@event.y);
				if (ret == null || retmin>dist) {
					ret=@event;
					retmin=dist;
				}
			}
			return ret;
		}

		public void UseKeyItemInField(Items item) {
			if (!ItemHandlers.triggerUseInField(item)) {
				GameMessage.Message(Game._INTL("Can't use that here."));
			}
		}

		public bool SpeciesCompatible (Pokemons species,Moves move) {
			//bool ret=false;
			if (species<=0) return false;
			//data=load_data("Data/tm.dat");
			//if (!data[move]) return false;
			//return data[move].Any(item => item==species);
			return Kernal.PokemonMovesData[species].Machine.Contains(move);
		}

		public int ForgetMove(IPokemon pokemon,Moves moveToLearn) {
			int ret=-1;
			FadeOutIn(99999, block: () => {
				IPokemonSummaryScene scene=Scenes.Summary; //new PokemonSummaryScene();
				IPokemonSummaryScreen screen=Screens.Summary.initialize(scene); //new PokemonSummary(scene);
				ret=screen.StartForgetScreen(pokemon,0,moveToLearn);
			});
			return ret;
		}

		public bool LearnMove(IPokemon pokemon,Moves move,bool ignoreifknown=false,bool bymachine=false) {
			if (!pokemon.IsNotNullOrNone()) return false;
			string movename=Game._INTL(move.ToString(TextScripts.Name));
			if (pokemon.isEgg && !Core.DEBUG) {
				GameMessage.Message(Game._INTL("{1} can't be taught to an Egg.",movename));
				return false;
			}
			if (pokemon is IPokemonShadowPokemon p && p.isShadow) {
				GameMessage.Message(Game._INTL("{1} can't be taught to this Pokémon.",movename));
				return false;
			}
			string pkmnname=pokemon.Name;
			for (int i = 0; i < 4; i++) {
				if (pokemon.moves[i].id==move) {
					if (!ignoreifknown) GameMessage.Message(Game._INTL("{1} already knows {2}.",pkmnname,movename));
					return false;
				}
				if (pokemon.moves[i].id==0) {
					pokemon.moves[i]=new Attack.Move(move);
					GameMessage.Message(Game._INTL("\\se[]{1} learned {2}!\\se[MoveLearnt]",pkmnname,movename));
					return true;
				}
			}
			do { //;loop
				GameMessage.Message(Game._INTL("{1} wants to learn the move {2}.",pkmnname,movename));
				GameMessage.Message(Game._INTL("However, {1} already knows four moves.",pkmnname));
				if (GameMessage.ConfirmMessage(Game._INTL("Should a move be deleted and replaced with {1}?",movename))) {
					GameMessage.Message(Game._INTL("Which move should be forgotten?"));
					int forgetmove=ForgetMove(pokemon,move);
					if (forgetmove>=0) {
						string oldmovename=Game._INTL(pokemon.moves[forgetmove].id.ToString(TextScripts.Name));
						int oldmovepp=pokemon.moves[forgetmove].PP;
						pokemon.moves[forgetmove]=new Attack.Move(move); // Replaces current/total PP
						if (bymachine) pokemon.moves[forgetmove].PP=Math.Min(oldmovepp,pokemon.moves[forgetmove].TotalPP);
						GameMessage.Message(Game._INTL("\\se[]1,\\wt[16] 2, and\\wt[16]...\\wt[16] ...\\wt[16] ... Ta-da!\\se[balldrop]"));
						GameMessage.Message(Game._INTL("\\se[]{1} forgot how to use {2}. And... {1} learned {3}!\\se[MoveLearnt]",pkmnname,oldmovename,movename));
						return true;
					} else if (GameMessage.ConfirmMessage(Game._INTL("Give up on learning the move {1}?",movename))) {
						GameMessage.Message(Game._INTL("{1} did not learn {2}.",pkmnname,movename));
						return false;
					}
				} else if (GameMessage.ConfirmMessage(Game._INTL("Give up on learning the move {1}?",movename))) {
					GameMessage.Message(Game._INTL("{1} did not learn {2}.",pkmnname,movename));
					return false;
				}
			} while (true);
		}

		public bool CheckUseOnPokemon(Items item,IPokemon pokemon,IScreen screen) {
			return pokemon.IsNotNullOrNone() && !pokemon.isEgg;
		}

		public bool ConsumeItemInBattle(IBag bag,Items item) {
			if (item!=0 && Kernal.ItemData[item].Flags.Consumable && //!=3 disappear after use
				//Kernal.ItemData[item].Flags!=4 && //used on enemy and disappears after use (i.e. pokeball)
				Kernal.ItemData[item].Flags.Useable_In_Battle) { //!=0 cannot be used in battle
				//  Delete the item just used from stock
				return Bag.DeleteItem(item);
			}
			return false;
		}

		/// <summary>
		/// Only called when in the party screen and having chosen an item to be used on
		/// the selected Pokémon
		/// </summary>
		/// <param name="item"></param>
		/// <param name="pokemon"></param>
		/// <param name="scene"></param>
		/// <returns></returns>
		public bool UseItemOnPokemon(Items item,IPokemon pokemon,IPartyDisplayScreen scene) {
			//if (Kernal.ItemData[item][ITEMUSE]==3 || Kernal.ItemData[item][ITEMUSE]==4) {		// TM or HM
			if (IsMachine(item)) {
				Moves machine=Kernal.MachineData[(int)item].Move;
				if (machine==Moves.NONE) return false;
				string movename=Game._INTL(machine.ToString(TextScripts.Name));
				if (pokemon is IPokemonShadowPokemon p && p.isShadow) { //? rescue false
					GameMessage.Message(Game._INTL("Shadow Pokémon can't be taught any moves."));
				} else if (!pokemon.isCompatibleWithMove(machine)) {
					GameMessage.Message(Game._INTL("{1} and {2} are not compatible.",pokemon.Name,movename));
					GameMessage.Message(Game._INTL("{1} can't be learned.",movename));
				} else {
					if (IsHiddenMachine(item)) {
						GameMessage.Message(Game._INTL("\\se[accesspc]Booted up an HM."));
						GameMessage.Message(Game._INTL(@"It contained {1}.\1",movename));
					} else {
						GameMessage.Message(Game._INTL("\\se[accesspc]Booted up a TM."));
						GameMessage.Message(Game._INTL(@"It contained {1}.\1",movename));
					}
					if (GameMessage.ConfirmMessage(Game._INTL("Teach {1} to {2}?",movename,pokemon.Name))) {
						if (LearnMove(pokemon,machine,false,true)) {
							if (IsTechnicalMachine(item) && !Core.INFINITETMS) Bag.DeleteItem(item);
							return true;
						}
					}
				}
				//return false;
			} else {
				bool ret=ItemHandlers.triggerUseOnPokemon(item,pokemon,scene);
				scene.ClearAnnotations();
				scene.HardRefresh();
				if (ret && Kernal.ItemData[item].Flags.Consumable) {		//[ITEMUSE]==1 Usable on Pokémon, consumed
					Bag.DeleteItem(item);
				}
				if (Bag.Quantity(item)<=0) {
					GameMessage.Message(Game._INTL("You used your last {1}.",Game._INTL(item.ToString(TextScripts.Name))));
				}
				return ret;
			}
			GameMessage.Message(Game._INTL("Can't use that on {1}.",pokemon.Name));
			return false;
		}

		public int UseItem(IBag bag,Items item,IScene bagscene=null) {
			//bool found=false;
			//if (Kernal.ItemData[item][ITEMUSE]==3 || Kernal.ItemData[item][ITEMUSE]==4) {		// TM or HM
			if (IsMachine(item)) {
				Moves machine=Kernal.MachineData[(int)item].Move;
				if (machine==Moves.NONE) return 0;
				if (Trainer.pokemonCount==0) {
					GameMessage.Message(Game._INTL("There is no Pokémon."));
					return 0;
				}
				string movename=Game._INTL(machine.ToString(TextScripts.Name));
				if (IsHiddenMachine(item)) {
					GameMessage.Message(Game._INTL("\\se[accesspc]Booted up an HM."));
					GameMessage.Message(Game._INTL(@"It contained {1}.\1",movename));
				} else {
					GameMessage.Message(Game._INTL("\\se[accesspc]Booted up a TM."));
					GameMessage.Message(Game._INTL(@"It contained {1}.\1",movename));
				}
				if (!GameMessage.ConfirmMessage(Game._INTL("Teach {1} to a Pokémon?",movename))) {
					return 0;
				} else if (MoveTutorChoose(machine,null,true)) {
					if (IsTechnicalMachine(item) && !Core.INFINITETMS) bag.DeleteItem(item);
					return 1;
				} else {
					return 0;
				}
			} else if (Kernal.ItemData[item].Flags.Consumable) {		//[ITEMUSE]==1|[ITEMUSE]==5 Item is usable on a Pokémon
				if (Trainer.pokemonCount==0) {
					GameMessage.Message(Game._INTL("There is no Pokémon."));
					return 0;
				}
				bool ret=false;
				List<string> annot=null;
				if (IsEvolutionStone(item)) {
					annot=new List<string>();
					foreach (var pkmn in Trainer.party) {
						bool elig=CheckEvolution(pkmn,item).Length>0;
						annot.Add(elig ? Game._INTL("ABLE") : Game._INTL("NOT ABLE"));
					}
				}
				FadeOutIn(99999, block: () => {
					IPartyDisplayScene scene=Scenes.Party.initialize(); //new PokemonScreen_Scene();
					IPartyDisplayScreen screen=Screens.Party.initialize(scene,Trainer.party); //new PokemonScreen(scene,Trainer.party);
					screen.StartScene(Game._INTL("Use on which Pokémon?"),false,annot.ToArray());
					do { //;loop
						scene.SetHelpText(Game._INTL("Use on which Pokémon?"));
						int chosen=screen.ChoosePokemon();
						if (chosen>=0) {
							IPokemon pokemon=Trainer.party[chosen];
							if (!CheckUseOnPokemon(item,pokemon,screen) && this is IGameAudioPlay gap) {
								gap.PlayBuzzerSE();
							} else {
								ret=ItemHandlers.triggerUseOnPokemon(item,pokemon,(IHasDisplayMessage)screen);
								if (ret && Kernal.ItemData[item].Flags.Consumable) {		//[ITEMUSE]==1 Usable on Pokémon, consumed
									bag.DeleteItem(item);
								}
								if (bag.Quantity(item)<=0) {
									GameMessage.Message(Game._INTL("You used your last {1}.",Game._INTL(item.ToString(TextScripts.Name))));
									break;
								}
							}
						} else {
							ret=false;
							break;
						}
					} while (true);
					screen.EndScene();
					if (bagscene!=null) bagscene.Refresh();
				});
				return ret ? 1 : 0;
			} else if (Kernal.ItemData[item].Flags.Useable_Overworld) {		//[ITEMUSE]==2 Item is usable from bag
				int intret=(int)ItemHandlers.triggerUseFromBag(item);
				switch (intret) {
					case 0:
						return 0;
					case 1: // Item used
						return 1;
					case 2: // Item used, end screen
						return 2;
					case 3: // Item used, consume item
						bag.DeleteItem(item);
						return 1;
					case 4: // Item used, end screen and consume item
						bag.DeleteItem(item);
						return 2;
					default:
						GameMessage.Message(Game._INTL("Can't use that here."));
						return 0;
				}
			} else {
				GameMessage.Message(Game._INTL("Can't use that here."));
				return 0;
			}
		}

		public Items ChooseItem(int var=0,params Items[] args) {
			Items ret=0; //int?
			IBagScene scene=Scenes.Bag.initialize(); //new PokemonBag_Scene();
			IBagScreen screen=Screens.Bag.initialize(scene,Bag); //new PokemonBagScreen(scene,Bag);
			FadeOutIn(99999, block: () => {
				ret=screen.ChooseItemScreen();
			});
			if (var>0) GameVariables[var]=ret;
			return ret;
		}

		/// <summary>
		/// Shows a list of items to choose from, with the chosen item's ID being stored
		/// in the given Global Variable. Only items which the player has are listed.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="variable"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public Items ChooseItemFromList(string message,int variable,params Items[] args) {
			List<string> commands=new List<string>();
			List<Items> itemid=new List<Items>();
			foreach (Items item in args) {
				//if (hasConst(Items,item)) {
					Items id=(Items)item;
					if (Bag.Quantity(id)>0) {
						commands.Add(Game._INTL(id.ToString(TextScripts.Name)));
						itemid.Add(id);
					}
				//}
			}
			if (commands.Count==0) {
				GameVariables[variable]=0;
				return 0;
			}
			commands.Add(Game._INTL("Cancel"));
			itemid.Add(0);
			int ret=GameMessage.Message(message,commands.ToArray(),-1);
			if (ret<0 || ret>=commands.Count-1) {
				GameVariables[variable]=-1;
				return Items.NONE;
			} else {
				GameVariables[variable]=itemid[ret];
				return itemid[ret];
			}
		}

		public void TopRightWindow(string text) {
			IWindow_AdvancedTextPokemon window = null; //new Window_AdvancedTextPokemon(text);
			window.initialize(text);
			window.z=99999;
			window.width=198;
			window.y=0;
			window.x=Graphics.width-window.width;
			if (this is IGameAudioPlay gap) gap.PlayDecisionSE();
			do { //;loop
				Graphics?.update();
				Input.update();
				window.update();
				if (Input.trigger(PokemonUnity.Input.A)) {
					break;
				}
			} while (true);
			window.Dispose();
		}
		#endregion
	}
}