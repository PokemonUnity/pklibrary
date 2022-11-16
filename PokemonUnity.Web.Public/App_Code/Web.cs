﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokemonUnity.Web.Public
{
	public class Page
	{
		public string Title;
		public string Url;

		public static class Titles
		{
			public const string Abilities							=	"Abilities";
			public const string About								=	"About";
			public const string Adding_new_encounter_methods		=	"Adding new encounter methods";
			public const string All_Animations_Project				=	"All Animations Project";
			public const string Attack_animations					=	"Attack animations";
			public const string Audio								=	"Audio";
			public const string Backgrounds_and_music				=	"Backgrounds and music";
			public const string Bag									=	"Bag";
			public const string Battle								=	"Battle";
			public const string Battle_AI							=	"Battle AI";
			public const string Battle_Frontier						=	"Battle Frontier";
			public const string Battle_Tower						=	"Battle Tower";
			public const string Battles								=	"Battles";
			public const string Berry_planting						=	"Berry planting";
			public const string Breeding							=	"Breeding";
			public const string Bridges								=	"Bridges";
			public const string Bug_Catching_Contest				=	"Bug Catching Contest";
			public const string Change_log							=	"Change log";
			public const string Choosing_a_starter					=	"Choosing a starter";
			public const string Compiler							=	"Compiler";
			public const string Connecting_maps						=	"Connecting maps";
			public const string Controlling_events_with_scripts		=	"Controlling events with scripts";
			public const string Controls							=	"Controls";
			public const string Credits								=	"Credits";
			public const string Day_Care							=	"Day Care";
			public const string Debug_mode							=	"Debug mode";
			public const string Defining_a_move						=	"Defining a move";
			public const string Defining_a_species					=	"Defining a species";
			public const string Defining_a_trainer					=	"Defining a trainer";
			public const string Defining_a_type						=	"Defining a type";
			public const string Defining_an_ability					=	"Defining an ability";
			public const string Defining_an_item					=	"Defining an item";
			public const string Dungeons							=	"Dungeons";
			public const string Editing_a_Pokemon					=	"Editing a Pokémon";
			public const string Eggs								=	"Eggs";
			public const string Encounters							=	"Encounters";
			public const string Error_messages						=	"Error messages";
			public const string Essentials_Docs_Wiki				=	"Essentials Docs Wiki";
			public const string Event_encounter						=	"Event encounter";
			public const string Event_encounters					=	"Event encounters";
			public const string Events								=	"Events";
			public const string Evolution							=	"Evolution";
			public const string Example_maps						=	"Example maps";
			public const string Fishing_encounters					=	"Fishing encounters";
			public const string Forms								=	"Forms";
			public const string Function_codes						=	"Function codes";
			public const string Game_Corner							=	"Game Corner";
			public const string Game_intro							=	"Game intro";
			public const string Getting_around						=	"Getting around";
			public const string Guides								=	"Guides";
			public const string How_you_can_help					=	"How you can help";
			public const string Item_balls							=	"Item balls";
			public const string Item_effects						=	"Item effects";
			public const string Itemfinder							=	"Itemfinder";
			public const string Items								=	"Items";
			public const string Jukebox								=	"Jukebox";
			public const string Learning_moves						=	"Learning moves";
			public const string List_of_abilities					=	"List of abilities";
			public const string Lottery								=	"Lottery";
			public const string Main_Page							=	"Main Page";
			public const string Manipulating_Pokemon				=	"Manipulating Pokémon";
			public const string Manipulating_items					=	"Manipulating items";
			public const string Map_transfers						=	"Map transfers";
			public const string Maps								=	"Maps";
			public const string Mega_Evolution						=	"Mega Evolution";
			public const string Messages							=	"Messages";
			public const string Metadata							=	"Metadata";
			public const string Mini_game							=	"Mini-game";
			public const string Mini_games							=	"Mini-games";
			public const string Mining_mini_game					=	"Mining mini-game";
			public const string Move_effects						=	"Move effects";
			public const string Moves								=	"Moves";
			public const string Multiple_forms						=	"Multiple forms";
			public const string Multiple_regions					=	"Multiple regions";
			public const string Mystery_Gift						=	"Mystery Gift";
			public const string Obstacles							=	"Obstacles";
			public const string Options_screen						=	"Options screen";
			public const string PBS_file							=	"PBS file";
			public const string PC									=	"PC";
			public const string Partner_trainer						=	"Partner trainer";
			public const string Party								=	"Party";
			public const string Pause_menu							=	"Pause menu";
			public const string Phone								=	"Phone";
			public const string Player								=	"Player";
			public const string Poke_Center							=	"Poké Center";
			public const string Poke_Mart							=	"Poké Mart";
			public const string Poke_Radar							=	"Poké Radar";
			public const string Pokedex								=	"Pokédex";
			public const string Pokegear							=	"Pokégear";
			public const string Pokemon								=	"Pokémon";
			public const string Pokemon_Gym							=	"Pokémon Gym";
			public const string Pokemon_storage						=	"Pokémon storage";
			public const string Region_map							=	"Region map";
			public const string Rematches							=	"Rematches";
			public const string Rival								=	"Rival";
			public const string Roaming_Pokemon						=	"Roaming Pokémon";
			public const string Safari_Zone							=	"Safari Zone";
			public const string Saving_and_loading					=	"Saving and loading";
			public const string Script_section						=	"Script section";
			public const string Script_sections						=	"Script sections";
			public const string Setting								=	"Setting";
			public const string Shadow_Pokemon						=	"Shadow Pokémon";
			public const string Shiny_Pokemon						=	"Shiny Pokémon";
			public const string Side_stairs							=	"Side stairs";
			public const string Slot_Machine						=	"Slot Machine";
			public const string Slot_Machines						=	"Slot Machines";
			public const string Special_NPCs						=	"Special NPCs";
			public const string Summary_screens						=	"Summary screens";
			public const string Tilesets							=	"Tilesets";
			public const string Time								=	"Time";
			public const string Time_sensitive_events				=	"Time-sensitive events";
			public const string Trading_Pokemon						=	"Trading Pokémon";
			public const string Trainer_card						=	"Trainer card";
			public const string Trainers							=	"Trainers";
			public const string Translating_the_game				=	"Translating the game";
			public const string Triple_Triad						=	"Triple Triad";
			public const string Using_moves_outside_battle			=	"Using moves outside battle";
			public const string Voltorb_Flip						=	"Voltorb Flip";
			public const string Weather								=	"Weather";
			public const string Wild_encounters						=	"Wild encounters";
		}
		public static class Urls
		{
			public const string Abilities							=	"Abilities";
			public const string About								=	"About";
			public const string Adding_new_encounter_methods		=	"Adding_new_encounter_methods";
			public const string All_Animations_Project				=	"All_Animations_Project";
			public const string Attack_animations					=	"Attack_animations";
			public const string Audio								=	"Audio";
			public const string Backgrounds_and_music				=	"Backgrounds_and_music";
			public const string Bag									=	"Bag";
			public const string Battle								=	"Battle";
			public const string Battle_AI							=	"Battle_AI";
			public const string Battle_Frontier						=	"Battle_Frontier";
			public const string Battle_Tower						=	"Battle_Tower";
			public const string Battles								=	"Battles";
			public const string Berry_planting						=	"Berry_planting";
			public const string Breeding							=	"Breeding";
			public const string Bridges								=	"Bridges";
			public const string Bug_Catching_Contest				=	"Bug_Catching_Contest";
			public const string Change_log							=	"Change_log";
			public const string Choosing_a_starter					=	"Choosing_a_starter";
			public const string Compiler							=	"Compiler";
			public const string Connecting_maps						=	"Connecting_maps";
			public const string Controlling_events_with_scripts		=	"Controlling_events_with_scripts";
			public const string Controls							=	"Controls";
			public const string Credits								=	"Credits";
			public const string Day_Care							=	"Day_Care";
			public const string Debug_mode							=	"Debug_mode";
			public const string Defining_a_move						=	"Defining_a_move";
			public const string Defining_a_species					=	"Defining_a_species";
			public const string Defining_a_trainer					=	"Defining_a_trainer";
			public const string Defining_a_type						=	"Defining_a_type";
			public const string Defining_an_ability					=	"Defining_an_ability";
			public const string Defining_an_item					=	"Defining_an_item";
			public const string Dungeons							=	"Dungeons";
			public const string Editing_a_Pokemon					=	"Editing_a_Pokemon";
			public const string Eggs								=	"Eggs";
			public const string Encounters							=	"Encounters";
			public const string Error_messages						=	"Error_messages";
			public const string Essentials_Docs_Wiki				=	"Essentials_Docs_Wiki";
			public const string Event_encounter						=	"Event_encounter";
			public const string Event_encounters					=	"Event_encounters";
			public const string Events								=	"Events";
			public const string Evolution							=	"Evolution";
			public const string Example_maps						=	"Example_maps";
			public const string Fishing_encounters					=	"Fishing_encounters";
			public const string Forms								=	"Forms";
			public const string Function_codes						=	"Function_codes";
			public const string Game_Corner							=	"Game_Corner";
			public const string Game_intro							=	"Game_intro";
			public const string Getting_around						=	"Getting_around";
			public const string Guides								=	"Guides";
			public const string How_you_can_help					=	"How_you_can_help";
			public const string Item_balls							=	"Item_balls";
			public const string Item_effects						=	"Item_effects";
			public const string Itemfinder							=	"Itemfinder";
			public const string Items								=	"Items";
			public const string Jukebox								=	"Jukebox";
			public const string Learning_moves						=	"Learning_moves";
			public const string List_of_abilities					=	"List_of_abilities";
			public const string Lottery								=	"Lottery";
			public const string Main_Page							=	"Main_Page";
			public const string Manipulating_Pokemon				=	"Manipulating_Pokemon";
			public const string Manipulating_items					=	"Manipulating_items";
			public const string Map_transfers						=	"Map_transfers";
			public const string Maps								=	"Maps";
			public const string Mega_Evolution						=	"Mega_Evolution";
			public const string Messages							=	"Messages";
			public const string Metadata							=	"Metadata";
			public const string Mini_game							=	"Mini-game";
			public const string Mini_games							=	"Mini-games";
			public const string Mining_mini_game					=	"Mining_mini-game";
			public const string Move_effects						=	"Move_effects";
			public const string Moves								=	"Moves";
			public const string Multiple_forms						=	"Multiple_forms";
			public const string Multiple_regions					=	"Multiple_regions";
			public const string Mystery_Gift						=	"Mystery_Gift";
			public const string Obstacles							=	"Obstacles";
			public const string Options_screen						=	"Options_screen";
			public const string PBS_file							=	"PBS_file";
			public const string PC									=	"PC";
			public const string Partner_trainer						=	"Partner_trainer";
			public const string Party								=	"Party";
			public const string Pause_menu							=	"Pause_menu";
			public const string Phone								=	"Phone";
			public const string Player								=	"Player";
			public const string Poke_Center							=	"Poke_Center";
			public const string Poke_Mart							=	"Poke_Mart";
			public const string Poke_Radar							=	"Poke_Radar";
			public const string Pokedex								=	"Pokedex";
			public const string Pokegear							=	"Pokegear";
			public const string Pokemon								=	"Pokemon";
			public const string Pokemon_Gym							=	"Pokemon_Gym";
			public const string Pokemon_storage						=	"Pokemon_storage";
			public const string Region_map							=	"Region_map";
			public const string Rematches							=	"Rematches";
			public const string Rival								=	"Rival";
			public const string Roaming_Pokemon						=	"Roaming_Pokemon";
			public const string Safari_Zone							=	"Safari_Zone";
			public const string Saving_and_loading					=	"Saving_and_loading";
			public const string Script_section						=	"Script_section";
			public const string Script_sections						=	"Script_sections";
			public const string Setting								=	"Setting";
			public const string Shadow_Pokemon						=	"Shadow_Pokemon";
			public const string Shiny_Pokemon						=	"Shiny_Pokemon";
			public const string Side_stairs							=	"Side_stairs";
			public const string Slot_Machine						=	"Slot_Machine";
			public const string Slot_Machines						=	"Slot_Machines";
			public const string Special_NPCs						=	"Special_NPCs";
			public const string Summary_screens						=	"Summary_screens";
			public const string Tilesets							=	"Tilesets";
			public const string Time								=	"Time";
			public const string Time_sensitive_events				=	"Time-sensitive_events";
			public const string Trading_Pokemon						=	"Trading_Pokemon";
			public const string Trainer_card						=	"Trainer_card";
			public const string Trainers							=	"Trainers";
			public const string Translating_the_game				=	"Translating_the_game";
			public const string Triple_Triad						=	"Triple_Triad";
			public const string Using_moves_outside_battle			=	"Using_moves_outside_battle";
			public const string Voltorb_Flip						=	"Voltorb_Flip";
			public const string Weather								=	"Weather";
		}
	}
}