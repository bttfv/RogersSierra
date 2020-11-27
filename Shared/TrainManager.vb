Imports BTTFVLibrary
Imports GTA
Imports BTTFVLibrary.Extensions

Public Module TrainManager

    Private _CurrentRogersSierra As RogersSierra
    Public Property CurrentRogersSierra As RogersSierra
        Get
            Return _CurrentRogersSierra
        End Get
        Friend Set(value As RogersSierra)
            _CurrentRogersSierra = value
        End Set
    End Property

    Private _ClosestRogersSierra As RogersSierra
    Public Property ClosestRogersSierra As RogersSierra
        Get
            Return _ClosestRogersSierra
        End Get
        Friend Set(value As RogersSierra)
            _ClosestRogersSierra = value
        End Set
    End Property

    Private _ClosestRogersSierraDist As Single = -1
    Public Property ClosestRogersSierraDist As Single
        Get
            Return _ClosestRogersSierraDist
        End Get
        Friend Set(value As Single)
            _ClosestRogersSierraDist = value
        End Set
    End Property


    Public ReadOnly RogersSierraList As New List(Of RogersSierra)

    Friend RogersSierraToRemove As New List(Of RogersSierra)

    Public Sub CreateRogersSierra(tPosition As Math.Vector3, Optional warpInPlayer As Boolean = False, Optional direction As Boolean = False)

        Dim tmpTrain = Utils.CreateMissionTrain(26, tPosition, direction)

        tmpTrain.setTrainCruiseSpeed(0)

        tmpTrain.setTrainSpeed(0)

        RogersSierraList.Add(New RogersSierra(tmpTrain, False))

        If warpInPlayer Then

            Utils.PlayerPed.Task.WarpIntoVehicle(tmpTrain.GetTrainCarriage(1), VehicleSeat.Driver)
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
