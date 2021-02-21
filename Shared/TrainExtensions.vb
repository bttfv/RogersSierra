Imports System.Runtime.CompilerServices

Public Module TrainExtensions

    <Extension>
    Public Function IsRogersSierra(vehicle As GTA.Vehicle) As Boolean

        If vehicle.Model = TrainModels.DMC12ColModel Then

            Return GetRogersSierraFromVehicle(vehicle) IsNot Nothing
        End If

        Return vehicle.Model = TrainModels.RogersSierraColModel Or vehicle.Model = TrainModels.RogersSierraModel
    End Function

    Public Enum SmokeColor

        Off
        [Default]
        Green
        Yellow
        Red
    End Enum

    Public Enum TrainCamera

        Off = -1
        TowardsRail
        Pilot
        Front
        RightFunnel
        RightWheels
        RightFrontWheels
        RightFront2Wheels
        RightSide
        TopCabin
        LeftSide
        LeftFunnel
        LeftWheels
        LeftFrontWheels
        LeftFront2Wheels
        Inside
        WheelieUp
        WheelieDown
    End Enum
End Module
