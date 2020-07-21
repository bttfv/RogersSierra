Imports GTA
Public Module Manager

    Public RogersSierra As cRogersSierra

    Public Sub CreateRogersSierra(tPosition As Math.Vector3, Optional warpInPlayer As Boolean = False, Optional direction As Boolean = False)

        If IsNothing(RogersSierra) = False Then

            If RogersSierra.isExploded Then

                RogersSierra.Delete()
            Else

                Return
            End If
        End If

        Dim tmpTrain = CreateMissionTrain(26, tPosition, direction)

        tmpTrain.setTrainCruiseSpeed(0)

        tmpTrain.setTrainSpeed(0)

        RogersSierra = New cRogersSierra(tmpTrain)

        If IsNothing(Models.DMC12Model) Then

            RogersSierra.AutomaticDeLoreanAttach = False
        End If

        If warpInPlayer Then

            getCurrentCharacter.Task.WarpIntoVehicle(tmpTrain.GetTrainCarriage(1), VehicleSeat.Driver)
        End If
    End Sub
End Module
