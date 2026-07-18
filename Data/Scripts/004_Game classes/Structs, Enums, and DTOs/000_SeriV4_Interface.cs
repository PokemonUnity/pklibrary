//Original Scripts by http://answers.unity3d.com/users/4275/vicenti.html
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PokemonEssentials
{
	public interface IQuaternion : IVector, IPoint
	{
		//float x { get; }
		//float y { get; }
		//float z { get; }
		float w { get; }
	}
}