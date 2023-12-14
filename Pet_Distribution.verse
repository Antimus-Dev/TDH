using { /Fortnite.com/Devices }
using { /Fortnite.com/Characters }
using { /Verse.org/Simulation }
using { /Verse.org/Random }
using { /Verse.org/Assets }
using { /UnrealEngine.com/Temporary/Diagnostics }
using { /UnrealEngine.com/Temporary/SpatialMath }

log_pet_distribution<internal> := class(log_channel){}

Pet_Distribution := class(creative_device):
    Logger<private> : log = log{Channel := log_pet_distribution}

    var CommonPassive<public> : int = 1
    var UncommonPassive<public> : int = 2
    var RarePassive<public> : int = 3
    var EpicPassive<public> : int = 4
    var PassiveToAssign : int = 0 

    var ChosenPet : Pet_Data = Pet_Data{}
    var BuyingLockout : [agent]logic = map{}
    var FirstPass : logic = true

    @editable
    var PetManager : Pet_Manager = Pet_Manager{}
    @editable
    PetAcquisitionButton : conditional_button_device = conditional_button_device{}
    @editable
    Pets: []Pet_Data = array{}
    @editable
    Egg: creative_prop_asset = DefaultCreativePropAsset
    @editable
    Cost : int = 100

    @editable
    tManager : Tycoon_Manager_v2 = Tycoon_Manager_v2{}


    OnBegin<override>()<suspends>:void=
        PetAcquisitionButton.ActivatedEvent.Subscribe(StartDistributePet)
        AllPlayers := GetPlayspace().GetPlayers()
        for(Player : AllPlayers):
            if(set BuyingLockout[Player] = false):

    StartDistributePet(thePlayer: agent):void=
        spawn:
            DistributePet(thePlayer)

    DistributePet(thePlayer: agent)<suspends>:void=       
        if(PetManager.AllDistributors[0].BuyingLockout[thePlayer] = false):
            if(set PetManager.AllDistributors[0].BuyingLockout[thePlayer] = true):

            Logger.Print("Distributing Pet!")
            var MaybeDistributionPlayer : ?agent = false
            var MaybeDistributionTeam : ?team = false

            set MaybeDistributionPlayer = option{thePlayer}
            if(set PetManager.PetCheckMap[thePlayer] = true, set PetManager.FollowingPet[thePlayer] = true):
        
            WhichPet := GetRandomInt(0, 2)
            CurrentPetRarity := GetWeightedRarity()

            if(FortCharacter := thePlayer.GetFortCharacter[], PetPropResult := SpawnProp(Pets[WhichPet].PetMeshAsset,FortCharacter.GetTransform().Translation, FortCharacter.GetTransform().Rotation)):
                EggPropResult := SpawnProp(Egg,FortCharacter.GetTransform().Translation, FortCharacter.GetTransform().Rotation)

                #Grab spawned prop instance as a creative_prop
                if(PetProp := PetPropResult(0)?, EggProp := EggPropResult(0)?): #, PetManager.HowManyPets[thePlayer] < 12):
                    PetProp.Hide()
                    EggProp.Show()

                    #Set instantiated prop material
                    if(CurrentPetRarity = Rarity.Common):      
                        PetProp.SetMaterial(Assets.NewPets.DH_MAT_CubeAnimals_PLAIN)
                        set PassiveToAssign = CommonPassive                        
                    else if(CurrentPetRarity = Rarity.Uncommon):
                        PetProp.SetMaterial(Assets.NewPets.DH_MAT_CubeAnimals_GREEN)
                        set PassiveToAssign = UncommonPassive
                    else if(CurrentPetRarity = Rarity.Rare):
                        PetProp.SetMaterial(Assets.NewPets.DH_MAT_CubeAnimals_BLUE)
                        set PassiveToAssign = RarePassive
                    else if(CurrentPetRarity = Rarity.Epic):
                        PetProp.SetMaterial(Assets.NewPets.DH_MAT_CubeAnimals_GOLD)
                        set PassiveToAssign = EpicPassive
                
                    if(set ChosenPet = Pets[WhichPet]):

                    if(temp := PetManager.PlayerPropMap[thePlayer]):
                        temp2 := array{PetProp} + temp 
                        if(set PetManager.PlayerPropMap[thePlayer] = temp2){}                  
                    else:
                        Logger.Print("First PlayerPropMap populate")
                        if(set PetManager.PlayerPropMap[thePlayer] = array{PetProp}){}

                    ThePet := ChosenPet.SpawnPet(thePlayer, CurrentPetRarity, PassiveToAssign, ChosenPet, EggProp)

                    if(set PetManager.PetOwnerMap[thePlayer]):
                        if(temp := PetManager.PetOwnerMap[thePlayer]):
                            temp2 := array{ThePet} + temp
                            if(set PetManager.PetOwnerMap[thePlayer] = temp2){}                 
                        else:
                            Logger.Print("First PetOwnerMap populate")
                            if(set PetManager.PetOwnerMap[thePlayer] = array{ThePet}){}

                    if(set PetManager.HowManyPets[thePlayer] = PetManager.HowManyPets[thePlayer] + 1):                    
                    if(set PetManager.PetOwnerMap[thePlayer][0].EggActive = true):

                    #if(FirstPass?):
                       # set FirstPass = false

                    if(thisPlayer := player[thePlayer]):
                        MaybeManager := tManager.GetAgentsHouse(thisPlayer)
                        if:
                            hManager := MaybeManager?
                        then:
                            hManager.SetPassiveGoldAmount(PassiveToAssign)
                            #Pass an amount to increase. Increase by 3 - pass 3 into this function
                            hManager.SetPassiveSecondsBetweenGrant(0.8)
                            #Pass percentage to drop .5, .25, etc

                    if(PetToPerscribe := PetManager.PetOwnerMap[thePlayer][0]):
                        for(button := 0..PetManager.FoodButton.Length - 1):
                            if(PetManager.FoodButton[button].InteractedWithEvent.Subscribe(PetToPerscribe.TryResetFood)):
                            if(PetManager.WaterButton[button].InteractedWithEvent.Subscribe(PetToPerscribe.TryResetWater)):
                            #if(PetManager.ShowerButton[button].InteractedWithEvent.Subscribe(PetToPerscribe.TryResetClean)):
                            if(PetManager.SleepButton[button].InteractedWithEvent.Subscribe(PetToPerscribe.TryResetClean)): #Change to TryResetSleep when sleep implemented

                    Logger.Print("Pet trying first follow")
                    spawn:
                        ThePet.StartFollowPlayer(thePlayer, option{EggProp})

                    if(FirstPet := PetManager.PlayerPropMap[thePlayer][0]):
                        spawn:
                            ThePet.WaitForHatch(thePlayer, CurrentPetRarity, FirstPet, EggProp)
                
                Sleep(55.0)
                if(set PetManager.AllDistributors[0].BuyingLockout[thePlayer] = false):                 
        else:
            Logger.Print("Lockout in effect")
        

