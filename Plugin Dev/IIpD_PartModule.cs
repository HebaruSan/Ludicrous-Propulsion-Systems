using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LudicrousPropulsionSystems
{
	public class InfiniteImprobabilityDrive : PartModule
	{
		[KSPField(isPersistant = true, guiActive = true, guiName = "Warp")]
		public bool warping = false;
		
		List<Part> partsInVessel = new List<Part> parts;
		public float Tea = //Get the amount of tea in the part here
		private bool teaAvalible;
		public void Part.Awake()
		{
			
		}
		public void Part.FixedUpdate()
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				if (/*parts detatch or explode*/)
				{
					//checkTeaStatus
				}
			}
		}
		private void TeaStatus(int amountRequired)
		{
			
				if( && Tea >= amountRequiredForWarp)
				{
					teaAvalible = true;
				}
				else
				{
					teaAvalible = false;
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