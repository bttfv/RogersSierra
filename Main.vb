﻿Imports System.Windows.Forms
Imports GTA

Friend Class Main
    Inherits Script

    Private initialSetup As Boolean = True

    Private Sub Main_Aborted(sender As Object, e As EventArgs) Handles Me.Aborted

        If IsNothing(RogersSierra) = False Then

            RogersSierra.Delete()
        End If
    End Sub

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        Select Case e.KeyData
            Case Keys.F9
                If IsNothing(RogersSierra) Then

                    CreateRogersSierra(getCurrentCharacter.Position, True)
                Else

                    RogersSierra.Delete()
                End If
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

        If IsNothing(RogersSierra) = False Then

            If getCurrentCharacter.IsInVehicle() = False AndAlso RogersSierra.isExploded = False Then

                If Game.IsControlJustPressed(GTA.Control.Enter) Then

                    Dim trainVeh = World.GetNearbyVehicles(getCurrentCharacter, 5, Models.RogersSierraColModel)

                    If IsNothing(trainVeh) = False Then

                        getCurrentCharacter.Task.WarpIntoVehicle(trainVeh(0), VehicleSeat.Driver)
                    End If
                End If
            End If

            RogersSierra.Tick()
        End If
    End Sub
End Class
