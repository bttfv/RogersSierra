Imports System.Runtime.CompilerServices

Public Module TrainExtensions

    <Extension>
    Public Function IsRogersSierra(vehicle As GTA.Vehicle) As Boolean

        Return vehicle.Model = TrainModels.RogersSierraColModel
    End Function
End Module
