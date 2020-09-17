Imports System.Windows.Forms
Imports GTA

Friend Class Main
    Inherits Script

    Private initialSetup As Boolean = True

    Private Sub Main_Aborted(sender As Object, e As EventArgs) Handles Me.Aborted

        RogersSierra.ForEach(Sub(x)

                                 x.Delete(False)
                             End Sub)
    End Sub

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        Commons.MenuManager.KeyDown(e)
    End Sub

    Private Sub Main_Tick(sender As Object, e As EventArgs) Handles Me.Tick

        On Error Resume Next

        If initialSetup Then

            Dim version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version
            Dim buildDate = New DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2)

            IO.File.AppendAllText(".\ScriptHookVDotNet.log", $"RogersSierra - {version} ({buildDate})" & Environment.NewLine)

            Models.LoadModels()

            initialSetup = False
        End If

        Native.Function.Call(Native.Hash.SET_RANDOM_TRAINS, False)

        Commons.MenuManager.Process()

        World.GetAllVehicles(Models.RogersSierraColModel).ToList.ForEach(Sub(x)

                                                                             If IsNothing(GetRogersSierraFromVehicle(x)) Then

                                                                                 RogersSierra.Add(New cRogersSierra(x))
                                                                             End If
                                                                         End Sub)

        If RogersSierraToRemove.Count > 0 Then

            RogersSierraToRemove.ForEach(Sub(x)
                                             RogersSierra.Remove(x)
                                         End Sub)

            RogersSierraToRemove.Clear()
        End If

        RogersSierra.ForEach(Sub(x)

                                 If x.Deleted Then

                                     RemoveRogersSierra(x)
                                     Exit Sub
                                 End If

                                 If getCurrentCharacter.IsInVehicle() = False AndAlso x.isExploded = False Then

                                     If Game.IsControlJustPressed(GTA.Control.Enter) Then

                                         If x.isExploded = False AndAlso x.GetBoneDistanceSquared(Bones.sDriverSeat, getCurrentCharacter) < 1.1 Then

                                             getCurrentCharacter.Task.WarpIntoVehicle(x.Locomotive, VehicleSeat.Driver)
                                         End If
                                     End If
                                 End If

                                 x.Tick()
                             End Sub)
    End Sub
End Class