#region Weighted Rarity System        
    GetWeightedRarity(): Rarity =

        # Define the items drop chances (This doesn't need to equal any specific number but each number represents it's percentage of the total)
        DropChance :[]int= array{4,3,2,1}
        var AddUp : int = 0

        var ChanceToRarity : [int]Rarity = map{}
        if(set ChanceToRarity[0] = Rarity.Common):
        if(set ChanceToRarity[1] = Rarity.Uncommon):
        if(set ChanceToRarity[2] = Rarity.Rare):
        if(set ChanceToRarity[3] = Rarity.Epic):

        var totalChance :int = 0;

        # Calculate the total drop chance           
        for(Chance : DropChance):
            set totalChance += Chance

        RandomNumber := GetRandomInt(0, totalChance)
                    
        # Determine which rarity to grant based on the random number
        for(Chance := 0..DropChance.Length - 1):
            if(RandomNumber <= DropChance[Chance] + AddUp):
                if(TheRarity := ChanceToRarity[Chance]):
                    Logger.Print("Weighted Rarity success")
                    return TheRarity
            else:
                if(set AddUp = AddUp + DropChance[Chance]):
                    
        # This should never happen if the chances are configured correctly
        Logger.Print("GetWeightedRarity() Bad")
        return Rarity.Uncommon
#endregion
