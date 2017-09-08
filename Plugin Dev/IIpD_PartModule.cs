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
		private List<CelestialBody> cbE = new List<CelestialBody>();
		private List<CelestialBody>cbU = new List<CelestialBody>();
		CelestialBody sun = new CelestialBody();
		Planetarium planetarium = new Planetarium();
		private double minAlt;
		private double maxAlt;
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
			Debug.Log("VM IntGen");
			return rand.Next(min, max);
		}
		private double GenDouble(double min, double max)//Not crypto, but we dont really need that here
		{
			Debug.Log("VM DoubleGen");
			return rand.NextDouble() * (max - min) + min;
		}
		private double CBRT(double num)
		{
			Debug.Log("VM CBRT");
			return (System.Math.Pow(num, (1.0/3.0)));
		}
		public void CreatePlanetList()
		{
			Debug.Log("VM CreatePlanetList");
			sun = planetarium.Sun;
			cbE.Add(sun);
			while(cbU.Count > 0)
			{
				cbU.AddRange(cbU[0].orbitingBodies);
				cbE.Add(cbU[0]);
				cbU.RemoveAt(0);
			}
		}
		private void PlanetPick()
		{
			Debug.Log("VM PlanetPick");
			if (!generated)
				Debug.Log("VM PlanetPick !Generated, generated");
				CreatePlanetList();
			planetPick = GenNum(1, cbE.Count);
		}
		private void ChoosePlanet()
		{
			Debug.Log("VM ChoosePlanet");
			for (int z = 0; z < cbE.Count; z++)
			{
				
				if (z == planetPick)
				{
					Debug.Log("VM ChoosePlanet PlanetChosen");
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
			
			double obtsMA;
			double obtEcc;
			double childMass;
			double parentMass;
			
			Orbit cbOrbit = new Orbit();
			cbOrbit = chosenPlanet.GetOrbit();
			obtsMA = cbOrbit.semiMajorAxis;
			obtEcc = cbOrbit.eccentricity;
			childMass = chosenPlanet.Mass;
			parentMass = chosenPlanet.referenceBody.Mass;
			
			Debug.Log("VM MaxAlt");
			return (obtsMA*(1-obtEcc)*CBRT(childMass/(3*parentMass)));
		}
		private double MinAlt()
		{
			Debug.Log("VM MinAlt");
			if (chosenPlanet.atmosphere)
			{
				Debug.Log("MinAlt PlanetHasAtmosphere");
				return (chosenPlanet.atmosphereDepth + 1000);
			}
			else
			{
				Debug.Log("MinAlt Planet!HaveAtmosphere");
				return (chosenPlanet.Radius + 1000);
			}
		}
		public void OnFixedUpdate()
		{
			Debug.Log("VM FixedUpdate");
			if (!generated)
			{
				Debug.Log("VM FU planetNotGenerated");
				PlanetPick();
				ChoosePlanet();
				while (chosenPlanet == FlightGlobals.currentMainBody)
				{
					PlanetPick();
					ChoosePlanet();
				}
				generated = true;
			}
			if (InfiniteImprobabilityDrive.TeaAvalible() && InfiniteImprobabilityDrive.Warping() && HighLogic.LoadedSceneIsFlight)
			{
				Debug.Log("VM FU Vessel Warping");
				InfiniteImprobabilityDrive.UpdateWarpStatus();
				//private string planet = GeneratePlanet();
				//Planet SOI stuff here
				//private double SOI = SOIFarReach();
				//End planet SOI calculations
				//this.Vessel.orbitDriver.orbit = new Orbit(GenerateInc(), GenerateE(), GenerateSMA(), GenerateLAN(), GenerateArgPE(), GenerateMEP(), GenerateT(), planet);
				this.Vessel.orbitDriver.orbit = Orbit.CreateRandomOrbitAround(chosenPlanet, MinAlt(), MaxAlt());
				Debug.Log("VM fU OrbitSet");
				//need to make sure that this actually creates a good random orbit, eccentric, backwards, hugely egg-shaped, all of the above. 
				InfiniteImprobabilityDrive.warping = false;
				generated = false;
			}
		}
	}
}
