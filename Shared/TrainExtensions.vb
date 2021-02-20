Imports System.Runtime.CompilerServices

Public Module TrainExtensions

    <Extension>
    Public Function IsRogersSierra(vehicle As GTA.Vehicle) As Boolean

        If vehicle.Model = TrainModels.DMC12ColModel Then

            Return GetRogersSierraFromVehicle(vehicle) IsNot Nothing
        End If

        Return vehicle.Model = TrainModels.RogersSierraColModel Or vehicle.Model = TrainModels.RogersSierraModel
    End Function
End Module
