using { /Verse.org/Simulation }
using { /Fortnite.com/Devices }
using { /Fortnite.com/Vehicles }
using { /Fortnite.com/Characters }
using { /Fortnite.com/Playspaces }
using { /Verse.org/Native }
using { /Verse.org/Random }
using { /UnrealEngine.com/Temporary/Diagnostics }
using { /UnrealEngine.com/Temporary/SpatialMath }
using { /UnrealEngine.com/Temporary/Curves }
using { /Verse.org/Simulation/Tags }
using { /Fortnite.com/FortPlayerUtilities }

log_Team_Manager<internal> := class(log_channel){}

Team_Manager := class(creative_device):
    Logger<private> : log = log{Channel := log_Team_Manager}

    @editable
    var PetManager : Pet_Manager = Pet_Manager{}
    @editable 
    var HUD : HUD_Mananger = HUD_Mananger{}
    
    var MaybeAgent : ?agent = false
    var MaybePlayerTeam : ?team = false
    var MaybePlayerInt : ?int = false

    var AllPlayers : []player = array{}
    var Players : []player = array{}
    var IsProcessedMap : [player]logic = map{}

    OnBegin<override>()<suspends>:void =
        set AllPlayers = GetPlayspace().GetPlayers()
        for(thePlayer : AllPlayers):
            if(set IsProcessedMap[thePlayer] = false):
       SetUpTeams()

    SetUpTeams() <suspends>: void = 
        FortTeamCollection := GetPlayspace().GetTeamCollection()
        Teams := FortTeamCollection.GetTeams()
        set AllPlayers = GetPlayspace().GetPlayers()
        if(PlayerTeam := Teams[0]):
            for(thePlayer : AllPlayers): 
                if(IsProcessedMap[thePlayer]?):
                   # Logger.Print("Already processed")
                else:
                    if(set IsProcessedMap[thePlayer] = false):

                for(tempVar1 := 0..AllPlayers.Length - 1): 
                
                    set MaybeAgent = option{thePlayer}
                    set MaybePlayerTeam = option{PlayerTeam}
                    set MaybePlayerInt = option{tempVar1}

                    if(tempVar := thePlayer.GetFortCharacter[]):
                        if(IsProcessedMap[thePlayer] = false):
                            if(set IsProcessedMap[thePlayer] = true):
                            tempVar.EliminatedEvent().Subscribe(PetManager.OnPlayerEliminated)      
                            spawn:
                                AsyncHandleNewPet(thePlayer, tempVar, MaybeAgent, MaybePlayerTeam)       
        CheckForNewPlayers()

    AsyncHandleNewPet(thePlayer : agent, FortCharacter : fort_character, PlayerAgent : ?agent, PlayerTeam : ?team)<suspends>:void=
        branch:                     
            PetManager.HandleSetup(thePlayer) 
            PetManager.DoFollowPlayer(FortCharacter, MaybePlayerInt, PlayerAgent, PlayerTeam)
                        

    CheckForNewPlayers() <suspends>: void =
        EveryPlayer := GetPlayspace().GetPlayers()
        CheckTime := 10.0
        
        if(EveryPlayer.Length = AllPlayers.Length):
            # Logger.Print("No new players")
        else:
            Logger.Print("A new player joined!")
            set AllPlayers = EveryPlayer
            SetUpTeams()
            return

        Sleep(CheckTime)
        CheckForNewPlayers()
