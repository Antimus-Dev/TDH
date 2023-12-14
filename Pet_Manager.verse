using { /Fortnite.com/Characters }
using { /Fortnite.com/Devices }
using { /Fortnite.com/Game }
using { /Fortnite.com/Teams }
using { /Verse.org/Simulation }
using { /Verse.org/Random }
using { /UnrealEngine.com/Temporary/SpatialMath }
using { /UnrealEngine.com/Temporary/Diagnostics }

#Enums
PetID := enum{UnicornExotic, DragonExotic, GryphonExotic, ParrotBird, ChickenBird, PenguinBird, DalmationDog, ShibaDog, PugDog, BlackCat, CalicoCat, TabbyCat}
Rarity := enum{Common, Uncommon, Rare, Epic}

#This makes the NewPets and UI folders in Assets.digest.verse public allowing runtime access
Assets := module:
    NewPets<public> := module {}
    UI<public> := module{}

log_pet_manager<internal> := class(log_channel){}

Pet_Manager := class(creative_device):

#region Variables
    Logger<private> : log = log{Channel := log_pet_manager}

    var RegisteredPet : ?Pet = false
    var MaybePlayerInt: ?int = false  
    var MaybeEggProp : [agent]?creative_prop = map{}

    var SittingPet : [agent]logic = map{}
    var FollowingPet : [agent]logic = map{}
    var RollOverPet : [agent]logic = map{}
    var PetPet : [agent]logic = map{}
    var BattlePet : [agent]logic = map{}

    # PlayerPropMap is a collection of each player and all creative props are that specific agent's current pets
    var PlayerPropMap : [agent] []creative_prop = map{} 
    var PetOwnerMap : [agent][]Pet = map{}
    var AgentIndexMap : [agent]int = map{} 
    var PetCheckMap : [agent]logic = map{}
    var HowManyPets : [agent]int = map{}

    #related to above but from old code
    var PlayerPosition : [agent]vector3 = map{}
    var PlayerRotation : [agent]rotation = map{}
    var PetPosition : [agent]vector3 = map{}
    var PetOffset : vector3 = vector3{}
    var ExtraPetsOffset :vector3 = vector3{}

    var time : float = 5.0
    var Moving : logic = false
    var DistanceToPlayer : float = 150.0
    var AllPlayers : []agent = array{}

    @editable
    var Animator : DH_Animator = DH_Animator{}
    @editable
    var AttributeEvaluators :[]attribute_evaluator_device = array{}
    @editable
    var AllDistributors : []Pet_Distribution = array{}
    @editable
    var SequenceDevices:[]cinematic_sequence_device = array{}

    @editable
    var Hotbar1 : signal_remote_manager_device = signal_remote_manager_device{}
    @editable
    var Hotbar2 : signal_remote_manager_device = signal_remote_manager_device{}
    @editable
    var Hotbar3 : signal_remote_manager_device = signal_remote_manager_device{}
    @editable
    var Hotbar4 : signal_remote_manager_device = signal_remote_manager_device{}
    @editable
    var Hotbar5 : signal_remote_manager_device = signal_remote_manager_device{}

    @editable 
    FoodButton : []button_device = array{}
    @editable 
    WaterButton : []button_device = array{}
    @editable 
    ShowerButton : []button_device = array{}
    @editable 
    SleepButton : []button_device = array{}

    @editable
    CommonVFX : vfx_spawner_device = vfx_spawner_device{}
    @editable
    UncommonVFX : vfx_spawner_device = vfx_spawner_device{}
    @editable
    RareVFX : vfx_spawner_device = vfx_spawner_device{}
    @editable
    EpicVFX : vfx_spawner_device = vfx_spawner_device{}
    @editable
    var PetLoveVFX : vfx_creator_device = vfx_creator_device{}
    @editable
    var PetSadVFX : vfx_creator_device = vfx_creator_device{}
#endregion

#region OnBegin()
    OnBegin<override>()<suspends>:void=
        GetPlayspace().PlayerAddedEvent().Subscribe(HandleSetup)

        CommonVFX.Disable()
        UncommonVFX.Disable()
        RareVFX.Disable()
        EpicVFX.Disable()

        Hotbar1.PrimarySignalEvent.Subscribe(HandleInput1)
        Hotbar2.PrimarySignalEvent.Subscribe(HandleInput2)
        Hotbar3.PrimarySignalEvent.Subscribe(HandleInput3)
        Hotbar4.PrimarySignalEvent.Subscribe(HandleInput4)
        Hotbar5.PrimarySignalEvent.Subscribe(HandleInput5)

        set AllPlayers = GetPlayspace().GetPlayers()
        for (thePlayer : AllPlayers): 

            if(set PetCheckMap[thePlayer] = false):
            if(set HowManyPets[thePlayer] = 0):

            if(set SittingPet[thePlayer] = false):
            if(set FollowingPet[thePlayer] = true):
            if(set RollOverPet[thePlayer] = false):
            if(set PetPet[thePlayer] = false):
            if(set BattlePet[thePlayer] = false):
            
