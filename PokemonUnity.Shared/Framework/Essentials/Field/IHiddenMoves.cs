﻿using System;
using System.Collections;
using System.Collections.Generic;
using PokemonUnity;
using PokemonUnity.Inventory;
using PokemonUnity.Overworld;
using PokemonEssentials.Interface.PokeBattle;
using PokemonEssentials.Interface.EventArg;
using PokemonUnity.Utility;

namespace PokemonEssentials.Interface
{
	namespace EventArg
	{
		#region Hidden Moves EventArgs
		/// <summary>
		/// Parameters:
		/// e[0] = Move being created
		/// e[1] = Pokemon using the move
		/// </summary>
		public interface IHiddenMoveEventArgs : IEventArgs
		{
			//readonly int EventId = typeof(HiddenMoveEventArgs).GetHashCode();

			//int Id { get; }
			/// <summary>
			/// Move being created
			/// </summary>
			Moves Move { get; set; }
			/// <summary>
			/// Pokemon using the move
			/// </summary>
			IPokemon Pokemon { get; set; }
		}
		#endregion
	}

	namespace Field
	{
		#region Interpolators
		public interface IRectInterpolator : IRect
		{
			IRectInterpolator initialize(IRect oldrect, IRect newrect, int frames);

			void restart(IRect oldrect, IRect newrect, int frames);

			void set(IRect rect);

			bool done();

			void update();
		}

		public interface IPointInterpolator : IPoint
		{
			IPointInterpolator initialize(float oldx, float oldy, float newx, float newy, int frames);

			void restart(float oldx, float oldy, float newx, float newy, int frames);

			float x { get; }
			float y { get; }

			bool done();

			void update();
		}
		#endregion

		#region Hidden move handlers
		//public interface IMoveHandlerHash : IHandlerHash {
		//    void initialize();
		//}

		public interface IHiddenMoveHandlers
		{
			IDictionary<Moves, Func<Moves, IPokemon, bool>> CanUseMove { get; }
			IDictionary<Moves, Func<Moves, IPokemon, bool>> UseMove { get; }

			//event EventHandler<IHiddenMoveEventArgs> OnCanUseMove;
			event Action<object,IHiddenMoveEventArgs> OnCanUseMove;
			//event EventHandler<IHiddenMoveEventArgs> OnUseMove;
			event Action<object,IHiddenMoveEventArgs> OnUseMove;

			void addCanUseMove(Moves item, Func<Moves, IPokemon, bool> proc);

			void addUseMove(Moves item, Func<Moves, IPokemon, bool> proc);

			bool hasHandler(Moves item);

			bool triggerCanUseMove(Moves item, IPokemon pokemon);
			bool triggerUseMove(Moves item, IPokemon pokemon);
		}

		/// <summary>
		/// Extension of <see cref="IGame"/>
		/// </summary>
		public interface IGameHiddenMoves : IGame
		{
			/// <summary>
			/// Triggers when the player presses the Action button on the map.
			/// </summary>
			event EventHandler OnAction;

			bool CanUseHiddenMove(IPokemon pkmn, Moves move);

			bool UseHiddenMove(IPokemon pokemon, Moves move);

			void HiddenMoveEvent();

			#region Hidden move animation
			void HiddenMoveAnimation(IPokemon pokemon);
			#endregion

			#region Cut
			bool Cut();

			//HiddenMoveHandlers.CanUseMove.add(:CUT,proc{|move,pkmn|

			//HiddenMoveHandlers.UseMove.add(:CUT, proc{| move,pokemon |
			#endregion

			#region Headbutt
			void HeadbuttEffect(IGameCharacter @event);

			void Headbutt(IGameCharacter @event);

			//HiddenMoveHandlers.CanUseMove.add(:HEADBUTT, proc{| move,pkmn |

			//HiddenMoveHandlers.UseMove.add(:HEADBUTT, proc{| move,pokemon |
			#endregion

			#region Rock Smash
			void RockSmashRandomEncounter();

			bool RockSmash();

			//HiddenMoveHandlers.CanUseMove.add(:ROCKSMASH, proc{| move,pkmn |

			//HiddenMoveHandlers.UseMove.add(:ROCKSMASH, proc{| move,pokemon |
			#endregion

			#region Strength
			bool Strength();

			//Events.onAction += proc{| sender,e |

			//HiddenMoveHandlers.CanUseMove.add(:STRENGTH, proc{| move,pkmn |

			//HiddenMoveHandlers.UseMove.add(:STRENGTH, proc{| move,pokemon |
			#endregion

			#region Surf
			bool Surf();

			void StartSurfing();

			bool EndSurf(float xOffset, float yOffset);

			void TransferSurfing(int mapid, float xcoord, float ycoord, float direction); //= Game.GameData.GamePlayer.direction

			//Events.onAction += proc{| sender,e |

			//HiddenMoveHandlers.CanUseMove.add(:SURF, proc{| move,pkmn |

			//HiddenMoveHandlers.UseMove.add(:SURF, proc{| move,pokemon |
			#endregion

			#region Waterfall
			void AscendWaterfall(IGameCharacter @event = null);

			void DescendWaterfall(IGameCharacter @event = null);

			bool Waterfall();

			//Events.onAction += proc{| sender,e |

			//HiddenMoveHandlers.CanUseMove.add(:WATERFALL, proc{| move,pkmn |

			//HiddenMoveHandlers.UseMove.add(:WATERFALL, proc{| move,pokemon |
			#endregion

			#region Dive
			bool Dive();

			bool Surfacing();

			void TransferUnderwater(int mapid, float xcoord, float ycoord, float direction); //= Game.GameData.GamePlayer.direction

			//Events.onAction += proc{| sender,e |

			//HiddenMoveHandlers.CanUseMove.add(:DIVE, proc{| move,pkmn |

			//HiddenMoveHandlers.UseMove.add(:DIVE, proc{| move,pokemon |
			#endregion

			#region Fly
			//HiddenMoveHandlers.CanUseMove.add(:FLY, proc{| move,pkmn |

			//HiddenMoveHandlers.UseMove.add(:FLY, proc{| move,pokemon |
			#endregion

			#region Flash
			//HiddenMoveHandlers.CanUseMove.add(:FLASH, proc{| move,pkmn |

			//HiddenMoveHandlers.UseMove.add(:FLASH, proc{| move,pokemon |
			#endregion

			#region Teleport
			//HiddenMoveHandlers.CanUseMove.add(:TELEPORT, proc{| move,pkmn |

			//HiddenMoveHandlers.UseMove.add(:TELEPORT, proc{| move,pokemon |
			#endregion

			#region Dig
			//HiddenMoveHandlers.CanUseMove.add(:DIG, proc{| move,pkmn |

			//HiddenMoveHandlers.UseMove.add(:DIG, proc{| move,pokemon |
			#endregion

			#region Sweet Scent
			void SweetScent();

			//HiddenMoveHandlers.CanUseMove.add(:SWEETSCENT,proc{|move,pkmn|

			//HiddenMoveHandlers.UseMove.add(:SWEETSCENT, proc{| move,pokemon |
			#endregion
		}
		#endregion
	}
}