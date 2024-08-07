﻿using System;
using System.Collections;
using System.Collections.Generic;
using PokemonUnity;
using PokemonUnity.Interface;
using PokemonEssentials.Interface.RPGMaker.Kernal;

namespace PokemonEssentials.Interface
{
	public interface IGameEvent : IGameCharacter
	{
		int trigger { get; set; }
		IList<IEventCommand> list { get; set; }
		IList<IEventPage> pages { get; set; }
		bool starting { get; set; }
		/// <summary>
		/// Temporary self-switches
		/// </summary>
		IDictionary<string, bool?> tempSwitches { get; set; }
		bool need_refresh { get; set; }

		//IGameEvent(int map_id, IEntity ev, Game_Map map = null);
		IGameEvent initialize(int map_id, IGameCharacter ev, IGameMap map = null);

		int map_id { get; }

		void clear_starting();

		bool over_trigger { get; }

		void start();

		void erase();

		void erase_route();

		string name { get; }

		//int id { get; }

		void CheckEventTriggerAfterTurning();

		bool tsOn(string c);

		bool tsOff(string c);

		void setTempSwitchOn(string c);

		void setTempSwitchOff(string c);

		object variable { get; }

		void setVariable(int variable);

		int varAsInt();

		bool expired(int secs = 86400);

		bool expiredDays(int days = 1);

		bool onEvent();

		bool isOff(string c);

		bool switchIsOn(int id);

		void refresh();

		void check_event_trigger_touch(float x, float y);

		void check_event_trigger_auto();

		//IEnumerator update();
	}
}