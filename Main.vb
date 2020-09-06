Imports System.Windows.Forms
Imports GTA

Friend Class Main
    Inherits Script

    Private initialSetup As Boolean = True

    Private Sub Main_Aborted(sender As Object, e As EventArgs) Handles Me.Aborted

        RogersSierra.ForEach(Sub(x)
                                 x.Delete()
                             End Sub)
    End Sub

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        Select Case e.KeyData
            Case Keys.F9
                CreateRogersSierra(getCurrentCharacter.Position, True)
        End Select

        'Commons.MenuManager.KeyDown(e)
    End Sub

    Private Sub Main_Tick(sender As Object, e As EventArgs) Handles Me.Tick

        On Error Resume Next

        If initialSetup Then

            Models.DMC12Model = LoadAndRequestModel("dmc12")
            Models.DMC12ColModel = LoadAndRequestModel("dmc_debug")
            Models.RogersSierraModel = LoadAndRequestModel("sierra")
            Models.RogersSierraColModel = LoadAndRequestModel("sierra_debug")
            Models.TenderModel = LoadAndRequestModel("sierratender")

            Models.tWheel = LoadAndRequestModel("rog_tender_wheel")

            Models.sWheelDrive = LoadAndRequestModel("rog_drive_wheel")
            Models.sWheelFront = LoadAndRequestModel("rog_front_wheel")

            Models.sRods = LoadAndRequestModel("rog_rods")
            Models.sPRods = LoadAndRequestModel("rog_prods")
            Models.sPistons = LoadAndRequestModel("rog_pistons")
            Models.sLevValves = LoadAndRequestModel("rog_lev_valves")
            Models.sValves = LoadAndRequestModel("rog_valves")
            Models.sValvesPist = LoadAndRequestModel("rog_valves_pist")

            Models.sBell = LoadAndRequestModel("bell")
            Models.sLight = LoadAndRequestModel("sierra_light")

            'Models.sBrakePadsFront = LoadAndRequestModel("brakepads_f")
            'Models.sBrakePadsMiddle = LoadAndRequestModel("brakepads_m")
            'Models.sBrakePadsRear = LoadAndRequestModel("brakepads_r")
            'Models.sBrakeBars = LoadAndRequestModel("brakebars")
            'Models.sBrakeLevers = LoadAndRequestModel("brakelevers")
            'Models.sBrakePistons = LoadAndRequestModel("brakepistons")

            initialSetup = False
        End If

        Native.Function.Call(Native.Hash.SET_RANDOM_TRAINS, False)

        Commons.MenuManager.Process()

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
