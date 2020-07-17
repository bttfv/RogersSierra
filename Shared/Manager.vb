Imports GTA
Public Module Manager

    Public RogersSierra As cRogersSierra

    Public Sub CreateRogersSierra(tPosition As Math.Vector3, Optional warpInPlayer As Boolean = False, Optional direction As Boolean = False)

        If IsNothing(RogersSierra) Then

            Dim tmpTrain = CreateMissionTrain(25, tPosition, direction)

            tmpTrain.setTrainCruiseSpeed(0)

            tmpTrain.setTrainSpeed(0)

            RogersSierra = New cRogersSierra(tmpTrain)

            If IsNothing(Models.DMC12Model) Then

                RogersSierra.AutomaticDeLoreanAttach = False
            End If

            If warpInPlayer Then

                getCurrentCharacter.Task.WarpIntoVehicle(tmpTrain.GetTrainCarriage(1), VehicleSeat.Driver)
            End If
        End If
    End Sub
End Module
