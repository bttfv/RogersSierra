Imports GTA
Public Module Manager

    Public CurrentRogersSierra As cRogersSierra

    Public RogersSierra As New List(Of cRogersSierra)

    Friend RogersSierraToRemove As New List(Of cRogersSierra)

    Public Sub CreateRogersSierra(tPosition As Math.Vector3, Optional warpInPlayer As Boolean = False, Optional direction As Boolean = False, Optional noTender As Boolean = False)

        Dim tmpTrain = CreateMissionTrain(If(noTender, 27, 26), tPosition, direction)

        tmpTrain.setTrainCruiseSpeed(0)

        tmpTrain.setTrainSpeed(0)

        RogersSierra.Add(New cRogersSierra(tmpTrain))

        If warpInPlayer Then

            getCurrentCharacter.Task.WarpIntoVehicle(tmpTrain.GetTrainCarriage(If(noTender, 0, 1)), VehicleSeat.Driver)
        End If
    End Sub

    Friend Sub RemoveRogersSierra(RogersSierra As cRogersSierra)

        If RogersSierraToRemove.Contains(RogersSierra) = False Then

            RogersSierraToRemove.Add(RogersSierra)
        End If
    End Sub

    Public Function GetRogersSierraFromVehicle(veh As Vehicle) As cRogersSierra

        For Each t In RogersSierraToRemove

            If t = veh Then

                Return t
            End If
        Next

        For Each t In RogersSierra

            If t = veh Then

                Return t
            End If
        Next

        Return Nothing
    End Function
End Module