#endregion

#region Pet Movement
    DoFollowPlayer(Player : ?agent, EggProp : ?creative_prop)<suspends>:void= 
        if:
            thePlayer := Player?
            FollowingPet[thePlayer]?
            FortCharacter := thePlayer.GetFortCharacter[]
        then:             
            var ClassIndex : int = GetPlayersClassIndex(thePlayer)
            set ClassIndex = GetPlayersClassIndex(thePlayer)
            set MaybePlayerInt = option{ClassIndex}
            if(set MaybeEggProp[thePlayer] = EggProp):

            if(set PlayerPosition[thePlayer] = FortCharacter.GetTransform().Translation, set PlayerRotation[thePlayer] = FortCharacter.GetTransform().Rotation):      

            # Pet positioning math to always be behind the player on follow

            if(PlayerYaw := PlayerRotation[thePlayer].GetYawPitchRollDegrees()[0]):
                set PetOffset.X =  DistanceToPlayer * Cos(0.02 * PlayerYaw)
                set PetOffset.Y =  DistanceToPlayer * Sin(0.02 * PlayerYaw) 
                set PetOffset.Z = 20.0

                if(set PetPosition[thePlayer] = PlayerPosition[thePlayer] - PetOffset):      
            
                if:
                    set AgentIndexMap[thePlayer] = ClassIndex
                    PetCheckMap[thePlayer]?
                    PlayerOrder := MaybePlayerInt?
                    ClassIndex = PlayerOrder
                    ThisPlayerPetPosition := PetPosition[thePlayer]
                    ThisPlayerRotation := PlayerRotation[thePlayer]
                    PetToMove := PlayerPropMap[thePlayer][0]
                then:  
                    PetToMove.MoveTo(ThisPlayerPetPosition, ThisPlayerRotation, time / 200.0)
                    if(Egg := EggProp?):
                        Egg.MoveTo(ThisPlayerPetPosition, ThisPlayerRotation, time / 200.0)

                    if(PlayerPropMap[thePlayer][1], Moving = false):
                        spawn:
                            DoExtraPetFollow(thePlayer, PlayerYaw, ThisPlayerRotation)

                    Sleep(time / 200.0)
                    DoFollowPlayer(Player, EggProp)  
                    return
            else:
                Logger.Print("Follow failed")
                Sleep(time / 200.0)
                DoFollowPlayer(Player, EggProp)     
        
    DoExtraPetFollow(thePlayer : agent, ypr : float, ThisPlayerRotation : rotation)<suspends>:void=
        set Moving = true
        for(thePet := 0..HowManyPets[thePlayer] - 1):
            if(ExtraPet := PlayerPropMap[thePlayer][thePet + 1], ThisPlayerPetPosition := PetPosition[thePlayer]):
                set ExtraPetsOffset = vector3{X:= ThisPlayerPetPosition.X - (PetOffset.X * (thePet + 1)), Y:= ThisPlayerPetPosition.Y - (PetOffset.Y * (thePet + 1)), Z:= ThisPlayerPetPosition.Z}
                spawn:
                    ExtraPet.MoveTo(ExtraPetsOffset, ThisPlayerRotation, time)  
                Sleep(time / 200.0)
        set Moving = false      

    <#Modulo(dividend : float, divisor : float):float=
            var modulo : float = 0.0
            if(divisor = 0.0):           
                return modulo
               
            var quotient : float = dividend / divisor;
            set modulo = dividend - (divisor * quotient)
    
            return modulo;#>
        

    # Function to return the required position 
   <# GetCircumferenceInversePosition(NumberOfPoints : float, PointToInvert : float):int= 
        { 
            if (PointToInvert > (NumberOfPoints / 2)):
                InversePoint := PointToInvert - (NumberOfPoints / 2)
                return InversePoint
                
            InversePoint := PointToInvert + (NumberOfPoints / 2)
            return InversePoint
        } 
            
 
            set NumberOfPoints = 4
            set PointToInvert = 5
            Console.WriteLine(); #>
            
#endregion

