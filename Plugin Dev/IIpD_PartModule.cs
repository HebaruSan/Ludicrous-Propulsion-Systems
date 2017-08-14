using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LudicrousPropulsionSystems
{
	public class InfiniteImprobabilityDrive : PartModule
	{
		[KSPField(isPersistant = true, guiActive = true, guiName = WarpStatus)] //fix this, seems incorrect. 
		public bool warping = false;
		
		public string WarpStatus()
		{
			if (teaAvalible)
			{
				return "Ready To Warp!";
			}
			else if (warping)
			{
				return "Warping";
			}
			else if (!teaAvalible(amountNeededForWarp))
			{
				return "Warp Unavalible";
			}
			else
			{
				return "Warped!"
			}
		}
		private double time()
		{
			return Planetarium.GetUniversalTime();
		}
		private double warpedTime;
		private bool waiting = false;
		private double amountNeededForWarp = //constant here, how much tea is consumed per warp
		public double Tea()
		{
			return FinePrint.Utilities.VesselUtilites.VesselResourceAmount(Tea, ActiveVessel);
		}
		public bool teaAvalible(amountNeededForWarp)
		{
			if (Tea() >= amountNeededForWarp)
				return true;
			else
				return false;
		}
		public void Part.Awake()
		{
			warping = false;
			UpdateWarpStatus();
		}
		public void UpdateWarpStatus()
		{
			WarpStatus();
		}
		public void Part.FixedUpdate()
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				UpdateWarpStatus();
				if (/*parts detatch or explode*/)
				{
					//checkTeaStatus
				}
				if (WarpStatus == "Warped" && !waiting)
				{
					warpedTime = time();
					waiting = true;
				}
				if (waiting)
				{
					if (warpedTime <= (time() - 2))
					{
						waiting = false;
						UpdateWarpStatus();
					}
				}
			}
		}
	}
	public class WarpingDrive : VesselModule
	{
		public class OrbGen()
		{
			private string GeneratePlanet()
			{
				
			}
			private double GenerateOrbVelocity()
			{
				
			}
			private double GenerateOrbInclination()
			{
				
			}
		}
		public void OnFixedUpdate()
		{
			if (!HighLogic.LoadedSceneIsFlight)
			{
				return;
			}
			if (teaAvalible(amountNeededForWarp) && warping)
			{
				UpdateWarpStatus();
				warpedPlanet = OrbGen.GeneratePlanet();
				warpedVel = OrbGen.GenerateOrbVelocity();
				warpedInc = OrbGen.GenerateOrbInclination();
			}
		}
	}
}
