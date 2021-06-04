Imports System.Windows.Forms
Imports FusionLibrary
Imports FusionLibrary.Extensions
Imports GTA

Friend Class Main
    Inherits Script

    Private SpawnMenu As New SpawnMenu

    Private initialSetup As Boolean = True

    Private Sub Main_Aborted(sender As Object, e As EventArgs) Handles Me.Aborted

        RogersSierraList.ForEach(Sub(x)

                                     x.Delete(x.IsExploded)
                                 End Sub)
    End Sub

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        Select Case e.KeyData
            Case Keys.F9
                If CustomNativeMenu.ObjectPool.AreAnyVisible = False Then

                    SpawnMenu.Open()
                End If
        End Select

        If Not IsNothing(CurrentRogersSierra) Then

            CurrentRogersSierra.KeyDown(e.KeyCode)
        End If
    End Sub

    Private Sub Main_Tick(sender As Object, e As EventArgs) Handles Me.Tick

        If Game.IsLoading OrElse FusionUtils.FirstTick Then

            Return
        End If

        On Error Resume Next

        If initialSetup Then

            Dim version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version
            Dim buildDate = New DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2)

            IO.File.AppendAllText(".\ScriptHookVDotNet.log", $"RogersSierra - {version} ({buildDate})" & Environment.NewLine)

            TrainModels.LoadModels()

            FusionUtils.RandomTrains = False

            initialSetup = False
        End If

        World.GetAllVehicles(TrainModels.DMC12ColModel).ToList.ForEach(Sub(x)

                                                                           Try
                                                                               If x.GetTrainCarriage(1).Model <> TrainModels.RogersSierraColModel Then

                                                                                   Exit Sub
                                                                               End If
                                                                           Catch ex As Exception

                                                                               Exit Sub
                                                                           End Try

                                                                           If IsNothing(GetRogersSierraFromVehicle(x)) Then

                                                                               RogersSierraList.Add(New RogersSierra(x, True))
                                                                           End If
                                                                       End Sub)

        If RogersSierraToRemove.Count > 0 Then

            RogersSierraToRemove.ForEach(Sub(x)
                                             RogersSierraList.Remove(x)
                                         End Sub)

            RogersSierraToRemove.Clear()
        End If

        RogersSierraList.ForEach(Sub(train)

                                     If train.Deleted Then

                                         RemoveRogersSierra(train)
                                         Exit Sub
                                     End If

                                     If FusionUtils.PlayerPed.IsInVehicle() = False AndAlso train.IsExploded = False Then

                                         If Game.IsControlJustPressed(GTA.Control.Enter) Then

                                             If FusionUtils.PlayerPed.DistanceToSquared2D(train, TrainBones.sDriverSeat, 1.3) Then

                                                 FusionUtils.PlayerPed.Task.EnterVehicle(train.Locomotive, VehicleSeat.Driver,,, EnterVehicleFlags.WarpIn)
                                             End If
                                         End If
                                     End If

                                     Dim tmpDist = FusionUtils.PlayerPed.DistanceToSquared2D(train)

                                     If ClosestRogersSierra Is train Then

                                         ClosestRogersSierraDist = tmpDist
                                     ElseIf tmpDist < ClosestRogersSierraDist OrElse ClosestRogersSierraDist = -1 Then

                                         ClosestRogersSierraDist = tmpDist
                                         ClosestRogersSierra = train
                                     End If

                                     train.Tick()
                                 End Sub)

        If RogersSierraList.Count = 0 AndAlso ClosestRogersSierraDist > -1 Then

            ClosestRogersSierraDist = -1
            ClosestRogersSierra = Nothing
        End If

        If Not IsNothing(ClosestRogersSierra) Then

            If ClosestRogersSierraDist <= 200 AndAlso ClosestRogersSierra.LocomotiveSpeed > 0 Then

                If FusionUtils.PlayerPed.CanRagdoll Then

                    FusionUtils.PlayerPed.CanRagdoll = False
                End If
            ElseIf Not FusionUtils.PlayerPed.CanRagdoll Then

                FusionUtils.PlayerPed.CanRagdoll = True
            End If
        End If

        If Not IsNothing(CurrentRogersSierra) AndAlso Not FusionUtils.PlayerPed.IsInVehicle Then

            FusionUtils.PlayerPed.Task.ClearAllImmediately()

            If CurrentRogersSierra.Camera <> TrainCamera.Off AndAlso Not CurrentRogersSierra.IsOnTrainMission Then

                CurrentRogersSierra.Camera = TrainCamera.Off
            End If

            CurrentRogersSierra = Nothing
        End If
    End Sub
End Class