#region GetClassIndex
    GetPlayersClassIndex(Agent : agent)<suspends>: int=
        ClassIndex:int = race:           
            EvaluateClass(Agent,0)
            EvaluateClass(Agent,1)
            EvaluateClass(Agent,2)
            EvaluateClass(Agent,3)
            EvaluateClass(Agent,4)
                    
        # This Print statement below is supposed to print the current class of the player, but instead 
        # prints the previous interaction with the UI. For example, if you clicked a UI button while in class 1,
        # no print message would show up on the log. If you then switched to class 2 and clicked a UI button again,
        # it would display that you are class 1. Current wokraround is to call this function twice whenever you want a result.
                                        
        # Print("Your Selected Class is {ClassIndex + 1}")
        return ClassIndex + 1
                                        
    EvaluateClass(Agent : agent, InCheckerIndex : int)<suspends>:int=
        if(ThisClassChecker := AttributeEvaluators[InCheckerIndex]):
            ThisClassChecker.EvaluateAgent(Agent)
            ThisClassChecker.PassEvent.Await()
            return InCheckerIndex 
        Sleep(10.0) <# give other threads time to succeed #>
        return -1    
#endregion

#region Inputs
    HandleInput1(Agent : agent):void=
        #if(PetOwner := PetOwnerMap[Agent][0]):
            # Logger.Print("Pet sit")
            if(set SittingPet[Agent] = true):
            if(set FollowingPet[Agent] = false):
            if(set PetPet[Agent] = false):
            if(set RollOverPet[Agent] = false):
            if(set BattlePet[Agent] = false):   
                
            if(tempVar := AgentIndexMap[Agent]):
                Logger.Print("Player {tempVar} sit command!")

    HandleInput2(Agent : agent):void=
        #if(PetOwner := PetOwnerMap[Agent][0]):
            # Logger.Print("Pet follow")
            if(set SittingPet[Agent] = false):
            if(set FollowingPet[Agent] = true):
            if(set PetPet[Agent] = false):
            if(set RollOverPet[Agent] = false):
            if(set BattlePet[Agent] = false):   

            #if(tempVar := RegisteredPet?):
            if(temp := MaybeEggProp[Agent]):
                spawn{DoFollowPlayer(option{Agent}, temp)}

            if(tempVar := AgentIndexMap[Agent]):
                Logger.Print("Player {tempVar} follow command!")

    HandleInput3(Agent : agent):void=
        #if(PetOwner := PetOwnerMap[Agent][0]):
            # Logger.Print("Pet pet")

            if(set PetPet[Agent] = true):
            if(set RollOverPet[Agent] = false):
            if(set BattlePet[Agent] = false):  
            spawn{SpawnLoveVFX(Agent)}

            if(tempVar := AgentIndexMap[Agent]):
                Logger.Print("Player {tempVar} Pet their pet!")

    HandleInput4(Agent : agent):void=
        #if(PetOwner := PetOwnerMap[Agent][0]):
            # Logger.Print("Pet roll over")

            if(set PetPet[Agent] = false):
            if(set RollOverPet[Agent] = true):  
            if(set BattlePet[Agent] = false):     
            spawn{PetRollOver(Agent)}

            if(tempVar := AgentIndexMap[Agent]):
                Logger.Print("Player {tempVar} rolled over!")

    HandleInput5(Agent : agent):void=
        #if(PetOwner := PetOwnerMap[Agent][0]):
            # Logger.Print("Pet battle!")

            if(set PetPet[Agent] = false):
            if(set RollOverPet[Agent] = false):
            if(set BattlePet[Agent] = true):  
       
    PetRollOver(Agent : agent)<suspends>:void=
        if(PetOwner := PlayerPropMap[Agent][0]):
            set Animator.PropToAnimate = PetOwner
            Animator.Animate()

    SpawnLoveVFX(Agent : agent)<suspends>:void=
            if(PlayerPetPosition := PetPosition[Agent], ThePlayerRotation := PlayerRotation[Agent]):
                branch{ PetLoveVFX.MoveTo(PlayerPetPosition, ThePlayerRotation, time)}   
                PetLoveVFX.Begin()

    SpawnSadVFX(Agent : agent)<suspends>:void=
            if(PlayerPetPosition := PetPosition[Agent], ThePlayerRotation := PlayerRotation[Agent]):
                branch{ PetSadVFX.MoveTo(PlayerPetPosition, ThePlayerRotation, time)}   
                PetSadVFX.Begin()
#endregion

#region Registration
    RegisterPet(ThePet : ?Pet) : void =
        set RegisteredPet = ThePet
#endregion

#region Player Pop Management
    HandleSetup(thePlayer : player):void=
        if(set PetCheckMap[thePlayer] = false):
        if(set HowManyPets[thePlayer] = 0):

        if(set SittingPet[thePlayer] = false):
        if(set FollowingPet[thePlayer] = true):
        if(set RollOverPet[thePlayer] = false):
        if(set PetPet[thePlayer] = false):
        if(set BattlePet[thePlayer] = false):           
#endregion