using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LudicrousPropulsionSystems
{
	public class InfiniteImprobabilityDrive : PartModule
	{
		[KSPField(isPersistant = true, guiActive = true, guiName = "Warp", guiActiveEditor = false)] //fix this, seems incorrect. 
		public static bool warping = false;
		
		[KSPField(isPersistant = false, guiActive = true, guiName = "Warp Status")]
		public static string WarpStatus;
		
		private double Time()
		{
			Debug.Log("IIpD Time");
			return Planetarium.GetUniversalTime();
			
		}
		private double warpedTime;
		private bool waiting = false;
		private static double amountNeededForWarp = 10; //TODO, constant here: how much tea is consumed per warp, change to include cfg file amounts
		public static double Tea()
		{
			Debug.Log("IIpD TeaAmount");
			return FinePrint.Utilities.VesselUtilities.VesselResourceAmount("Tea", FlightGlobals.ActiveVessel);
		}
		public static bool TeaAvalible()
		{
			Debug.Log("IIpD TeaAvalible");
			if (Tea() >= amountNeededForWarp)
			{
				Debug.Log("TeaAvalible - true");
				return true;
			}
			else
			{
				Debug.Log("TeaAvalible - false");
				return false;
			}
		}
		public static bool Warping()
		{
			Debug.Log("IIpD Warping");
			if (warping)
			{
				Debug.Log("Warping - true");
				return true;
			}
			else
			{
				Debug.Log("Warping - false");
				return false;
			}
		}
		public static bool WarpAvalible()
		{
			Debug.Log("IIpD WarpAvalible");
			if (TeaAvalible() == true && Warping() == false)
			{
				Debug.Log("WarpAvalible - true");
				return true;
			}
			else
			{
				Debug.Log("WarpAvalible - false");
				return false;
			}
		}
		public static void UpdateWarpStatus()
		{
			Debug.Log("IIpD UpdateWarpStatus");
			if (TeaAvalible())
			{
				Debug.Log("UpdateWarpStatus - Ready");
				WarpStatus = "Ready To Warp!";
			}
			else if (warping)
			{
				Debug.Log("UpdateWarpStatus - Warping");
				WarpStatus = "Warping";
			}
			else if (!TeaAvalible())
			{
				Debug.Log("UpdateWarpStatus - Unavalible");
				WarpStatus = "Warp Unavalible";
			}
			else
			{
				Debug.Log("UpdateWarpStatus - Warped");
				WarpStatus = "Warped!";
			}
		}
		/*
		private bool partHasBeenDestroyed = false;
		public void onPartDesroyed()
		{
			UpdateWarpStatus();
			TeaAvalible();
			partHasBeenDestroyed = true;
		}
		*/
		public void FixedUpdate()
		{
			Debug.Log("IIpD FixedUpdate");
			if (HighLogic.LoadedSceneIsFlight)
			{
				Debug.Log("FU HL.LoadedSceneIsFlight");
				UpdateWarpStatus();
				TeaAvalible();
				if(warping)
				{
				        Debug.Log("FU Warping");
					UpdateWarpStatus();
				}
				if (!waiting)
				{
					Debug.Log("FU !waiting");
					warpedTime = Time();
					waiting = true;
				}
				if (waiting)
				{
					Debug.Log("FU waiting");
					if (warpedTime <= (Time() - 2))
					{
						Debug.Log("FU waiting false");
						waiting = false;
						UpdateWarpStatus();
						//partHasBeenDestroyed = false;
					}
				}
			}
		}
	}
}
