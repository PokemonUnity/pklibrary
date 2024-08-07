﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonUnity;
using PokemonUnity.Inventory;
using PokemonUnity.Monster;
using PokemonUnity.Overworld;
using PokemonEssentials.Interface;
using PokemonEssentials.Interface.Item;
using PokemonEssentials.Interface.Field;
using PokemonEssentials.Interface.PokeBattle;
using PokemonEssentials.Interface.Screen;
using PokemonEssentials.Interface.Battle;
using PokemonUnity.Utility;
using PokemonEssentials.Interface.RPGMaker.Kernal;
using PokemonEssentials.Interface.EventArg;

namespace PokemonUnity
{
	namespace EventArg {

		public class OnEncounterCreateEventArgs : EventArgs, IEncounterModifierEventArgs
		{
			public static readonly int EventId = typeof(OnEncounterCreateEventArgs).GetHashCode();

			public int Id { get { return EventId; } }
			/// <summary>
			/// Pokémon being created
			/// </summary>
			public IPokemon Pokemon { get; set; }
			Pokemons IEncounterPokemon.Pokemon	{ get { return Pokemon.Species; } }
			int IEncounterPokemon.MinLevel		{ get { return Pokemon.Level; } }
			int IEncounterPokemon.MaxLevel		{ get { return Pokemon.Level; } }
		}
	}

	// ===============================================================================
	// Battles
	// ===============================================================================
	public partial class GameTemp {
		public ISprite background_bitmap			{ get; protected set; }
	}

	public partial class PokemonTemp : ITempMetadataField
	{
		public Method? encounterType			{ get; set; }
		public int[] evolutionLevels			{ get; set; }


		public bool batterywarning				{ get; protected set; }
		public IAudioBGM cueBGM					{ get; set; }
		public float? cueFrames				    { get; set; }
	}

	/// <summary>
	/// This module stores encounter-modifying events that can happen during the game.
	/// A procedure can subscribe to an event by adding itself to the event.
	/// It will then be called whenever the event occurs.
	/// </summary>
	/// ToDo: Integrate into Game class?
	public partial class EncounterModifier : PokemonEssentials.Interface.Field.IEncounterModifier { //
		//public static List<Func<IEncounterPokemon,IEncounterPokemon>> @procs=new List<Func<IEncounterPokemon,IEncounterPokemon>>();
		private List<Func<IPokemon,IPokemon>> @procs=new List<Func<IPokemon,IPokemon>>();
		private List<Action> @procsEnd=new List<Action>();
		public event EventHandler<EventArg.OnEncounterCreateEventArgs> OnEncounter;
		public event EventHandler OnEncounterEnd;
		event Action<object, IEncounterModifierEventArgs> IEncounterModifier.OnEncounter
		{
			add
			{
				OnEncounter += new EventHandler<EventArg.OnEncounterCreateEventArgs>((sender, args) => { value.Invoke(sender, args); }); //value;
			}

			remove
			{
				OnEncounter -= new EventHandler<EventArg.OnEncounterCreateEventArgs>((sender, args) => { value.Invoke(sender, args); }); //value;
			}
		}

		//public static void register(Func<IEncounterPokemon,IEncounterPokemon> p) {
		public void register(Func<IPokemon,IPokemon> p) {
			@procs.Add(p);
		}

		public void registerEncounterEnd(Action p) {
			@procsEnd.Add(p);
		}

		//public static IEncounterPokemon trigger(IEncounterPokemon encounter) {
		//public IPokemon trigger(IPokemon encounter) {
		public IPokemon triggerEncounter(IPokemon encounter) {
			//foreach (var prc in @procs) {
			//	//encounter=prc.call(encounter);
			//	encounter=prc.Invoke(encounter);
			//}
			//return encounter;
			if (OnEncounter != null)
			{
				//OnEncounter(sender, new EventArg.OnEncounterCreateEventArgs()
				EventArg.OnEncounterCreateEventArgs arg = new EventArg.OnEncounterCreateEventArgs()
				{
					Pokemon = encounter,
					//Pokemon = encounter.Pokemon,
					//MinLevel = encounter.MinLevel,
					//MaxLevel = encounter.MaxLevel
				};
				triggerEncounter(this,arg);
				return arg.Pokemon;
			}
			return encounter;
		}

		public void triggerEncounter(object sender, EventArg.OnEncounterCreateEventArgs args)
		{
			if (OnEncounter != null)
			{
				OnEncounter.Invoke(sender, args);
				//OnEncounter(sender, new EventArg.OnEncounterCreateEventArgs()
				//{
				//	Pokemon = encounter,
				//	//Pokemon = encounter.Pokemon,
				//	//MinLevel = encounter.MinLevel,
				//	//MaxLevel = encounter.MaxLevel
				//});
			}
		}

