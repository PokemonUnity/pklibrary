using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials
{
	/// <summary>
	/// Interface for the SaveData module (SaveData module).
	/// </summary>
	public interface ISaveData
	{
		/// <summary>
		/// Gets the file path of the save file.
		/// </summary>
		string FILE_PATH { get; }

		/// <summary>
		/// Checks if the save file exists.
		/// </summary>
		//bool exists();
		bool Exists { get; }

		/// <summary>
		/// Fetches the save data from the given file.
		/// </summary>
		/// <param name="file_path">path of the file to load from</param>
		/// <returns>loaded save data</returns>
		/// <exception cref="System.IO.IOException">if file opening fails</exception>
		//object get_data_from_file(string file_path);
		ISaveDataObject GetDataFromFile(string file_path);

		/// <summary>
		/// Fetches save data from the given file. If it needed converting, resaves it.
		/// </summary>
		/// <param name="file_path">path of the file to read from</param>
		/// <returns>save data in Hash format</returns>
		/// <exception cref="System.IO.IOException"><see cref="GetDataFromFile"/></exception>
		//IDictionary<string, object> read_from_file(string file_path);
		IDictionary<string, object> ReadFromFile(string file_path);

		/// <summary>
		/// Compiles the save data and saves a marshaled version of it into the given file.
		/// </summary>
		//void save_to_file(string file_path);
		void SaveToFile(string file_path);

		/// <summary>
		/// Deletes the save file and backup files.
		/// </summary>
		//void delete_file();
		void DeleteFile();

		/// <summary>
		/// Converts pre-v19 format data to the new format.
		/// </summary>
			/// <param name="old_format">pre-v19 format save data</param>
			/// <returns>save data in new format</returns>
		//IDictionary<string, object> to_hash_format(IList<object> old_format);
		IDictionary<int, object> ToHashFormat(IList<object> old_format);
	}
}