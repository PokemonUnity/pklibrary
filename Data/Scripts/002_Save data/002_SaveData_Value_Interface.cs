using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials
{
	public partial interface ISaveDataStaticFunctions : ISaveData
	{
		/// <summary>
		/// Registers a save value to be included in save data operations.
		/// Takes a block which defines the value's saving (<see cref="ISaveDataValue.save_value(Func{object})"/>)
		/// and loading (<see cref="ISaveDataValue.load_value(Action{object})"/>) procedures.
		/// </summary>
		/// <remarks>
		/// It is also possible to provide a proc for fetching the value
		/// from the pre-v19 format (<see cref="ISaveDataValue.from_old_format(Func{IList{object}, object})"/>), define
		/// a value to be set upon starting a new game with <see cref="ISaveDataValue.new_game_value(Func{object})"/>
		/// and ensure that the saved and loaded value is of the correct
		/// class with <see cref="ISaveDataValue.ensure_class(string)"/>.
		///
		/// Values can be registered to be loaded on bootup with
		/// <see cref="ISaveDataValue.ShouldLoadInBootup"/>. If a new_game_value proc is defined, it
		/// will be called when the game is launched for the first time,
		/// or if the save data does not contain the value in question.
		/// </remarks>
		/// <example>
		/// <code>
		/// @example Registering a new value
		///   SaveData.register(:foo) do
		///     ensure_class :Foo
		///     save_value { $foo }
		///     load_value { |value| $foo = value }
		///     new_game_value { Foo.new }
		///   end
		/// @example Registering a value to be loaded on bootup
		///   SaveData.register(:bar) do
		///     load_in_bootup
		///     save_value { $bar }
		///     load_value { |value| $bar = value }
		///     new_game_value { Bar.new }
		///   end
		/// </code>
		/// </example>
		/// <param name="id">value id</param>
		/// <param name="block">yield the block of code to be saved as a Value</param>
		void Register(int id, Action<ISaveDataValue> block = null);

		/// <summary>
		/// Unregisters a previously registered save value.
		/// </summary>
		void Unregister(int id);

		/// <summary>
		/// Validates that the given save data is valid.
		/// </summary>
		/// <param name="save_data">save data to validate</param>
		//bool valid(IDictionary<string, object> save_data);
		bool IsValid(IDictionary<int, ISaveDataObject> save_data);

		/// <summary>
		/// Loads values from the given save data based on an optional condition.
		/// </summary>
		/// <param name="save_data">save data to load from</param>
		/// <param name="condition_block">optional condition</param>
		//void load_values(IDictionary<string, object> save_data, Func<ISaveDataValue, bool> condition_block = null);
		void LoadValues(IDictionary<int, ISaveDataObject> save_data, Func<ISaveDataValue, bool> condition_block = null);

		/// <summary>
		/// Loads all registered save values from the given save data.
		/// </summary>
		/// <remarks>
		/// Loads the values from the given save data by
		/// calling each {Value} object's <see cref="ISaveDataValue.load_value(Action{object})"/> proc.
		/// Values that are already loaded are skipped.
		/// If a value does not exist in the save data and has
		/// a <see cref="ISaveDataValue.new_game_value(Func{object})"/> proc defined, that value
		/// is loaded instead.
		/// </remarks>
		//void load_all_values(IDictionary<string, object> save_data);
		void LoadAllValues(IDictionary<int, ISaveDataObject> save_data);

		/// <summary>
		/// Marks all values that aren't loaded on bootup as unloaded.
		/// </summary>
		//void mark_values_as_unloaded();
		void MarkValuesAsUnloaded();

		/// <summary>
		/// Loads only the save values configured to be loaded during bootup.
		/// </summary>
		//void load_bootup_values(IDictionary<string, object> save_data);
		void LoadBootupValues(IDictionary<int, ISaveDataObject> save_data);

		/// <summary>
		/// Initializes bootup values with their new game defaults when no save file exists.
		/// </summary>
		//void initialize_bootup_values();
		void InitializeBootupValues();

		/// <summary>
		/// Loads new game values for all registered save values.
		/// </summary>
		//void load_new_game_values();
		void LoadNewGameValues();

		/// <summary>
		/// Compiles all registered save values into a save data hash.
		/// </summary>
		//IDictionary<string, object> compile_save_hash();
		IDictionary<int, ISaveDataObject> CompileSaveHash();
	}

	/// <summary>
	/// Represents a single value in save data (SaveData::Value class).
	/// </summary>
	public interface ISaveDataValue
	{
		/// <summary>
		/// Gets the value identifier.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// </summary>
		/// <param name="id">value id</param>
		/// <param name="block"></param>
		/// <returns></returns>
		ISaveDataValue Initialize(int id, Action<ISaveDataValue> block);

		/// <summary>
		/// Checks if the given value is valid.
		/// </summary>
		/// <param name="value">value to check</param>
		bool IsValid(object value);

		/// <summary>
		/// Loads a value.
		/// </summary>
		/// <param name="value">load proc argument</param>
		/// <exception cref="">if an invalid value is being loaded</exception>
		void Load(ISaveDataObject value);

		/// <summary>
		/// Saves and returns the current value.
		/// </summary>
		/// <exception cref="">if an invalid value is being saved</exception>
		ISaveDataObject Save();

		/// <summary>
		/// Checks if this save value has a new game value procedure defined.
		/// </summary>
		//bool has_new_game_proc();
		bool HasNewGameProc();

		/// <summary>
		/// Loads the new game value.
		/// </summary>
		//void load_new_game_value();
		void LoadNewGameValue();

		/// <summary>
		/// Checks if this save value should be loaded during bootup.
		/// </summary>
		//bool load_in_bootup();
		bool ShouldLoadInBootup { get; }

		/// <summary>
		/// Configures this save value to be reset when starting a new game.
		/// </summary>
		//void reset_on_new_game();
		void ResetOnNewGame();

		/// <summary>
		/// Checks if this save value should be reset when starting a new game.
		/// </summary>
		//bool reset_on_new_game_query();
		bool ShouldResetOnNewGame { get; }

		/// <summary>
		/// Checks if this save value has been loaded.
		/// </summary>
		//bool loaded();
		bool IsLoaded { get; }

		/// <summary>
		/// Marks this save value as unloaded.
		/// </summary>
		//void mark_as_loaded(); // In case needed, but Ruby only has mark_as_unloaded
		//void mark_as_unloaded();
		void MarkAsUnloaded();

		/// <summary>
		/// Extracts this save value's data from the old pre-v19 save format.
		/// </summary>
		//object get_from_old_format(IList<object> old_format);
		object GetFromOldFormat(IList<object> old_format);

		/// <summary>
		/// Enforces that this save value must be of the specified class.
		/// </summary>
		//void ensure_class(string class_name);
		void EnsureClass(string class_name);

		/// <summary>
		/// Defines how the loaded value is placed into the game state.
		/// </summary>
		//void load_value(Action<object> block);
		void LoadValue(Action<ISaveDataObject> block);

		/// <summary>
		/// Defines what data is saved from the current game state.
		/// </summary>
		//void save_value(Func<object> block);
		void SaveValue(Func<ISaveDataObject> block);

		/// <summary>
		/// Defines the default value to use when starting a new game.
		/// </summary>
		//void new_game_value(Func<object> block);
		void NewGameValue(Func<ISaveDataObject> block);

		/// <summary>
		/// Configures this save value to be loaded during bootup.
		/// </summary>
		//void load_in_bootup_config(); // Config call inside registration block
		void LoadInBootup();

		/// <summary>
		/// Defines how to extract this value from the old pre-v19 save format.
		/// </summary>
		//void from_old_format(Func<IList<object>, object> block);
		void FromOldFormat(Func<IList<object>, object> block);
	}
}