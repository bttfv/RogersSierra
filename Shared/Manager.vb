Imports GTA
Public Module Manager

    Public CurrentRogersSierra As cRogersSierra

    Public RogersSierra As New List(Of cRogersSierra)

    Friend RogersSierraToRemove As New List(Of cRogersSierra)

    Public Sub CreateRogersSierra(tPosition As Math.Vector3, Optional warpInPlayer As Boolean = False, Optional direction As Boolean = False)

        Dim tmpTrain = CreateMissionTrain(26, tPosition, direction)

        tmpTrain.setTrainCruiseSpeed(0)

        tmpTrain.setTrainSpeed(0)

        RogersSierra.Add(New cRogersSierra(tmpTrain))

        If warpInPlayer Then

            getCurrentCharacter.Task.WarpIntoVehicle(tmpTrain.GetTrainCarriage(1), VehicleSeat.Driver)
        End If
    End Sub

    Friend Sub RemoveRogersSierra(RogersSierra As cRogersSierra)

        If RogersSierraToRemove.Contains(RogersSierra) = False Then

            RogersSierraToRemove.Add(RogersSierra)
        End If
    End Sub
End Module
