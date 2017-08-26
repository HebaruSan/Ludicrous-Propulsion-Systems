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
		public string WarpStatus;
		
		private double Time()
		{
			return Planetarium.GetUniversalTime();
		}
		private double warpedTime;
		private bool waiting = false;
		private bool teaAvalible = false;
		private Vessel ves;
		private double amountNeededForWarp = 10; //constant here, how much tea is consumed per warp, will change to include cfg file amounts
		public double Tea()
		{
			ves = Part.Vessel;
			return FinePrint.Utilities.VesselUtilities.VesselResourceAmount(Tea, FlightGlobals.ActiveVessel());
		}
		public void TeaAvalible()
		{
			if (Tea() >= amountNeededForWarp)
				teaAvalible = true;
			else
				teaAvalible = false;
		}
		public bool WarpAvalible()
		{
			if (teaAvalible == true && warping == false)
				return true;
			else
				return false;
		}
		public void UpdateWarpStatus()
		{
			if (teaAvalible)
			{
				WarpStatus = "Ready To Warp!";
			}
			else if (warping)
			{
				WarpStatus = "Warping";
			}
			else if (!teaAvalible)
			{
				WarpStatus = "Warp Unavalible";
			}
			else
			{
				WarpStatus = "Warped!";
			}
		}
		private bool partHasBeenDestroyed = false;
		public void onPartDesroyed()
		{
			UpdateWarpStatus();
			TeaAvalible();
			partHasBeenDestroyed = true;
		}
		public void FixedUpdate()
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				UpdateWarpStatus();
				TeaAvalible();
				if(warping)
				{
				UpdateWarpStatus();
				}
				if (!waiting)
				{
					warpedTime = Time();
					waiting = true;
				}
				if (waiting)
				{
					if (warpedTime <= (Time() - 2))
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
			private bool generated = false;
			private int planetPick;
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
				return rand.NextDouble() * (max - min) + min;
			}
			private void PlanetPick()
			{
				if (!generated)
					CreatePlanetList();
				planetPick = GenNum(1, cbE.Count);
			}
			private void ChoosePlanet()
			{
				for (int z = 0; z < cbE.Count; z++)
				{
					if (z == planetPick)
					{
						chosenPlanet = cbE[z];
					}
				}
			}
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
			public void CreatePlanetList()
			{
				CelestialBody sun = new CelestialBody();
				sun = Planetarium.Sun;
				List<CelestialBody> cbE = new List<CelestialBody>();
				cbE.Add(sun);
				List<CelestialBody> cbU = cbE[0].orbitingBodies;
				while(cbU.Count > 0)
				{
					cbU.AddRange(cbU[0].orbitingBodies);
					cbE.Add(cbU[0]);
					cbU.RemoveAt(0);
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
			if (!generated)
			{
				CreatePlanetList();
				PlanetPick();
				ChoosePlanet();
				while (chosenPlanet == FlightGlobals.currentMainBody)
				{
					PlanetPick();
					ChoosePlanet();
				}
			}
			if (teaAvalible && warping && HighLogic.LoadedSceneIsFlight)
			{
				UpdateWarpStatus();
				chosenPlanet = RandPlanet();
				//private string planet = GeneratePlanet();
				//Planet SOI stuff here
				//private double SOI = SOIFarReach();
				//End planet SOI calculations
				//this.Vessel.orbitDriver.orbit = new Orbit(GenerateInc(), GenerateE(), GenerateSMA(), GenerateLAN(), GenerateArgPE(), GenerateMEP(), GenerateT(), planet);
				this.Vessel.orbitDriver.orbit = new Orbit.CreateRandomOrbitAround(chosenPlanet, MinAlt(), MaxAlt());
				//need to make sure that this actually creates a good random orbit, eccentric, backwards, hugely egg-shaped, all of the above. 
				warping = false;
				generated = false;
			}
		}
	}
}
