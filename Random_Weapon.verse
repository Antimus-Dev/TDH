using { /Fortnite.com/Devices }
using { /Verse.org/Verse }
using { /Verse.org/Simulation }
using { /Verse.org/Random }
using { /UnrealEngine.com/Temporary/Diagnostics }

log_random_weapon<internal> := class(log_channel){}

#Grants a random weapon from an array of weapons at each tower
Random_Weapon := class(creative_device):
    Logger<private> : log = log{Channel := log_random_weapon}
    @editable
    var ItemRemoversList : []item_remover_device = array{}
    @editable
    var ItemsList : []item_granter_device = array{}
    @editable
    var RoundDevice : round_settings_device = round_settings_device{}

    var chosenItems : []item_granter_device = array{}
    var chosenRemovers : []item_remover_device = array{}

    # Runs when the device is started in a running game
    OnBegin<override>()<suspends>:void=
        InitDevices()
        
        RoundDevice.RoundBeginEvent.Subscribe(InitChosenItems)

    InitChosenItems() : void =
        var ItemsListCopy : []item_granter_device = ItemsList
        var ItemRemoverCopy : []item_remover_device = ItemRemoversList
        for(Val := 0..6):
            RandomNum:int = GetRandomInt(0, ItemsListCopy.Length-1)

            if:
                #set Array2 = Array1 + Array2 + array{30, 31}
                set chosenItems = chosenItems + array{ItemsListCopy[RandomNum]}
                set chosenRemovers = chosenRemovers + array{ItemRemoverCopy[RandomNum]}
            then:
                Logger.Print("ARRAY POPULATED: {RandomNum}")
                #only want to remove item if it was assigned
                if:
                    temp := ItemsListCopy.RemoveElement[RandomNum]
                    temp1 := ItemRemoverCopy.RemoveElement[RandomNum]
                then:
                    set ItemsListCopy = temp
                    set ItemRemoverCopy = temp1
            else:
                Logger.Print("NOT POPULATED: {RandomNum}")

    InitDevices() : void=
        for(Val := 0..29):
            if:
                tempVar := ItemRemoversList[Val]
            then:
                 tempVar.Disable

    OnGrantingPlayer(InPlayer : player, ID : int) : void = 
        if:
            tempVar := chosenItems[ID]
        
        then:
            Logger.Print("GRANTING ITEM: {ID}")
            tempVar.GrantItem(InPlayer)
        else:
            Logger.Print("FAILED TO GRANT ITEM: {ID}") 

    OnRemovingPlayer(OutPlayer : player, ID : int) : void =
        if:
            Out := agent[OutPlayer]
            tempVar := chosenRemovers[ID]

        then:
            Logger.Print("REMOVING ITEM : {ID}")
            tempVar.Enable()
            tempVar.Remove(Out);
            tempVar.Disable()
        else: 
            Logger.Print("FAILED TO REMOVE ITEM: {ID}")