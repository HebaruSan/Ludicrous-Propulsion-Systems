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
		public bool TeaAvailable()
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
			private int planetGenNum;
			private int planetGenNumDiff;
			private int planetForLoopTimesThrough;
			private int a;
			private int b;
			private int c;
			private int d;
			private string RandPlanGen()
			{
				for (i = random.Next(0,100), i < 101, i += random.Next(5)
				{
					planetGenNum += random.Next(1,14);
					planetForLoopTimesThrough++;
				}
				planetGenNum = planetGenNum / planetForLoopTimesThrough;
				for (j = random.Next(1,14), j < 15, j++)
				{
					a += j;
					b *= a;
					d = (a * j)/(b % 3);
					c = ((d * a) % b) + j;
					for (k = random.Next(random.Next(1,50),random.Next(51,100), k < random.Next(120, 150), k ++)
					{
						a *= k;
						b += k;
						c -= k;
						d /= k;
					}
				}//finish randomizer TODO
				planetGenNum *= ((a + b) / (c * d));
				planetGenNum /= ((b * c) / (a % d));
				if (planetGenNum > 14)
				{
					for(p = planetGenNum, p < 15, p--)
					{
						planetGenNumDiff++
					}
					planetGenNum -= random.Next(planetGenNumDiff, planetGenNumDiff + 14);
				}
				//Here is the part where the GenNum
			}
			private void ResetGenNum()
			{
				planetGenNum = null;
				planetForLoopTimesThrough = null;
			}
			private string GeneratePlanet()
			{
				return RandPlanGen();
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
				Vessel.obt_velocity = warpedVel;
			}
		}
	}
}
