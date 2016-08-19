using UnityEngine;
using System.Collections;

namespace UKI 
{	
	public abstract class LightingController : MonoBehaviour
	{
		public abstract void Startup ();

		public abstract void Shutdown ();

		public abstract void Stop ();
	}
}
