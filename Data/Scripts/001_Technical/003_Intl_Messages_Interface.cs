using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials
{
	/// <summary>
	/// Global methods for internationalization and message handling (IMain interface).
	/// </summary>
	public interface IMainIntlMessages : IMain
	{
		/// <summary>
		/// Gets a message by type and ID.
		/// </summary>
		string pbGetMessage(int type, object id);

		/// <summary>
		/// Gets a message from a hash by type and ID.
		/// </summary>
		string pbGetMessageFromHash(int type, string id);

		/// <summary>
		/// Formats a message with the provided arguments.
		/// </summary>
		string _INTL(string format, params object[] args);

		/// <summary>
		/// Formats a message with printf-style formatting.
		/// </summary>
		string _ISPRINTF(string format, params object[] args);

		/// <summary>
		/// Formats a map-specific message with the provided arguments.
		/// </summary>
		string _I(string str, params object[] args);

		/// <summary>
		/// Formats a map-specific message with the provided arguments.
		/// </summary>
		string _MAPINTL(int mapid, params object[] args);

		/// <summary>
		/// Formats a map-specific message with printf-style formatting.
		/// </summary>
		string _MAPISPRINTF(int mapid, params object[] args);
	}

	/// <summary>
	/// Provides utility functions for translating and managing game text (Translator module).
	/// </summary>
	public interface ITranslator
	{
		/// <summary>
		/// Gathers translatable text from scripts and event data.
		/// </summary>
		//void gather_script_and_event_texts();
		void GatherScriptAndEventTexts();

		/// <summary>
		/// Finds translatable text from scripts.
		/// </summary>
		/// <param name="items">The list to add translatable texts to.</param>
		/// <param name="script">The script content.</param>
		//void find_translatable_text_from_RGSS_script(IList<string> items, string script);
		void FindTranslatableTextFromRGSSScript(IList<string> items, string script);

		/// <summary>
		/// Finds translatable text from event scripts.
		/// </summary>
		/// <param name="items">The list to add translatable texts to.</param>
		/// <param name="script">The script content.</param>
		//void find_translatable_text_from_event_script(IList<string> items, string script);
		void FindTranslatableTextFromEventScript(IList<string> items, string script);

		/// <summary>
		/// Normalizes text values for storage (e.g., replaces newlines with <<n>>).
		/// </summary>
		/// <param name="value">The value to normalize.</param>
		/// <returns>The normalized value.</returns>
		//string normalize_value(string value);
		string NormalizeValue(string value);

		/// <summary>
		/// Denormalizes stored text values for display (e.g., replaces <<n>> with newlines).
		/// </summary>
		/// <param name="value">The value to denormalize.</param>
		/// <returns>The denormalized value.</returns>
		//string denormalize_value(string value);
		string DenormalizeValue(string value);

		/// <summary>
		/// Extracts text for translation.
		/// </summary>
		/// <param name="language_name">The name of the language.</param>
		/// <param name="core_text">True if extracting core texts, false for game texts.</param>
		/// <param name="separate_map_files">True to create separate files for map texts.</param>
		//void extract_text(string language_name = "default", bool core_text = false, bool separate_map_files = false);
		void ExtractText(string language_name = "default", bool core_text = false, bool separate_map_files = false);

		/// <summary>
		/// Writes section texts to a file.
		/// </summary>
		/// <param name="f">The file stream.</param>
		/// <param name="section_name">The name of the section.</param>
		/// <param name="language_msgs">Translated messages for the section.</param>
		/// <param name="original_msgs">Original messages for the section.</param>
		//void write_section_texts_to_file(object f, string section_name, object language_msgs, object original_msgs = null);
		void WriteSectionTextsToFile(System.IO.FileStream f, string section_name, IDictionary<string, string> language_msgs, IDictionary<string, string> original_msgs = null);

		/// <summary>
		/// Compiles text from translation files.
		/// </summary>
			/// <param name="dirName">The directory name.</param>
			/// <param name="datFilename">The data filename.</param>
		//void compile_text(string dir_name, string dat_filename);
		void CompileText(string dir_name, string dat_filename);

		/// <summary>
		/// Compiles text from a file.
		/// </summary>
			/// <param name="textFile">The path to the text file.</param>
			/// <param name="allText">The list to add compiled text to.</param>
		//void compile_text_from_file(string text_file, object all_text);
		void CompileTextFromFile(string text_file, System.Collections.Generic.IList<string> all_text);
	}

	/// <summary>
	/// Manages game text translations and message handling (Translation class).
	/// </summary>
	public interface ITranslation
	{
		/// <summary>
		/// Gets the default core messages.
		/// </summary>
		object default_core_messages { get; }

		/// <summary>
		/// Gets the default game messages.
		/// </summary>
		object default_game_messages { get; }

		/// <summary>
		/// Loads message files from the specified path.
		/// </summary>
		//void load_message_files(string filename);
		void LoadMessageFiles(string filename);

		/// <summary>
		/// Loads the default messages.
		/// </summary>
		//void load_default_messages();
		void LoadDefaultMessages();

		/// <summary>
		/// Saves the default messages.
		/// </summary>
		//void save_default_messages();
		void SaveDefaultMessages();

		/// <summary>
		/// Sets messages for a specific type.
		/// </summary>
		//void setMessages(int type, IList<string> array);
		void SetMessages(int type, IList<string> array);

		/// <summary>
		/// Adds messages to a specific type.
		/// </summary>
		//void addMessages(int type, IList<string> array);
		void AddMessages(int type, IList<string> array);

		/// <summary>
		/// Sets messages as a hash for a specific type.
		/// </summary>
		//void setMessagesAsHash(int type, IList<string> array);
		void SetMessagesAsHash(int type, IList<string> array);

		/// <summary>
		/// Adds messages as a hash for a specific type.
		/// </summary>
		//void addMessagesAsHash(int type, IList<string> array);
		void AddMessagesAsHash(int type, IList<string> array);

		/// <summary>
		/// Sets map-specific messages as a hash.
		/// </summary>
		//void setMapMessagesAsHash(int map_id, IList<string> array);
		void SetMapMessagesAsHash(int map_id, IList<string> array);

		/// <summary>
		/// Adds map messages as a hash for a specific map ID.
		/// </summary>
		//void addMapMessagesAsHash(int map_id, IList<string> array);
		void AddMapMessagesAsHash(int map_id, IList<string> array);

		/// <summary>
		/// Gets a message by type and ID.
		/// </summary>
		string Get(int type, int id);

		/// <summary>
		/// Gets a message from a hash by type and text.
		/// </summary>
		string GetFromHash(int type, string text);

		/// <summary>
		/// Gets a map-specific message from a hash.
		/// </summary>
		string GetFromMapHash(int map_id, string text);
	}

	/// <summary>
	/// Provides message type constants and helper methods for internationalization (MessageTypes module).
	/// </summary>
	public interface IMessageTypes
	{
		int EVENT_TEXTS { get; }
		int SPECIES_NAMES { get; }
		int SPECIES_CATEGORIES { get; }
		int POKEDEX_ENTRIES { get; }
		int SPECIES_FORM_NAMES { get; }
		int MOVE_NAMES { get; }
		int MOVE_DESCRIPTIONS { get; }
		int ITEM_NAMES { get; }
		int ITEM_NAME_PLURALS { get; }
		int ITEM_DESCRIPTIONS { get; }
		int ABILITY_NAMES { get; }
		int ABILITY_DESCRIPTIONS { get; }
		int TYPE_NAMES { get; }
		int TRAINER_TYPE_NAMES { get; }
		int TRAINER_NAMES { get; }
		int FRONTIER_INTRO_SPEECHES { get; }
		int FRONTIER_END_SPEECHES_WIN { get; }
		int FRONTIER_END_SPEECHES_LOSE { get; }
		int REGION_NAMES { get; }
		int REGION_LOCATION_NAMES { get; }
		int REGION_LOCATION_DESCRIPTIONS { get; }
		int MAP_NAMES { get; }
		int PHONE_MESSAGES { get; }
		int TRAINER_SPEECHES_LOSE { get; }
		int SCRIPT_TEXTS { get; }
		int RIBBON_NAMES { get; }
		int RIBBON_DESCRIPTIONS { get; }
		int STORAGE_CREATOR_NAME { get; }
		int ITEM_PORTION_NAMES { get; }
		int ITEM_PORTION_NAME_PLURALS { get; }
		int POKEMON_NICKNAMES { get; }

		//void load_default_messages();
		void LoadDefaultMessages();

		//void load_message_files(string filename);
		void LoadMessageFiles(string filename);

		//void save_default_messages();
		void SaveDefaultMessages();

		//void setMessages(int type, IList<string> array);
		void SetMessages(int type, IList<string> array);

		//void addMessages(int type, IList<string> array);
		void AddMessages(int type, IList<string> array);

		//void SetMessagesAsHash(int type, IList<string> array);
		void SetMessagesAsHash(int type, IList<string> array);

		//void addMessagesAsHash(int type, IList<string> array);
		void AddMessagesAsHash(int type, IList<string> array);

		//void setMapMessagesAsHash(int type, IList<string> array);
		void SetMapMessagesAsHash(int type, IList<string> array);

		//void addMapMessagesAsHash(int type, IList<string> array);
		void AddMapMessagesAsHash(int type, IList<string> array);

		//string get(int type, object id);
		string Get(int type, int id);

		//string getFromHash(int type, string key);
		string GetFromHash(int type, string key);

		//string getFromMapHash(int type, string key);
		string GetFromMapHash(int type, string key);
	}
}