using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials
{
	/// <summary>
	/// Interface for the Plugin Manager system (PluginManager module).
	/// </summary>
	public interface IPluginManager
	{
		/// <summary>
		/// Registers a plugin with the plugin manager system.
		/// </summary>
		void Register(IPluginMetadata options);

		/// <summary>
		/// Throws a plugin error message and exits the application.
		/// </summary>
		void Error(string msg);

		/// <summary>
		/// Checks if a specific plugin is installed, optionally with version checking.
		/// </summary>
		bool Installed(string plugin_name, string plugin_version = null, bool mustequal = false);

		/// <summary>
		/// Gets the names of all currently installed plugins.
		/// </summary>
		/// <returns>A list of plugin names.</returns>
		IList<string> Plugins { get; }

		/// <summary>
		/// Gets the version of a specific installed plugin.
		/// </summary>
		/// <param name="pluginName">The name of the plugin.</param>
		/// <returns>The version string, or null if the plugin is not installed.</returns>
		string Version(string plugin_name);

		/// <summary>
		/// Gets the download/website link for a specific plugin.
		/// </summary>
		/// <param name="pluginName">The name of the plugin.</param>
		/// <returns>The link string, or null if the plugin is not installed or has no link.</returns>
		string Link(string plugin_name);

		/// <summary>
		/// Gets the credits list for a specific plugin.
		/// </summary>
		/// <param name="pluginName">The name of the plugin.</param>
		/// <returns>A list of credit strings, or null if the plugin is not installed or has no credits.</returns>
		IList<string> Credits(string plugin_name);

		/// <summary>
		/// Compares two version strings to determine their relative ordering.
		/// </summary>
		/// <remarks>
		/// Compares two versions given in string form. v1 should be the plugin version
		/// you actually have, and v2 should be the minimum/desired plugin version.
		/// Return values:
		///     1 if v1 is higher than v2
		///     0 if v1 is equal to v2
		///     -1 if v1 is lower than v2
		/// </remarks>
		/// <param name="v1">The first version string (actual plugin version).</param>
		/// <param name="v2">The second version string (minimum/desired plugin version).</param>
		/// <returns>1 if v1 is higher than v2, 0 if v1 is equal to v2, -1 if v1 is lower than v2.</returns>
		//int compare_versions(string v1, string v2);
		int CompareVersions(string v1, string v2);

		/// <summary>
		/// Formats and displays a plugin error message.
		/// </summary>
		/// <param name="name">The name of the plugin.</param>
		/// <param name="script">The script file where the error occurred.</param>
		/// <param name="exception">The exception that occurred.</param>
		//void pluginErrorMsg(string name, string script);
		void PluginErrorMsg(string name, string script, System.Exception exception);

		/// <summary>
		/// Used to read the metadata file.
		/// </summary>
		/// <remarks>
		/// Reads and parses a plugin's meta.txt file to extract metadata.
		/// </remarks>
		/// <param name="dir">The directory of the plugin.</param>
		/// <param name="file">The name of the meta file (e.g., "meta.txt").</param>
		/// <returns>A dictionary containing the plugin's metadata.</returns>
		IPluginMetadata ReadMeta(string dir, string file);

		/// <summary>
		/// Gets a list of all plugin directories to inspect for valid plugins.
		/// </summary>
		IList<string> ListAll();

		/// <summary>
		/// Validates plugin dependencies to catch circular dependency loops.
		/// </summary>
		void ValidateDependencies(string name, IDictionary<string, IPluginMetadata> meta, IList<string> og = null);

		/// <summary>
		/// Sorts the plugin load order based on dependencies.
		/// </summary>
		IList<string> SortLoadOrder(IList<string> order, IDictionary<string, IPluginMetadata> plugins);

		/// <summary>
		/// Determines the correct order to load plugins based on their dependencies.
		/// </summary>
		KeyValuePair<IList<string>, IDictionary<string, IPluginMetadata>> GetPluginOrder();

		/// <summary>
		/// Checks if plugins need to be recompiled based on file modification times.
		/// </summary>
		bool NeedCompiling(IList<string> order, IDictionary<string, IPluginMetadata> plugins);

		/// <summary>
		/// Compiles all plugins.
		/// </summary>
		void CompilePlugins(IList<string> order, IDictionary<string, IPluginMetadata> plugins);

		/// <summary>
		/// Main entry point that runs the complete plugin system.
		/// </summary>
		void RunPlugins();

		/// <summary>
		/// Finds the directory path for a plugin by its name.
		/// </summary>
		string FindDirectory(string name);
	}

	/// <summary>
	/// Interface for plugin metadata and configuration.
	/// </summary>
	public interface IPluginMetadata
	{
		string Name { get; set; }
		string Version { get; set; }
		IList<string> Essentials { get; set; }
		string Link { get; set; }
		IList<object> Dependencies { get; set; }
		IList<string> Incompatibilities { get; set; }
		IList<string> Credits { get; set; }
		IList<string> Scripts { get; set; }
		string Dir { get; set; }
	}
}