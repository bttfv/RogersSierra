Imports System.Drawing
Imports FusionLibrary
Imports FusionLibrary.Enums
Imports FusionLibrary.Extensions
Imports GTA
Imports GTA.Math

Partial Public Class RogersSierra

    Private CustomLights As LightHandler

    Private pWhistle As New PtfxEntityBonePlayer(TrainParticles.sWhistle)
    Private pSteam As New List(Of PtfxEntityBonePlayer)
    Private pWaterDrops As New List(Of PtfxEntityBonePlayer)
    Private pSteamVent As New List(Of PtfxEntityBonePlayer)
    Private pSteamRunning As New List(Of PtfxEntityBonePlayer)

    Private pFireboxFire As New PtfxEntityBonePlayer(TrainParticles.sFireboxFire)
    Private pFireboxFireSize As Single = 0.3

    Private pFunnelSmoke As New PtfxEntityBonePlayer(TrainParticles.sColoredSmoke)
    Private pFunnelFire As New PtfxEntityBonePlayer(TrainParticles.sFunnelFire)
    Private pFunnelInterval As Integer
    Private pFunnelTime As Integer

    Private pTrainExpl As New PtfxEntityPlayer(TrainParticles.sTrainExpl)
    Private pPrestoLogExpl As New PtfxEntityBonePlayer(TrainParticles.sPrestoLogExpl)

    Private CabLight As Light

#Region "Properties"
    ''' <summary>
    ''' Gets or sets funnel smoke's color.
    ''' </summary>
    ''' <returns><seealso cref="SmokeColor"/></returns>
    Public Property FunnelSmoke As SmokeColor = SmokeColor.Default
    ''' <summary>
    ''' Gets or sets status of boiler light.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property IsLightOn As Boolean
        Get
            Return sLight.Visible
        End Get
        Set(value As Boolean)
            sLight.Visible = value
            CabLight.IsEnabled = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets status of whistle.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property Whistle As Boolean
        Get
            Return sWhistleSound.IsAnyInstancePlaying
        End Get
        Set(value As Boolean)
            If value Then

                sWhistleSound.Play()
                pWhistle.CreateLoopedOnEntityBone(Locomotive, TrainBones.sWhistle, Vector3.Zero)
            Else

                sWhistleSound.Stop()
                pWhistle.StopNaturally()
            End If
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets status of funnel fire.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property FunnelFire As Boolean
        Get
            Return pFunnelFire.Handle <> 0
        End Get
        Set(value As Boolean)
            If value Then

                pFunnelFire.CreateLoopedOnEntityBone(Locomotive, TrainBones.sFunnel, New Vector3(0, -0.5, -0.25),, 0.75)
                'pFunnelFire.SetLoopedEvolution("fade", 1)
            Else

                pFunnelFire.Stop()
            End If
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets size of firebox fire.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property FireboxFireSize As Single
        Get
            Return pFireboxFireSize
        End Get
        Set(value As Single)
            pFireboxFireSize = value

            If FireboxFire Then

                FireboxFire = False
                FireboxFire = True
            End If
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets status of firebox fire.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property FireboxFire As Boolean
        Get
            Return pFireboxFire.Handle <> 0
        End Get
        Set(value As Boolean)
            If value Then

                pFireboxFire.CreateLoopedOnEntityBone(Locomotive, TrainBones.sFireboxFire, New Vector3(0, 0, 0), New Vector3(0, 0, 0), pFireboxFireSize)
                'pFunnelFire.SetLoopedEvolution("fade", 1)
            Else

                pFireboxFire.Stop()
            End If
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets status of piston's steam while train is stationary.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property PistonSteam As Boolean
        Get
            Return pSteam.Count > 0
        End Get
        Set(value As Boolean)
            If value Then

                For i = 0 To 3

                    With pSteam

                        .Add(New PtfxEntityBonePlayer(TrainParticles.sSteam))
                        .Last.CreateLoopedOnEntityBone(Locomotive, TrainBones.sSteam(i), Vector3.Zero, New Vector3(90, 0, 0))
                    End With

                    With pWaterDrops

                        .Add(New PtfxEntityBonePlayer(TrainParticles.sWaterDrop))
                        .Last.CreateLoopedOnEntityBone(Locomotive, TrainBones.sSteam(i), Vector3.Zero, New Vector3(0, 0, 0), 3)
                    End With
                Next
            Else

                pSteam.ForEach(Sub(x)
                                   x.Stop()
                               End Sub)
                pSteam.Clear()
                pWaterDrops.ForEach(Sub(x)
                                        x.Stop()
                                    End Sub)
                pWaterDrops.Clear()
                pWhistle.Stop()
            End If
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets status of venting steam from pistons.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property PistonSteamVent As Boolean
        Get
            Return sTrainStart.IsAnyInstancePlaying
        End Get
        Set(value As Boolean)
            If value Then

                sTrainStart.Play()

                For i = 0 To 3

                    With pSteamVent

                        .Add(New PtfxEntityBonePlayer(TrainParticles.sWhistle))

                        If i = 1 Or i = 3 Then

                            .Last.CreateLoopedOnEntityBone(Locomotive, TrainBones.sSteam(i), New Vector3(0, -0, -0.09), New Vector3(0, 90, 0))
                        Else

                            .Last.CreateLoopedOnEntityBone(Locomotive, TrainBones.sSteam(i), New Vector3(0, 0, -0.09), New Vector3(0, -90, 0))
                        End If
                    End With
                Next
            Else

                pSteamVent.ForEach(Sub(x)
                                       x.Stop()
                                   End Sub)
                pSteamVent.Clear()
                sTrainStart.Stop()
            End If
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets status of piston's steam while train is running.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property PistonSteamRunning As Boolean
        Get
            Return pSteamRunning.Count > 0
        End Get
        Set(value As Boolean)
            If value Then

                For i = 0 To 1

                    With pSteamRunning

                        .Add(New PtfxEntityBonePlayer(TrainParticles.sSteam))
                        .Last.CreateLoopedOnEntityBone(Locomotive, TrainBones.sSteamRunning(i), New Vector3(If(i = 0, 0.5, -0.5), -0.5, -0.5), New Vector3(0, 0, 0), 3)
                    End With
                Next
            Else

                pSteamRunning.ForEach(Sub(x)
                                          x.StopNaturally()
                                      End Sub)
                pSteamRunning.Clear()
            End If
        End Set
    End Property
