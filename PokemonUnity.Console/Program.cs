﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using PokemonUnity;
using PokemonUnity.Localization;
using PokemonUnity.Attack.Data;
using PokemonUnity.Combat;
using PokemonUnity.Inventory;
using PokemonUnity.Monster;
using PokemonUnity.Utility;
using PokemonEssentials.Interface;
using PokemonEssentials.Interface.Battle;
using PokemonEssentials.Interface.Item;
using PokemonEssentials.Interface.Field;
using PokemonEssentials.Interface.Screen;
using PokemonEssentials.Interface.PokeBattle;
using PokemonEssentials.Interface.PokeBattle.Effects;

namespace PokemonUnity.ConsoleApp
{
	class Program
	{
		public static void ResetSqlConnection(string db)
		{
			Game.con = (System.Data.IDbConnection)new System.Data.SQLite.SQLiteConnection(db);
			Game.ResetSqlConnection(db);
		}

		static void Main(string[] args)
		{
			System.Console.OutputEncoding = System.Text.Encoding.UTF8;
			Debugger.Instance.OnLog += GameDebug_OnLog;
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
			System.Console.WriteLine("######################################");
			System.Console.WriteLine("# Hello - Welcome to Console Battle! #");
			System.Console.WriteLine("######################################");

			string englishLocalization = "..\\..\\..\\LocalizationStrings.xml";
			//System.Console.WriteLine(System.IO.Directory.GetParent(englishLocalization).FullName);
			Game.LocalizationDictionary = new XmlStringRes(null); //new Debugger());
			Game.LocalizationDictionary.Initialize(englishLocalization, (int)Languages.English);

			//Game.ResetAndOpenSql(@"Data\veekun-pokedex.sqlite");
			ResetSqlConnection(Game.DatabasePath);//@"Data\veekun-pokedex.sqlite"

			//IPokeBattle_DebugSceneNoGraphics pokeBattle = new PokeBattleScene();
			//pokeBattle.initialize();


			IPokemon[] p1 = new IPokemon[] { new PokemonUnity.Monster.Pokemon(Pokemons.ABRA), new PokemonUnity.Monster.Pokemon(Pokemons.EEVEE) };
			IPokemon[] p2 = new IPokemon[] { new PokemonUnity.Monster.Pokemon(Pokemons.MONFERNO) }; //, new PokemonUnity.Monster.Pokemon(Pokemons.SEEDOT) };

			p1[0].moves[0] = new PokemonUnity.Attack.Move(Moves.POUND);
			p1[1].moves[0] = new PokemonUnity.Attack.Move(Moves.POUND);

			p2[0].moves[0] = new PokemonUnity.Attack.Move(Moves.POUND);
			//p2[1].moves[0] = new PokemonUnity.Attack.Move(Moves.POUND);

			//PokemonUnity.Character.TrainerData trainerData = new PokemonUnity.Character.TrainerData("FlakTester", true, 120, 002);
			//Game.GameData.Player = new PokemonUnity.Character.Player(trainerData, p1);
			//Game.GameData.Trainer = new Trainer("FlakTester", true, 120, 002);

			(p1[0] as PokemonUnity.Monster.Pokemon).SetNickname("Test1");
			(p1[1] as PokemonUnity.Monster.Pokemon).SetNickname("Test2");

			(p2[0] as PokemonUnity.Monster.Pokemon).SetNickname("OppTest1");
			//(p2[1] as PokemonUnity.Monster.Pokemon).SetNickname("OppTest2");

			//ITrainer player = new Trainer(Game.GameData.Trainer.name, TrainerTypes.PLAYER);
			ITrainer player = new Trainer("FlakTester",TrainerTypes.CHAMPION);
			//ITrainer pokemon = new Trainer("Wild Pokemon", TrainerTypes.WildPokemon);
			Game.GameData.Trainer = player;
			Game.GameData.Trainer.party = p1;

			//IBattle battle = new Battle(pokeBattle, Game.GameData.Trainer.party, p2, Game.GameData.Trainer, null, 2);
			IBattle battle = new Battle(new PokeBattleScene(), p1, p2, Game.GameData.Trainer, null);

			battle.rules.Add(BattleRule.SUDDENDEATH, false);
			battle.rules.Add("drawclause", false);
			battle.rules.Add(BattleRule.MODIFIEDSELFDESTRUCTCLAUSE, false);

			battle.weather = Weather.SUNNYDAY;

			battle.StartBattle(true);
		}

