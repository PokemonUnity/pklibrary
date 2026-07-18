using System;
using System.Collections;
using System.Collections.Generic;

namespace PokemonEssentials
{
	public interface ITextPosition
	{
		/// <summary>
		/// Text to draw
		/// </summary>
		string Text { get; set; }
		/// <summary>
		/// X coordinate
		/// </summary>
		float X { get; set; }
		/// <summary>
		/// Y coordinate
		/// </summary>
		float Y { get; set; }
		/// <summary>
		/// If true or 1, the text is right aligned. If 2, the text is centered.
		/// Otherwise, the text is left aligned.
		/// </summary>
		/// <remarks>
		/// Is one of :left (or false or 0), :right (or true or 1) or
		/// :center (or 2). If anything else, the text is left aligned.
		/// </remarks>
		bool? LeftAligned { get; set; }
		/// <summary>
		/// Base color
		/// </summary>
		IColor Base { get; set; }
		/// <summary>
		/// Shadow color. If null, there is no shadow.
		/// </summary>
		/// <remarks>
		/// If :outline (or true or 1), the text has a full outline. If :none (or the
		/// shadow color is null), there is no shadow. Otherwise, the text has a shadow.
		/// </remarks>
		IColor Shadow { get; set; }
	}
}