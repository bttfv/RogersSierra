﻿Imports GTA
Imports BTTFVLibrary

Friend Class TrainModels

    'Public Shared DMC12Model As Model
    Public Shared DMC12ColModel As Model
    Public Shared RogersSierraModel As Model
    Public Shared RogersSierraColModel As Model
    Public Shared TenderModel As Model

#Region "Sierra Wheels"
    Public Shared sWheelFront As Model
    Public Shared sWheelDrive As Model
#End Region

#Region "Tender Wheels"
    Public Shared tWheel As Model
#End Region

#Region "Wheels Props"
    Public Shared sRods As Model
    Public Shared sPRods As Model
    Public Shared sPistons As Model
    Public Shared sLevValves As Model
    Public Shared sValves As Model
    Public Shared sValvesPist As Model
#End Region

#Region "Other Props"
    Public Shared sBell As Model
    Public Shared sLight As Model
    Public Shared sCabCols As Model
    Public Shared sFireboxDoor As Model
#End Region

    '#Region "Brake Props"
    '    Public Shared sBrakePadsFront As Model
    '    Public Shared sBrakePadsMiddle As Model
    '    Public Shared sBrakePadsRear As Model
    '    Public Shared sBrakeBars As Model
    '    Public Shared sBrakeLevers As Model
    '    Public Shared sBrakePistons As Model
    '#End Region

    Public Shared Sub LoadModels()

        'DMC12Model = LoadAndRequestModel("dmc12")
        DMC12ColModel = Utils.LoadAndRequestModel("dmc_debug")
        RogersSierraModel = Utils.LoadAndRequestModel("sierra")
        RogersSierraColModel = Utils.LoadAndRequestModel("sierra_debug")
        TenderModel = Utils.LoadAndRequestModel("sierratender")

        tWheel = Utils.LoadAndRequestModel("rog_tender_wheel")

        sWheelDrive = Utils.LoadAndRequestModel("rog_drive_wheel")
        sWheelFront = Utils.LoadAndRequestModel("rog_front_wheel")

        sRods = Utils.LoadAndRequestModel("rog_rods")
        sPRods = Utils.LoadAndRequestModel("rog_prods")
        sPistons = Utils.LoadAndRequestModel("rog_pistons")
        sLevValves = Utils.LoadAndRequestModel("rog_lev_valves")
        sValves = Utils.LoadAndRequestModel("rog_valves")
        sValvesPist = Utils.LoadAndRequestModel("rog_valves_pist")

        sBell = Utils.LoadAndRequestModel("bell")
        sLight = Utils.LoadAndRequestModel("sierra_light")
        sCabCols = Utils.LoadAndRequestModel("rog_cab_col")
        sFireboxDoor = Utils.LoadAndRequestModel("rog_furnace_door")

        'sBrakePadsFront = LoadAndRequestModel("brakepads_f")
        'sBrakePadsMiddle = LoadAndRequestModel("brakepads_m")
        'sBrakePadsRear = LoadAndRequestModel("brakepads_r")
        'sBrakeBars = LoadAndRequestModel("brakebars")
        'sBrakeLevers = LoadAndRequestModel("brakelevers")
        'sBrakePistons = LoadAndRequestModel("brakepistons")

    End Sub

End Class