		public void triggerEncounterEnd() {
			//foreach (var prc in @procsEnd) {
			//	//prc.call();
			//	prc.Invoke();
			//}
			if (OnEncounterEnd != null)
			{
				OnEncounterEnd.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public partial class Game : IGameField
	{
		//event EventHandler IGameField.OnStartBattle { add { OnStartBattle += value; } remove { OnStartBattle -= value; } }
		//event Action<object, PokemonEssentials.Interface.EventArg.IOnEndBattleEventArgs> IGameField.OnEndBattle { add { OnEndBattle += value; } remove { OnEndBattle -= value; } }

		public IPokeBattle_Scene NewBattleScene()
		{
			return Scenes.BattleScene.initialize(); //new PokeBattle_Scene();
		}

		public System.Collections.IEnumerator SceneStandby(Action block = null) {
			if (Scene != null && Scene is ISceneMap s0) {
				s0.disposeSpritesets();
			}
			GC.Collect();
			Graphics.frame_reset();
			yield return null;
			block.Invoke();
			if (Scene != null && Scene is ISceneMap s1) {
				s1.createSpritesets();
			}
		}

		public virtual void BattleAnimation(IAudioBGM bgm=null,int trainerid=-1,string trainername="", Action block = null) {
			bool handled=false;
			IAudioBGS playingBGS=null;
			IAudioBGM playingBGM=null;
			if (Game.GameData.GameSystem != null && Game.GameData.GameSystem is IGameSystem s) {
				playingBGS=s.getPlayingBGS();
				playingBGM=s.getPlayingBGM();
				s.bgm_pause();
				s.bgs_pause();
			}
			if (this is IGameAudioPlay a0) a0.MEFade(0.25f);
			Wait(10);
			if (this is IGameAudioPlay a1) a1.MEStop();
			if (bgm != null) {
				if (this is IGameAudioPlay a2) a2.BGMPlay(bgm);
			} else {
				if (this is IGameAudioPlay a2) a2.BGMPlay(GetWildBattleBGM(0));
			}
			IViewport viewport=null; //new Viewport(0,0,Graphics.width,Graphics.height);
			//viewport.z=99999;
			// Fade to gray a few times.
			viewport.color=null; //new Color(17*8,17*8,17*8);
			int z = 0; do { //3.times ;
				viewport.color.alpha=0;
				int x = 0; do { //6.times do;
					viewport.color.alpha+=30;
					Graphics?.update();
					Input.update();
					if (this is IGameMessage m) m.UpdateSceneMap(); x++;
				} while (x < 6);
				int y = 0; do { //6.times do;
					viewport.color.alpha-=30;
					Graphics?.update();
					Input.update();
					if (this is IGameMessage m) m.UpdateSceneMap(); y++;
				} while (y < 6); z++;
			} while (z < 3);
			if (GameTemp.background_bitmap != null) {
				GameTemp.background_bitmap.Dispose();
			}
			GameTemp.background_bitmap=Graphics.snap_to_bitmap();
			//  Check for custom battle intro animations
			handled=BattleAnimationOverride(viewport,trainerid,trainername);
			//  Default battle intro animation
			if (!handled) {
				//if (Sprite.method_defined(:wave_amp) && Core.Rand.Next(15)==0) {
				//	viewport.color=new Color(0,0,0,255);
				//	ISprite sprite = new Sprite();
				//	bitmap=Graphics.snap_to_bitmap();
				//	IBitmap bm=bitmap.clone();
				//	sprite.z=99999;
				//	sprite.bitmap = bm;
				//	sprite.wave_speed=500;
				//	for (int i = 0; i < 25; i++) {
				//		sprite.opacity-=10;
				//		sprite.wave_amp+=60;
				//		sprite.update();
				//		sprite.wave_speed+=30;
				//		do {
				//			Graphics?.update();
				//		} while (); //2.times
				//	}
				//	bitmap.Dispose();
				//	bm.Dispose();
				//	sprite.Dispose();
				//} else if (Bitmap.method_defined(:radial_blur) && Core.Rand.Next(15)==0) {
				//	viewport.color=new Color(0,0,0,255);
				//	sprite = new Sprite();
				//	bitmap=Graphics.snap_to_bitmap;
				//	bm=bitmap.clone();
				//	sprite.z=99999;
				//	sprite.bitmap = bm;
				//	for (int i = 0; i < 15; i++) {
				//		bm.radial_blur(i,2);
				//		sprite.opacity-=15;
				//		do {
				//			Graphics?.update();
				//		} while ();//2.times
				//	}
				//	bitmap.Dispose();
				//	bm.Dispose();
				//	sprite.Dispose();
				//} else
				if (Core.Rand.Next(10)==0) {		// Custom transition method
					string[] scroll=new string[] {"ScrollDown","ScrollLeft","ScrollRight","ScrollUp",
							"ScrollDownRight","ScrollDownLeft","ScrollUpRight","ScrollUpLeft" };
					Graphics.freeze();
					viewport.color=null; //new Color(0,0,0,255);
					Graphics.transition(50,string.Format("Graphics/Transitions/{0}",scroll[Core.Rand.Next(scroll.Length)]));
				} else {
					string[] transitions= new string[] {
						//  Transitions with graphic files
						"021-Normal01","022-Normal02",
						"Battle","battle1","battle2","battle3","battle4",
						"computertr","computertrclose",
						"hexatr","hexatrc","hexatzr",
						"Image1","Image2","Image3","Image4",
						//  Custom transition methods
						"Splash","Random_stripe_v","Random_stripe_h",
						"RotatingPieces","ShrinkingPieces",
						"BreakingGlass","Mosaic","zoomin"
					};
					int rnd=Core.Rand.Next(transitions.Length);
					Graphics.freeze();
					viewport.color=null; //new Color(0,0,0,255);
					Graphics.transition(40,string.Format("Graphics/Transitions/%s",transitions[rnd]));
				}
				int i = 0; do { //5.times do;
					Graphics?.update();
					Input.update();
					if (this is IGameMessage m) m.UpdateSceneMap();
				} while (i < 5);
			}
			PushFade();
			//if (block_given?) yield;
			if (block != null) block.Invoke();
			PopFade();
			if (GameSystem != null && Game.GameData.GameSystem is IGameSystem s1) {
				s1.bgm_resume(playingBGM);
				s1.bgs_resume(playingBGS);
			}
			Global.nextBattleBGM=null;
			Global.nextBattleME=null;
			Global.nextBattleBack=null;
			PokemonEncounters.clearStepCount();
			for (int j = 0; j < 17; j++) {
				viewport.color=null; //new Color(0,0,0,(17-j)*15);
				Graphics?.update();
				Input.update();
				if (this is IGameMessage m) m.UpdateSceneMap();
			}
			viewport.Dispose();
		}

		// Alias and use this method if you want to add a custom battle intro animation
		// e.g. variants of the Vs. animation.
		// Note that Game.GameData.GameTemp.background_bitmap contains an image of the current game
		// screen.
		// When the custom animation has finished, the screen should have faded to black
		// somehow.
		public virtual bool BattleAnimationOverride(IViewport viewport,int trainerid=-1,string trainername="") {
			//  The following example runs a common event that ought to do a custom
			//  animation if some condition is true:
			//
			//  if (GameMap != null && GameMap.map_id==20) {   // If on map 20
			//		CommonEvent(20);
			//		return true;                          // Note that the battle animation is done
			//  }
			//
			// #### VS. animation, by Luka S.J. #####
			// #### Tweaked by Maruno           #####
			/*if (trainerid>=0) {
				string tbargraphic=string.Format("Graphics/Transitions/vsBar%s",trainerid.ToString()); //getConstantName(Trainers,trainerid) rescue null;
				if (ResolveBitmap(tbargraphic) == null) tbargraphic=string.Format("Graphics/Transitions/vsBar%d",trainerid);
				string tgraphic=string.Format("Graphics/Transitions/vsTrainer%s",trainerid.ToString()); //getConstantName(Trainers,trainerid) rescue null;
				if (ResolveBitmap(tgraphic) == null) tgraphic=string.Format("Graphics/Transitions/vsTrainer%d",trainerid);
				if (ResolveBitmap(tbargraphic) != null && ResolveBitmap(tgraphic) != null) {
					int outfit=Trainer != null ? Trainer.outfit??0 : 0;
					//  Set up
					IViewport viewplayer=new Viewport(0,Graphics.height/3,Graphics.width/2,128);
					//viewplayer.z=viewport.z;
					IViewport viewopp=new Viewport(Graphics.width/2,Graphics.height/3,Graphics.width/2,128);
					viewopp.z=viewport.z;
					IViewport viewvs=new Viewport(0,0,Graphics.width,Graphics.height);
					viewvs.z=viewport.z;
					double xoffset=(Graphics.width/2)/10;
					xoffset=Math.Round(xoffset);
					xoffset=xoffset*10;
					ISprite fade=new Sprite(viewport);
					fade.bitmap=BitmapCache.load_bitmap("Graphics/Transitions/vsFlash");
					fade.tone=new Tone(-255,-255,-255);
					fade.opacity=100;
					ISprite overlay=new Sprite(viewport);
					overlay.bitmap=new Bitmap(Graphics.width,Graphics.height);
					SetSystemFont(overlay.bitmap);
					ISprite bar1=new Sprite(viewplayer);
					string argraphic=string.Format("Graphics/Transitions/vsBar%s_%d",Trainer.trainertype,outfit); //getConstantName(Trainers,Trainer.trainertype) rescue null;
					if (ResolveBitmap(argraphic) == null) argraphic=string.Format("Graphics/Transitions/vsBar%d_%d",Trainer.trainertype,outfit);
					if (ResolveBitmap(argraphic) == null) {
						argraphic=string.Format("Graphics/Transitions/vsBar%s",Trainer.trainertype); //getConstantName(Trainers,Trainer.trainertype) rescue null;
					}
					if (ResolveBitmap(argraphic) == null) argraphic=string.Format("Graphics/Transitions/vsBar%d",Trainer.trainertype);
					bar1.bitmap=BitmapCache.load_bitmap(argraphic);
					bar1.x=-xoffset;
					ISprite bar2=new Sprite(viewopp);
					bar2.bitmap=BitmapCache.load_bitmap(tbargraphic);
					bar2.x=xoffset;
					ISprite vs=new Sprite(viewvs);
					vs.bitmap=BitmapCache.load_bitmap("Graphics/Transitions/vs");
					vs.ox=vs.bitmap.width/2;
					vs.oy=vs.bitmap.height/2;
					vs.x=Graphics.width/2;
					vs.y=Graphics.height/1.5;
					vs.visible=false;
					ISprite flash=new Sprite(viewvs);
					flash.bitmap=BitmapCache.load_bitmap("Graphics/Transitions/vsFlash");
					flash.opacity=0;
					//  Animation
					int i = 0; do { //10.times do;
						bar1.x+=xoffset/10;
						bar2.x-=xoffset/10;
						Wait(1); i++;
					} while (i < 10);
					SEPlay("Flash2");
					SEPlay("Sword2");
					flash.opacity=255;
					bar1.Dispose();
					bar2.Dispose();
					bar1=new AnimatedPlane(viewplayer);
					bar1.bitmap=BitmapCache.load_bitmap(argraphic);
					ISprite player=new Sprite(viewplayer);
					string pgraphic=string.Format("Graphics/Transitions/vsTrainer%s_%d",getConstantName(Trainers,Game.GameData.Trainer.trainertype),outfit) rescue null;
					if (ResolveBitmap(pgraphic) == null) pgraphic=string.Format("Graphics/Transitions/vsTrainer%d_%d",Game.GameData.Trainer.trainertype,outfit);
					if (ResolveBitmap(pgraphic) == null) {
						pgraphic=string.Format("Graphics/Transitions/vsTrainer%s",getConstantName(Trainers,Game.GameData.Trainer.trainertype)) rescue null;
					}
					if (ResolveBitmap(pgraphic) == null) pgraphic=string.Format("Graphics/Transitions/vsTrainer%d",Game.GameData.Trainer.trainertype);
					player.bitmap=BitmapCache.load_bitmap(pgraphic);
					player.x=-xoffset;
					bar2=new AnimatedPlane(viewopp);
					bar2.bitmap=BitmapCache.load_bitmap(tbargraphic);
					trainer=new Sprite(viewopp);
					trainer.bitmap=BitmapCache.load_bitmap(tgraphic);
					trainer.x=xoffset;
					trainer.tone=new Tone(-255,-255,-255);
					i = 0; do { //25.times do;
						if (flash.opacity>0) flash.opacity-=51;
						bar1.ox-=16;
						bar2.ox+=16;
						Wait(1); i++;
					} while (i < 25);
					i = 0; do { //11.times do;
						bar1.ox-=16;
						bar2.ox+=16;
						player.x+=xoffset/10;
						trainer.x-=xoffset/10;
						Wait(1); i++;
					} while (i < 11);
					i = 0; do { //2.times do;
						bar1.ox-=16;
						bar2.ox+=16;
						player.x-=xoffset/20;
						trainer.x+=xoffset/20;
						Wait(1); i++;
					} while (i < 2);
					i = 0; do { //10.times do;
						bar1.ox-=16;
						bar2.ox+=16;
						Wait(1); i++;
					} while (i < 10);
					int val=2;
					flash.opacity=255;
					vs.visible=true;
					trainer.tone=new Tone(0,0,0);
					textpos=new {
						new { _INTL("{1}",Game.GameData.Trainer.name),Graphics.width/4,(Graphics.height/1.5)+10,2,
						new Color(248,248,248),new Color(12*6,12*6,12*6) },
						new { _INTL("{1}",trainername),(Graphics.width/4)+(Graphics.width/2),(Graphics.height/1.5)+10,2,
						new Color(248,248,248),new Color(12*6,12*6,12*6) };
					};
					DrawTextPositions(overlay.bitmap,textpos);
					SEPlay("Sword2");
					i = 0; do { //70.times do;
						bar1.ox-=16;
						bar2.ox+=16;
						if (flash.opacity>0) flash.opacity-=25.5;
						vs.x+=val;
						vs.y-=val;
						if (vs.x<=(Graphics.width/2)-2) val=2;
						if (vs.x>=(Graphics.width/2)+2) val=-2;
						Wait(1); i++;
					} while (i < 70);
					i = 0; do { //30.times do;
						bar1.ox-=16;
						bar2.ox+=16;
						vs.zoom_x+=0.2;
						vs.zoom_y+=0.2;
						Wait(1); i++;
					} while (i < 30);
					flash.tone=new Tone(-255,-255,-255);
					i = 0; do { //10.times do;
						bar1.ox-=16;
						bar2.ox+=16;
						flash.opacity+=25.5;
						Wait(1); i++;
					} while (i < 10);
			//  }
					player.Dispose();
					trainer.Dispose();
					flash.Dispose();
					vs.Dispose();
					bar1.Dispose();
					bar2.Dispose();
					overlay.Dispose();
					fade.Dispose();
					viewvs.Dispose();
					viewopp.Dispose();
					viewplayer.Dispose();
					viewport.color=new Color(0,0,0,255);
					return true;
				}
			}*/
			return false;
		}

		public void PrepareBattle(IBattle battle) {
			switch (GameScreen.weather_type) {
				case FieldWeathers.Rain: case FieldWeathers.HeavyRain: case FieldWeathers.Thunderstorm:
					battle.weather=Combat.Weather.RAINDANCE;
					battle.weatherduration=-1;
					break;
				case FieldWeathers.Snow: case FieldWeathers.Blizzard:
					battle.weather=Combat.Weather.HAIL;
					battle.weatherduration=-1;
					break;
				case FieldWeathers.Sandstorm:
					battle.weather=Combat.Weather.SANDSTORM;
					battle.weatherduration=-1;
					break;
				case FieldWeathers.Sunny:
					battle.weather=Combat.Weather.SUNNYDAY;
					battle.weatherduration=-1;
					break;
			}
			battle.shiftStyle=PokemonSystem.battlestyle==0;
			battle.battlescene=PokemonSystem.battlescene==0;
			battle.environment=GetEnvironment();
		}

		public Environments GetEnvironment() {
			if (GameMap == null) return Environments.None;
			if (Global != null && Global.diving) {
				return Environments.Underwater;
			} else if (PokemonEncounters != null && PokemonEncounters.IsCave) {
				return Environments.Cave;
			//} else if (GetMetadata(GameMap.map_id,MapMetadatas.MetadataOutdoor) == null) {
			} else if (GameMap is IGameMapOrgBattle gmo && !GetMetadata(gmo.map_id).Map.Outdoor) {
				return Environments.None;
			} else {
				switch (GamePlayer.terrain_tag) {
					case Terrains.Grass:
						return Environments.Grass;       // Normal grass
					case Terrains.Sand:
						return Environments.Sand;
					case Terrains.Rock:
						return Environments.Rock;
					case Terrains.DeepWater:
						return Environments.MovingWater;
					case Terrains.StillWater:
						return Environments.StillWater;
					case Terrains.Water:
						return Environments.MovingWater;
					case Terrains.TallGrass:
						return Environments.TallGrass;   // Tall grass
					case Terrains.SootGrass:
						return Environments.Grass;       // Sooty tall grass
					case Terrains.Puddle:
						return Environments.StillWater;
				}
			}
			return Environments.None;
		}

		public IPokemon GenerateWildPokemon(Pokemons species,int level,bool isroamer=false) {
			Pokemon genwildpoke=new Monster.Pokemon(species,level: (byte)level);//,Trainer
			Items[] items=genwildpoke.wildHoldItems;
			//Items[] items=Kernal.PokemonItemsData[species] //ToDo: Return Items[3];
			//			.OrderByDescending(x => x.Rarirty)
			//			.Select(x => x.ItemId).ToArray();
			IPokemon firstpoke=Trainer.firstParty;
			int[] chances=new int[]{ 50,5,1 };
			if (firstpoke != null && !firstpoke.isEgg &&
				firstpoke.Ability == Abilities.COMPOUND_EYES) chances=new int[]{ 60,20,5 };
			int itemrnd=Core.Rand.Next(100);
			if (itemrnd<chances[0] || (items[0]==items[1] && items[1]==items[2])) {
				genwildpoke.setItem(items[0]);
			} else if (itemrnd<(chances[0]+chances[1])) {
				genwildpoke.setItem(items[1]);
			} else if (itemrnd<(chances[0]+chances[1]+chances[2])) {
				genwildpoke.setItem(items[2]);
			}
			if (Bag.Quantity(Items.SHINY_CHARM)>0) { //hasConst(Items,:SHINYCHARM) &&
				for (int i = 0; i < 2; i++) {	// 3 times as likely
					if (genwildpoke.IsShiny) break;
					//genwildpoke.PersonalId=Core.Rand.Next(65536)|(Core.Rand.Next(65536)<<16);
					genwildpoke.shuffleShiny();
				}
			}
			if (Core.Rand.Next(65536)<Core.POKERUSCHANCE) {
				genwildpoke.GivePokerus();
			}
			if (firstpoke != null && !firstpoke.isEgg) {
				if (firstpoke.Ability == Abilities.CUTE_CHARM &&
					!genwildpoke.IsSingleGendered) {
					if (firstpoke.IsMale) {
						if (Core.Rand.Next(3)<2) genwildpoke.makeFemale(); else genwildpoke.makeMale();
					} else if (firstpoke.IsFemale) {
						if (Core.Rand.Next(3)<2) genwildpoke.makeMale(); else genwildpoke.makeFemale();
					}
				} else if (firstpoke.Ability == Abilities.SYNCHRONIZE) {
					if (!isroamer && Core.Rand.Next(10)<5) genwildpoke.setNature(firstpoke.Nature);
				}
			}
			//Events.onWildPokemonCreate.trigger(null,genwildpoke);
			//Events.OnWildPokemonCreateEventArgs eventArgs = new Events.OnWildPokemonCreateEventArgs()
			PokemonEssentials.Interface.EventArg.IOnWildPokemonCreateEventArgs eventArgs = new PokemonUnity.EventArg.OnWildPokemonCreateEventArgs()
			{
				Pokemon = genwildpoke
			};
			//Events.OnWildPokemonCreate?.Invoke(this,eventArgs);
			Events.OnWildPokemonCreateTrigger(this,genwildpoke);
			//if (PokemonEncounters != null)
			//	PokemonEncounters.OnWildPokemonCreate?.Invoke(null, eventArgs);
			return genwildpoke;
		}

		/// <summary>
		/// </summary>
		/// https://www.pokecommunity.com/showthread.php?p=10308001#post10308001
		/// <param name="species"></param>
		/// <param name="level"></param>
		/// <param name="moves"></param>
		/// <param name="ability"></param>
		/// <param name="nature"></param>
		/// <param name="gender"></param>
		/// <param name="item"></param>
		/// <param name="shiny"></param>
		/// <param name="outcomeVar"></param>
		/// <param name="canRun"></param>
		/// <param name="canLose"></param>
		/// <returns></returns>
		public bool ControlledWildBattle(Pokemons species, int level, Moves[] moves = null, int? ability = null,
						Natures? nature = null, bool? gender = null, Items? item = null, bool? shiny = null,
						int outcomeVar = 1, bool canRun = true, bool canLose = false) {
			// Create an instance
			//species = getConst(Species, species)
			IPokemon pkmn = new Pokemon(species, level: (byte)level);

			// Give moves.
			// Should be a list of moves:
			if (moves != null) {
				for (int i = 0; i < 4; i++) {
					if (moves.Length > i) //moves[i] != null)
						pkmn.moves[i] = new Attack.Move(moves[i]);
					else //set to none...
						pkmn.moves[i] = new Attack.Move();
				}
			}

			// Give ability
			// NOTE that the ability should be 0, 1 or 2.
			if (ability != null && (new List<int>() { 0, 1, 2 }).Contains(ability.Value)) pkmn.setAbility(ability.Value);

			// Give nature
			if (nature != null) pkmn.setNature(nature.Value);

			// Give gender
			// 0 if male, 1 if female.
			if (gender != null) //pkmn.setGender(gender.Value ? 1 : 0);
				if (gender.Value) pkmn.makeMale(); else pkmn.makeFemale();

			// Give item
			if (item != null) pkmn.setItem(item.Value);

			// Shiny or not.
			if (shiny != null) pkmn.makeShiny();

			// Start the battle.
			// This is copied from WildBattle.

			// Potentially call a different WildBattle-type method instead (for roaming
			// Pokémon, Safari battles, Bug Contest battles)
			bool?[] handled=new bool?[]{ null };
			//Events.onWildBattleOverride.trigger(null,species,level,handled);
			PokemonEssentials.Interface.EventArg.IOnWildBattleOverrideEventArgs e1 = new PokemonUnity.EventArg.OnWildBattleOverrideEventArgs()
			{
				Species = species,
				Level = level
				//,Result = handled
			};
			Events.OnWildBattleOverrideTrigger(this,e1);
			//Events.OnWildBattleOverrideTrigger(this,species,level,handled);
			if (handled[0]!=null) {
				return handled[0].Value;
				//return handled[0].Value ? Combat.BattleResults.WON : Combat.BattleResults.ABORTED;
			}
			// Set some battle rules
			//if (outcomeVar != 1) battle.rules["outcomeVar"] = outcomeVar; //setBattleRule("outcomeVar", outcomeVar);
			//if (!canRun) setBattleRule("cannotRun");
			//if (canLose) setBattleRule("canLose");
			// Perform the battle
			//Combat.BattleResults decision = WildBattleCore(pkmn,outcomeVar,canRun,canLose);
			Combat.BattleResults decision = WildBattleCore(pkmn.Species, pkmn.Level, outcomeVar, canRun, canLose) ? Combat.BattleResults.WON : Combat.BattleResults.LOST;
			// Used by the Poké Radar to update/break the chain
			//Events.onWildBattleEnd.trigger(null,species,level,decision);
			PokemonEssentials.Interface.EventArg.IOnWildBattleEndEventArgs e3 = new PokemonUnity.EventArg.OnWildBattleEndEventArgs()
			{
				Species = species,
				Level = level
				,Result = decision
			};
			//Events.OnWildBattleEnd?.Invoke(null,e3);
			Events.OnWildBattleEndTrigger(this,species, level, decision);
			// Return false if the player lost or drew the battle, and true if any other result
			return (decision != Combat.BattleResults.LOST && decision != Combat.BattleResults.DRAW);
		}

		/// <summary>
		/// </summary>
		/// <param name="species"></param>
		/// <param name="level"></param>
		/// <param name="variable"></param>
		/// <param name="canescape"></param>
		/// <param name="canlose"></param>
		/// <returns></returns>
		public bool WildBattleCore(Pokemons species,int level,int? variable=null,bool canescape=true,bool canlose=false) {
		//public Combat.BattleResults WildBattle(Pokemons species,int level,int? variable=null,bool canescape=true,bool canlose=false) {
			if ((Input.press(PokemonUnity.Input.CTRL) && Core.DEBUG) || Trainer.pokemonCount==0) {
				if (Trainer.pokemonCount>0 && this is IGameMessage m) {
					m.Message(Game._INTL("SKIPPING BATTLE..."));
				}
				Set(variable,1);
				Global.nextBattleBGM=null;
				Global.nextBattleME=null;
				Global.nextBattleBack=null;
				return true; //Combat.BattleResults.WON;
			}
			//if (species is String || species is Symbol) {
			//  species=getID(Species,species);
			//}
			bool?[] handled=new bool?[]{ null };
			//Events.onWildBattleOverride.trigger(null,species,level,handled);
			PokemonEssentials.Interface.EventArg.IOnWildBattleOverrideEventArgs e1 = new PokemonUnity.EventArg.OnWildBattleOverrideEventArgs()
			{
				Species = species,
				Level = level
				//,Result = handled
			};
			//Events.OnWildBattleOverride?.Invoke(null, e1);
			Events.OnWildBattleOverrideTrigger(null, e1); //if using listener to skip battles
			handled[0] = e1.Result.HasValue ? e1.Result==Combat.BattleResults.WON || e1.Result==Combat.BattleResults.CAPTURED : false; //ToDo: Convert Results to a nullable bool?
			if (handled[0]!=null) {
				return handled[0].Value;
				//return handled[0].Value ? Combat.BattleResults.WON : Combat.BattleResults.ABORTED;
			}
			List<int> currentlevels=new List<int>();
			foreach (IPokemon i in Trainer.party) {
				currentlevels.Add(i.Level);
			}
			IPokemon genwildpoke=GenerateWildPokemon(species,level);
			//Events.onStartBattle.trigger(null,genwildpoke);
			//PokemonEssentials.Interface.EventArg.IOnWildPokemonCreateEventArgs eventArgs = new PokemonUnity.EventArg.OnWildPokemonCreateEventArgs()
			//{
			//	Pokemon = genwildpoke
			//};
			//Events.OnStartBattle?.Invoke(null, EventArgs.Empty);
			Events.OnStartBattleTrigger(this);
			IPokeBattle_Scene scene=NewBattleScene();
			IBattle battle=new Combat.Battle(scene,Trainer.party,new IPokemon[] { genwildpoke },new ITrainer[] { Trainer },null);
			battle.internalbattle=true;
			battle.cantescape=!canescape;
			PrepareBattle(battle);
			Combat.BattleResults decision=0;
			BattleAnimation(GetWildBattleBGM(species), block: () => {
				SceneStandby(() => {
					decision=battle.StartBattle(canlose);
				});
				foreach (IPokemon i in Trainer.party) { if (i is IPokemonMegaEvolution f) f.makeUnmega(); } //rescue null
				if (Global.partner != null) {
					HealAll();
					foreach (IPokemon i in Global.partner.party) { //partner[3]
						i.Heal();
						if (i is IPokemonMegaEvolution f) f.makeUnmega(); //rescue null
					}
				}
				if (decision==Combat.BattleResults.LOST || decision==Combat.BattleResults.DRAW) {		// If loss or draw
					if (canlose) {
						foreach (var i in Trainer.party) { i.Heal(); }
						for (int i = 0; i < 10; i++) {
							Graphics?.update();
						}
//					} else {
//						if (Game.GameData.GameSystem != null && Game.GameData.GameSystem is IGameSystem s) {
//							s.bgm_pause();
//							s.bgs_pause();
//						}
//						Game.GameData.StartOver();
					}
				}
				//Events.onEndBattle.trigger(null,decision,canlose);
				PokemonEssentials.Interface.EventArg.IOnEndBattleEventArgs e2 = new PokemonUnity.EventArg.OnEndBattleEventArgs()
				{
					Decision = decision,
					CanLose = canlose
				};
				//Events.OnBattleEnd?.Invoke(null,e2);
				//Events.OnEndBattleTrigger(this, e2);
				Events.OnEndBattleTrigger(this, decision, canlose);
			});
			Input.update();
			Set(variable,decision);
			//Events.onWildBattleEnd.trigger(null,species,level,decision);
			PokemonEssentials.Interface.EventArg.IOnWildBattleEndEventArgs e3 = new PokemonUnity.EventArg.OnWildBattleEndEventArgs()
			{
				Species = species,
				Level = level
				,Result = decision
			};
			//Events.OnWildBattleEnd?.Invoke(this,e3);
			Events.OnWildBattleEndTrigger(this,species,level,decision);
			return decision!=Combat.BattleResults.LOST;
		}

		/// <summary>
		/// </summary>
		/// <param name="species"></param>
		/// <param name="level"></param>
		/// <param name="variable"></param>
		/// <param name="canescape"></param>
		/// <param name="canlose"></param>
		/// <returns></returns>
		public bool WildBattle(Pokemons species,int level,int? variable=null,bool canescape=true,bool canlose=false) {
		//public Combat.BattleResults WildBattle(Pokemons species,int level,int? variable=null,bool canescape=true,bool canlose=false) {
			//IPokemon genwildpoke=GenerateWildPokemon(species,level);
			//return WildBattleCore(genwildpoke.Species, genwildpoke.Level, variable, canescape, canlose);
			if ((Input.press(PokemonUnity.Input.CTRL) && Core.DEBUG) || Trainer.pokemonCount==0) {
				if (Trainer.pokemonCount>0 && this is IGameMessage m) {
					m.Message(Game._INTL("SKIPPING BATTLE..."));
				}
				Set(variable,1);
				Global.nextBattleBGM=null;
				Global.nextBattleME=null;
				Global.nextBattleBack=null;
				return true; //Combat.BattleResults.WON;
			}
			//if (species is String || species is Symbol) {
			//  species=getID(Species,species);
			//}
			bool?[] handled=new bool?[]{ null };
			//Events.onWildBattleOverride.trigger(null,species,level,handled);
			PokemonEssentials.Interface.EventArg.IOnWildBattleOverrideEventArgs e1 = new PokemonUnity.EventArg.OnWildBattleOverrideEventArgs()
			{
				Species = species,
				Level = level
				//,Result = handled
			};
			//Events.OnWildBattleOverride?.Invoke(null, e1);
			Events.OnWildBattleOverrideTrigger(null, e1); //if using listener to skip battles
			handled[0] = e1.Result.HasValue ? e1.Result==Combat.BattleResults.WON || e1.Result==Combat.BattleResults.CAPTURED : false; //ToDo: Convert Results to a nullable bool?
			if (handled[0]!=null) {
				return handled[0].Value;
				//return handled[0].Value ? Combat.BattleResults.WON : Combat.BattleResults.ABORTED;
			}
			List<int> currentlevels=new List<int>();
			foreach (IPokemon i in Trainer.party) {
				currentlevels.Add(i.Level);
			}
			IPokemon genwildpoke=GenerateWildPokemon(species,level);
			//Events.onStartBattle.trigger(null,genwildpoke);
			//PokemonEssentials.Interface.EventArg.IOnWildPokemonCreateEventArgs eventArgs = new PokemonUnity.EventArg.OnWildPokemonCreateEventArgs()
			//{
			//	Pokemon = genwildpoke
			//};
			//Events.OnStartBattle?.Invoke(null, EventArgs.Empty);
			Events.OnStartBattleTrigger(this);
			IPokeBattle_Scene scene=NewBattleScene();
			IBattle battle=new Combat.Battle(scene,Trainer.party,new IPokemon[] { genwildpoke },new ITrainer[] { Trainer },null);
			battle.internalbattle=true;
			battle.cantescape=!canescape;
			PrepareBattle(battle);
			Combat.BattleResults decision=0;
			BattleAnimation(GetWildBattleBGM(species), block: () => {
				SceneStandby(() => {
					decision=battle.StartBattle(canlose);
				});
				foreach (IPokemon i in Trainer.party) { if (i is IPokemonMegaEvolution f) f.makeUnmega(); } //rescue null
				if (Global.partner != null) {
					HealAll();
					foreach (IPokemon i in Global.partner.party) { //partner[3]
						i.Heal();
						if (i is IPokemonMegaEvolution f) f.makeUnmega(); //rescue null
					}
				}
				if (decision==Combat.BattleResults.LOST || decision==Combat.BattleResults.DRAW) {		// If loss or draw
					if (canlose) {
						foreach (var i in Trainer.party) { i.Heal(); }
						for (int i = 0; i < 10; i++) {
							Graphics?.update();
						}
//					} else {
//						if (Game.GameData.GameSystem != null && Game.GameData.GameSystem is IGameSystem s) {
//							s.bgm_pause();
//							s.bgs_pause();
//						}
//						Game.GameData.StartOver();
					}
				}
				//Events.onEndBattle.trigger(null,decision,canlose);
				PokemonEssentials.Interface.EventArg.IOnEndBattleEventArgs e2 = new PokemonUnity.EventArg.OnEndBattleEventArgs()
				{
					Decision = decision,
					CanLose = canlose
				};
				//Events.OnBattleEnd?.Invoke(null,e2);
				Events.OnEndBattleTrigger(this, e2);
			});
			Input.update();
			Set(variable,decision);
			//Events.onWildBattleEnd.trigger(null,species,level,decision);
			PokemonEssentials.Interface.EventArg.IOnWildBattleEndEventArgs e3 = new PokemonUnity.EventArg.OnWildBattleEndEventArgs()
			{
				Species = species,
				Level = level
				,Result = decision
			};
			//Events.OnWildBattleEnd?.Invoke(this,e3);
			Events.OnWildBattleEndTrigger(this,species,level,decision);
			return decision!=Combat.BattleResults.LOST;
		}

		public bool DoubleWildBattle(Pokemons species1,int level1,Pokemons species2,int level2,int? variable=null,bool canescape=true,bool canlose=false) {
		//public Combat.BattleResults DoubleWildBattle(Pokemons species1,int level1,Pokemons species2,int level2,int? variable=null,bool canescape=true,bool canlose=false) {
			if ((Input.press(PokemonUnity.Input.CTRL) && Core.DEBUG) || Trainer.pokemonCount==0) {
				if (Trainer.pokemonCount>0 && this is IGameMessage m) {
					m.Message(Game._INTL("SKIPPING BATTLE..."));
				}
				Set(variable,1);
				Global.nextBattleBGM=null;
				Global.nextBattleME=null;
				Global.nextBattleBack=null;
				return true; //Combat.BattleResults.WON;
			}
			//if (species1 is String || species1 is Symbol) {
			//  species1=getID(Species,species1);
			//}
			//if (species2 is String || species2 is Symbol) {
			//  species2=getID(Species,species2);
			//}
			List<int> currentlevels=new List<int>();
			foreach (IPokemon i in Trainer.party) {
				currentlevels.Add(i.Level);
			}
			IPokemon genwildpoke=GenerateWildPokemon(species1,level1);
			IPokemon genwildpoke2=GenerateWildPokemon(species2,level2);
			PokemonEssentials.Interface.EventArg.IOnWildPokemonCreateEventArgs eventArgs = new PokemonUnity.EventArg.OnWildPokemonCreateEventArgs()
			{
				Pokemon = genwildpoke
			};
			//Events.onStartBattle.trigger(null,genwildpoke);
			//Events.OnStartBattle?.Invoke(this, eventArgs);
			Events.OnStartBattleTrigger(this);//, genwildpoke
			IPokeBattle_Scene scene=NewBattleScene();
			IBattle battle;
			if (Global.partner != null) {
				ITrainer othertrainer=new Trainer(
					Global.partner.trainerTypeName,Global.partner.trainertype);//[1]|[0]
				othertrainer.id=Global.partner.id;//[2]
				othertrainer.party=Global.partner.party;//[3]
				IList<IPokemon> combinedParty=new List<IPokemon>();
				for (int i = 0; i < Trainer.party.Length; i++) { //length should equal 6?
					//combinedParty[i]=Trainer.party[i];
					combinedParty.Add(Trainer.party[i]);
				}
				for (int i = 0; i < othertrainer.party.Length; i++) {
					//combinedParty[6+i]=othertrainer.party[i];
					combinedParty.Add(othertrainer.party[i]);
				}
				battle=new Combat.Battle(scene,combinedParty.ToArray(),new IPokemon[] { genwildpoke, genwildpoke2 },
					new ITrainer[] { Trainer,othertrainer },null);
				battle.fullparty1=true;
			} else {
				battle=new Combat.Battle(scene,Trainer.party,new IPokemon[] { genwildpoke, genwildpoke2 },
					new ITrainer[] { Trainer },null);
				battle.fullparty1=false;
			}
			battle.internalbattle=true;
			battle.doublebattle=battle.DoubleBattleAllowed();
			battle.cantescape=!canescape;
			PrepareBattle(battle);
			Combat.BattleResults decision=0;
			BattleAnimation(GetWildBattleBGM(species1), block: () => {
				SceneStandby(() => {
					decision=battle.StartBattle(canlose);
				});
				foreach (IPokemon i in Trainer.party) { if (i is IPokemonMegaEvolution f) f.makeUnmega(); } //rescue null
				if (Global.partner != null) {
					HealAll();
					foreach (IPokemon i in Global.partner.party) {//[3]
						i.Heal();
						if (i is IPokemonMegaEvolution m) m.makeUnmega(); //rescue null
					}
				}
				if (decision==Combat.BattleResults.LOST || decision==Combat.BattleResults.DRAW) {
					if (canlose) {
						foreach (var i in Trainer.party) { i.Heal(); }
						for (int i = 0; i < 10; i++) {
							Graphics?.update();
						}
//					} else {
//						if (Game.GameData.GameSystem != null && Game.GameData.GameSystem is IGameSystem s) {
//							s.bgm_pause();
//							s.bgs_pause();
//						}
//						Game.GameData.StartOver();
					}
				}
				//Events.onEndBattle.trigger(null,decision,canlose);
				PokemonEssentials.Interface.EventArg.IOnEndBattleEventArgs e2 = new PokemonUnity.EventArg.OnEndBattleEventArgs()
				{
					Decision = decision,
					CanLose = canlose
				};
				//Events.OnBattleEnd?.Invoke(null,e2);
				//Events.OnEndBattleTrigger(this,e2);
				Events.OnEndBattleTrigger(this, decision, canlose);
			});
			Input.update();
			Set(variable,decision);
			return decision!=Combat.BattleResults.LOST && decision!=Combat.BattleResults.DRAW;
		}

		public void CheckAllFainted() {
			if (AllFainted() && this is IGameMessage m) {
				m.Message(Game._INTL("{1} has no usable Pokémon!\\1",Trainer.name));
				m.Message(Game._INTL("{1} blacked out!",Trainer.name));
				if (this is IGameAudioPlay a) a.BGMFade(1.0f);
				if (this is IGameAudioPlay a1) a1.BGSFade(1.0f);
				FadeOutIn(99999, block: () => {
					StartOver();
				});
			}
		}

		public void EvolutionCheck(int[] currentlevels) {
			//  Check conditions for evolution
			for (int i = 0; i < currentlevels.Length; i++) {
				IPokemon pokemon=Trainer.party[i];
				if (pokemon.IsNotNullOrNone() && (currentlevels[i] == null || pokemon.Level!=currentlevels[i])) {
					Pokemons newspecies=CheckEvolution(pokemon)[0];
					if (newspecies>0) {
						//  Start evolution scene
						IPokemonEvolutionScene evo=Scenes.EvolvingScene; //new PokemonEvolutionScene();
						evo.StartScreen(pokemon,newspecies);
						evo.Evolution();
						evo.EndScreen();
					}
				}
			}
		}

		public Items[] DynamicItemList(params Items[] args) {
			List<Items> ret=new List<Items>();
			for (int i = 0; i < args.Length; i++) {
				//if (hasConst(Items,args[i])) {
				//  ret.Add(getConst(Items,args[i].to_sym));
					ret.Add(args[i]);
				//}
			}
			return ret.ToArray();
		}

		/// <summary>
		/// Runs the Pickup event after a battle if a Pokemon has the ability Pickup.
		/// </summary>
		/// <param name="pokemon"></param>
		public void Pickup(IPokemon pokemon) {
			if (pokemon.Ability != Abilities.PICKUP || pokemon.isEgg) return;
			if (pokemon.Item!=0) return;
			if (Core.Rand.Next(10)!=0) return;
			Items[] pickupList=new Items[] {
				Items.POTION,
				Items.ANTIDOTE,
				Items.SUPER_POTION,
				Items.GREAT_BALL,
				Items.REPEL,
				Items.ESCAPE_ROPE,
				Items.FULL_HEAL,
				Items.HYPER_POTION,
				Items.ULTRA_BALL,
				Items.REVIVE,
				Items.RARE_CANDY,
				Items.SUN_STONE,
				Items.MOON_STONE,
				Items.HEART_SCALE,
				Items.FULL_RESTORE,
				Items.MAX_REVIVE,
				Items.PP_UP,
				Items.MAX_ELIXIR
			};
			Items[] pickupListRare=new Items[] {
				Items.HYPER_POTION,
				Items.NUGGET,
				Items.KINGS_ROCK,
				Items.FULL_RESTORE,
				Items.ETHER,
				Items.IRON_BALL,
				Items.DESTINY_KNOT,
				Items.ELIXIR,
				Items.DESTINY_KNOT,
				Items.LEFTOVERS,
				Items.DESTINY_KNOT
			};
			if (pickupList.Length!=18) return;
			if (pickupListRare.Length!=11) return;
			int[] randlist= new int[]{ 30,10,10,10,10,10,10,4,4,1,1 };
			List<Items> items=new List<Items>();
			int plevel=Math.Min(100,pokemon.Level);
			int itemstart=(plevel-1)/10;
			if (itemstart<0) itemstart=0;
			for (int i = 0; i < 9; i++) {
				items.Add(pickupList[itemstart+i]);
			}
			items.Add(pickupListRare[itemstart]);
			items.Add(pickupListRare[itemstart+1]);
			int rnd=Core.Rand.Next(100);
			int cumnumber=0;
			for (int i = 0; i < 11; i++) {
				cumnumber+=randlist[i];
				if (rnd<cumnumber) {
					pokemon.setItem(items[i]);
					break;
				}
			}
		}

		public bool Encounter(EncounterOptions enctype) {
		//public bool Encounter(Method enctype) {
			if (Global.partner != null) {
				IPokemon encounter1=PokemonEncounters.EncounteredPokemon(enctype);
				if (!encounter1.IsNotNullOrNone()) return false;
				IPokemon encounter2=PokemonEncounters.EncounteredPokemon(enctype);
				if (!encounter2.IsNotNullOrNone()) return false;
				if (PokemonTemp is ITempMetadataField f0) f0.encounterType=(Method)enctype;
				DoubleWildBattle(encounter1.Species,encounter1.Level,encounter2.Species,encounter2.Level); //[0]|[1]
				if (PokemonTemp is ITempMetadataField f1) f1.encounterType=null;
				return true;
			} else {
				IPokemon encounter=PokemonEncounters.EncounteredPokemon(enctype);
				if (!encounter.IsNotNullOrNone()) return false;
				if (PokemonTemp is ITempMetadataField f0) f0.encounterType=(Method)enctype;
				WildBattle(encounter.Species,encounter.Level); //[0]|[1]
				if (PokemonTemp is ITempMetadataField f1) f1.encounterType=null;
				return true;
			}
		}

		//Events.onStartBattle+=delegate(object sender, EventArgs e) {
		//Events.OnStartBattle+=delegate(object sender, PokemonEssentials.Interface.EventArg.IOnStartBattleEventArgs e) {
		protected virtual void Events_OnStartBattle(object sender, EventArgs e) {
			if (PokemonTemp is ITempMetadataField m)
			{
				//PokemonTemp.evolutionLevels = new int[6];
				m.evolutionLevels = new int[6];
				for (int i = 0; i < Trainer.party.Length; i++)
				{
					//PokemonTemp.evolutionLevels[i] = Trainer.party[i].Level;
					m.evolutionLevels[i] = Trainer.party[i].Level;
				}
			}
		}

		//Events.OnEndBattle+=delegate(object sender, PokemonEssentials.Interface.EventArg.IOnEndBattleEventArgs e) {
		//protected virtual void Events_OnEndBattle(object sender, PokemonEssentials.Interface.EventArg.IOnEndBattleEventArgs e) {
		protected virtual void Events_OnEndBattle(object sender, PokemonUnity.EventArg.OnEndBattleEventArgs e) {
			int decision = (int)e.Decision; //[0];
			bool canlose = e.CanLose; //[1];
			if (Core.USENEWBATTLEMECHANICS || (decision!=2 && decision!=5)) {		// not a loss or a draw
				if (PokemonTemp is ITempMetadataField m && m.evolutionLevels != null) {
					//EvolutionCheck(PokemonTemp.evolutionLevels);
					//PokemonTemp.evolutionLevels=null;
					EvolutionCheck(m.evolutionLevels);
					m.evolutionLevels=null;
				}
			}
			if (decision==1) {
				foreach (IPokemon pkmn in Trainer.party) {
					Pickup(pkmn);
					if (pkmn.Ability == Abilities.HONEY_GATHER && !pkmn.isEgg && !pkmn.hasItem()) {
						//if (hasConst(PBItems,:HONEY)) {
							int chance = 5 + (int)Math.Floor((pkmn.Level-1)/10d)*5;
							if (Core.Rand.Next(100)<chance) pkmn.setItem(Items.HONEY);
						//}
					}
				}
			}
			if ((decision==2 || decision==5) && !canlose) {
				GameSystem.bgm_unpause();
				GameSystem.bgs_unpause();
				StartOver();
			}
		}

		#region Scene_Map and Spriteset_Map
		public partial class Scene_Map {
			public virtual void createSingleSpriteset(int map) {
				//temp=Game.GameData.Scene.spriteset().getAnimations();
				//@spritesets[map]=new Spriteset_Map(MapFactory.maps[map]);
				//Game.GameData.Scene.spriteset().restoreAnimations(temp);
				//MapFactory.setSceneStarted(this);
				//updateSpritesets();
			}
		}

		public partial class Spriteset_Map {
			private int usersprites;
			public int getAnimations() {
				return @usersprites;
			}

			public void restoreAnimations(int anims) {
				@usersprites=anims;
			}
		}

		//Events.onSpritesetCreate+=delegate(object sender, EventArgs e) {
		protected virtual void Events_OnSpritesetCreate(object sender, PokemonUnity.EventArg.OnSpritesetCreateEventArgs e) {
			ISpritesetMap spriteset=e.SpritesetId; //[0] Spriteset being created
			IViewport viewport=e.Viewport; //[1] Viewport used for tilemap and characters
			IGameMap map=spriteset.map; // Map associated with the spriteset (not necessarily the current map).
			foreach (int i in map.events.Keys) {
				if (map.events[i] is IGameEvent ge) {
					if (ge.name.Contains("OutdoorLight")) { //[/^OutdoorLight\((\w+)\)$/]
						//string filename=$~[1].ToString();
						//spriteset.addUserSprite(new LightEffect_DayNight(map.events[i],viewport,map,filename));
					//} else if (map.events[i].name=="OutdoorLight") {
					//	spriteset.addUserSprite(new LightEffect_DayNight(map.events[i],viewport,map));
					//} else if (map.events[i].name[/^Light\((\w+)\)$/]) {
					//	string filename=$~[1].ToString();
					//	spriteset.addUserSprite(new LightEffect_Basic(map.events[i],viewport,map,filename));
					} else if (ge.name=="Light") {
						//spriteset.addUserSprite(new LightEffect_Basic(map.events[i],viewport,map));
					}
				}
			}
			//spriteset.addUserSprite(new Particle_Engine(viewport,map));
		}

		public void OnSpritesetCreate(ISpritesetMap spriteset,IViewport viewport) {
			//Events.onSpritesetCreate.trigger(null,spriteset,viewport);
			//Events.OnSpritesetCreate.Invoke(null,spriteset,viewport);
		}
		#endregion

		#region Field movement
		public bool Ledge(float xOffset,float yOffset) {
			if (Terrain.isLedge(FacingTerrainTag())) {
				if (JumpToward(2,true)) {
					if (Scene.spriteset is ISpritesetMapAnimation s) s.addUserAnimation(Core.DUST_ANIMATION_ID,GamePlayer.x,GamePlayer.y,true);
					GamePlayer.increase_steps();
					GamePlayer.check_event_trigger_here(new int[] { 1, 2 });
				}
				return true;
			}
			return false;
		}

		public virtual void SlideOnIce(IGamePlayer @event=null) {
			if (@event == null) @event=GamePlayer;
			if (@event == null) return;
			if (!Terrain.isIce(GetTerrainTag(@event))) return;
			Global.sliding=true;
			int direction=@event.direction;
			//if (@event is IGamePlayerOrgBattle gpo)
			//	direction=gpo.direction;
			bool oldwalkanime=@event.walk_anime;
			@event.straighten();
			@event.pattern=1;
			@event.walk_anime=false;
			do { //;loop
				if (!@event.passable(@event.x,@event.y,direction)) break;
				if (!Terrain.isIce(GetTerrainTag(@event))) break;
				@event.move_forward();
				while (@event.moving) {
					Graphics?.update();
					Input.update();
					if (this is IGameMessage s) s.UpdateSceneMap();
				}
			} while (true);
			@event.center(@event.x,@event.y);
			@event.straighten();
			@event.walk_anime=oldwalkanime;
			Global.sliding=false;
		}

		/// <summary>
		/// Poison event on each step taken
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//Events.onStepTakenTransferPossible+=delegate(object sender, EventArgs e) {
		//protected virtual void Events_OnStepTakenTransferPossible(object sender, PokemonEssentials.Interface.EventArg.IOnStepTakenTransferPossibleEventArgs e) {
		protected virtual void Events_OnStepTakenTransferPossible(object sender, PokemonUnity.EventArg.OnStepTakenTransferPossibleEventArgs e) {
			bool[] handled = new bool[] { e.Index }; //[0];
			if (handled[0]) return; //continue;
			if (Global.stepcount % 4 == 0 && Core.POISONINFIELD) {
				bool flashed=false;
				foreach (Pokemon i in Trainer.party) {
					if (i.Status==Status.POISON && i.HP>0 && !i.isEgg &&
						i.Ability != Abilities.IMMUNITY) {
						if (!flashed) {
							//GameScreen.start_flash(new Color(255,0,0,128), 4);
							flashed=true;
						}
						if (i.HP==1 && !Core.POISONFAINTINFIELD && this is IGameMessage m0) {
							i.Status=0;
							m0.Message(Game._INTL("{1} survived the poisoning.\\nThe poison faded away!\\1",i.Name));
							continue;
						}
						i.HP-=1;
						if (i.HP==1 && !Core.POISONFAINTINFIELD && this is IGameMessage m1) {
							i.Status=0;
							m1.Message(Game._INTL("{1} survived the poisoning.\\nThe poison faded away!\\1",i.Name));
						}
						if (i.HP==0 && this is IGameMessage m2) {
							//i.ChangeHappiness("faint");
							i.ChangeHappiness(HappinessMethods.FAINT);
							i.Status=0;
							m2.Message(Game._INTL("{1} fainted...\\1",i.Name));
						}
						if (AllFainted()) handled[0]=true;
						CheckAllFainted();
					}
				}
			}
		}

		//Events.onStepTaken+=proc{
		protected virtual void Events_OnStepTaken(object sender, EventArgs e) {
			//if (Global.happinessSteps == null) Global.happinessSteps=0;
			Global.happinessSteps+=1;
			if (Global.happinessSteps==128) {
				foreach (var pkmn in Trainer.party) {
					if (pkmn.HP>0 && !pkmn.isEgg) {
						if (Core.Rand.Next(2)==0)
							//pkmn.changeHappiness("walking");
							pkmn.ChangeHappiness(HappinessMethods.WALKING);
					}
				}
				Global.happinessSteps=0;
			}
		}

		//Events.onStepTakenFieldMovement+=delegate(object sender, EventArgs e) {
		//protected virtual void Events_OnStepTakenFieldMovement(object sender, PokemonEssentials.Interface.EventArg.IOnStepTakenFieldMovementEventArgs e) {
		protected virtual void Events_OnStepTakenFieldMovement(object sender, PokemonUnity.EventArg.OnStepTakenFieldMovementEventArgs e) {
			//IGamePlayer @event=e[0]; // Get the event affected by field movement
			IGamePlayer @event=e.Index; // Get the event affected by field movement
			ITilePosition thistile=null;
			//thistile=MapFactory.getRealTilePos(@event.map.map_id,@event.x,@event.y);
			if (@event.map is IGameMapOrgBattle gmo) //GameMap
				thistile=MapFactory.getRealTilePos(gmo.map_id,@event.x,@event.y);
			IGameMap map=MapFactory.getMap(thistile.MapId); //thistile[0]
			//IGameMapOrgBattle map = MapFactory.getMap(thistile.MapId) as IGameMapOrgBattle; //thistile[0]
			int sootlevel=-1;
			foreach (int i in new int[] { 2, 1, 0 }) { //elevation/layer
				//int? tile_id = map.data[thistile[1],thistile[2],i];
				int? tile_id = map.data[(int)thistile.X,(int)thistile.Y,i];
				if (tile_id == null) continue;
				if (//map.terrain_tags[tile_id.Value] != Terrains.NULL &&
					map.terrain_tags[tile_id.Value]==Terrains.SootGrass) {
					sootlevel=i;
					break;
				}
			}
			if (sootlevel>=0) { //&& hasConst(Items,:SOOTSACK)
				//if (Global.sootsack == null) Global.sootsack=0;
				//map.data[thistile[1],thistile[2],sootlevel]=0
				map.data[(int)thistile.X,(int)thistile.Y,sootlevel]=0;
				if (@event==GamePlayer && Bag.Quantity(Items.SOOT_SACK)>0) {
					Global.sootsack+=1;
				}
				if (Scene is ISceneMapField m && map is IGameMapOrgBattle mob) m.createSingleSpriteset(mob.map_id);
			}
		//}
		//
		////Events.onStepTakenFieldMovement+=delegate(object sender, EventArgs e) {
		//protected virtual void OnStepTakenFieldMovement(object sender, PokemonEssentials.Interface.EventArg.IOnStepTakenFieldMovementEventArgs e) {
		//	//IGamePlayer @event=e[0]; // Get the event affected by field movement
		//	IGamePlayer @event=e.Index; // Get the event affected by field movement
			if (Scene is ISceneMap) {
				Terrains currentTag=GetTerrainTag(@event);
				if (Terrain.isJustGrass(GetTerrainTag(@event,true))) {		// Won't show if under bridge
					if (Scene.spriteset is ISpritesetMapAnimation sma) sma.addUserAnimation(Core.GRASS_ANIMATION_ID,@event.x,@event.y,true);
				} else if (@event==GamePlayer && currentTag==Terrains.WaterfallCrest) {
					//Descend waterfall, but only if this event is the player
					if (this is IGameHiddenMoves f0) f0.DescendWaterfall(@event);
				} else if (@event==GamePlayer && Terrain.isIce(currentTag) && !Global.sliding) {
					if (this is IGameField f1) f1.SlideOnIce(@event);
				}
			}
		}

		public void BattleOnStepTaken() {
			if (Trainer.party.Length>0) {
				EncounterOptions? encounterType=PokemonEncounters.EncounterType();
				if (encounterType>=0) {
					if (PokemonEncounters.IsEncounterPossibleHere) {
						IPokemon encounter=PokemonEncounters.GenerateEncounter(encounterType.Value);
						encounter=EncounterModifier.triggerEncounter(encounter);
						if (PokemonEncounters.CanEncounter(encounter)) {
							if (Global.partner != null) {
								IPokemon encounter2=PokemonEncounters.EncounteredPokemon(encounterType.Value);
								DoubleWildBattle(encounter.Species,encounter.Level,encounter2.Species,encounter2.Level);
							} else {
								WildBattle(encounter.Species,encounter.Level);
							}
						}
						EncounterModifier.triggerEncounterEnd();
					}
				}
			}
		}

		public void OnStepTaken(bool eventTriggered) {
			if (GamePlayer.move_route_forcing || (this is IGameMessage g && g.MapInterpreterRunning()) || Trainer == null) {
				//  if forced movement or if no trainer was created yet
				//Events.onStepTakenFieldMovement.trigger(null,GamePlayer);
				PokemonEssentials.Interface.EventArg.IOnStepTakenFieldMovementEventArgs e0 = new PokemonUnity.EventArg.OnStepTakenFieldMovementEventArgs()
				{
					Index = GamePlayer
				};
				//Events.OnStepTakenFieldMovement?.Invoke(null,e0);
				Events.OnStepTakenFieldMovementTrigger(this,e0);
				return;
			}
			//if (Global.stepcount == null) Global.stepcount=0;
			Global.stepcount+=1;
			Global.stepcount&=0x7FFFFFFF;
			//Events.onStepTaken.trigger(null);
			//Events.OnStepTaken?.Invoke(null,EventArgs.Empty);
			Events.OnStepTakenTrigger(this);
			//Events.onStepTakenFieldMovement.trigger(null,GamePlayer)
			//Events.OnStepTakenFieldMovementTrigger(null,GamePlayer); //Was commented out in essentials
			bool?[] handled= new bool?[]{ null };
			//Events.OnStepTakenTransferPossible.trigger(null,handled);
			PokemonEssentials.Interface.EventArg.IOnStepTakenTransferPossibleEventArgs e1 = new PokemonUnity.EventArg.OnStepTakenTransferPossibleEventArgs();
			//Events.OnStepTakenTransferPossible?.Invoke(null,e1);
			Events.OnStepTakenTransferPossibleTrigger(this, e1);
			handled[0]=e1.Index;
			if (handled[0]==true) return;
			if (!eventTriggered) {
				BattleOnStepTaken();
			}
		}

		// This method causes a lot of lag when the game is encrypted
		public string GetPlayerCharset(MetadataPlayer meta,int charset,ITrainer trainer=null) {
			if (trainer == null) trainer=Trainer;
			int outfit=trainer != null ? trainer.outfit.Value : 0;
			string ret=meta[charset];
			if (ret == null || ret=="") ret=meta[1];
//			if FileTest.image_exist("Graphics/Characters/"+ret+"_"+outfit.ToString())
			if (ResolveBitmap("Graphics/Characters/"+ret+"_"+outfit.ToString()) != null) {
				ret=ret+"_"+outfit.ToString();
			}
			return ret;
		}

		public virtual void UpdateVehicle() {
			//string[] meta=(string[])Game.GetMetadata(0, GlobalMetadatas.MetadataPlayerA+Global.playerID);
			MetadataPlayer meta=GetMetadata(0).Global.Players[Global.playerID];
			//if (meta != null) {
				if (Global.diving) {
					GamePlayer.character_name=GetPlayerCharset(meta,5); // Diving graphic
				} else if (Global.surfing) {
					GamePlayer.character_name=GetPlayerCharset(meta,3); // Surfing graphic
				} else if (Global.bicycle) {
					GamePlayer.character_name=GetPlayerCharset(meta,2); // Bicycle graphic
				} else {
					GamePlayer.character_name=GetPlayerCharset(meta,1); // Regular graphic
				}
			//}
		}

		public void CancelVehicles(int? destination=null) {
			Global.surfing=false;
			Global.diving=false;
			if (destination == null || !CanUseBike(destination.Value)) {
				Global.bicycle=false;
			}
			UpdateVehicle();
		}

		public bool CanUseBike (int mapid) {
			//if ((bool)GetMetadata(mapid,MapMetadatas.MetadataBicycleAlways)) return true;
			if (GetMetadata(mapid).Map.BicycleAlways) return true;
			//bool? val=(bool?)GetMetadata(mapid,MapMetadatas.MetadataBicycle);
			bool? val=GetMetadata(mapid).Map.Bicycle;
			//if (val==null) val=(bool?)GetMetadata(mapid,MapMetadatas.MetadataOutdoor);
			if (val==null) val=GetMetadata(mapid).Map.Outdoor;
			return val != null ? true : false;
		}

		public void MountBike() {
			if (Global.bicycle) return;
			Global.bicycle=true;
			UpdateVehicle();
			//IAudioObject bikebgm=(IAudioObject)GetMetadata(0,GlobalMetadatas.MetadataBicycleBGM);
			string bikebgm=GetMetadata(0).Global.BicycleBGM;
			if (bikebgm != null) {
				if (this is IGameField a) a.CueBGM(bikebgm,0.5f);
			}
		}

		public void DismountBike() {
			if (!Global.bicycle) return;
			Global.bicycle=false;
			UpdateVehicle();
			GameMap.autoplayAsCue();
		}

		public void SetPokemonCenter() {
			if (GameMap is IGameMapOrgBattle gmo)
				Global.pokecenterMapId=gmo.map_id;
			Global.pokecenterX=GamePlayer.x;
			Global.pokecenterY=GamePlayer.y;
			Global.pokecenterDirection=GamePlayer.direction;
		}
		#endregion

		#region Fishing
		public virtual void FishingBegin() {
			Global.fishing=true;
			if (!CommonEvent(Core.FISHINGBEGINCOMMONEVENT)) {
				int patternb = 2*GamePlayer.direction - 1;
				//TrainerTypes playertrainer=GetPlayerTrainerType();
				//string[] meta=(string[])GetMetadata(0,GlobalMetadatas.MetadataPlayerA+Global.playerID);
				MetadataPlayer meta=GetMetadata(0).Global.Players[Global.playerID];
				int num=(Global.surfing) ? 7 : 6;
				//if (meta != null && meta[num]!=null && meta[num]!="") {
					string charset=GetPlayerCharset(meta,num);
					int pattern = 0; do { //4.times |pattern|
						if (GamePlayer is IGamePlayerRunMovement p) p.setDefaultCharName(charset,patternb-pattern);
						int i = 0; do { //;2.times
							Graphics?.update();
							Input.update();
							if (this is IGameMessage a) a.UpdateSceneMap(); i++;
						} while (i < 2); pattern++;
					} while (pattern < 4);
				//}
			}
		}

		public virtual void FishingEnd() {
			if (!CommonEvent(Core.FISHINGENDCOMMONEVENT)) {
				int patternb = 2*(GamePlayer.direction - 2);
				//TrainerTypes playertrainer=GetPlayerTrainerType();
				//string[] meta=(string[])GetMetadata(0,GlobalMetadatas.MetadataPlayerA+Global.playerID);
				MetadataPlayer meta=GetMetadata(0).Global.Players[Global.playerID];
				int num=(Global.surfing) ? 7 : 6;
				//if (meta != null && meta[num]!=null && meta[num]!="") {
					string charset=GetPlayerCharset(meta,num);
					int pattern = 0; do { //4.times |pattern|
						if (GamePlayer is IGamePlayerRunMovement p) p.setDefaultCharName(charset,patternb+pattern);
						int i = 0; do { //;2.times
							Graphics?.update();
							Input.update();
							if (this is IGameMessage a) a.UpdateSceneMap(); i++;
						} while (i < 2); pattern++;
					} while (pattern < 4);
				//}
			}
			Global.fishing=false;
		}

		public virtual bool Fishing(bool hasencounter,int rodtype=1) {
			bool speedup=(Trainer.firstParty.IsNotNullOrNone() && !Trainer.firstParty.isEgg &&
				(Trainer.firstParty.Ability == Abilities.STICKY_HOLD ||
				Trainer.firstParty.Ability == Abilities.SUCTION_CUPS));
			float bitechance=20+(25*rodtype);   // 45, 70, 95
			if (speedup) bitechance*=1.5f;
			int hookchance=100;
			int oldpattern=GamePlayer is IGamePlayerRunMovement p ? p.fullPattern() : 0;
			FishingBegin();
			IWindow_AdvancedTextPokemon msgwindow=this is IGameMessage m ? m.CreateMessageWindow() : null;
			do { //;loop
				int time=2+Core.Rand.Next(10);
				if (speedup) time=Math.Min(time,2+Core.Rand.Next(10));
				string message="";
				int i = 0; do { //;time.times
					message+=". "; i++;
				} while (i < time);
				if (WaitMessage(msgwindow,time)) {
					FishingEnd();
					if (GamePlayer is IGamePlayerRunMovement p0) p0.setDefaultCharName(null,oldpattern);
					if (this is IGameMessage m0) m0.MessageDisplay(msgwindow,Game._INTL("Not even a nibble..."));
					if (this is IGameMessage m1) m1.DisposeMessageWindow(msgwindow);
					return false;
				}
				if (Core.Rand.Next(100)<bitechance && hasencounter) {
					int frames=Core.Rand.Next(21)+20;
					if (!WaitForInput(msgwindow,message+Game._INTL("\r\nOh! A bite!"),frames)) {
						FishingEnd();
						if (GamePlayer is IGamePlayerRunMovement p0) p0.setDefaultCharName(null,oldpattern);
						if (this is IGameMessage m0) m0.MessageDisplay(msgwindow,Game._INTL("The Pokémon got away..."));
						if (this is IGameMessage m1) m1.DisposeMessageWindow(msgwindow);
						return false;
					}
					if (Core.Rand.Next(100)<hookchance || Core.FISHINGAUTOHOOK) {
					if (this is IGameMessage m0) m0.MessageDisplay(msgwindow,Game._INTL("Landed a Pokémon!"));
					if (this is IGameMessage m1) m1.DisposeMessageWindow(msgwindow);
					FishingEnd();
					if (GamePlayer is IGamePlayerRunMovement p0) p0.setDefaultCharName(null,oldpattern);
						return true;
					}
//					bitechance+=15
//					hookchance+=15
				} else {
					FishingEnd();
					if (GamePlayer is IGamePlayerRunMovement p0) p0.setDefaultCharName(null,oldpattern);
					if (this is IGameMessage m0) m0.MessageDisplay(msgwindow,Game._INTL("Not even a nibble..."));
					if (this is IGameMessage m1) m1.DisposeMessageWindow(msgwindow);
					return false;
				}
			} while (true);
			if (this is IGameMessage m2) m2.DisposeMessageWindow(msgwindow);
			return false;
		}

		public virtual bool WaitForInput(IWindow msgwindow,string message,int frames) {
			if (Core.FISHINGAUTOHOOK) return true;
			if (this is IGameMessage m0) m0.MessageDisplay(msgwindow,message,false);
			int i = 0; do { //;frames.times
				Graphics?.update();
				Input.update();
				if (this is IGameMessage m1) m1.UpdateSceneMap();
				if (Input.trigger((int)PokemonUnity.Input.C) || Input.trigger((int)PokemonUnity.Input.B)) {
					return true;
				} i++;
			} while (i < frames);
			return false;
		}

		public virtual bool WaitMessage(IWindow msgwindow,int time) {
			string message="";
			int i = 0; do { //(time+1).times |i|
				if (i>0) message+=". ";
				if (this is IGameMessage m0) m0.MessageDisplay(msgwindow,message,false);
				int j = 0; do { //20.times ;
					Graphics?.update();
					Input.update();
					if (this is IGameMessage m1) m1.UpdateSceneMap();
					if (Input.trigger((int)PokemonUnity.Input.C) || Input.trigger((int)PokemonUnity.Input.B)) {
						return true;
					} j++;
				} while (j < 20); i++;
			} while (i < time+1);
			return false;
		}
		#endregion

		#region Moving between maps
		//Events.onMapChange+=delegate(object sender, EventArgs e) {
		//protected virtual void Events_OnMapChange(object sender, PokemonEssentials.Interface.EventArg.IOnMapChangeEventArgs e) {
		protected virtual void Events_OnMapChange(object sender, PokemonUnity.EventArg.OnMapChangeEventArgs e) {
			if (GameMap is IGameMapOrgBattle gmo)
			{
				int oldid=e.MapId; //[0] previous map ID, 0 if no map ID
				ITilePosition healing=GetMetadata(gmo.map_id).Map.HealingSpot;
				if (healing != null) Global.healingSpot=healing;
				if (PokemonMap != null) PokemonMap.clear();
				if (PokemonEncounters != null) PokemonEncounters.setup(gmo.map_id);
				Global.visitedMaps[gmo.map_id]=true;
				if (oldid!=0 && oldid!=gmo.map_id) {
					IDictionary<int, string> mapinfos = new Dictionary<int, string>(); //$RPGVX ? load_data("Data/MapInfos.rvdata") : load_data("Data/MapInfos.rxdata");
					MetadataWeather? weather=GetMetadata(gmo.map_id).Map.Weather;
					if (GameMap.name!=mapinfos[oldid]) { //GameMap.name!=mapinfos[oldid].name
						//if (weather != null && Core.Rand.Next(100)<weather[1]) GameScreen.weather(weather[0],8,20);
						if (weather != null && Core.Rand.Next(100)<weather.Value.Chance) GameScreen.weather(weather.Value.Weather,8,20);
					} else {
						MetadataWeather? oldweather=GetMetadata(oldid).Map.Weather;
						//if (weather && !oldweather && Core.Rand.Next(100)<weather[1]) GameScreen.weather(weather[0],8,20);
						if (weather!=null && oldweather==null && Core.Rand.Next(100)<weather.Value.Chance) GameScreen.weather(weather.Value.Weather,8,20);
					}
				}
			}
		}

		//Events.onMapChanging+=delegate(object sender, EventArgs e) {
		//protected virtual void Events_OnMapChanging(object sender, PokemonEssentials.Interface.EventArg.IOnMapChangingEventArgs e) {
		protected virtual void Events_OnMapChanging(object sender, PokemonUnity.EventArg.OnMapChangingEventArgs e) {
			int newmapID = e.MapId; //[0];
			IGameMap newmap = e.GameMap; //[1];
			//  Undo the weather (GameMap still refers to the old map)
			IDictionary<int, string> mapinfos = new Dictionary<int, string>(); //$RPGVX ? load_data("Data/MapInfos.rvdata") : load_data("Data/MapInfos.rxdata");
			if (newmapID>0 && GameMap is IGameMapOrgBattle gmo) {
				MetadataWeather? oldweather=GetMetadata(gmo.map_id).Map.Weather;
				if (GameMap.name!=mapinfos[newmapID]) { //GameMap.name!=mapinfos[newmapID].name
					if (oldweather != null) GameScreen.weather(0,0,0);
				} else {
					MetadataWeather? newweather=GetMetadata(newmapID).Map.Weather;
					if (oldweather!= null && newweather == null) GameScreen.weather(0,0,0);
				}
			}
		}

		//Events.onMapSceneChange+=delegate(object sender, EventArgs e) {
		//protected virtual void Events_OnMapSceneChange(object sender, PokemonEssentials.Interface.EventArg.IOnMapSceneChangeEventArgs e) {
		protected virtual void Events_OnMapSceneChange(object sender, PokemonUnity.EventArg.OnMapSceneChangeEventArgs e) {
			ISceneMap scene = e.Object; //[0];
			bool mapChanged = e.NewMap; //[1];
			if (scene == null || scene.spriteset == null) return;
			if (GameMap != null && GameMap is IGameMapOrgBattle gmo) {
				ITilePosition lastmapdetails=Global.mapTrail[0] != null ?
					GetMetadata(Global.mapTrail[0]).Map.MapPosition : new TilePosition(-1, 0, 0); //new int[] { -1, 0, 0 };
				if (lastmapdetails == null) lastmapdetails = new TilePosition(-1, 0, 0); //new int[]{ -1,0,0 };
				ITilePosition newmapdetails=gmo.map_id != null ?
					GetMetadata(gmo.map_id).Map.MapPosition : new TilePosition(-1, 0, 0); //new int[] { -1, 0, 0 };
				if (newmapdetails == null) newmapdetails = new TilePosition(-1, 0, 0); //new int[]{ -1,0,0 };
				if (Global.mapTrail == null) Global.mapTrail=new int[4];
				if (Global.mapTrail[0]!=gmo.map_id) {
					if (Global.mapTrail.Count > 3) Global.mapTrail[3]=Global.mapTrail[2]; //(Global.mapTrail[2] != null)
					if (Global.mapTrail.Count > 2) Global.mapTrail[2]=Global.mapTrail[1]; //(Global.mapTrail[1] != null)
					if (Global.mapTrail.Count > 1) Global.mapTrail[1]=Global.mapTrail[0]; //(Global.mapTrail[0] != null)
				}
				Global.mapTrail[0]=gmo.map_id;   // Update map trail
			}
			bool darkmap=false;//GetMetadata(GameMap.map_id).Map.DarkMap;
			if (GameMap is IGameMapOrgBattle gmo1)
				darkmap=GetMetadata(gmo1.map_id).Map.DarkMap;
			if (darkmap) {
				if (Global.flashUsed) {
					PokemonTemp.darknessSprite.initialize(); //=new DarknessSprite();
					if (scene.spriteset is PokemonEssentials.Interface.ISpritesetMapAnimation s0) s0.addUserSprite(PokemonTemp.darknessSprite);
					//darkness=PokemonTemp.darknessSprite;
					//darkness.radius=176;
				} else {
					PokemonTemp.darknessSprite.initialize(); //=new DarknessSprite();
					if (scene.spriteset is PokemonEssentials.Interface.ISpritesetMapAnimation s1) s1.addUserSprite(PokemonTemp.darknessSprite);
				}
			} else if (!darkmap) {
				Global.flashUsed=false;
				if (PokemonTemp.darknessSprite != null) {
					PokemonTemp.darknessSprite.Dispose();
					PokemonTemp.darknessSprite=null;
				}
			}
			if (mapChanged && GameMap is IGameMapOrgBattle gmo2) {
				if (GetMetadata(gmo2.map_id).Map.ShowArea) {
					bool nosignpost=false;
					if (Global.mapTrail.Count > 1) { //(Global.mapTrail[1] != null)
						for (int i = 0; i < Core.NOSIGNPOSTS.Length/2; i++) {
							if (Core.NOSIGNPOSTS[2*i]==Global.mapTrail[1] && Core.NOSIGNPOSTS[2*i+1]==gmo2.map_id) nosignpost=true;
							if (Core.NOSIGNPOSTS[2*i+1]==Global.mapTrail[1] && Core.NOSIGNPOSTS[2*i]==gmo2.map_id) nosignpost=true;
							if (nosignpost) break;
						}
						IDictionary<int, string> mapinfos = new Dictionary<int, string>(); //$RPGVX ? load_data("Data/MapInfos.rvdata") : load_data("Data/MapInfos.rxdata");
						string oldmapname=mapinfos[Global.mapTrail[1]]; //mapinfos[Global.mapTrail[1]].name
						if (GameMap.name==oldmapname) nosignpost=true;
					}
					if (!nosignpost)
						//if (scene.spriteset is PokemonEssentials.Interface.ISpritesetMapAnimation s2)
						//{
						//	//s2.addUserSprite(new LocationWindow(GameMap.name));
							LocationWindow.initialize(GameMap.name);
						//	//s2.addUserSprite(LocationWindow);
						//}
				}
			}
			if (GameMap is IGameMapOrgBattle gmo3 && GetMetadata(gmo3.map_id).Map.BicycleAlways) {
				MountBike();
			} else {
				if (GameMap is IGameMapOrgBattle gmo4 && !CanUseBike(gmo4.map_id)) {
					DismountBike();
				}
			}
		}

		public void StartOver(bool gameover=false) {
			if (this is IGameBugContest c && c.InBugContest) {
				c.BugContestStartOver();
				return;
			}
			HealAll();
			if (Global.pokecenterMapId != null && Global.pokecenterMapId>= 0) {
				if (gameover) {
					if (this is IGameMessage m) m.Message(Game._INTL("\\w[]\\wm\\c[8]\\l[3]After the unfortunate defeat, {1} scurried to a Pokémon Center.",Trainer.name));
				} else {
					if (this is IGameMessage m) m.Message(Game._INTL("\\w[]\\wm\\c[8]\\l[3]{1} scurried to a Pokémon Center, protecting the exhausted and fainted Pokémon from further harm.",Trainer.name));
				}
				CancelVehicles();
				if (this is IGameDependantEvents d) d.RemoveDependencies();
				GameSwitches[Core.STARTING_OVER_SWITCH]=true;
				//Global.StartingOver(); //ToDo: Uncomment and implement a starting over count-up
				GameTemp.player_new_map_id=Global.pokecenterMapId;
				GameTemp.player_new_x=Global.pokecenterX;
				GameTemp.player_new_y=Global.pokecenterY;
				GameTemp.player_new_direction=Global.pokecenterDirection;
				if (Scene is ISceneMap s) s.transfer_player();
				GameMap.refresh();
			} else {
				//int[] homedata=(int[])GetMetadata(0,GlobalMetadatas.MetadataHome);
				//ITilePosition homedata=Game.GetMetadata(0,GlobalMetadatas.MetadataHome);
				MetadataPosition? homedata=GetMetadata(0).Global.Home;
				if (homedata != null && !RxdataExists(string.Format("Data/Map%03d",homedata.Value.MapId))) { //homedata[0]
					if (Core.DEBUG && this is IGameMessage m) {
						m.Message(string.Format("Can't find the map 'Map{0}' in the Data folder. The game will resume at the player's position.",homedata.Value.MapId)); //homedata[0]
					}
					HealAll();
					return;
				}
				if (gameover) {
					if (this is IGameMessage m) m.Message(Game._INTL("\\w[]\\wm\\c[8]\\l[3]After the unfortunate defeat, {1} scurried home.",Trainer.name));
				} else {
					if (this is IGameMessage m) m.Message(Game._INTL("\\w[]\\wm\\c[8]\\l[3]{1} scurried home, protecting the exhausted and fainted Pokémon from further harm.",Trainer.name));
				}
				if (homedata != null) {
					CancelVehicles();
					if (this is IGameDependantEvents d) d.RemoveDependencies();
					GameSwitches[Core.STARTING_OVER_SWITCH]=true;
					//Global.StartingOver(); //ToDo: Uncomment and implement a starting over count-up
					GameTemp.player_new_map_id=homedata.Value.MapId;		//homedata[0]
					GameTemp.player_new_x=homedata.Value.X;					//homedata[1]
					GameTemp.player_new_y=homedata.Value.Y;					//homedata[2]
					GameTemp.player_new_direction=homedata.Value.Direction;	//homedata[3]
					if (Scene is ISceneMap s) s.transfer_player();
					GameMap.refresh();
				} else {
					HealAll();
				}
			}
			EraseEscapePoint();
		}

		public virtual void CaveEntranceEx(bool exiting) {
			//sprite=new BitmapSprite(Graphics.width,Graphics.height);
			//Bitmap sprite=new BitmapSprite(Graphics.width,Graphics.height);
			////sprite.z=100000;
			//int totalBands=15;
			//int totalFrames=15;
			//float bandheight=((Graphics.height/2)-10f)/totalBands;
			//float bandwidth=((Graphics.width/2)-12f)/totalBands;
			//List<double> grays=new List<double>();
			//int tbm1=totalBands-1;
			//for (int i = 0; i < totalBands; i++) {
			//  grays.Add(exiting ? 0 : 255);
			//}
			//int j = 0; do { //totalFrames.times |j|
			//  float x=0;
			//  float y=0;
			//  float rectwidth=Graphics.width;
			//  float rectheight=Graphics.height;
			//  for (int k = 0; k < j; k++) {
			//    double t=(255.0f)/totalFrames;
			//    if (exiting) {
			//      t=1.0-t;
			//      t*=1.0f+((k)/totalFrames);
			//    } else {
			//      t*=1.0+0.3f*Math.Pow(((totalFrames-k)/totalFrames),0.7f);
			//    }
			//    //grays[k]-=t;
			//    grays[k]=grays[k]-t;
			//    if (grays[k]<0) grays[k]=0;
			//  }
			//  for (int i = 0; i < totalBands; i++) {
			//    double currentGray=grays[i];
			//    sprite.bitmap.fill_rect(new Rect(x,y,rectwidth,rectheight),
			//       new Color(currentGray,currentGray,currentGray));
			//    x+=bandwidth;
			//    y+=bandheight;
			//    rectwidth-=bandwidth*2;
			//    rectheight-=bandheight*2;
			//  }
			//  Graphics?.update();
			//  Input.update(); j++;
			//} while (j < totalFrames);
			//if (exiting) {
			//  ToneChangeAll(new Tone(255,255,255),0);
			//} else {
			//  ToneChangeAll(new Tone(-255,-255,-255),0);
			//}
			//for (j = 0; j < 15; j++) {
			//  if (exiting) {
			//    sprite.color=new Color(255,255,255,j*255/15);
			//  } else {
			//    sprite.color=new Color(0,0,0,j*255/15) ;
			//  }
			//  Graphics?.update();
			//  Input.update();
			//}
			//ToneChangeAll(new Tone(0,0,0),8);
			//for (j = 0; j < 5; j++) {
			//  Graphics?.update();
			//  Input.update();
			//}
			//sprite.Dispose();
		}

		public void CaveEntrance() {
			SetEscapePoint();
			CaveEntranceEx(false);
		}

		public void CaveExit() {
			EraseEscapePoint();
			CaveEntranceEx(true);
		}

		public void SetEscapePoint() {
			if (Global.escapePoint == null) Global.escapePoint=new MetadataPosition(); //float[0];
			float xco=GamePlayer.x;
			float yco=GamePlayer.y;
			int dir = 0;
			switch (GamePlayer.direction) {
				case 2:   // Down
					yco-=1; dir=8;
					break;
				case 4:   // Left
					xco+=1; dir=6;
					break;
				case 6:   // Right
					xco-=1; dir=4;
					break;
				case 8:   // Up
					yco+=1; dir=2;
					break;
			}
			//Global.escapePoint=new float[] { GameMap.map_id, xco, yco, dir };
			Global.escapePoint=new MetadataPosition { MapId = GameMap is IGameMapOrgBattle gmo ? gmo.map_id : 0, X = xco, Y = yco, Direction = dir };
		}

		public void EraseEscapePoint() {
			Global.escapePoint=new MetadataPosition(); //float[0];
		}
		#endregion

		#region Partner trainer
		public void RegisterPartner(TrainerTypes trainerid,string trainername,int partyid=0) {
			CancelVehicles();
			ITrainer trainer = null; //Trainer.LoadTrainer(trainerid,trainername,partyid);
			if (Game.GameData is IGameTrainer gt) trainer = gt.LoadTrainer(trainerid,trainername,partyid);
			//Events.OnTrainerPartyLoad.trigger(null,trainer);
			Events.OnTrainerPartyLoadTrigger(this,trainer);
			ITrainer trainerobject=new Trainer(Game._INTL(trainer.name),trainerid);
			trainerobject.setForeignID(Trainer);
			foreach (IPokemon i in trainer.party) {
				i.trainerID=trainerobject.id;
				i.ot=trainerobject.name;
				//i.SetCatchInfos(trainer:trainerobject);
				i.calcStats();
			}
			//Global.partner=new Trainer(trainerid,trainerobject.name,trainerobject.id,trainer.party);
			ITrainer t=new Trainer(trainertype:trainerid,name:trainerobject.name); t.party = trainer.party;
			Global.partner=t;//new Combat.Trainer(trainerid,trainerobject.name,trainerobject.id,trainer.party);
		}

		public void DeregisterPartner() {
			Global.partner=null;
		}
		#endregion

		#region Constant checks
		// ===============================================================================
		//Events.onMapUpdate+=delegate(object sender, EventArgs e) {   // Pokérus check
		protected virtual void Events_OnMapUpdate(object sender, EventArgs e) {   // Pokérus check
			DateTime? last=Global.pokerusTime;
			DateTime now=GetTimeNow;
			if (last == null || last?.Year!=now.Year || last?.Month!=now.Month || last?.Day!=now.Day) {
				if (Trainer != null && Trainer.party != null) {
					foreach (IPokemon i in Trainer.pokemonParty) {
						i.lowerPokerusCount();
					}
					Global.pokerusTime=now;
				}
			}
		//}
		//Mostly just about showing low batt on screen...
		//Events.onMapUpdate+=delegate(object sender, EventArgs e) {
		//	DateTime time=GetTimeNow();
		//	if (time.Second==0 && Trainer != null && Global != null && GamePlayer != null && GameMap != null &&
		//		!PokemonTemp.batterywarning && !GamePlayer.move_route_forcing &&
		//		!MapInterpreterRunning && !GameTemp.message_window_showing) {
		//		//&& BatteryLow()) {
		//		PokemonTemp.batterywarning=true;
		//		if (this is IGameMessage m) m.Message(Game._INTL("The game has detected that the battery is low. You should save soon to avoid losing your progress."));
		//	}
		//	if (PokemonTemp.cueFrames) {
		//		PokemonTemp.cueFrames-=1;
		//		if (PokemonTemp.cueFrames<=0) {
		//			PokemonTemp.cueFrames=null;
		//			if (GameSystem.getPlayingBGM==null) {
		//				BGMPlay(PokemonTemp.cueBGM);
		//			}
		//		}
		//	}
		}

		/// <summary>
		/// Returns whether the Poké Center should explain Pokérus to the player, if a
		/// healed Pokémon has it.
		/// </summary>
		/// <returns></returns>
		public bool Pokerus() {
			if (GameSwitches[Core.SEEN_POKERUS_SWITCH]) return false;
			//if (Core.SEEN_POKERUS_SWITCH) return false;
			if (Global.Features.SeenPokerusSwitch) return false;
			foreach (IPokemon i in Trainer.party) {
				//if (i.PokerusStage==1) return true;
				if (i.PokerusStage==true) return true;
			}
			return false;
		}


		/// <summary>
		/// Connects to User's OS and checks if laptop battery
		/// life alert should be displayed on screen
		/// </summary>
		/// <returns></returns>
		//public bool BatteryLow() {
		//  //int[] power="\0"*12;
		//  int[] power=new int[12];
		//  try {
		//    sps=new Win32API('kernel32.dll','GetSystemPowerStatus','p','l');
		//  } catch (Exception) {
		//    return false;
		//  }
		//  if (sps.call(power)==1) {
		//    status=power;//.unpack("CCCCVV")
		//    //  Battery Flag
		//    if (status[1]!=255 && (status[1]&6)!=0) {		// Low or Critical
		//      return true;
		//    }
		//    //  Battery Life Percent
		//    if (status[2]<3) {		// Less than 3 percent
		//      return true;
		//    }
		//    //  Battery Life Time
		//    if (status[4]<300) {		// Less than 5 minutes
		//      return true;
		//    }
		//  }
		//  return false;
		//}
		#endregion

		#region Audio playing
		public void CueBGM(string bgm, float seconds, int? volume = null, float? pitch = null) {
			if (bgm == null) return;
			if (this is IGameAudioPlay a)
				CueBGM((IAudioBGM)a.ResolveAudioFile(bgm,volume,pitch), seconds, volume, pitch);
			//IAudioBGM playingBGM=GameSystem.playing_bgm;
			//if (playingBGM == null || playingBGM.name!=bgm.name || playingBGM.pitch!=bgm.pitch) {
			//	BGMFade(seconds);
			//	if (PokemonTemp.cueFrames == null) {
			//		PokemonTemp.cueFrames=(int)((seconds*Graphics.frame_rate)*3/5);
			//	}
			//	PokemonTemp.cueBGM=bgm;
			//} else if (playingBGM != null) {
			//	BGMPlay(bgm);
			//}
		}

		public void CueBGM(IAudioBGM bgm, float seconds, int? volume = null, float? pitch = null) {
			if (bgm == null) return;
			if (this is IGameAudioPlay a)
				bgm=(IAudioBGM)a.ResolveAudioFile(bgm,volume,pitch);
			IAudioBGM playingBGM=GameSystem.playing_bgm;
			if (playingBGM == null || playingBGM.name!=bgm.name || playingBGM.pitch!=bgm.pitch) {
				if (this is IGameAudioPlay a1) a1.BGMFade(seconds);
				if (PokemonTemp is ITempMetadataField f0 && f0.cueFrames == null) {
					f0.cueFrames=(seconds*Graphics.frame_rate)*3/5;
				}
				if (PokemonTemp is ITempMetadataField f1) f1.cueBGM=bgm;
			} else if (playingBGM != null && this is IGameAudioPlay a2) {
				a2.BGMPlay(bgm);
			}
		}

		public void AutoplayOnTransition() {
			//string surfbgm=GetMetadata(0,MetadataSurfBGM);
			string surfbgm=GetMetadata(0).Global.SurfBGM;
			if (Global.surfing && surfbgm != null && this is IGameAudioPlay a) {
				a.BGMPlay(surfbgm);
			} else {
				GameMap.autoplayAsCue();
			}
		}

		public void AutoplayOnSave() {
			//string surfbgm=GetMetadata(0,MetadataSurfBGM);
			string surfbgm=GetMetadata(0).Global.SurfBGM;
			if (Global.surfing && surfbgm != null && this is IGameAudioPlay a) {
				a.BGMPlay(surfbgm);
			} else {
				GameMap.autoplay();
			}
		}
		#endregion

		#region Voice recorder
		public virtual IWaveData Record(string text,float maxtime=30.0f) {
			if (text == null) text="";
			IWindow_UnformattedTextPokemon textwindow = null; //new Window_UnformattedTextPokemon().WithSize(text,
			//	0,0,Graphics.width,Graphics.height-96);
			//textwindow.z=99999;
			if (text=="") {
				textwindow.visible=false;
			}
			IWaveData wave=null;
			IWindow_AdvancedTextPokemon msgwindow=(this as IGameMessage).CreateMessageWindow();
			float oldvolume=this is IGameAudio a ? a.Audio_bgm_get_volume : 0;
			if (this is IGameAudio a1) a1.Audio_bgm_set_volume(0);
			int delay=2;
			int i = 0; do { //delay.times |i|
				if (this is IGameMessage m) m.MessageDisplay(msgwindow,
					string.Format("Recording in {0} second(s)...\nPress ESC to cancel.",delay-i),false);
				int n = 0; do { //;Graphics.frame_rate.times
					Graphics?.update();
					Input.update();
					textwindow.update();
					msgwindow.update();
					if (Input.trigger(PokemonUnity.Input.B)) {
						if (this is IGameAudio a2) a2.Audio_bgm_set_volume(oldvolume);
						(this as IGameMessage).DisposeMessageWindow(msgwindow);
						textwindow.Dispose();
						return null;
					} n++;
				} while(n < Graphics.frame_rate); i++;
			} while(i < delay);
			(this as IGameMessage).MessageDisplay(msgwindow,
				Game._INTL("NOW RECORDING\nPress ESC to stop recording."),false);
			if (beginRecordUI()) {
				int frames=(int)(maxtime*Graphics.frame_rate);
				i = 0; do { //;frames.times
					Graphics?.update();
					Input.update();
					textwindow.update();
					msgwindow.update();
					if (Input.trigger(PokemonUnity.Input.B)) {
						break;
					}
				} while(i < frames);
				string tmpFile="\\record.wav";//ENV["TEMP"]+
				endRecord(tmpFile); //ToDo: Stops recording and saves the recording to a file.
				wave=getWaveDataUI(tmpFile,true);
				if (wave != null) {
					(this as IGameMessage).MessageDisplay(msgwindow,Game._INTL("PLAYING BACK..."),false);
					textwindow.update();
					msgwindow.update();
					Graphics?.update();
					Input.update();
					wave.play();
					i = 0; do { //(Graphics.frame_rate*wave.time).to_i.times
						Graphics?.update();
						Input.update();
						textwindow.update();
						msgwindow.update();
					} while(i < (Graphics.frame_rate*wave.time));
				}
			}
			if (this is IGameAudio a3) a3.Audio_bgm_set_volume(oldvolume);
			(this as IGameMessage).DisposeMessageWindow(msgwindow);
			textwindow.Dispose();
			return wave;
		}

		public bool RxdataExists(string file) {
			if (false) { //$RPGVX
				return RgssExists(file+".rvdata");
			} else {
				return RgssExists(file+".rxdata") ;
			}
		}
		#endregion

		#region Gaining items
		public bool ItemBall(Items item,int quantity=1) {
			//if (item is String || item is Symbol) {
			//  item=getID(Items,item);
			//}
			if (item == null || item<=0 || quantity<1) return false;
			string itemname=(quantity>1) ? Kernal.ItemData[item].Plural : Game._INTL(item.ToString(TextScripts.Name));
			//int pocket=GetPocket(item);
			ItemPockets pocket=Kernal.ItemData[item].Pocket??ItemPockets.MISC;
			if (Bag.StoreItem(item,quantity)) {		// If item can be picked up
				if (Kernal.ItemData[item].Category==ItemCategory.ALL_MACHINES) { //[ITEMUSE]==3 || Kernal.ItemData[item][ITEMUSE]==4) { If item is TM=>3 or HM=>4
					(this as IGameMessage).Message(Game._INTL("\\se[ItemGet]{1} found \\c[1]{2}\\c[0]!\\nIt contained \\c[1]{3}\\c[0].\\wtnp[30]",
						Trainer.name,itemname,Game._INTL(Kernal.ItemData[item].Id.ToString(TextScripts.Name))));//ToDo:[ITEMMACHINE] param for Machine-to-Move Id
				} else if (item == Items.LEFTOVERS) {
					(this as IGameMessage).Message(Game._INTL("\\se[ItemGet]{1} found some \\c[1]{2}\\c[0]!\\wtnp[30]",Trainer.name,itemname));
				} else if (quantity>1) {
					(this as IGameMessage).Message(Game._INTL("\\se[ItemGet]{1} found {2} \\c[1]{3}\\c[0]!\\wtnp[30]",Trainer.name,quantity,itemname));
				} else {
					(this as IGameMessage).Message(Game._INTL("\\se[ItemGet]{1} found one \\c[1]{2}\\c[0]!\\wtnp[30]",Trainer.name,itemname));
				}
				(this as IGameMessage).Message(Game._INTL("{1} put the \\c[1]{2}\\c[0]\r\nin the <icon=bagPocket#{pocket}>\\c[1]{3}\\c[0] Pocket.",
					Trainer.name,itemname,pocket.ToString())); //PokemonBag.pocketNames()[pocket]
				return true;
			} else {   // Can't add the item
				if (Kernal.ItemData[item].Category==ItemCategory.ALL_MACHINES) { //[ITEMUSE]==3 || Kernal.ItemData[item][ITEMUSE]==4) {
					(this as IGameMessage).Message(Game._INTL("{1} found \\c[1]{2}\\c[0]!\\wtnp[20]",Trainer.name,itemname));
				} else if (item == Items.LEFTOVERS) {
					(this as IGameMessage).Message(Game._INTL("{1} found some \\c[1]{2}\\c[0]!\\wtnp[20]",Trainer.name,itemname));
				} else if (quantity>1) {
					(this as IGameMessage).Message(Game._INTL("{1} found {2} \\c[1]{3}\\c[0]!\\wtnp[20]",Trainer.name,quantity,itemname));
				} else {
					(this as IGameMessage).Message(Game._INTL("{1} found one \\c[1]{2}\\c[0]!\\wtnp[20]",Trainer.name,itemname));
				}
				(this as IGameMessage).Message(Game._INTL("Too bad... The Bag is full..."));
				return false;
			}
		}

		public bool ReceiveItem(Items item,int quantity=1) {
			//if (item is String || item is Symbol) {
			//  item=getID(Items,item);
			//}
			if (item == null || item<=0 || quantity<1) return false;
			string itemname=(quantity>1) ? Kernal.ItemData[item].Plural : Game._INTL(item.ToString(TextScripts.Name));
			//int pocket=GetPocket(item);
			ItemPockets pocket=Kernal.ItemData[item].Pocket??ItemPockets.MISC;
			if (Kernal.ItemData[item].Category==ItemCategory.ALL_MACHINES) { //[ITEMUSE]==3 || Kernal.ItemData[item][ITEMUSE]==4) {
				(this as IGameMessage).Message(Game._INTL("\\se[ItemGet]Obtained \\c[1]{1}\\c[0]!\\nIt contained \\c[1]{2}\\c[0].\\wtnp[30]",
					itemname,Game._INTL(Kernal.ItemData[item].Id.ToString(TextScripts.Name))));//ToDo:[ITEMMACHINE] param for Machine-to-Move Id
			} else if (item == Items.LEFTOVERS) {
				(this as IGameMessage).Message(Game._INTL("\\se[ItemGet]Obtained some \\c[1]{1}\\c[0]!\\wtnp[30]",itemname));
			} else if (quantity>1) {
				(this as IGameMessage).Message(Game._INTL("\\se[ItemGet]Obtained \\c[1]{1}\\c[0]!\\wtnp[30]",itemname));
			} else {
				(this as IGameMessage).Message(Game._INTL("\\se[ItemGet]Obtained \\c[1]{1}\\c[0]!\\wtnp[30]",itemname));
			}
			if (Bag.StoreItem(item,quantity)) {       // If item can be added
				(this as IGameMessage).Message(Game._INTL("{1} put the \\c[1]{2}\\c[0]\r\nin the <icon=bagPocket#{pocket}>\\c[1]{3}\\c[0] Pocket.",
					Trainer.name,itemname,pocket.ToString())); //PokemonBag.pocketNames()[pocket]
				return true;
			}
			return false;   // Can't add the item
		}

		public void UseKeyItem() {
			if (Bag.registeredItem== 0 && this is IGameMessage m) {
				m.Message(Game._INTL("A Key Item in the Bag can be registered to this key for instant use."));
			} else {
				UseKeyItemInField(Bag.registeredItem);
			}
		}
		#endregion

		#region Bridges
		public void BridgeOn(float height=2) {
			Global.bridge=height;
		}

		public void BridgeOff() {
			Global.bridge=0;
		}
		#endregion

		#region Event locations, terrain tags
		public bool EventFacesPlayer (IGameCharacter @event,IGamePlayer player,float distance) {
			if (distance<=0) return false;
			//  Event can't reach player if no coordinates coincide
			if (@event.x!=player.x && @event.y!=player.y) return false;
			float deltaX = (@event.direction == 6 ? 1 : @event.direction == 4 ? -1 : 0);
			float deltaY = (@event.direction == 2 ? 1 : @event.direction == 8 ? -1 : 0);
			//  Check for existence of player
			float curx=@event.x;
			float cury=@event.y;
			bool found=false;
			for (int i = 0; i < distance; i++) {
				curx+=deltaX;
				cury+=deltaY;
				if (player.x==curx && player.y==cury) {
					found=true;
					break;
				}
			}
			return found;
		}

		public bool EventCanReachPlayer (IGameCharacter @event,IGamePlayer player,float distance) {
			if (distance<=0) return false;
			//  Event can't reach player if no coordinates coincide
			if (@event.x!=player.x && @event.y!=player.y) return false;
			float deltaX = (@event.direction == 6 ? 1 : @event.direction == 4 ? -1 : 0);
			float deltaY =  (@event.direction == 2 ? 1 : @event.direction == 8 ? -1 : 0);
			//  Check for existence of player
			float curx=@event.x;
			float cury=@event.y;
			bool found=false;
			float realdist=0;
			for (int i = 0; i < distance; i++) {
				curx+=deltaX;
				cury+=deltaY;
				if (player.x==curx && player.y==cury) {
					found=true;
					break;
				}
				realdist+=1;
			}
			if (!found) return false;
			//  Check passibility
			curx=@event.x;
			cury=@event.y;
			for (int i = 0; i < realdist; i++) {
				if (!@event.passable(curx,cury,@event.direction)) {
					return false;
				}
				curx+=deltaX;
				cury+=deltaY;
			}
			return true;
		}

		public ITilePosition FacingTileRegular(float? direction=null,IGameCharacter @event=null) {
			if (@event == null) @event=GamePlayer;
			if (@event == null) return new TilePosition();
			float x=@event.x;
			float y=@event.y;
			if (direction == null) direction=@event.direction;
			switch (direction) {
				case 1:
					y+=1; x-=1;
					break;
				case 2:
					y+=1;
					break;
				case 3:
					y+=1; x+=1;
					break;
				case 4:
					x-=1;
					break;
				case 6:
					x+=1;
					break;
				case 7:
					y-=1; x-=1;
					break;
				case 8:
					y-=1;
					break;
				case 9:
					y-=1; x+=1;
					break;
			}
			return GameMap != null && GameMap is IGameMapOrgBattle gmo ? new TilePosition(gmo.map_id, x, y) : new TilePosition(0, x, y);
		}

		public ITilePosition FacingTile(float? direction=null,IGameCharacter @event=null) {
			if (MapFactory != null) {
				return MapFactory.getFacingTile(direction,@event);
			} else {
				return FacingTileRegular(direction,@event);
			}
		}

		public bool FacingEachOther(IGameCharacter event1,IGameCharacter event2) {
			if (event1 == null || event2 == null) return false; ITilePosition tile1, tile2;
			if (MapFactory != null) {
				tile1=MapFactory.getFacingTile(null,event1);
				tile2=MapFactory.getFacingTile(null,event2);
				if (tile1 == null || tile2 == null) return false;
				if (event2.map is IGameMapOrgBattle gmo2 &&
					tile1.MapId==gmo2.map_id &&
					tile1.X==event2.x && tile1.Y==event2.y &&
					event1.map is IGameMapOrgBattle gmo1 &&
					tile2.MapId==gmo1.map_id &&
					tile2.X==event1.x && tile2.Y==event1.y) {
					return true;
				} else {
					return false;
				}
			} else {
				tile1=FacingTile(null,event1);
				tile2=FacingTile(null,event2);
				if (tile1 == null || tile2 == null) return false;
				if (tile1.X==event2.x && tile1.Y==event2.y &&
					tile2.X==event1.x && tile2.Y==event1.y) {
					return true;
				} else {
					return false;
				}
			}
		}

		public Terrains GetTerrainTag(IGameCharacter @event=null,bool countBridge=false) {
			if (@event == null) @event=GamePlayer;
			if (@event == null) return 0;
			if (MapFactory != null && @event.map is IGameMapOrgBattle gmo) {
				return MapFactory.getTerrainTag(gmo.map_id,@event.x,@event.y,countBridge).Value;
			} else {
				return GameMap.terrain_tag(@event.x,@event.y,countBridge);
			}
		}

		public Terrains? FacingTerrainTag(IGameCharacter @event=null,float? dir=null) {
			if (MapFactory != null) {
				return MapFactory.getFacingTerrainTag(dir,@event);
			} else {
				if (@event == null) @event=GamePlayer;
				if (@event == null) return 0;
				ITilePosition facing=FacingTile(dir,@event);
				return GameMap.terrain_tag(facing.X,facing.Y); //(facing[1],facing[2]);
			}
		}
		#endregion

		#region Event movement
		public virtual void TurnTowardEvent(IGameCharacter @event,IGameCharacter otherEvent) {
			float sx=0;
			float sy=0;
			if (MapFactory != null) {
				IPoint relativePos=MapFactory.getThisAndOtherEventRelativePos(otherEvent,@event);
				sx = relativePos.x;
				sy = relativePos.y;
			} else {
				sx = @event.x - otherEvent.x;
				sy = @event.y - otherEvent.y;
			}
			if (sx == 0 && sy == 0) {
				return;
			}
			if (Math.Abs(sx) > Math.Abs(sy)) {
				//sx > 0 ? @event.turn_left : @event.turn_right;
				if(sx > 0) @event.turn_left(); else @event.turn_right();
			} else {
				//sy > 0 ? @event.turn_up : @event.turn_down;
				if(sy > 0) @event.turn_up(); else @event.turn_down();
			}
		}

		public virtual void MoveTowardPlayer(IGameCharacter @event) {
			int maxsize=Math.Max(GameMap.width,GameMap.height);
			if (!EventCanReachPlayer(@event,GamePlayer,maxsize)) return;
			do { //;loop
				float x=@event.x;
				float y=@event.y;
				@event.move_toward_player();
				if (@event.x==x && @event.y==y) break;
				while (@event.moving) {
					Graphics?.update();
					Input.update();
					if (this is IGameMessage m) m.UpdateSceneMap();
				}
			} while (true);
			if (PokemonMap != null) PokemonMap.addMovedEvent(@event.id);
		}

		public virtual bool JumpToward(int dist=1,bool playSound=false,bool cancelSurf=false) {
			float x=GamePlayer.x;
			float y=GamePlayer.y;
			switch (GamePlayer.direction) {
				case 2: // down
					GamePlayer.jump(0,dist);
					break;
				case 4: // left
					GamePlayer.jump(-dist,0);
					break;
				case 6: // right
					GamePlayer.jump(dist,0);
					break;
				case 8: // up
					GamePlayer.jump(0,-dist);
					break;
			}
			if (GamePlayer.x!=x || GamePlayer.y!=y) {
				if (playSound && this is IGameAudioPlay a) a.SEPlay("jump");
				if (cancelSurf) CancelVehicles();
				while (GamePlayer.jumping) {
					Graphics?.update();
					Input.update();
					if (this is IGameMessage m) m.UpdateSceneMap();
				}
				return true;
			}
			return false;
		}

		//ToDo: One solid coroutine function... change to IEnumerator, and add yield return frame
		public virtual void Wait(int numframes) {
			if (Core.INTERNAL) return; //if there's no ui connected...
			int i = 0; do {
				Graphics?.update();
				Input.update();
				if (this is IGameMessage m) m.UpdateSceneMap(); i++;
			} while (i < numframes); //;numframes.times
		}

		public virtual IMoveRoute MoveRoute(IGameCharacter @event,int[] commands,bool waitComplete=false) {
			//IMoveRoute route=new RPG.MoveRoute();
			//route.repeat=false;
			//route.skippable=true;
			//route.list.Clear();
			//route.list.Add(new RPG.MoveCommand(MoveRoutes.ThroughOn));
			//int i=0; while (i<commands.Length) {
			//	switch (commands[i]) {
			//		case MoveRoutes.Wait: case MoveRoutes.SwitchOn: case MoveRoutes.SwitchOff: case
			//			MoveRoutes.ChangeSpeed: case MoveRoutes.ChangeFreq: case MoveRoutes.Opacity: case
			//			MoveRoutes.Blending: case MoveRoutes.PlaySE: case MoveRoutes.Script:
			//			route.list.Add(new RPG.MoveCommand(commands[i],new int[] { commands[i + 1] }));
			//			i+=1;
			//			break;
			//		case MoveRoutes.ScriptAsync:
			//			route.list.Add(new RPG.MoveCommand(MoveRoutes.Script,new int[] { commands[i + 1] }));
			//			route.list.Add(new RPG.MoveCommand(MoveRoutes.Wait,new int[] { 0 }));
			//			i+=1;
			//			break;
			//		case MoveRoutes.Jump:
			//			route.list.Add(new RPG.MoveCommand(commands[i],new int[] { commands[i + 1], commands[i + 2] }));
			//			i+=2;
			//			break;
			//		case MoveRoutes.Graphic:
			//			route.list.Add(new RPG.MoveCommand(commands[i],
			//				new int[] { commands[i + 1], commands[i + 2], commands[i + 3], commands[i + 4] }));
			//			i+=4;
			//			break;
			//		default:
			//			route.list.Add(new RPG.MoveCommand(commands[i]));
			//			break;
			//	}
			//	i+=1;
			//}
			//route.list.Add(new RPG.MoveCommand(MoveRoutes.ThroughOff));
			//route.list.Add(new RPG.MoveCommand(0));
			//if (@event != null) {
			//	@event.force_move_route(route);
			//}
			//return route;
			return null;
		}
		#endregion

		#region Screen effects
		public void ToneChangeAll(ITone tone,float duration) {
			GameScreen.start_tone_change(tone,duration * 2);
			foreach (var picture in GameScreen.pictures) {
				if (picture != null) picture.start_tone_change(tone,duration * 2);
			}
		}

		public void Shake(int power,int speed,int frames) {
			GameScreen.start_shake(power,speed,frames * 2);
		}

		public void Flash(IColor color,int frames) {
			GameScreen.start_flash(color,frames * 2);
		}

		public virtual void ScrollMap(int direction, int distance, float speed) {
			if (GameMap == null) return;
			if (speed==0) {
				switch (direction) {
					case 2:
						GameMap.scroll_down(distance * 128);
						break;
					case 4:
						GameMap.scroll_left(distance * 128);
						break;
					case 6:
						GameMap.scroll_right(distance * 128);
						break;
					case 8:
						GameMap.scroll_up(distance * 128);
						break;
				}
			} else {
				GameMap.start_scroll(direction, distance, speed);
				float oldx=GameMap.display_x;
				float oldy=GameMap.display_y;
				do { //;loop
					Graphics?.update();
					Input.update();
					if (!GameMap.scrolling) {
						break;
					}
					if (this is IGameMessage m) m.UpdateSceneMap();
					if (GameMap.display_x==oldx && GameMap.display_y==oldy) {
						break;
					}
					oldx=GameMap.display_x;
					oldy=GameMap.display_y;
				} while (true);
			}
		}
		#endregion
	}

	// ===============================================================================
	// Events
	// ===============================================================================
	//public partial class GameEvent {
	//	public bool cooledDown (int seconds) {
	//		if (!(expired(seconds) && tsOff("A"))) {
	//			this.need_refresh=true;
	//			return false;
	//		} else {
	//			return true;
	//		}
	//	}
	//
	//	public bool cooledDownDays (int days) {
	//		if (!(expiredDays(days) && tsOff("A"))) {
	//			this.need_refresh=true;
	//			return false;
	//		} else {
	//			return true;
	//		}
	//	}
	//}

	//public partial class InterpreterFieldMixin : IInterpreterFieldMixin
	//{
	//	private int @event_id;
	//	private int @map_id;
	//	private IGameCharacter @event;
	//	/// <summary>
	//	/// Used in boulder events. Allows an event to be pushed.
	//	/// </summary>
	//	/// <remarks>
	//	/// To be used in a script event command.
	//	/// </remarks>
	//	public void PushThisEvent() {
	//		@event=Game.GameData.Interpreter.get_character(0);
	//		float oldx=@event.x;
	//		float oldy=@event.y;
	//		//  Apply strict version of passable, which makes impassable
	//		//  tiles that are passable only from certain directions
	//		if (!@event.passableStrict(@event.x,@event.y,Game.GameData.GamePlayer.direction)) {
	//			return;
	//		}
	//		switch (Game.GameData.GamePlayer.direction) {
	//			case 2: // down
	//				@event.move_down();
	//				break;
	//			case 4: // left
	//				@event.move_left();
	//				break;
	//			case 6: // right
	//				@event.move_right();
	//				break;
	//			case 8: // up
	//				@event.move_up();
	//				break;
	//		}
	//		if (Game.GameData.PokemonMap != null) Game.GameData.PokemonMap.addMovedEvent(@event_id);
	//		if (oldx!=@event.x || oldy!=@event.y) {
	//			Game.GameData.GamePlayer._lock();
	//			do {
	//				Game.GameData.Graphics?.update();
	//				Input.update();
	//				if (Game.GameData is IGameMessage m) m.UpdateSceneMap();
	//			} while (@event.moving);
	//			Game.GameData.GamePlayer.unlock();
	//		}
	//	}
	//
	//	public bool PushThisBoulder() {
	//		if (Game.GameData.PokemonMap.strengthUsed) {
	//			PushThisEvent();
	//		}
	//		return true;
	//	}
	//
	//	public bool Headbutt() {
	//		if (Game.GameData is IGameHiddenMoves f) f.Headbutt(Game.GameData.Interpreter.get_character(0));
	//		return true;
	//	}
	//
	//	public bool TrainerIntro(TrainerTypes symbol) {
	//		//if (Core.DEBUG) {
	//		//  if (!Game.TrainerTypeCheck(symbol)) return false;
	//		//}
	//		TrainerTypes trtype=symbol; //Trainers.const_get(symbol);
	//		if (this is IInterpreterMixinMessage m) m.GlobalLock();
	//		if (Game.GameData is IGameUtility a) a.PlayTrainerIntroME(trtype);
	//		return true;
	//	}
	//
	//	public void TrainerEnd() {
	//		if (this is IInterpreterMixinMessage m) m.GlobalUnlock();
	//		IGameEvent e=(IGameEvent)Game.GameData.Interpreter.get_character(0);
	//		if (e != null) e.erase_route();
	//	}
	//
	//	//public object[] Params { get {
	//	//	return @parameters != null ? @parameters : @params;
	//	//} }
	//
	//	public IPokemon GetPokemon(int id) {
	//		return Game.GameData.Trainer.party[Game.GameData is IGameUtility g ? (int)g.Get(id) : id];
	//	}
	//
	//	public void SetEventTime(object arg) { //params int[]
	//		if (Game.GameData.Global.eventvars == null) Game.GameData.Global.eventvars=new Dictionary<KeyValuePair<int, int>, long>();
	//		long time=Game.GetTimeNow.Ticks;
	//		//time=time.to_i;
	//		if (Game.GameData is IInterpreterMixinMessage i) i.SetSelfSwitch(@event_id,"A",true);
	//		Game.GameData.Global.eventvars[new KeyValuePair<int, int>(@map_id,@event_id)]=time;
	//		foreach (int otherevt in (int[])arg) {
	//			if (Game.GameData is IInterpreterMixinMessage i0) i0.SetSelfSwitch(otherevt,"A",true);
	//			Game.GameData.Global.eventvars[new KeyValuePair<int, int>(@map_id,otherevt)]=time;
	//		}
	//	}
	//
	//	public object getVariable(object arg) { //params int[]
	//		//if (arg.Length==0) {
	//			if (Game.GameData.Global.eventvars == null) return null;
	//			return Game.GameData.Global.eventvars[new KeyValuePair<int, int>(@map_id,@event_id)];
	//		//} else {
	//		//	return Game.GameData.GameVariables[arg[0]];
	//		//}
	//	}
	//
	//	public void setVariable(object arg) { //params int[]
	//		//if (arg.Length==1) {
	//			if (Game.GameData.Global.eventvars == null) Game.GameData.Global.eventvars=new Dictionary<KeyValuePair<int, int>, long>();
	//			Game.GameData.Global.eventvars[new KeyValuePair<int, int>(@map_id,@event_id)]=((int[])arg)[0]; //arg[0]
	//		//} else {
	//		//	Game.GameData.GameVariables[arg[0]]=arg[1];
	//		//	Game.GameData.GameMap.need_refresh=true;
	//		//}
	//	}
	//
	//	public bool tsOff (string c) {
	//		//return Game.GameData.Interpreter.get_character(0).tsOff(c);
	//		if (Game.GameData.Interpreter.get_character(0) is IGameEvent ge) return ge.tsOff(c);
	//		return true;
	//	}
	//
	//	public bool tsOn (string c) {
	//		//return Game.GameData.Interpreter.get_character(0).tsOn(c);
	//		if (Game.GameData.Interpreter.get_character(0) is IGameEvent ge) return ge.tsOn(c);
	//		return false;
	//	}
	//
	//	//alias isTempSwitchOn? tsOn?;
	//	//alias isTempSwitchOff? tsOff?;
	//
	//	public void setTempSwitchOn(string c) {
	//		//Game.GameData.Interpreter.get_character(0).setTempSwitchOn(c);
	//		if (Game.GameData.Interpreter.get_character(0) is IGameEvent ge) ge.setTempSwitchOn(c);
	//	}
	//
	//	public void setTempSwitchOff(string c) {
	//		//Game.GameData.Interpreter.get_character(0).setTempSwitchOff(c);
	//		if (Game.GameData.Interpreter.get_character(0) is IGameEvent ge) ge.setTempSwitchOff(c);
	//	}
	//
	//	// Must use this approach to share the methods because the methods already
	//	// defined in a class override those defined in an included module
	//	//CustomEventCommands=<<_END_;
	//
	//	public bool command_352() {
	//		bool ret = false;
	//		ISaveScene scene = Game.GameData.Scenes.Save; //new PokemonSaveScene();
	//		ISaveScreen screen = Game.GameData.Screens.Save.initialize(scene); //new PokemonSave(scene);
	//		ret = screen.SaveScreen();
	//		return ret; //hardcode true?
	//	}
	//
	//	public bool command_125() {
	//		//int value = operate_value(Params[0], Params[1], Params[2]);
	//		//Game.GameData.Trainer.Money+=value;
	//		return true;
	//	}
	//
	//	public bool command_132() {
	//		//Game.GameData.Global.nextBattleBGM=(Params[0]) ? Params[0].clone : null;
	//		return true;
	//	}
	//
	//	public bool command_133() {
	//		//Game.GameData.Global.nextBattleME=(Params[0]) ? Params[0].clone : null;
	//		return true;
	//	}
	//
	//	public void command_353() {
	//		//BGMFade(1.0);
	//		//BGSFade(1.0);
	//		//FadeOutIn(99999, () => { Game.StartOver(true); });
	//	}
	//
	//	public bool command_314() {
	//		//if (Params[0] == 0 && Game.GameData.Trainer!=null && Game.GameData.Trainer.party) {
	//		//	HealAll();
	//		//}
	//		return true;
	//	}
	//	//_END_;*/
	//}

	//public partial class Interpreter : InterpreterFieldMixin {
	//  //include InterpreterFieldMixin;
	//  //eval(InterpreterFieldMixin.CustomEventCommands);
	//}
	//
	//public partial class Game_Interpreter : InterpreterFieldMixin {
	//  //include InterpreterFieldMixin;
	//  //eval(InterpreterFieldMixin.CustomEventCommands);
	//}
}