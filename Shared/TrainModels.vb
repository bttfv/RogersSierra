Imports GTA
Imports FusionLibrary

Friend Class TrainModels
    Inherits CustomModelHandler

    Public Shared DMC12ColModel As New CustomModel("dmc_debug")
    Public Shared RogersSierraModel As New CustomModel("sierra")
    Public Shared RogersSierraColModel As New CustomModel("sierra_debug")
    Public Shared TenderModel As New CustomModel("sierratender")

#Region "Sierra Wheels"
    Public Shared sWheelFront As New CustomModel("rog_front_wheel")
    Public Shared sWheelDrive As New CustomModel("rog_drive_wheel")
#End Region

#Region "Tender Wheels"
    Public Shared tWheel As New CustomModel("rog_tender_wheel")
#End Region

#Region "Wheels Props"
    Public Shared sRods As New CustomModel("rog_rods")
    Public Shared sPRods As New CustomModel("rog_prods")
    Public Shared sPistons As New CustomModel("rog_pistons")
    Public Shared sLevValves As New CustomModel("rog_lev_valves")
    Public Shared sValves As New CustomModel("rog_valves")
    Public Shared sValvesPist As New CustomModel("rog_valves_pist")
#End Region

#Region "Other Props"
    Public Shared sBell As New CustomModel("bell")
    Public Shared sLight As New CustomModel("sierra_light")
    Public Shared sCabCols As New CustomModel("rog_cab_col")
    Public Shared sFireboxDoor As New CustomModel("rog_furnace_door")
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

        GetAllModels(GetType(TrainModels)).ForEach(Sub(x)
                                                       PreloadModel(x)
                                                   End Sub)

        Native.Function.Call(Native.Hash.BUSYSPINNER_OFF)

        'sBrakePadsFront = LoadAndRequestModel("brakepads_f")
        'sBrakePadsMiddle = LoadAndRequestModel("brakepads_m")
        'sBrakePadsRear = LoadAndRequestModel("brakepads_r")
        'sBrakeBars = LoadAndRequestModel("brakebars")
        'sBrakeLevers = LoadAndRequestModel("brakelevers")
        'sBrakePistons = LoadAndRequestModel("brakepistons")
    End Sub
End Class
