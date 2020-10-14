Imports System.Windows.Forms
Imports GTA

Friend Class Main
    Inherits Script

    Private initialSetup As Boolean = True

    Private Sub Main_Aborted(sender As Object, e As EventArgs) Handles Me.Aborted

        RogersSierraList.ForEach(Sub(x)

                                     x.Delete(False)
                                 End Sub)
    End Sub

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        Commons.MenuManager.KeyDown(e)

        If Not IsNothing(CurrentRogersSierra) Then

            CurrentRogersSierra.KeyDown(e.KeyCode)
        End If
    End Sub

    Private Sub Main_Tick(sender As Object, e As EventArgs) Handles Me.Tick

        On Error Resume Next

        If initialSetup Then

            Dim version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version
            Dim buildDate = New DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2)

            IO.File.AppendAllText(".\ScriptHookVDotNet.log", $"RogersSierra - {version} ({buildDate})" & Environment.NewLine)

            TrainModels.LoadModels()

            initialSetup = False
        End If

        Commons.MenuManager.Process()

        World.GetAllVehicles(TrainModels.DMC12ColModel).ToList.ForEach(Sub(x)

                                                                           Try
                                                                               If x.GetTrainCarriage(1).Model <> TrainModels.RogersSierraColModel Then

                                                                                   Exit Sub
                                                                               End If
                                                                           Catch ex As Exception

                                                                               Exit Sub
                                                                           End Try

                                                                           If IsNothing(GetRogersSierraFromVehicle(x)) Then

                                                                               RogersSierraList.Add(New RogersSierra(x))
                                                                           End If
                                                                       End Sub)

        If RogersSierraToRemove.Count > 0 Then

            RogersSierraToRemove.ForEach(Sub(x)
                                             RogersSierraList.Remove(x)
                                         End Sub)

            RogersSierraToRemove.Clear()
        End If

        RogersSierraList.ForEach(Sub(x)

                                     If x.Deleted Then

                                         RemoveRogersSierra(x)
                                         Exit Sub
                                     End If

                                     If PlayerPed.IsInVehicle() = False AndAlso x.IsExploded = False Then

                                         If Game.IsControlJustPressed(GTA.Control.Enter) Then

                                             If x.IsExploded = False AndAlso x.GetBoneDistanceSquared(TrainBones.sDriverSeat, PlayerPed) < 1.1 Then

                                                 PlayerPed.Task.EnterVehicle(x.Locomotive, VehicleSeat.Driver)
                                             End If
                                         End If
                                     End If

                                     Dim tmpDist = PlayerPed.Position.DistanceToSquared(x.Locomotive.Position)

                                     If ClosestRogersSierra Is x Then

                                         ClosestRogersSierraDist = tmpDist
                                     ElseIf tmpDist < ClosestRogersSierraDist OrElse ClosestRogersSierraDist = -1 Then

                                         ClosestRogersSierraDist = tmpDist
                                         ClosestRogersSierra = x
                                     End If

                                     x.Tick()
                                 End Sub)

        If RogersSierraList.Count = 0 AndAlso ClosestRogersSierraDist > -1 Then

            ClosestRogersSierraDist = -1
            ClosestRogersSierra = Nothing
        End If

        If Not IsNothing(CurrentRogersSierra) AndAlso Not PlayerPed.IsInVehicle Then

            If CurrentRogersSierra.Camera <> TrainCamera.Off AndAlso Not CurrentRogersSierra.IsOnTrainMission Then

                CurrentRogersSierra.Camera = TrainCamera.Off
            End If

            CurrentRogersSierra = Nothing
        End If
    End Sub
End Class
