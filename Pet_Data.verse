using { /Fortnite.com/Devices }
using { /Fortnite.com/Characters }
using { /Verse.org/Simulation }
using { /Verse.org/Random }
using { /Verse.org/Assets }
using { /UnrealEngine.com/Temporary/Diagnostics }
using { /UnrealEngine.com/Temporary/SpatialMath }

log_pet_data<internal> := class(log_channel){}
log_pet<internal> := class(log_channel){}

#Pet class includes all functions of the pet 
Pet := class():

#region Variables
    Logger<private> : log = log{Channel := log_pet}

    var ID : PetID = PetID.UnicornExotic
    var PetRarity : Rarity = Rarity.Common

    var UIManager : UI_Manager = UI_Manager{}

    var PetManager : Pet_Manager = Pet_Manager{}
    var MaybePlayer : ?agent = false

    var Following : logic = false
    var EggActive : logic = true
    var FirstPass : logic = true

    var FoodGauge : float = 100.0
    var WaterGauge : float = 100.0
    var CleanGauge : float = 100.0
    var SleepGauge : float = 100.0
#endregion

#region Pet Needs
        PerscribePetNeeds(Player: agent)<suspends>: void =    
            #HudManager.GrantPlayerHUD(Result)
            sync:
                TryLowerFood(Player, 6.0)
                TryLowerWater(Player, 5.0)
                TryLowerClean(Player, 8.0)
                TryLowerSleep(Player, 7.0)
                Logger.Print("Needs Perscribed")

        TryLowerFood(Player: agent, Rate : float)<suspends>: void=
            Sleep(Rate)
            #Print("LowerFood: {FoodGauge}")
            set FoodGauge -= 1.0
            if:
                temp := Floor[FoodGauge]
            then:
                if(temp > 0):
                    UIManager.SetPetFoodValue(Player, "{temp}")

            TryLowerFood(Player, Rate)

        TryLowerWater(Player: agent, Rate : float)<suspends>: void=
            Sleep(Rate)
            set WaterGauge -= 1.0
            if:
                temp := Floor[WaterGauge]
            then:
                if(temp > 0):
                    UIManager.SetPetWaterValue(Player, "{temp}")

            TryLowerWater(Player, Rate)

        TryLowerClean(Player: agent, Rate : float)<suspends>: void=
            Sleep(Rate)
            set CleanGauge -= 1.0
            #UIManager.SetPetCleanValue(Player, "{CleanGauge}")

            TryLowerClean(Player, Rate) 

        TryLowerSleep(Player: agent, Rate : float)<suspends>: void=
            Sleep(Rate)
            set SleepGauge -= 1.0
            if:
                temp := Floor[SleepGauge]
            then:
                if(temp > 0):
                    UIManager.SetPetCleanValue(Player, "{temp}")
            
            TryLowerSleep(Player, Rate) 

        TryResetFood(Player: agent): void=
            set FoodGauge = 100.0
            UIManager.SetPetFoodValue(Player, "{FoodGauge}")

        TryResetWater(Player: agent): void=
            set WaterGauge = 100.0
            UIManager.SetPetWaterValue(Player, "{WaterGauge}")

        TryResetClean(Player: agent): void=
            set CleanGauge = 100.0
            UIManager.SetPetCleanValue(Player, "{CleanGauge}")

        TryResetSleep(Player: agent): void=
            set SleepGauge = 100.0
           # UIManager.SetPetSleepValue(Player, "{SleepGauge}")
#endregion    

#region Sequencing
    (Agent:agent).ReturnSequence(theRarity : Rarity)<transacts>:cinematic_sequence_device =
        var SelectedSequenceDevice : cinematic_sequence_device = cinematic_sequence_device{}
            
        if(theRarity = Rarity.Common):
            if(set SelectedSequenceDevice = PetManager.SequenceDevices[0]):
        else if(theRarity = Rarity.Uncommon):
            if(set SelectedSequenceDevice = PetManager.SequenceDevices[1]):
        else if(theRarity = Rarity.Rare):
            if(set SelectedSequenceDevice = PetManager.SequenceDevices[2]):
        else if(theRarity = Rarity.Epic):
            if(set SelectedSequenceDevice = PetManager.SequenceDevices[3]):     

        return SelectedSequenceDevice

    (Agent:agent).PlaySequence(theRarity : Rarity)<suspends>:void =
        Sequence := Agent.ReturnSequence(theRarity)
            Sequence.Play(Agent)
            Sequence.StoppedEvent.Await()
            #set PetManager.SequenceDevices += array{Sequence}

    #Function call to play verse triggered sequence to 1 specified player
    ShowToOne(theRarity : Rarity):void =
        if (Agent := MaybePlayer?):
            spawn{Agent.PlaySequence(theRarity)}
