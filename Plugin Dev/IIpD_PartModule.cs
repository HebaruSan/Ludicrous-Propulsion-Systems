using KSP;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LudicrousPropulsionSystems
{
	public class InfiniteImprobabilityDrive : PartModule
	{
		[KSPField(isPersistant = true, guiActive = true, guiName = "Warp", guiActiveEditor = false)] //fix this, seems incorrect. 
		public bool warping = false;
		
		[KSPField(isPersistant = false, guiActive = true, guiName = "Warp Status")]
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
				return "Warped!";
			}
		}
		private double time()
		{
			return Planetarium.GetUniversalTime();
		}
		private double warpedTime;
		private bool waiting = false;
		private double amountNeededForWarp = 10; //constant here, how much tea is consumed per warp
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
		private bool partHasBeenDestroyed = false;
		public void Part.onPartDesroyed()
		{
			UpdateWarpStatus();
			TeaAvalible();
			partHasBeenDestroyed = true;
		}
		public void Part.FixedUpdate()
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				UpdateWarpStatus();
				if (warping)
				{
					if (TeaAvalible)
						continue;
					else
						return;
				}
				if (!waiting)
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
						partHasBeenDestroyed = false;
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
			*/
			private System.Random rand = new System.Random();//This is constructor for System.Random(used for .Next and .NextDouble)
			private CelestialBody chosenPlanet;
			/*
			//Crypto Int Generator
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
			private int GenNum(int min, int max)
			{
				return rand.Next(min, max);
			}
			private double GenDouble(double min, double max)//Not crypto, but we dont really need that here
			{
				return rand.NextDouble(min, max);
			}
			private CelestialBody RandPlanet()
			{
				private int planetPick = rand.Next(1, cbE.Count);
				chosenPlanet = cbE[planetPick];
				return cbE[planetPick];
			}
			List<CelestialBody> cbE = new List<CelestialBody>();
			cbE.Add(Planetarium.Sun);
			List<CelestialBody> cbU = cbE[0].OrbitingBodies;
			private double MaxAlt()
			{
				//need to get CB's semimajor axis, mass, parentBody's mass
				return chosenPlanet.GetSOI;
			}
			private double MinAlt()
			{
				if (chosenPlanet.atmosphere)
				{
					return (chosenPlanet.atmosphereDepth + 1000);
				}
				else
				{
					return (chosenPlanet.Radius + 1000);
				}
			}
			/*
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
			if (TeaAvalible() && warping && HighLogic.LoadedSceneIsFlight)
			{
				UpdateWarpStatus();
				while(cbU.Count > 0)
				{
					cbU.AddRange(cbU[0].OrbitingBodies);
					cbE.Add(cbU[0]);
					cbU.RemoveAt(0);
				}
				chosenPlanet = RandPlanet();
				//private string planet = GeneratePlanet();
				//Planet SOI stuff here
				//private double SOI = SOIFarReach();
				//End planet SOI calculations
				//this.Vessel.orbitDriver.orbit = new Orbit(GenerateInc(), GenerateE(), GenerateSMA(), GenerateLAN(), GenerateArgPE(), GenerateMEP(), GenerateT(), planet);
				this.Vessel.orbitDriver.orbit = new Orbit.CreateRandomOrbitAround(chosenPlanet, MinAlt(), MaxAlt());
				//need to make sure that this actually creates a good random orbit, eccentric, backwards, hugely egg-shaped, all of the above. 
				warping = false;
			}
		}
	}
}
