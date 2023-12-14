using { /Fortnite.com/Characters }
using { /Fortnite.com/Devices }
using { /Fortnite.com/Game }
using { /Fortnite.com/Teams }
using { /Verse.org/Simulation }
using { /UnrealEngine.com/Temporary/SpatialMath }

# This is a type alias!
player_map := [player]int 

log_spotlight<internal> := class(log_channel){}

# Tracks all players in the level with a spotlight positioned above them
Spotlight_Players := class(creative_device):
    Logger<private> : log = log{Channel := log_spotlight}

    @editable
    var TrackPlayers : logic = true
    @editable
    var PlayerSpawners : []player_spawner_device = array{}
    @editable
    EndGameDevice : end_game_device = end_game_device{}
    @editable
    var Lights : []creative_prop = array{}

    var Teams : []team = array{}

    # Map of Team Maps, where the key is the team and the value is a map of
    # player->int key-value pairs
    var TeamMap : [team]player_map = map{}  

    OnBegin<override>()<suspends>:void=
       # PopulateTeamsAndPlayers()

        # Subscribe to each player spawn pad
        for (Spawner : PlayerSpawners):
            Spawner.SpawnedEvent.Subscribe(OnPlayerSpawn)
        DoFollowPlayer()

    PopulateTeamsAndPlayers() : void =
        Logger.Print("Beginning to populate players")
        for (Team : Teams, TeamPlayers := GetPlayspace().GetTeamCollection().GetAgents[Team]):
            var PlayerMap : player_map = map {}
            for (Agent : TeamPlayers, TeamPlayer := player[Agent], FortCharacter := Agent.GetFortCharacter[]): 
                if(set PlayerMap[TeamPlayer] = 0, WeaponTier := PlayerMap[TeamPlayer]): 
                    Logger.Print("Assigned Player to PlayerMap with Tier {WeaponTier}")
                FortCharacter.EliminatedEvent().Subscribe(OnPlayerEliminated) 
            if(set TeamMap[Team] = PlayerMap):
                Logger.Print("Successfully set this team in the TeamMap") 

    # Places a light above each player on spawn
    OnPlayerSpawn(InPlayer : agent): void =
        Logger.Print("A player just spawned!")
        #DoFollowPlayer()

    OnPlayerEliminated(Result : elimination_result) : void =
        Logger.Print("A Player was eliminated!")
        Eliminator := Result.EliminatedCharacter
         if (FortCharacter := Eliminator, EliminatorAgent := FortCharacter.GetAgent[]):
           # GiveNextWeapon(EliminatorAgent)

    DoFollowPlayer()<suspends>  : void =
        loop:
            if(TrackPlayers?):
                PlayerList := GetPlayspace().GetPlayers()
                for(Players := 0..PlayerList.Length - 1, FortniteCharacter := PlayerList[Players].GetFortCharacter[]):
                    PlayerPosition : vector3 = FortniteCharacter.GetTransform().Translation
                    if(tempVar := Lights[Players]):
                        tempVar.MoveTo(PlayerPosition, rotation{}, 0.2)
            else:
                break
                