#endregion

#region Pet Movement
    #Begins DoFollowPlayer function in PetManager
    StartFollowPlayer(Agent : agent, EggProp : ?creative_prop)<suspends>:void=            
        Logger.Print("INIT StartFollowPlayer")

        var MaybeFollowPlayer : ?agent = false            
        set MaybeFollowPlayer = option{Agent}

        if(set PetManager.FollowingPet[Agent] = true):
        if(set PetManager.PetCheckMap[Agent] = true):

        spawn:
            PetManager.DoFollowPlayer(MaybeFollowPlayer, EggProp)  

    #Sequence origin movement for pinning sequence to valid pet at runtime
    FollowPet(PetProp : creative_prop, TheRarity : Rarity, thePlayer : agent)<suspends>:void=
        if(Following?):
            #var VFXPosition := vector3{X := PetProp.GetTransform().Translation.X, Y := PetProp.GetTransform().Translation.Y, Z := PetProp.GetTransform().Translation.Z + 200.0}
            if(VFXVector := PetManager.PetPosition[thePlayer]):
                VFXPosition := vector3{X := VFXVector.X, Y := VFXVector.Y, Z := VFXVector.Z + 200.0}

                if(TheRarity = Rarity.Common, FirstPass?):  
                    PetManager.CommonVFX.Disable()   
                    Sleep(0.1)           
                    PetManager.CommonVFX.Enable()
                else if(TheRarity = Rarity.Uncommon, FirstPass?):
                    PetManager.UncommonVFX.Disable()   
                    Sleep(0.1)    
                    PetManager.UncommonVFX.Enable()
                else if(TheRarity = Rarity.Rare, FirstPass?):
                    PetManager.RareVFX.Disable()   
                    Sleep(0.1)    
                    PetManager.RareVFX.Enable()
                else if(TheRarity = Rarity.Epic, FirstPass?):
                    PetManager.EpicVFX.Disable()   
                    Sleep(0.1)    
                    PetManager.EpicVFX.Enable()

                if(FirstPass?):
                    ShowToOne(TheRarity)

                set FirstPass = false

                if(TheRarity = Rarity.Common):
                    PetManager.CommonVFX.MoveTo(VFXPosition, PetProp.GetTransform().Rotation, 0.1)
                else if(TheRarity = Rarity.Uncommon):
                    PetManager.UncommonVFX.MoveTo(VFXPosition, PetProp.GetTransform().Rotation, 0.1)
                else if(TheRarity = Rarity.Rare):
                    PetManager.RareVFX.MoveTo(VFXPosition, PetProp.GetTransform().Rotation, 0.1)
                else if(TheRarity = Rarity.Epic): 
                    PetManager.EpicVFX.MoveTo(VFXPosition, PetProp.GetTransform().Rotation, 0.1)

            Sleep(0.2)
            FollowPet(PetProp, TheRarity, thePlayer)
#endregion
        
#region Pet Age
    WaitForHatch(Agent : agent, TheRarity : Rarity, PetProp : creative_prop, EggProp : creative_prop)<suspends>: void=
        set EggActive = true
        Logger.Print("Waiting for Hatch")

        Sleep(100.0)
        Logger.Print("Hatch timer success")

        set Following = true
        if(FirstPass?):
            spawn{FollowPet(PetProp, TheRarity, Agent)}

        Sleep(3.0)
        EggProp.Hide()
        PetProp.Show()
        UIManager.CreateUIForPlayer(Agent)
        PerscribePetNeeds(Agent)
        UIManager.CreateUIForPlayer(Agent)
        Sleep(2.0)
        Logger.Print("Sequence played")

        set Following = false
        set EggActive = false
        set FirstPass = true
#endregion

Pet_Data := class(creative_device):

#region Variables
    Logger<private> : log = log{Channel := log_pet_data}

    @editable
    PetMeshAsset : creative_prop_asset = DefaultCreativePropAsset
    @editable
    var ID : PetID = PetID.UnicornExotic
    @editable
    PetManager : Pet_Manager = Pet_Manager{}
    @editable
    UIManager : UI_Manager = UI_Manager{}
#endregion

    SpawnPet(thePlayer : agent, Chance : Rarity, RarityMultiplier : int, ThisPetData : Pet_Data, PetEgg : creative_prop)<transacts>:Pet=
        var tempArray : []creative_prop_asset = array{}
        set tempArray = tempArray + array{PetMeshAsset}

        SpawnedPet := Pet{}
        set SpawnedPet.ID = ThisPetData.ID
        set SpawnedPet.PetRarity = Chance
        set SpawnedPet.MaybePlayer = option{thePlayer}
        set SpawnedPet.PetManager = PetManager
        set SpawnedPet.UIManager  = UI_Manager{}

        return SpawnedPet