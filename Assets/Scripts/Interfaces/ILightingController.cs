using UnityEngine;
using System.Collections;

namespace UKI 
{	
	public interface ILightingController 
	{
		void Startup ();

		void Shutdown ();

		void Stop ();
	}
}
