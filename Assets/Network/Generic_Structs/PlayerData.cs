using UnityEngine;
using System.Collections;

public class PlayerInfo {
	
	public int playerListIndex = -1;
	public NetworkPlayer networkPlayer;
	
	// base player data struct
	public PlayerIDData playeridData = new PlayerIDData();
	
	
	// TODO
	// information about the parasite object
	
	
	// current class data
		
	
	// vehicle data
	public VehicleData vehicleInfo = new VehicleData();
	
	// loadout data
		
	
	// armour data
	
	
}

public enum Team {
// TODO
// fill this in
	none
}

// no need for references here, this is raw data
public struct PlayerIDData {
	public string playerName;
	public Team team;
	public NetworkViewID netViewID;
}

public struct VehicleData {
	// vehicle class
	// vehicle hp
	// vehicle class upgrade
}

public struct LoadoutData {
	// primary weapon
	// secondary weapon
}

public struct ArmourData {
	// primary colour
	// secondary colour
}
