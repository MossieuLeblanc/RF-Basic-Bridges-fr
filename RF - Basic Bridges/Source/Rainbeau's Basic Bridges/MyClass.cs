using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RBB_Code {

	public class PlaceWorker_BasicBridge : PlaceWorker {
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null) {
			TerrainDef terrainDef = base.Map.terrainGrid.TerrainAt(loc);
			if (terrainDef == TerrainDef.Named("WaterShallow") || terrainDef == TerrainDef.Named("WaterOceanShallow") || terrainDef == TerrainDef.Named("WaterMovingShallow") || terrainDef == TerrainDef.Named("Marsh") || terrainDef == TerrainDef.Named("Mud")) {
				List<Thing> things = base.Map.thingGrid.ThingsListAtFast(loc);
				for (int j = 0; j < things.Count; j++) {
					if (things[j] != thingToIgnore) {
						if (things[j].def.defName == "RBB_FishingSpot") {
							return new AcceptanceReport("RBB.NotOnFS".Translate());
						}
					}
				}
				return true;
			}
			return new AcceptanceReport("RBB.BasicBridge".Translate());
		}
	}

	public class PlaceWorker_DeepWaterBridge : PlaceWorker {
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null) {
			TerrainDef terrainDef = base.Map.terrainGrid.TerrainAt(loc);
			if (terrainDef == TerrainDef.Named("WaterDeep") || terrainDef == TerrainDef.Named("WaterOceanDeep") || terrainDef == TerrainDef.Named("WaterMovingDeep")) {
				List<Thing> things = base.Map.thingGrid.ThingsListAtFast(loc);
				for (int j = 0; j < things.Count; j++) {
					if (things[j] != thingToIgnore) {
						if (things[j].def.defName == "RBB_FishingSpot") {
							return new AcceptanceReport("RBB.NotOnFS".Translate());
						}
					}
				}
				return true;
			}
			return new AcceptanceReport("RBB.DeepWaterBridge".Translate());
		}
	}
			
	public class PlaceWorker_Boardwalk : PlaceWorker {
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null) {
			TerrainDef terrainDef = base.Map.terrainGrid.TerrainAt(loc);
			if (terrainDef.defName.Contains("Bridge")) {
			    return new AcceptanceReport("RBB.NoFloorOnBridge".Translate());
			}
			string waterNear = "no";
			for (int i = -1; i < 2; i++) {
				for (int j = -1; j < 2; j++) {
					int x = loc.x + i;
					int z = loc.z + j;
					IntVec3 newSpot = new IntVec3(x, 0, z);
					string terrainCheck = base.Map.terrainGrid.TerrainAt(newSpot).defName;
					if (terrainCheck.Contains("Water") || terrainCheck.Equals("Marsh") || terrainCheck.Equals("BridgeMarsh")) {
						waterNear = "yes";
						break;
					}
				}
			}
			if (waterNear == "no") {
				for (int i = -2; i < 3; i++) {
					for (int j = -2; j < 3; j++) {
						int x = loc.x + i;
						int z = loc.z + j;
						IntVec3 newSpot = new IntVec3(x, 0, z);
						string terrainCheck = base.Map.terrainGrid.TerrainAt(newSpot).defName;
						if (terrainCheck.Contains("Water") || terrainCheck.Equals("Marsh") || terrainCheck.Equals("BridgeMarsh")) {
							waterNear = "yes";
							break;
						}
					}
				}
			}
			if (waterNear == "no") {
				for (int i = -3; i < 4; i++) {
					for (int j = -3; j < 4; j++) {
						int x = loc.x + i;
						int z = loc.z + j;
						IntVec3 newSpot = new IntVec3(x, 0, z);
						string terrainCheck = base.Map.terrainGrid.TerrainAt(newSpot).defName;
						if (terrainCheck.Contains("Water") || terrainCheck.Equals("Marsh") || terrainCheck.Equals("BridgeMarsh")) {
							waterNear = "yes";
							break;
						}
					}
				}
			}
			if (waterNear == "yes") {
				return true;
			}
			return new AcceptanceReport("RBB.Boardwalk".Translate());
		}
	}

	public class PlaceWorker_WatergazingSpot : PlaceWorker {
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null) {
			TerrainDef terrainDef = base.Map.terrainGrid.TerrainAt(loc);
			if (!terrainDef.defName.Contains("Water") && !terrainDef.defName.Equals("Marsh")) {
				return new AcceptanceReport("RBB.FishingSpot1".Translate());
			}
			ThingDef thingDef = checkingDef as ThingDef;
			IntVec3 intVec3 = Thing.InteractionCellWhenAt(thingDef, loc, rot, base.Map);
			if (!intVec3.InBounds(base.Map)) {
				return new AcceptanceReport("InteractionSpotOutOfBounds".Translate());
			}
			List<Thing> things = base.Map.thingGrid.ThingsListAtFast(intVec3);
			bool onPier = false;
			for (int j = 0; j < things.Count; j++) {
				if (things[j] != thingToIgnore) {
					if (things[j].def.passability == Traversability.Impassable) {
						return new AcceptanceReport("InteractionSpotBlocked".Translate(new object[] { things[j].LabelNoCount }).CapitalizeFirst());
					}
					Blueprint blueprint = things[j] as Blueprint;
					if (blueprint != null && blueprint.def.entityDefToBuild.passability == Traversability.Impassable) {
						return new AcceptanceReport("InteractionSpotWillBeBlocked".Translate(new object[] { blueprint.LabelNoCount }).CapitalizeFirst());
					}
					if (things[j].def.defName == "RBB_Bridge" || things[j].def.defName == "RBB_DeepBridge" ||things[j].def.defName == "RBB_Boardwalk"
					  || things[j].def.defName == "RBB_Bridge_Stone" || things[j].def.defName == "RBB_DeepBridge_Stone" ||things[j].def.defName == "RBB_Boardwalk_Stone") {
						onPier = true;
					}
				}
			}
			if (onPier.Equals(true)) {
				return true;
			}
			return new AcceptanceReport("RBB.FishingSpot2".Translate());
		}
	}

	public class PlaceWorker_FloorBase : PlaceWorker {
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null) {
			List<Thing> things = base.Map.thingGrid.ThingsListAtFast(loc);
			for (int k = 0; k < things.Count; k++) {
				if (things[k] != thingToIgnore) {
					if (things[k].def.defName == "RBB_Bridge" || things[k].def.defName == "RBB_DeepBridge" ||things[k].def.defName == "RBB_Boardwalk") {
						return new AcceptanceReport("RBB.NoFloorOnBridge".Translate());
					}
				}
			}
			return true;
		}
	}
	
	public class Building_Bridge : Building {
		public string TerrainTypeAtBaseCellDefAsString;
		public override void Destroy(DestroyMode mode = 0) {
			base.Map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named(this.TerrainTypeAtBaseCellDefAsString));
			base.Destroy(mode);
		}
		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look<string>(ref this.TerrainTypeAtBaseCellDefAsString, "TerrainTypeAtBaseCellDefAsString", null, false);
		}
		public override void SpawnSetup(Map map, bool flag) {
			base.SpawnSetup(map, flag);
			TerrainDef terrainDef = map.terrainGrid.TerrainAt(base.Position);
			if ((object)terrainDef == (object)TerrainDef.Named("WaterShallow")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("BridgeWaterShallow"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("WaterOceanShallow")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("BridgeWaterOceanShallow"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("WaterMovingShallow")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("BridgeWaterMovingShallow"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("WaterDeep")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("BridgeWaterDeep"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("WaterOceanDeep")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("BridgeWaterOceanDeep"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("WaterMovingDeep")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("BridgeWaterMovingDeep"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("Marsh")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("BridgeMarsh"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("Mud")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("BridgeMud"));
			}
		}
	}

	public class Building_Bridge_Stone : Building {
		public string TerrainTypeAtBaseCellDefAsString;
		public override void Destroy(DestroyMode mode = 0) {
			base.Map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named(this.TerrainTypeAtBaseCellDefAsString));
			base.Destroy(mode);
		}
		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look<string>(ref this.TerrainTypeAtBaseCellDefAsString, "TerrainTypeAtBaseCellDefAsString", null, false);
		}
		public override void SpawnSetup(Map map, bool flag) {
			base.SpawnSetup(map, flag);
			TerrainDef terrainDef = map.terrainGrid.TerrainAt(base.Position);
			if ((object)terrainDef == (object)TerrainDef.Named("WaterShallow")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("StoneBridgeWaterShallow"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("WaterOceanShallow")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("StoneBridgeWaterOceanShallow"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("WaterMovingShallow")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("StoneBridgeWaterMovingShallow"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("WaterDeep")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("StoneBridgeWaterDeep"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("WaterOceanDeep")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("StoneBridgeWaterOceanDeep"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("WaterMovingDeep")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("StoneBridgeWaterMovingDeep"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("Marsh")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("StoneBridgeMarsh"));
			}
			if ((object)terrainDef == (object)TerrainDef.Named("Mud")) {
				this.TerrainTypeAtBaseCellDefAsString = terrainDef.ToString();
				map.terrainGrid.SetTerrain(base.Position, TerrainDef.Named("StoneBridgeMud"));
			}
		}
	}

}
