Imports GTA
Public Class cInvisibleTrain

    Friend Carriage As New List(Of Vehicle)

    Private tDeLorean As Vehicle
    Private isDeLoreanAttached As Boolean = False
    Private justAttached As Boolean = True
    Private attachOffset As Math.Vector3 'New Math.Vector3(0, 7.5, -1.4)

    Private direction As Boolean

    Private tSpeed As Single = 0

    Private tFreight As Vehicle
    Private FreightDeleteCounter As Single = 0
    Private deloreanExploded As Boolean = False

    Private isOnTrainMission As Boolean

    Sub New(tTrain As Vehicle, isOnTrainMission As Boolean, direction As Boolean, Optional tVehicle As Vehicle = Nothing)

        Carriage.Add(tTrain)
        Carriage.Add(tTrain.GetTrainCarriage(1))

        Carriage(0).IsCollisionEnabled = False

        Carriage.ForEach(Sub(x)

                             x.IsVisible = False

                             Native.Function.Call(Native.Hash.SET_HORN_ENABLED, x.Handle, False)

                             Native.Function.Call(Native.Hash._FORCE_VEHICLE_ENGINE_AUDIO, x.Handle, "")
                         End Sub)

        Me.isOnTrainMission = isOnTrainMission

        Me.direction = direction

        If isOnTrainMission Then

            tDeLorean = tVehicle
            'oldDistance = Carriage(0).Position.DistanceTo(tVehicle.Position)
        End If

        InvisibleTrain.Add(Me)
    End Sub

    Private Sub UpdateAttachOffset()

        attachOffset = Carriage(1).GetPositionOffset(Carriage(0).Position)

        attachOffset.Z -= 1.35
    End Sub

    Public Function CheckDeLorean() As Vehicle

        UpdateAttachOffset()

        Dim tDel = World.GetNearbyVehicles(Carriage(1).GetOffsetPosition(attachOffset), 4.0, Models.DMC12Model)

        If tDel.Count > 0 Then

            If (Int(Carriage(0).Heading) <= Int(tDel(0).Heading) + 1 Or Int(Carriage(0).Heading) >= Int(tDel(0).Heading) - 1) AndAlso Carriage(1).isGoingForward Then

                AttachDeLorean(tDel(0))

                Return tDel(0)
            End If
        End If

        Return Nothing
    End Function

    Public Sub AttachDeLorean(tDeLorean As Vehicle)

        Me.tDeLorean = tDeLorean

        tDeLorean.IsInvincible = True
        tDeLorean.CanBeVisiblyDamaged = False

        UpdateAttachOffset()

        isDeLoreanAttached = True
    End Sub

    Public Sub DetachDeLorean()

        Native.Function.Call(Native.Hash.DETACH_ENTITY, tDeLorean.Handle, False, False)
        tDeLorean.IsInvincible = False
        tDeLorean.CanBeVisiblyDamaged = True
        isDeLoreanAttached = False
    End Sub

    Public Sub setTrainSpeed(speed As Single)

        tSpeed = speed
    End Sub

    Public Sub setTrainSpeedMPH(speed As Integer)

        tSpeed = MphToMs(speed)
    End Sub

    Friend Sub Tick()

        If isDeLoreanAttached Then

            tDeLorean.Rotation = Carriage(0).Rotation
            tDeLorean.AttachToPhisically(Carriage(1), attachOffset, Math.Vector3.Zero, 1000000.0)

            If isOnTrainMission AndAlso justAttached Then

                FreightDeleteCounter = 0
                setTrainSpeedMPH(80)
                justAttached = False
            End If
        Else

            CheckDeLorean()
        End If

        If tSpeed > 0 Then

            tSpeed -= 2 * Game.LastFrameTime

            If tSpeed < 0 Then

                tSpeed = 0
            End If
        ElseIf tSpeed < 0 Then

            tSpeed += 2 * Game.LastFrameTime

            If tSpeed > 0 Then

                tSpeed = 0
            End If
        End If

        If isOnTrainMission AndAlso justAttached Then

            setTrainSpeedMPH(500)

            FreightDeleteCounter += Game.LastFrameTime

            If FreightDeleteCounter >= 5 Then

                Delete()
            End If
        End If

        Carriage(1).setTrainSpeed(tSpeed)

        If isOnTrainMission AndAlso tSpeed = 0 Then

            If isDeLoreanAttached Then

                DetachDeLorean()

                Carriage(1).IsCollisionEnabled = False

                tDeLorean.IsDriveable = False
                tDeLorean.HealthFloat = 1

                tFreight = CreateFreightTrain(tDeLorean.GetOffsetPosition(New Math.Vector3(0, 100, 0)), Not direction)

                tFreight.setTrainCruiseSpeedMPH(40)

                'oldDistance = tFreight.Position.DistanceTo(tDeLorean.Position)

            ElseIf IsNothing(tFreight) = False Then

                'If deloreanExploded = False Then

                If deloreanExploded = False AndAlso tDeLorean.HasBeenDamagedBy(tFreight) Then

                    tDeLorean.Explode()
                    deloreanExploded = True
                End If

                'If oldDistance < tFreight.Position.DistanceTo(tDeLorean.Position) Then

                '    ShowSubtitle("qui")

                '    tFreight.DeleteFreightTrain
                '    Delete()
                'End If

                'oldDistance = tFreight.Position.DistanceTo(tDeLorean.Position)
                'End If

                FreightDeleteCounter += Game.LastFrameTime

                If FreightDeleteCounter >= 45 Then

                    tFreight.DeleteFreightTrain
                    Delete()
                End If
            End If
        End If
    End Sub

    Public Sub Delete()

        If isDeLoreanAttached Then

            DetachDeLorean()
        End If

        Carriage.ForEach(Sub(x)
                             x.Delete()
                         End Sub)

        InvisibleTrain.Remove(Me)
    End Sub
End Class
