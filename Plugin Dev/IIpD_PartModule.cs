using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LudicrousPropulsionSystems
{
	public class InfiniteImprobabilityDrive : PartModule
	{
		[KSPField(isPersistant = true, guiActive = true, guiName = WarpStatus)]
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
		public void OnFixedUpdate
		{
			if (!HighLogic.LoadedSceneIsFlight)
			{
				return;
			}
			if (teaAvalible )
		}
	}
}