#End Region

    Private Sub LoadParticles()

        pFunnelFire.SetEvolutionParam("fade", 1)

        PistonSteam = True
        FireboxFire = True

        CustomLights = New LightHandler(Locomotive, RogersSierraList.Count + 1)

        'Cab
        CustomLights.Add("boilerlight", "boilerlightdir", Color.White, 34, 5, 0, 45, 100)
        CabLight = CustomLights.Lights.Last
        IsLightOn = TimeHandler.IsNight
    End Sub

    Private Sub ParticlesTick()

        pFunnelInterval = 1500 - (1450 / 51) * Locomotive.GetMPHSpeed

        If pFunnelInterval < 50 Then pFunnelInterval = 50

        If pFunnelTime < Game.GameTime Then

            If FunnelSmoke <> SmokeColor.Off Then

                pFunnelSmoke.CreateOnEntityBone(Locomotive, TrainBones.sFunnel, New Vector3(0, -1.6, 0), New Math.Vector3(90, 0, 0), 1)

                Select Case FunnelSmoke
                    Case SmokeColor.Default

                        '.sFunnelSmoke.Color(132 / 255, 144 / 255, 118 / 255)
                    Case SmokeColor.Green

                        pFunnelSmoke.Color(132 / 255, 144 / 255, 118 / 255)
                    Case SmokeColor.Yellow

                        pFunnelSmoke.Color(217 / 255, 194 / 255, 75 / 255)
                    Case SmokeColor.Red

                        pFunnelSmoke.Color(184 / 255, 81 / 255, 94 / 255)
                End Select
            End If

            pFunnelTime = Game.GameTime + pFunnelInterval
        End If

        If PistonSteamVent = False AndAlso pSteamVent.Count > 0 Then

            PistonSteam = False
            PistonSteamVent = False
        End If

        If Locomotive.Speed > 0 Then

            If PistonSteam Then

                PistonSteam = False
                PistonSteamVent = True
            ElseIf Not PistonSteamVent AndAlso Not PistonSteamRunning Then

                PistonSteamRunning = True
            End If
        ElseIf PistonSteamVent = False Then

            If PistonSteamRunning Then

                PistonSteamRunning = False
            End If

            If PistonSteam = False AndAlso IsExploded = False Then

                PistonSteam = True
            End If
        End If

        If Whistle = False AndAlso pWhistle.Handle <> 0 Then

            Whistle = False
        End If

        If Game.IsControlJustPressed(Control.VehicleHorn) AndAlso Utils.PlayerPed.IsInVehicle(Locomotive) Then

            Whistle = True
        End If

        If Game.IsControlJustReleased(Control.VehicleHorn) AndAlso Whistle Then

            Whistle = False
        End If
    End Sub
End Class
