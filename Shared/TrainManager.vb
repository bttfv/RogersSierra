Imports GTA
Public Module TrainManager

    Public CurrentRogersSierra As RogersSierra

    Public RogersSierraList As New List(Of RogersSierra)

    Friend RogersSierraToRemove As New List(Of RogersSierra)

    Public Sub CreateRogersSierra(tPosition As Math.Vector3, Optional warpInPlayer As Boolean = False, Optional direction As Boolean = False, Optional noTender As Boolean = False)

        Dim tmpTrain = CreateMissionTrain(If(noTender, 27, 26), tPosition, direction)

        tmpTrain.setTrainCruiseSpeed(0)

        tmpTrain.setTrainSpeed(0)

        RogersSierraList.Add(New RogersSierra(tmpTrain))

        If warpInPlayer Then

            PlayerPed.Task.WarpIntoVehicle(tmpTrain.GetTrainCarriage(If(noTender, 0, 1)), VehicleSeat.Driver)
        End If
    End Sub

    Friend Sub RemoveRogersSierra(RogersSierra As RogersSierra)

        If RogersSierraToRemove.Contains(RogersSierra) = False Then

            RogersSierraToRemove.Add(RogersSierra)
        End If
    End Sub

    Public Function GetRogersSierraFromVehicle(veh As Vehicle) As RogersSierra

        For Each t In RogersSierraToRemove

            If t.ColDeLorean = veh Then

                Return t
            End If

            If t = veh Then

                Return t
            End If
        Next

        For Each t In RogersSierraList

            If t.ColDeLorean = veh Then

                Return t
            End If

            If t = veh Then

                Return t
            End If
        Next

        Return Nothing
    End Function
End Module
