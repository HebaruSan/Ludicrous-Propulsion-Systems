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
		public void Part.onPartDesroyed()
		{
		
		}
		public void Part.FixedUpdate()
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				UpdateWarpStatus();
				if (Part.onPartDestroyed)
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
			private string currentBody;
			private double currentVel;
			private double currentAlt;
			private RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
			private int PlanetGenNum(int min, int max)
			{
				uint scale = uint.MaxValue;
				while (scale == uint.MaxValue)
				{
					byte[] four_bytes = new byte[4];
					rand.GetBytes(four_bytes);
					scale = BitConverter.ToUInt32(four_bytes, 0);
				}
				return (int)(min + (max - min) * (scale / (double)uint.MaxValue));
			}
			private string GeneratePlanet()
			{
				swtich(PlanetGenNum(1,14))
				{
					case 1:
						return "Moho";
					case 2:
						return "Eve";
					case 3:
						return "Mun";
					case 4:
						return "Minmus";
					case 5:
						return "Duna";
					case 6: 
						return "Ike";
					case 7:
						return "Dres";
					case 8:
						return "Jool";
					case 9:
						return "Tylo";
					case 10:
						return "Vall";
					case 11:
						return "Laythe";
					case 12:
						return "Bop";
					case 13:
						return "Pol";
					case 14:
						return "Eeloo";
				}
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
				//CBset
				Vessel.SetWorldVelocity() = GenerateOrbVelocity(); //or obt_velocity
				//set inclination
				currentAlt = Vessel.altitude;//set altitude
			}
		}
	}
}
