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
			if (TeaAvalible)
			{
				return "Ready To Warp!";
			}
			else if (warping)
			{
				return "Warping";
			}
			else if (!TeaAvalible())
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
			UpdateWarpStatus();
			TeaAvalible();
		}
		public void Part.FixedUpdate()
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				UpdateWarpStatus();
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
		/*
		public class OrbGen()
		{
			//All to generate a crypto int, not really nessesary, slow, but fun!
			//Should we remove this for speed issues?
			//private RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();//Constructor for CryptoInt
			private System.Random rand = new System.Random();//This is constructor for System.Random(used for .Next and .NextDouble)
			 Crypto Int Generator
			private int GenNum(int min, int max)
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
			//End crypto int
			*/
			private int GenNum(int max, int min)
			{
				return rand.Next(max, min);
			}
			private double GenDouble(double min, double max)//Not crypto, but we dont really need that here
			{
				return rand.NextDouble(min, max);
			}
			private string GeneratePlanet()
			{
				swtich(GenNum(1,14))
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
			/*
			private double SOIFarReach()
			{
				//need to get CB's semimajor axis, mass, parentBody's mass
				return /* SMA 
			}
			private double GenerateInc()
			{
				return GenNum(-180, 180);
			}
			private double GenerateE()
			{
				private double T = 
				private double n = (360/T);//what the averge rate of sweep is
				private double r = time();//universal time
				private double t = r + GenDouble(100, 500);//arbitrary time after pericenter
				private double M = n*(t-
				return M;
			}
			private double GenerateSMA()
			{
				
			}
			private double GenerateLAN()
			{
			
			}
			private double GenerateArgPE()
			{
			
			}
			private double GenerateMEP()
			{
			
			}
			private double GenerateT()
			{
			
			}
		}
		*/
		public void OnFixedUpdate()
		{
			if (!HighLogic.LoadedSceneIsFlight)
			{
				return;
			}
			if (teaAvalible(amountNeededForWarp) && warping)
			{
				UpdateWarpStatus();
				//private string planet = GeneratePlanet();
				//Planet SOI stuff here
				//private double SOI = SOIFarReach();
				//End planet SOI calculations
				//this.Vessel.orbitDriver.orbit = new Orbit(GenerateInc(), GenerateE(), GenerateSMA(), GenerateLAN(), GenerateArgPE(), GenerateMEP(), GenerateT(), planet);
				this.Vessel.orbitDriver.orbit = new Orbit.CreateRandomOrbitAround(GeneratePlanet());
				warping = false;
			}
		}
	}
}
