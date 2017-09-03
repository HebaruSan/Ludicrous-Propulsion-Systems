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
		private double amountNeededForWarp = 10; //TODO, constant here: how much tea is consumed per warp, change to include cfg file amounts
		public double Tea()
		{
			return FinePrint.Utilities.VesselUtilities.VesselResourceAmount(Tea, FlightGlobals.ActiveVessel);
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
		//All to generate a crypto int, not really nessesary, slow, but fun!
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
		private double CBRT(double num)
		{
			return (System.Math.Pow(num, (1.0/3.0)));
		}
		public void CreatePlanetList()
		{
			CelestialBody sun = new CelestialBody();
			Planetarium planetarium = new Planetarium();
			sun = planetarium.Sun;
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
			/*
			Hill Sphere is a method of calculating SOI. This is done by getting the radius through the calculation r = a(1 - e)CBRT(m/3M)
			Where a = semimajor axis, e = eccentricity, m = mass(child body), and M = mass(parent body)
			*/
			
			private double obtsMA;
			private double obtEcc;
			private double childMass;
			private double parentMass;
			
			Orbit cbOrbit = new Orbit();
			cbOrbit = chosenPlanet.GetOrbit();
			obtsMA = cbOrbit.semiMajorAxis;
			obtEcc = cbOrbit.eccentricity;
			childMass = chosenPlanet.Mass;
			parentMass = chosenPlanet.referenceBody.Mass;
			
			return (a*(1-obtEcc)*CBRT(childMass/(3*parentMass)));
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
		public void OnFixedUpdate()
		{
			if (!generated)
			{
				PlanetPick();
				ChoosePlanet();
				while (chosenPlanet == FlightGlobals.currentMainBody)
				{
					PlanetPick();
					ChoosePlanet();
				}
				generated = true;
			}
			if (InfiniteImprobabilityDrive.teaAvalible && InfiniteImprobabilityDrive.warping && HighLogic.LoadedSceneIsFlight)
			{
				InfiniteImprobabilityDrive.UpdateWarpStatus();
				//private string planet = GeneratePlanet();
				//Planet SOI stuff here
				//private double SOI = SOIFarReach();
				//End planet SOI calculations
				//this.Vessel.orbitDriver.orbit = new Orbit(GenerateInc(), GenerateE(), GenerateSMA(), GenerateLAN(), GenerateArgPE(), GenerateMEP(), GenerateT(), planet);
				this.Vessel.orbitDriver.orbit = new Orbit.CreateRandomOrbitAround(chosenPlanet, MinAlt(), MaxAlt());
				//need to make sure that this actually creates a good random orbit, eccentric, backwards, hugely egg-shaped, all of the above. 
				InfiniteImprobabilityDrive.warping = false;
				generated = false;
			}
		}
	}
}