		private static void GameDebug_OnLog(object sender, OnDebugEventArgs e)
		{
			if (e != null || e != System.EventArgs.Empty)
				if (e.Error == true)
					System.Console.WriteLine("[ERR]: " + e.Message);
				else if (e.Error == false)
					System.Console.WriteLine("[WARN]: " + e.Message);
				else
					System.Console.WriteLine("[LOG]: " + e.Message);
		}
	}

	public class PokeBattleScene : IPokeBattle_DebugSceneNoGraphics, IPokeBattle_SceneNonInteractive //IPokeBattle_Scene,
	{
		private PokemonEssentials.Interface.PokeBattle.IBattle battle;
		private bool aborted;
		private bool abortable;
		private MenuCommands[] lastcmd;
		private int[] lastmove;
		private int messageCount = 0;

		public int Id { get { return 0; } }

		public PokeBattleScene()
		{
			initialize();
		}

		public IPokeBattle_DebugSceneNoGraphics initialize()
		{
			battle = null;
			lastcmd = new MenuCommands[] { 0, 0, 0, 0 };
			lastmove = new int[] { 0, 0, 0, 0 };
			//@pkmnwindows = new GameObject[] { null, null, null, null };
			//@sprites = new Dictionary<string, GameObject>();
			//@battlestart = true;
			//@messagemode = false;
			abortable = true;
			aborted = false;

			return this;
		}

		public void Display(string v)
		//void IHasDisplayMessage.Display(string v)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			//GameDebug.Log(v);
			System.Console.WriteLine(v);
		}

