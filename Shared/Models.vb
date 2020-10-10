Imports GTA
Friend Class Models

    Public Shared DMC12Model As Model
    Public Shared DMC12ColModel As Model
    Public Shared RogersSierraModel As Model
    Public Shared RogersSierraColModel As Model
    Public Shared TenderModel As Model

#Region "Sierra Wheels"
    Public Shared sWheelFront As Model '= LoadAndRequestModel("rog_front_wheel")

    Public Shared sWheelDrive As Model '= LoadAndRequestModel("rog_drive_wheel")
#End Region

#Region "Tender Wheels"
    Public Shared tWheel As Model '= LoadAndRequestModel("rog_tender_wheel")
#End Region

#Region "Wheels Props"
    Public Shared sRods As Model '= LoadAndRequestModel("rog_rods")
    Public Shared sPRods As Model '= LoadAndRequestModel("rog_prods")
    Public Shared sPistons As Model '= LoadAndRequestModel("rog_pistons")
    Public Shared sLevValves As Model '= LoadAndRequestModel("rog_lev_valves")
    Public Shared sValves As Model '= LoadAndRequestModel("rog_valves")
    Public Shared sValvesPist As Model '= LoadAndRequestModel("rog_valves_pist")
#End Region

#Region "Other Props"
    Public Shared sBell As Model '= LoadAndRequestModel("bell")
    Public Shared sLight As Model '= LoadAndRequestModel("sierra_light")
#End Region

#Region "Brake Props"
    Public Shared sBrakePadsFront As Model '= LoadAndRequestModel("brakepads_f")
    Public Shared sBrakePadsMiddle As Model '= LoadAndRequestModel("brakepads_m")
    Public Shared sBrakePadsRear As Model '= LoadAndRequestModel("brakepads_r")
    Public Shared sBrakeBars As Model '= LoadAndRequestModel("brakebars")
    Public Shared sBrakeLevers As Model '= LoadAndRequestModel("brakelevers")
    Public Shared sBrakePistons As Model '= LoadAndRequestModel("brakepistons")
#End Region

    Public Shared Sub LoadModels()

        DMC12Model = LoadAndRequestModel("dmc12")
        DMC12ColModel = LoadAndRequestModel("dmc_debug")
        RogersSierraModel = LoadAndRequestModel("sierra")
        RogersSierraColModel = LoadAndRequestModel("sierra_debug")
        TenderModel = LoadAndRequestModel("sierratender")

        tWheel = LoadAndRequestModel("rog_tender_wheel")

        sWheelDrive = LoadAndRequestModel("rog_drive_wheel")
        sWheelFront = LoadAndRequestModel("rog_front_wheel")

        sRods = LoadAndRequestModel("rog_rods")
        sPRods = LoadAndRequestModel("rog_prods")
        sPistons = LoadAndRequestModel("rog_pistons")
        sLevValves = LoadAndRequestModel("rog_lev_valves")
        sValves = LoadAndRequestModel("rog_valves")
        sValvesPist = LoadAndRequestModel("rog_valves_pist")

        sBell = LoadAndRequestModel("bell")
        sLight = LoadAndRequestModel("sierra_light")

        'sBrakePadsFront = LoadAndRequestModel("brakepads_f")
        'sBrakePadsMiddle = LoadAndRequestModel("brakepads_m")
        'sBrakePadsRear = LoadAndRequestModel("brakepads_r")
        'sBrakeBars = LoadAndRequestModel("brakebars")
        'sBrakeLevers = LoadAndRequestModel("brakelevers")
        'sBrakePistons = LoadAndRequestModel("brakepistons")

    End Sub

End Class