		void IPokeBattle_DebugSceneNoGraphics.DisplayMessage(string msg, bool brief)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			Display(msg);
			@messageCount += 1;
		}

		void IPokeBattle_DebugSceneNoGraphics.DisplayPausedMessage(string msg)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			Display(msg);
			@messageCount += 1;
		}

		bool IPokeBattle_DebugSceneNoGraphics.DisplayConfirmMessage(string msg)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			Display(msg);
			@messageCount += 1;

			System.Console.WriteLine("Y/N?");
			bool appearing = true;
			bool result = false;
			do
			{
				ConsoleKeyInfo fs = System.Console.ReadKey(true);

				if (fs.Key == ConsoleKey.Y)
				{
					appearing = false;
					result = true;
				}
				else if (fs.Key == ConsoleKey.N)
				{
					appearing = false;
					result = false;
				}
			} while (appearing);
			return result;
		}

		bool IHasDisplayMessage.DisplayConfirm(string v)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			return (this as IPokeBattle_DebugSceneNoGraphics).DisplayConfirmMessage(v);
		}

		bool IPokeBattle_DebugSceneNoGraphics.ShowCommands(string msg, string[] commands, bool defaultValue)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			GameDebug.Log(msg);
			@messageCount += 1;
			return false;
		}

		int IPokeBattle_DebugSceneNoGraphics.ShowCommands(string msg, string[] commands, int defaultValue)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			GameDebug.Log(msg);
			@messageCount += 1;
			return 0;
		}

		void IPokeBattle_DebugSceneNoGraphics.BeginCommandPhase()
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			if (@messageCount > 0)
			{
				GameDebug.Log($"[message count: #{@messageCount}]");
			}
			@messageCount = 0;
		}

		void IPokeBattle_DebugSceneNoGraphics.StartBattle(PokemonEssentials.Interface.PokeBattle.IBattle battle)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			this.battle = battle;
			lastcmd = new MenuCommands[] { 0, 0, 0, 0 };
			lastmove = new int[] { 0, 0, 0, 0 };
			@messageCount = 0;

			if (battle.player?.Length == 1)
			{
				GameDebug.Log("One player battle!");
			}

			if (battle.opponent != null)
			{
				GameDebug.Log("Opponent found!");
				if (battle.opponent.Length == 1)
				{
					GameDebug.Log("One opponent battle!");
				}
				if (battle.opponent.Length > 1)
				{
					GameDebug.Log("Multiple opponents battle!");
				}
				else
					GameDebug.Log("Wild Pokemon battle!");
			}

			if (battle.player?.Length > 0 && battle.opponent?.Length > 0 && !battle.doublebattle)
			{
				GameDebug.Log("Single Battle");
				System.Console.WriteLine("Player: {0} has {1} in their party", battle.player[0].name, battle.party1.Length);
				System.Console.WriteLine("Opponent: {0} has {1} in their party", battle.opponent?[0].name, battle.party2.Length);
			}
		}

		void IPokeBattle_DebugSceneNoGraphics.EndBattle(BattleResults result)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		//void IPokeBattle_DebugSceneNoGraphics.TrainerSendOut(IBattle battle, IPokemon pkmn)
		//{
		//	GameDebug.Log("Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		//}

		void IPokeBattle_DebugSceneNoGraphics.TrainerWithdraw(IBattle battle, IBattler pkmn)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		void IPokeBattle_DebugSceneNoGraphics.Withdraw(IBattle battle, IBattler pkmn)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		int IPokeBattle_DebugSceneNoGraphics.ForgetMove(PokemonEssentials.Interface.PokeBattle.IPokemon pokemon, Moves moveToLearn)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			IMove[] moves = pokemon.moves;
			string[] commands = new string[4] {
			   MoveString(moves[0], 1),
			   MoveString(moves[1], 2),
			   MoveString(moves[2], 3),
			   MoveString(moves[3], 4) };
			for (int i = 0; i < commands.Length; i++)
			{
				System.Console.WriteLine(commands[i]);
			}
			System.Console.WriteLine("Press 0 to Cancel");
			bool appearing = true;
			do
			{
				ConsoleKeyInfo fs = System.Console.ReadKey(true);

				if (fs.Key == ConsoleKey.D0)
				{
					appearing = false;
					return -1;
				}
				else if (fs.Key == ConsoleKey.D1)
				{
					appearing = false;
					return 0;
				}
				else if (fs.Key == ConsoleKey.D2)
				{
					appearing = false;
					return 1;
				}
				else if (fs.Key == ConsoleKey.D3)
				{
					appearing = false;
					return 2;
				}
				else if (fs.Key == ConsoleKey.D4)
				{
					appearing = false;
					return 3;
				}
			} while (appearing);

			return -1;
		}

		void IPokeBattle_DebugSceneNoGraphics.BeginAttackPhase()
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

		}

		int IPokeBattle_DebugSceneNoGraphics.CommandMenu(int index)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			bool shadowTrainer = //(hasConst(Types,:SHADOW) && //Game has shadow pokemons
				//@battle.opponent != null;
				battle.battlers[index] is IPokemonShadowPokemon p && p.hypermode;

			System.Console.WriteLine("Enemy: {0} HP: {1}/{2}", battle.battlers[index].Opposing1.Name, battle.battlers[index].Opposing1.HP, battle.battlers[index].Opposing1.TotalHP);
			if (battle.battlers[index].Opposing2.IsNotNullOrNone())
				System.Console.WriteLine("Enemy: {0} HP: {1}/{2}", battle.battlers[index].Opposing2.Name, battle.battlers[index].Opposing2.HP, battle.battlers[index].Opposing2.TotalHP);

			System.Console.WriteLine("What will {0} do?", battle.battlers[index].Name);
			System.Console.WriteLine("Fight - 0");
			System.Console.WriteLine("Bag - 1");
			System.Console.WriteLine("Pokémon - 2");
			System.Console.WriteLine(shadowTrainer ? "Call - 3" : "Run - 3");

			bool appearing = true;
			int result = -1;
			do
			{
				ConsoleKeyInfo fs = System.Console.ReadKey(true);
				if (fs.Key == ConsoleKey.D0)
				{
					result = 0;
					appearing = false;
				}
				else if (fs.Key == ConsoleKey.D1)
				{
					result = 1;
					appearing = false;
				}
				else if (fs.Key == ConsoleKey.D2)
				{
					result = 2;
					appearing = false;
				}
				else if (fs.Key == ConsoleKey.D3)
				{
					if (shadowTrainer)
						result = 4;
					else
						result = 3;
					appearing = false;
				}
			}
			while (appearing);

			//GameDebug.LogError("Invalid Input!");

			return result;
			//if (ret == 3 && shadowTrainer) ret = 4; // Convert "Run" to "Call"
			//return ret;
		}

		int IPokeBattle_DebugSceneNoGraphics.FightMenu(int index)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			IBattleMove[] moves = @battle.battlers[index].moves;
			string[] commands = new string[4] {
			   MoveString(moves[0].thismove, 1),
			   MoveString(moves[1].thismove, 2),
			   MoveString(moves[2].thismove, 3),
			   MoveString(moves[3].thismove, 4) };
			int index_ = @lastmove[index];
			for (int i = 0; i < commands.Length; i++)
			{
				System.Console.WriteLine(commands[i]);
			}
			System.Console.WriteLine("Press Q to return back to Command Menu");
			bool appearing = true;
			int result = -2;
			do
			{
				ConsoleKeyInfo fs = System.Console.ReadKey(true);

				if (fs.Key == ConsoleKey.D1)
				{
					lastmove[index] = index_;
					appearing = false;
					result = 0;
					GameDebug.Log($"int=#{result}, pp=#{moves[result].PP}");
				}
				else if (fs.Key == ConsoleKey.D2)
				{
					lastmove[index] = index_;
					appearing = false;
					result = 1;
					GameDebug.Log($"int=#{result}, pp=#{moves[result].PP}");
				}
				else if (fs.Key == ConsoleKey.D3)
				{
					lastmove[index] = index_;
					appearing = false;
					result = 2;
					GameDebug.Log($"int=#{result}, pp=#{moves[result].PP}");
				}
				else if (fs.Key == ConsoleKey.D4)
				{
					lastmove[index] = index_;
					appearing = false;
					result = 3;
					GameDebug.Log($"int=#{result}, pp=#{moves[result].PP}");
				}
				else if (fs.Key == ConsoleKey.Q)
				{
					appearing = false;
					result = -1; //CANCEL FIGHT MENU
				}
			} while (appearing && (result == -2 || battle.battlers[index].moves[result].id == Moves.NONE));

			return result;
		}

		Items IPokeBattle_DebugSceneNoGraphics.ItemMenu(int index)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			//System.Console.WriteLine("Need to implement item system in textbased-line");
			return Items.NONE;
		}

		int IPokeBattle_DebugSceneNoGraphics.ChooseTarget(int index, PokemonUnity.Attack.Targets targettype)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			//Doesnt include multiple targets at once...
			List<int> targets = new List<int>();
			for (int i = 0; i < 4; i++)
			{
				//if (@battle.battlers[index].IsOpposing(i) &&
				//   !@battle.battlers[i].isFainted()) targets.Add(i);
				if (!@battle.battlers[i].isFainted())
					if ((targettype == PokemonUnity.Attack.Targets.RANDOM_OPPONENT
						//|| targettype == PokemonUnity.Attack.Targets.ALL_OPPONENTS
						//|| targettype == PokemonUnity.Attack.Targets.OPPONENTS_FIELD
						|| targettype == PokemonUnity.Attack.Targets.SELECTED_POKEMON
						|| targettype == PokemonUnity.Attack.Targets.SELECTED_POKEMON_ME_FIRST) &&
						@battle.battlers[index].IsOpposing(i))
						targets.Add(i);
					else if ((targettype == PokemonUnity.Attack.Targets.ALLY
						//|| targettype == PokemonUnity.Attack.Targets.USERS_FIELD
						//|| targettype == PokemonUnity.Attack.Targets.USER_AND_ALLIES
						|| targettype == PokemonUnity.Attack.Targets.USER_OR_ALLY) &&
						!@battle.battlers[index].IsOpposing(i))
						targets.Add(i);
			}
			if (targets.Count == 0) return -1;
			//return targets[Core.Rand.Next(targets.Count)];

			for (int i = 0; i < targets.Count; i++)
			{
				System.Console.WriteLine("Target {0}: {1} HP: {2}/{3} => {4}", targets[i] % 2 == 1 ? "Enemy" : "Ally", battle.battlers[targets[i]].Name, battle.battlers[targets[i]].HP, battle.battlers[targets[i]].TotalHP, i);
			}
			bool appearing = true;
			int result = 0;
			do
			{
				ConsoleKeyInfo fs = System.Console.ReadKey(true);

				if (fs.Key == ConsoleKey.D1)
				{
					appearing = false;
					result = 0;
				}
				else if (fs.Key == ConsoleKey.D2)
				{
					appearing = false;
					result = 1;
				}
				else if (fs.Key == ConsoleKey.D3)
				{
					appearing = false;
					result = 2;
				}
				else if (fs.Key == ConsoleKey.D4)
				{
					appearing = false;
					result = 3;
				}
			} while (appearing && targets.Contains(result));

			return result;
		}

		public void Refresh()
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		//int IPokeBattle_DebugSceneNoGraphics.Switch(int index, bool lax, bool cancancel)
		int IPokeBattle_SceneNonInteractive.Switch(int index, bool lax, bool cancancel)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			IPokemon[] party = @battle.Party(index);
			IList<string> commands = new List<string>();
			bool[] inactives = new bool[] { true, true, true, true, true, true };
			IList<int> partypos = new List<int>();
			//int activecmd = 0; //if cursor is on first or second pokemon when viewing ui
			int numactive = battle.doublebattle ? 2 : 1;
			IBattler battler = @battle.battlers[0];
			//commands[commands.Count] = PokemonString(party[battler.pokemonIndex]);
			commands.Add(PokemonString(party[battler.pokemonIndex]));
			//if (battler.Index == index) activecmd = 0;
			inactives[battler.pokemonIndex] = false;
			//partypos[partypos.Count] = battler.pokemonIndex;
			partypos.Add(battler.pokemonIndex);
			if (@battle.doublebattle)
			{
				battler = @battle.battlers[2];
				//commands[commands.Count] = PokemonString(party[battler.pokemonIndex]);
				commands.Add(PokemonString(party[battler.pokemonIndex]));
				//if (battler.Index == index) activecmd = 1;
				inactives[battler.pokemonIndex] = false;
				//partypos[partypos.Count] = battler.pokemonIndex;
				partypos.Add(battler.pokemonIndex);
			}
			for (int i = 0; i < party.Length; i++)
			{
				if (inactives[i])
				{
					//commands[commands.Count] = PokemonString(party[i]);
					commands.Add(PokemonString(party[i]));
					//System.Console.WriteLine(PokemonString(party[i]));
					//partypos[partypos.Count] = i;
					partypos.Add(i);
				}
			}
			for (int i = 0; i < commands.Count; i++)
			{
				System.Console.WriteLine("Press {0} => {1}",i+1,commands[i]);
			}
			System.Console.WriteLine("Press Q to return back to Command Menu");
			bool appearing = true;
			int ret = -2;
			do
			{
				ConsoleKeyInfo fs = System.Console.ReadKey(true);
				bool canswitch = false; int pkmnindex = -1;
				if (fs.Key == ConsoleKey.D1)
				{
					pkmnindex = partypos[0];
					canswitch = lax ? @battle.CanSwitchLax(index, pkmnindex, true) :
					   @battle.CanSwitch(index, pkmnindex, true);
					if (canswitch)
					{
						ret = pkmnindex;
						appearing = false;
						//break;
					}
				}
				else if (fs.Key == ConsoleKey.D2)
				{
					pkmnindex = partypos[1];
					canswitch = lax ? @battle.CanSwitchLax(index, pkmnindex, true) :
					   @battle.CanSwitch(index, pkmnindex, true);
					if (canswitch)
					{
						ret = pkmnindex;
						appearing = false;
						//break;
					}
				}
				else if (fs.Key == ConsoleKey.D3)
				{
					pkmnindex = partypos[2];
					canswitch = lax ? @battle.CanSwitchLax(index, pkmnindex, true) :
					   @battle.CanSwitch(index, pkmnindex, true);
					if (canswitch)
					{
						ret = pkmnindex;
						appearing = false;
						//break;
					}
				}
				else if (fs.Key == ConsoleKey.D4)
				{
					pkmnindex = partypos[3];
					canswitch = lax ? @battle.CanSwitchLax(index, pkmnindex, true) :
					   @battle.CanSwitch(index, pkmnindex, true);
					if (canswitch)
					{
						ret = pkmnindex;
						appearing = false;
						//break;
					}
				}
				else if (fs.Key == ConsoleKey.D5)
				{
					pkmnindex = partypos[4];
					canswitch = lax ? @battle.CanSwitchLax(index, pkmnindex, true) :
					   @battle.CanSwitch(index, pkmnindex, true);
					if (canswitch)
					{
						ret = pkmnindex;
						appearing = false;
						//break;
					}
				}
				else if (fs.Key == ConsoleKey.D6)
				{
					pkmnindex = partypos[5];
					canswitch = lax ? @battle.CanSwitchLax(index, pkmnindex, true) :
					   @battle.CanSwitch(index, pkmnindex, true);
					if (canswitch)
					{
						ret = pkmnindex;
						appearing = false;
						//break;
					}
				}
				else if (fs.Key == ConsoleKey.Q && cancancel)
				{
					appearing = false;
					ret = -1; //CANCEL POKEMON MENU
				}
			} while (appearing && (ret == -2 || ret == -2 || inactives[ret]));//!battle.Party(index)[ret].IsNotNullOrNone()

			return ret;
		}

		//public IEnumerator HPChanged(PokemonEssentials.Interface.PokeBattle.IBattler pkmn, int oldhp, bool animate)
		void IPokeBattle_DebugSceneNoGraphics.HPChanged(IBattler pkmn, int oldhp, bool anim)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			int hpchange = pkmn.HP - oldhp;
			if (hpchange < 0)
			{
				hpchange = -hpchange;
				GameDebug.Log($"[HP change] #{pkmn.ToString()} lost #{hpchange} HP (#{oldhp}=>#{pkmn.HP})");
			}
			else
			{
				GameDebug.Log($"[HP change] #{pkmn.ToString()} gained #{hpchange} HP (#{oldhp}=>#{pkmn.HP})");
			}
			Refresh();

			//System.Console.WriteLine("[HP Changed] {0}: oldhp: {1} and animate: {2}", pkmn.Name, oldhp, animate.ToString());
			//System.Console.WriteLine("[HP Changed] {0}: CurrentHP: {1}", pkmn.Name, pkmn.HP);

			//yield return null;
		}

		void IPokeBattle_DebugSceneNoGraphics.Fainted(IBattler pkmn)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		//void IPokeBattle_DebugSceneNoGraphics.ChooseEnemyCommand(int index)
		void IPokeBattle_SceneNonInteractive.ChooseEnemyCommand(int index)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			if (battle is IBattleAI b) b.DefaultChooseEnemyCommand(index);
		}

		//void IPokeBattle_DebugSceneNoGraphics.ChooseNewEnemy(int index, IPokemon[] party)
		int IPokeBattle_SceneNonInteractive.ChooseNewEnemy(int index, IPokemon[] party)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			if (battle is IBattleAI b) return b.DefaultChooseNewEnemy(index, party);
			return -1;
		}

		void IPokeBattle_DebugSceneNoGraphics.WildBattleSuccess()
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		void IPokeBattle_DebugSceneNoGraphics.TrainerBattleSuccess()
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		void IPokeBattle_DebugSceneNoGraphics.EXPBar(IBattler battler, IPokemon thispoke, int startexp, int endexp, int tempexp1, int tempexp2)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		void IPokeBattle_DebugSceneNoGraphics.LevelUp(IBattler battler, IPokemon thispoke, int oldtotalhp, int oldattack, int olddefense, int oldspeed, int oldspatk, int oldspdef)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		int IPokeBattle_DebugSceneNoGraphics.Blitz(int keys)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			return battle.Random(30);
		}

		void ISceneHasChatter.Chatter(PokemonEssentials.Interface.PokeBattle.IBattler attacker, PokemonEssentials.Interface.PokeBattle.IBattler opponent)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		//void IPokeBattle_DebugSceneNoGraphics.Chatter(IBattler attacker, IBattler opponent)
		//{
		//	GameDebug.Log("Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		//
		//	(this as ISceneHasChatter).Chatter(attacker, opponent);
		//}

		void IPokeBattle_DebugSceneNoGraphics.ShowOpponent(int opp)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		void IPokeBattle_DebugSceneNoGraphics.HideOpponent()
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		void IPokeBattle_DebugSceneNoGraphics.Recall(int battlerindex)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		void IPokeBattle_DebugSceneNoGraphics.DamageAnimation(IBattler pkmn, TypeEffective effectiveness)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
		}

		void IPokeBattle_DebugSceneNoGraphics.BattleArenaJudgment(IBattle b1, IBattle b2, int[] r1, int[] r2)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			//GameDebug.Log($"[Judgment] #{b1.ToString()}:#{r1.Inspect()}, #{b2.ToString()}:#{r2.Inspect()}");
			GameDebug.Log($"[Judgment] #{b1.ToString()}:#[{r1.JoinAsString(", ")}], #{b2.ToString()}:#[{r2.JoinAsString(", ")}]");
		}

		void IPokeBattle_DebugSceneNoGraphics.BattleArenaBattlers(IBattle b1, IBattle b2)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			GameDebug.Log($"[#{b1.ToString()} VS #{b2.ToString()}]");
		}

		void IPokeBattle_DebugSceneNoGraphics.CommonAnimation(Moves moveid, IBattler attacker, IBattler opponent, int hitnum)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			if (attacker.IsNotNullOrNone())
			{
				if (opponent.IsNotNullOrNone())
				{
					GameDebug.Log($"[CommonAnimation] #{moveid}, #{attacker.ToString()}, #{opponent.ToString()}");
				}
				else
				{
					GameDebug.Log($"[CommonAnimation] #{moveid}, #{attacker.ToString()}");
				}
			}
			else
			{
				GameDebug.Log($"[CommonAnimation] #{moveid}");
			}
		}

		void IPokeBattle_DebugSceneNoGraphics.Animation(Moves moveid, PokemonEssentials.Interface.PokeBattle.IBattler user, PokemonEssentials.Interface.PokeBattle.IBattler target, int hitnum)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			System.Console.WriteLine("{0} attack {1} With {2} for {3} hit times", user.Name, target.Name, moveid.ToString(), hitnum);

			if (user.IsNotNullOrNone())
			{
				if (target.IsNotNullOrNone())
				{
					GameDebug.Log($"[Animation] #{user.ToString()}, #{target.ToString()}");
				}
				else
				{
					GameDebug.Log($"[Animation] #{user.ToString()}");
				}
			}
			else
			{
				GameDebug.Log($"[Animation]");
			}
		}

		#region Non Interactive Battle Scene
		int IPokeBattle_SceneNonInteractive.CommandMenu(int index)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			//if (battle.Random(15) == 0) return 1;
			//return 0;
			return (this as IPokeBattle_DebugSceneNoGraphics).CommandMenu(index);
		}

		int IPokeBattle_SceneNonInteractive.FightMenu(int index)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			//IBattler battler = @battle.battlers[index];
			//int i = 0;
			//do {
			//	i = Core.Rand.Next(4);
			//} while (battler.moves[i].id==0);
			//GameDebug.Log($"i=#{i}, pp=#{battler.moves[i].PP}");
			////Debug.flush;
			//return i;
			return (this as IPokeBattle_DebugSceneNoGraphics).FightMenu(index);
		}

		Items IPokeBattle_SceneNonInteractive.ItemMenu(int index)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			//return -1;
			return (this as IPokeBattle_DebugSceneNoGraphics).ItemMenu(index);
		}

		int IPokeBattle_SceneNonInteractive.ChooseTarget(int index, PokemonUnity.Attack.Targets targettype)
		{
			GameDebug.Log(message: "Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			//List<int> targets = new List<int>();
			//for (int i = 0; i < 4; i++)
			//{
			//	if (@battle.battlers[index].IsOpposing(i) &&
			//	   !@battle.battlers[i].isFainted())
			//	{
			//		targets.Add(i);
			//	}
			//}
			//if (targets.Count == 0) return -1;
			//return targets[Core.Rand.Next(targets.Count)];
			return (this as IPokeBattle_DebugSceneNoGraphics).ChooseTarget(index, targettype);
		}

		/*int IPokeBattle_SceneNonInteractive.Switch(int index, bool lax, bool cancancel)
		{
			GameDebug.Log("Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			for (int i = 0; i < @battle.Party(index).Length - 1; i++)
			{
				if (lax)
				{
					if (@battle.CanSwitchLax(index, i, false)) return i;
				}
				else
				{
					if (@battle.CanSwitch(index, i, false)) return i;
				}
			}
			return -1;
		}

		void IPokeBattle_SceneNonInteractive.ChooseEnemyCommand(int index)
		{
			GameDebug.Log("Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			throw new NotImplementedException();
		}

		void IPokeBattle_SceneNonInteractive.ChooseNewEnemy(int index, IPokemon[] party)
		{
			GameDebug.Log("Run: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name);

			throw new NotImplementedException();
		}*/
		#endregion

		private string PokemonString(IPokemon pkmn)
		{
			string status = string.Empty;
			if (pkmn.HP <= 0)
			{
				status = " [FNT]";
			}
			else
			{
				switch (pkmn.Status)
				{
					case Status.SLEEP:
						status = " [SLP]";
						break;
					case Status.FROZEN:
						status = " [FRZ]";
						break;
					case Status.BURN:
						status = " [BRN]";
						break;
					case Status.PARALYSIS:
						status = " [PAR]";
						break;
					case Status.POISON:
						status = " [PSN]";
						break;
				}
			}
			return $"#{pkmn.Name} (Lv. #{pkmn.Level})#{status} HP: #{pkmn.HP}/#{pkmn.TotalHP}";
		}

		private string PokemonString(IBattler pkmn)
		{
			if (!pkmn.pokemon.IsNotNullOrNone())
			{
				return "";
			}
			string status = string.Empty;
			if (pkmn.HP <= 0)
			{
				status = " [FNT]";
			}
			else
			{
				switch (pkmn.Status)
				{
					case Status.SLEEP:
						status = " [SLP]";
						break;
					case Status.FROZEN:
						status = " [FRZ]";
						break;
					case Status.BURN:
						status = " [BRN]";
						break;
					case Status.PARALYSIS:
						status = " [PAR]";
						break;
					case Status.POISON:
						status = " [PSN]";
						break;
				}
			}
			return $"#{pkmn.Name} (Lv. #{pkmn.Level})#{status} HP: #{pkmn.HP}/#{pkmn.TotalHP}";
		}

		private string MoveString(IMove move, int index)
		{
			string ret = string.Format("{0} - Press {1}", Game._INTL(move.id.ToString(TextScripts.Name)), index);
			string typename = Game._INTL(move.Type.ToString(TextScripts.Name));
			if (move.id > 0)
			{
				ret += string.Format(" ({0}) PP: {1}/{2}", typename, move.PP, move.TotalPP);
			}
			return ret;
		}
	}
}