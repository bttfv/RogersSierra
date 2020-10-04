Imports System.Drawing
Imports GTA
Imports GTA.Math
Imports GTA.UI
Imports KlangRageAudioLibrary

Public Class cRogersSierra

    Public AudioEngine As New AudioEngine()

    Public ReadOnly Property ID As Integer

    ''' <summary>
    ''' Points to the Rogers Sierra vehicle.
    ''' </summary>
    Public Locomotive As Vehicle

    ''' <summary>
    ''' Points to the tender.
    ''' </summary>
    Private Tender As Vehicle

    ''' <summary>
    ''' Points to the fake DeLorean on the front.
    ''' </summary>
    Public ColDeLorean As Vehicle

    Public VisibleLocomotive As Vehicle

    Public ReadOnly Property Type As TrainType

    Public ReadOnly Property isExploded As Boolean

    ''' <summary>
    ''' Points to the vehicle, if it is attached.
    ''' </summary>
    Public AttachedVehicle As Vehicle

    ''' <summary>
    ''' If true, there is a vehicle attached on the front.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property isVehicleAttached As Boolean = AttachedVehicle <> Nothing

    ''' <summary>
    ''' If true, the train is in Rocket mode.
    ''' </summary>
    ''' <returns></returns>
    Public Property isOnTrainMission As Boolean = False

    ''' <summary>
    ''' Modifier for the train's acceleration.
    ''' </summary>
    ''' <returns></returns>
    Friend Property LocomotiveAccModifier As Single = 1

    ''' <summary>
    ''' Setted speed of the train.
    ''' </summary>
    ''' <returns></returns>
    Public Property LocomotiveSpeed As Single = 0

    Public Property ForceHandbrake As Boolean = False

    ''' <summary>
    ''' If true, the cruise control is enabled.
    ''' </summary>
    ''' <returns></returns>
    Public Property isCruiseControlOn As Boolean = False

    Public ReadOnly Property Deleted As Boolean

    Private CustomCamera As New CustomerCameraManager

    Private SmokeTime As Integer

    Private WheelRadius As Single
    Private SmallWheelRadius As Single

    Private PistonRelativePosY As Single
    Private PistonRelativePosZ As Single

    Private aAllProps As New AnimatePropHandler

    Private aSmallWheelsTender As New AnimatePropHandler

    Private aWheels As New AnimatePropHandler
    Private aSmallWheels As New AnimatePropHandler
    Private aRods As AnimateProp
    Private aPRods As AnimateProp
    Private aPistons As AnimateProp
    Private aLevValves As AnimateProp
    Private aValves As AnimateProp
    Private aValvesPist As AnimateProp
    Private aBell As AnimateProp
    Private sLight As AnimateProp

    Private aBrakePads As New AnimatePropHandler
    Private aBrakeBars As AnimateProp
    Private aBrakeLevers As AnimateProp
    Private aBrakePistons As AnimateProp

    Private BellRope As Rope
    Private BellAnimation As AnimationStep
    Private BellAnimationCounter As Single
    Private BellAnimationLength As Integer = 10
    Private BellAnimationChangedDirection As Boolean = True

    Private pWhistle As New PTFX(Particles.sWhistle)
    Private pSteam As New List(Of PTFX)
    Private pSteamVent As New List(Of PTFX)

    Private sTrainStart As AudioPlayer
    Private sTrainMove1 As AudioPlayer
    Private sTrainMove2 As AudioPlayer
    Private sWhistleSound As AudioPlayer
    Private sPistonSteamVentSound As AudioPlayer
    Private sBellSound As AudioPlayer
    Private sTrainMoving As New List(Of AudioPlayer)
    Private sTrainMovingIndex As Integer = -1

    Public Property FunnelSmoke As SmokeColor = SmokeColor.Default
    Friend pFunnelSmoke As New PTFX(Particles.sColoredSmoke)
    Private pFunnelFire As New PTFX(Particles.sFunnelFire)

    Private pTrainExpl As New PTFX(Particles.sTrainExpl)

    Private PistonOldPos As Single
    Private PistonGoingForward As Boolean = False
    Public Sub New(mTrain As Vehicle)

        _ID = RndGenerator.Next

        Select Case mTrain.Model
            Case Models.DMC12ColModel
                ColDeLorean = mTrain
                Locomotive = ColDeLorean.GetTrainCarriage(1)

                ColDeLorean.IsVisible = False
                ColDeLorean.IsCollisionEnabled = False

                Tender = ColDeLorean.GetTrainCarriage(2)

                If IsNothing(Tender) OrElse Tender.Exists = False Then

                    Type = TrainType.NoTender
                Else

                    Type = TrainType.Complete
                End If
            Case Models.RogersSierraColModel
                Locomotive = mTrain

                Tender = Locomotive.GetTrainCarriage(1)

                If IsNothing(Tender) OrElse Tender.Exists = False Then

                    Type = TrainType.OnlyLocomotive
                Else

                    Type = TrainType.NoColDelorean
                End If
        End Select

        Native.Function.Call(Native.Hash.SET_HORN_ENABLED, Locomotive.Handle, False)

        Locomotive.IsVisible = False

        Locomotive.Mods.PrimaryColor = VehicleColor.MetallicShadowSilver
        Locomotive.Mods.SecondaryColor = VehicleColor.MetallicAnthraciteGray

        VisibleLocomotive = World.CreateVehicle(Models.RogersSierraModel, Locomotive.Position)
        VisibleLocomotive.IsCollisionEnabled = False
        VisibleLocomotive.AttachTo(Locomotive)
        VisibleLocomotive.ToggleExtra(1, False)

        VisibleLocomotive.Mods.PrimaryColor = VehicleColor.MetallicShadowSilver
        VisibleLocomotive.Mods.SecondaryColor = VehicleColor.MetallicAnthraciteGray

        PistonRelativePosY = Locomotive.Bones.Item(Bones.sPistons).RelativePosition.Y

        PistonRelativePosZ = Locomotive.Bones.Item(Bones.sPistons).RelativePosition.Z

        TrainProperties.connPointRadius = Locomotive.Bones.Item(Bones.sWheelDrive2).RelativePosition.DistanceTo(Locomotive.Bones.Item(Bones.sRods).RelativePosition) * -1

        TrainProperties.pRodsLength = Locomotive.Bones.Item(Bones.sRodsEnd).RelativePosition.DistanceTo(Locomotive.Bones.Item(Bones.sRods).RelativePosition)

        With aWheels
            WheelRadius = System.Math.Abs(Models.sWheelDrive.Dimensions.frontTopRight.Z)

            .Props.Add(New AnimateProp(Models.sWheelDrive, Locomotive, Bones.sWheelDrive1, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp(Models.sWheelDrive, Locomotive, Bones.sWheelDrive2, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp(Models.sWheelDrive, Locomotive, Bones.sWheelDrive3, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
        End With

        With aSmallWheels
            SmallWheelRadius = System.Math.Abs(Models.sWheelFront.Dimensions.frontTopRight.Z)

            .Props.Add(New AnimateProp(Models.sWheelFront, Locomotive, Bones.sWheelFront1, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp(Models.sWheelFront, Locomotive, Bones.sWheelFront2, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
        End With

        If Type <> TrainType.NoTender AndAlso Type <> TrainType.OnlyLocomotive Then

            With aSmallWheelsTender

                .Props.Add(New AnimateProp(Models.tWheel, Tender, Bones.sWheelTender1, Vector3.Zero, Vector3.Zero))
                aAllProps.Props.Add(.Props.Last)
                .Props.Add(New AnimateProp(Models.tWheel, Tender, Bones.sWheelTender2, Vector3.Zero, Vector3.Zero))
                aAllProps.Props.Add(.Props.Last)
                .Props.Add(New AnimateProp(Models.tWheel, Tender, Bones.sWheelTender3, Vector3.Zero, Vector3.Zero))
                aAllProps.Props.Add(.Props.Last)
                .Props.Add(New AnimateProp(Models.tWheel, Tender, Bones.sWheelTender4, Vector3.Zero, Vector3.Zero))
                aAllProps.Props.Add(.Props.Last)
            End With
        End If

        aRods = New AnimateProp(Models.sRods, Locomotive, Bones.sWheelDrive2, New Vector3(0, TrainProperties.connPointRadius, 0), Vector3.Zero)
        aAllProps.Props.Add(aRods)
        aPRods = New AnimateProp(Models.sPRods, Locomotive, Bones.sWheelDrive2, New Vector3(0, TrainProperties.connPointRadius, 0), Vector3.Zero)
        aAllProps.Props.Add(aPRods)
        aPistons = New AnimateProp(Models.sPistons, Locomotive, Bones.sPistons, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aPistons)

        aLevValves = New AnimateProp(Models.sLevValves, Locomotive, Bones.sLevValves, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aLevValves)

        aValves = New AnimateProp(Models.sValves, Locomotive, Bones.sValves, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aValves)

        aValvesPist = New AnimateProp(Models.sValvesPist, Locomotive, Bones.sValvesPist, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aValvesPist)

        aBell = New AnimateProp(Models.sBell, Locomotive, Bones.sBell, Vector3.Zero, Vector3.Zero, True)
        aBell.setRotationSettings(Coordinate.X, True, True, -70, 70, 2, False, 1, False, 1)
        BellAnimation = AnimationStep.Off
        aAllProps.Props.Add(aBell)

        sLight = New AnimateProp(Models.sLight, Locomotive, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(sLight)

        'With aBrakePads

        '    .Props.Add(New AnimateProp(Models.sBrakePadsFront, Locomotive, Bones.sBrakePadsFront, Vector3.Zero, Vector3.Zero))
        '    .Props.Add(New AnimateProp(Models.sBrakePadsMiddle, Locomotive, Bones.sBrakePadsMiddle, Vector3.Zero, Vector3.Zero))
        '    .Props.Add(New AnimateProp(Models.sBrakePadsRear, Locomotive, Bones.sBrakePadsRear, Vector3.Zero, Vector3.Zero))
        'End With

        'aBrakeBars = New AnimateProp(Models.sBrakeBars, Locomotive, Bones.sBrakeBars, Vector3.Zero, Vector3.Zero)
        'aBrakeLevers = New AnimateProp(Models.sBrakeLevers, Locomotive, Bones.sBrakeLevers, Vector3.Zero, Vector3.Zero)
        'aBrakePistons = New AnimateProp(Models.sBrakePistons, Locomotive, Bones.sBrakePistons, Vector3.Zero, Vector3.Zero)

        AnimationProcess()

        Dim dRopeLength = Locomotive.Bones(Bones.sBellRopeStart).Position.DistanceTo(Locomotive.Bones(Bones.sBellRopeEnd).Position)

        Dim num = dRopeLength + 0.1
        BellRope = World.AddRope(RopeType.ThickRope, Locomotive.Bones(Bones.sBellRopeStart).Position, DirectionToRotation(Locomotive.Bones(Bones.sBellRopeEnd).Position - Locomotive.Bones(Bones.sBellRopeStart).Position, 0.0F), num, System.Math.Min(dRopeLength, num), False)
        BellRope.Connect(Locomotive, Locomotive.Bones(Bones.sBellRopeStart).Position, Locomotive, Locomotive.Bones(Bones.sBellRopeEnd).Position, num)
        BellRope.ActivatePhysics()

        PistonSteam = True

        Dim rightWindowLight = new Light(1.505823, -0.8163379, 2.993675, -0.9864501, 0.09655763, -0.1326379, Color.White, 18, 6, 0, 100, 100)

        Dim leftBoilerLight = new Light(-10.21394, -0.04113722, 5.636269, 0.7291078, 0.6141885, -0.3019508, Color.White, 85, 12, 0, 100, 100)
 
        Dim rightBoilerLight = new Light(11.13646, 0.7100044, 5.407509, -0.7580729, 0.6161212, -0.2138226, Color.White, 85, 12, 0, 100, 100)

        Dim cabLight = new Light(-0.0152235, 8.436013, 3.378627, 0.6139234, 0.7880148, 0.04615904, Color.White, 34, 5, 0, 75, 100)

        LightHandler.Lights.Add(rightWindowLight)
        LightHandler.Lights.Add(leftBoilerLight)
        LightHandler.Lights.Add(rightBoilerLight)
        LightHandler.Lights.Add(cabLight)

        Locomotive.Mods.PrimaryColor = VehicleColor.MetallicStoneSilver
        Locomotive.Mods.SecondaryColor = VehicleColor.MetallicStoneSilver
        'TowardsRail
        CustomCamera.Add(Locomotive, New Vector3(0, 10, 1), New Vector3(0, 20, 1), 75)

        'Pilot
        CustomCamera.Add(Locomotive, New Vector3(0, 12, 0.1), New Vector3(0, 10, 1.1), 75)

        'Front
        CustomCamera.Add(Locomotive, New Vector3(0, 15, 5), New Vector3(0, 0, 0), 75)

        'LeftWheels
        CustomCamera.Add(Locomotive, New Vector3(-2, 6.5, 1), New Vector3(-2, -6.5, 1), 50)

        'LeftFrontWheels
        CustomCamera.Add(Locomotive, New Vector3(-3, -4, 1), New Vector3(-2, 6.5, 1), 50)

        'LeftSide
        CustomCamera.Add(Locomotive, New Vector3(-7.5, 0, 8), New Vector3(0, 0, 0), 75)

        'TopCabin
        CustomCamera.Add(Locomotive, New Vector3(0, -2, 7), New Vector3(0, 7, 5), 75)

        'RightSide
        CustomCamera.Add(Locomotive, New Vector3(7.5, 0, 8), New Vector3(0, 0, 0), 75)

        'RightWheels
        CustomCamera.Add(Locomotive, New Vector3(2, 6.5, 1), New Vector3(2, -6.5, 1), 50)

        'RightFrontWheels
        CustomCamera.Add(Locomotive, New Vector3(3, -4, 1), New Vector3(2, 6.5, 1), 50)

        'Inside
        CustomCamera.Add(Locomotive, New Vector3(0, -2, 2.5), New Vector3(0, 1, 2.5), 75)

        LoadSounds()
    End Sub

    Private Sub LoadSounds()

        AudioEngine.DefaultSourceEntity = Locomotive

        sTrainStart = AudioEngine.Create("trainstart" & _ID, My.Resources.TrainStart, Presets.Exterior)

        sTrainMove1 = AudioEngine.Create("trainmove1" & _ID, My.Resources.TrainMove1, Presets.Exterior)
        sTrainMove1.MinimumDistance = 10

        sTrainMove2 = AudioEngine.Create("trainmove2" & _ID, My.Resources.TrainMove2, Presets.Exterior)
        sTrainMove2.MinimumDistance = 10

        sWhistleSound = AudioEngine.Create("whistle" & _ID, My.Resources.Whistle, Presets.Exterior)
        sWhistleSound.MinimumDistance = 10

        sPistonSteamVentSound = AudioEngine.Create("pistonsteamvent" & _ID, My.Resources.PistonSteamVent, Presets.Exterior)

        sBellSound = AudioEngine.Create("bell" & _ID, My.Resources.Bell, Presets.Exterior)

        sTrainMoving.Clear()

        With sTrainMoving
            .Add(AudioEngine.Create("trainmoving1" & _ID, My.Resources.ambient_moving1, Presets.ExteriorLoop))
            .Last.StartFadeIn = True
            .Last.FadeInMultiplier = 0.7
            .Last.StopFadeOut = True
            .Last.FadeOutMultiplier = 0.7
            .Last.MinimumDistance = 0.5

            .Add(AudioEngine.Create("trainmoving2" & _ID, My.Resources.ambient_moving2, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving3" & _ID, My.Resources.ambient_moving3, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving5" & _ID, My.Resources.ambient_moving5, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving6" & _ID, My.Resources.ambient_moving6, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving7" & _ID, My.Resources.ambient_moving7, Presets.ExteriorLoop))
        End With
    End Sub

    Public Sub Derail()

        Locomotive.MakeTrainDerail()
        BellRope.Delete()

        ForceHandbrake = True
        _isExploded = True
    End Sub

    Public Sub Explode()

        pTrainExpl.Create(Locomotive, Vector3.Zero)

        pTrainExpl.Create(Locomotive, Vector3.Zero, New Vector3(0, 0, 180))

        VisibleLocomotive.Explode()

        Derail()
    End Sub

    Public Function GetBonePosition(boneName As String) As Vector3

        Return Locomotive.Bones(boneName).Position
    End Function

    Public Function GetBoneDistanceSquared(boneName As String, entity As Entity) As Single

        Return Locomotive.Bones(boneName).Position.DistanceToSquared(entity.Position)
    End Function

    Public Function GetBoneDistanceSquared(boneName As String, pos As Vector3) As Single

        Return Locomotive.Bones(boneName).Position.DistanceToSquared(pos)
    End Function

    Public Property Camera As TrainCamera
        Get

            Return CustomCamera.CurrentCameraIndex
        End Get
        Set(value As TrainCamera)

            If value = TrainCamera.Off Then

                CustomCamera.Abort()
            Else

                CustomCamera.Show(value)
            End If
        End Set
    End Property

    Public Property CycleCameras As Boolean
        Get
            Return CustomCamera.CycleCameras
        End Get
        Set(value As Boolean)
            CustomCamera.CycleCameras = value
        End Set
    End Property

    Public Property CycleCamerasInterval As Integer
        Get
            Return CustomCamera.CycleInterval
        End Get
        Set(value As Integer)
            CustomCamera.CycleInterval = value
        End Set
    End Property

    ''' <summary>
    ''' Returns state of main boiler light
    ''' </summary>
    ''' <returns></returns>
    Public Property IsLightOn As Boolean
        Get
            Return sLight.Visible
        End Get
        Set(value As Boolean)
            sLight.Visible = value
        End Set
    End Property

    Public Property Bell As Boolean
        Get
            Return BellAnimation <> AnimationStep.Off
        End Get
        Set(value As Boolean)
            If value Then

                BellAnimation = AnimationStep.First
            End If
        End Set
    End Property

    Public Property Whistle As Boolean
        Get
            Return sWhistleSound.IsAnyInstancePlaying
        End Get
        Set(value As Boolean)
            If value Then

                sWhistleSound.Play()
                pWhistle.CreateLoopedOnEntityBone(Locomotive, Bones.sWhistle, Vector3.Zero)
            Else

                sWhistleSound.Stop()
                pWhistle.Stop()
            End If
        End Set
    End Property

    Public Property FunnelFire As Boolean
        Get
            Return pFunnelFire.Handle <> 0
        End Get
        Set(value As Boolean)
            If value Then

                pFunnelFire.CreateLoopedOnEntityBone(Locomotive, Bones.sFunnel, New Vector3(0, -0.5, -0.25),, 0.75)
                pFunnelFire.setLoopedEvolution("fade", 1)
            Else

                pFunnelFire.Stop()
            End If
        End Set
    End Property

    Friend Property FunnelInterval As Integer

    Friend Property PistonSteam As Boolean
        Get
            Return pSteam.Count > 0
        End Get
        Set(value As Boolean)
            If value Then

                For i = 0 To 3

                    With pSteam

                        .Add(New PTFX(Particles.sSteam))
                        .Last.CreateLoopedOnEntityBone(Locomotive, Bones.sSteam(i), Vector3.Zero, New Vector3(90, 0, 0))
                    End With
                Next
            Else

                pSteam.ForEach(Sub(x)
                                   x.Stop()
                               End Sub)
                pSteam.Clear()
                pWhistle.Stop()
            End If
        End Set
    End Property

    Friend Property PistonSteamVent As Boolean
        Get
            Return sTrainStart.IsAnyInstancePlaying
        End Get
        Set(value As Boolean)
            If value Then

                sTrainStart.Play()

                For i = 0 To 3

                    With pSteamVent

                        .Add(New PTFX(Particles.sWhistle))

                        If i = 1 Or i = 3 Then

                            .Last.CreateLoopedOnEntityBone(Locomotive, Bones.sSteam(i), New Vector3(0, -0, -0.09), New Vector3(0, 90, 0))
                        Else

                            .Last.CreateLoopedOnEntityBone(Locomotive, Bones.sSteam(i), New Vector3(0, 0, -0.09), New Vector3(0, -90, 0))
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

    Private Sub AnimationProcess()

        Dim modifier As Single = If(Locomotive.SpeedMPH <= 10, 1 + (2.5 / 10) * Locomotive.SpeedMPH, 2.5)
        Dim wheelRot As Single = GetAngularSpeedRotation(Locomotive.Speed, WheelRadius, aWheels.Rotation(0).X, Locomotive.isGoingForward, modifier)

        aWheels.AllRotation(Coordinate.X) = wheelRot

        aSmallWheels.AllRotation(Coordinate.X) = GetAngularSpeedRotation(Locomotive.Speed, SmallWheelRadius, aSmallWheels.Rotation(0).X, Locomotive.isGoingForward, modifier)

        If Type <> TrainType.NoTender AndAlso Type <> TrainType.OnlyLocomotive Then

            aSmallWheelsTender.AllRotation(Coordinate.X) = aSmallWheels.AllRotation(Coordinate.X)
        End If

        wheelRot = PositiveAngle(wheelRot)

        Dim dY = System.Math.Cos(DegToRad(wheelRot)) * TrainProperties.connPointRadius
        Dim dZ = System.Math.Sin(DegToRad(wheelRot)) * TrainProperties.connPointRadius

        aRods.Position(Coordinate.Y) = dY
        aRods.Position(Coordinate.Z) = dZ

        aPRods.Position(Coordinate.Y) = dY
        aPRods.Position(Coordinate.Z) = dZ

        Dim dAngle = 90 - RadToDeg(ArcCos((PistonRelativePosZ - aPRods.RelativePosition.Z) / TrainProperties.pRodsLength))

        aPRods.Rotation(Coordinate.X) = dAngle

        aPistons.Position(Coordinate.Y) = TrainProperties.pRodsLength * System.Math.Cos(DegToRad(dAngle)) - (PistonRelativePosY - aPRods.RelativePosition.Y)

        aLevValves.Rotation(Coordinate.X) = (TrainProperties.maxLevValvesRot / TrainProperties.maxPistonPos) * aPistons.Position(Coordinate.Y)

        aValvesPist.Position(Coordinate.Y) = (TrainProperties.minValvesPistPos / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X

        aValves.Position(Coordinate.Y) = aValvesPist.Position(Coordinate.Y)
        aValves.Position(Coordinate.Z) = (TrainProperties.maxValvesPos / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X
        aValves.Rotation(Coordinate.X) = (TrainProperties.minValesRot / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X
    End Sub

    Private Sub AnimationTick()

        If VisibleLocomotive.Position.DistanceToSquared(Game.Player.Character.Position) > 100 * 100 AndAlso Locomotive.Speed = 0 Then

            CheckPropsExists()
        Else

            AnimationProcess()
        End If
    End Sub

    Private Sub ParticlesTick()

        FunnelInterval = 1500 - (1450 / 51) * Locomotive.SpeedMPH

        If FunnelInterval < 50 Then FunnelInterval = 50

        If SmokeTime < Game.GameTime Then

            If FunnelSmoke <> SmokeColor.Off Then

                pFunnelSmoke.CreateOnEntityBone(Locomotive, Bones.sFunnel, New Math.Vector3(0, -1.6, -0.5), New Math.Vector3(90, 0, 0), 1)

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

            SmokeTime = Game.GameTime + FunnelInterval
        End If

        If PistonSteamVent = False AndAlso pSteamVent.Count > 0 Then

            PistonSteam = False
            PistonSteamVent = False
        End If

        If Locomotive.Speed > 0 Then

            If PistonSteam Then

                PistonSteam = False
                PistonSteamVent = True
            End If
        ElseIf PistonSteamVent = False Then

            If PistonSteam = False AndAlso isExploded = False Then

                PistonSteam = True
            End If
        End If

        If Whistle = False AndAlso pWhistle.Handle <> 0 Then

            Whistle = False
        End If

        If Game.IsControlJustPressed(Control.VehicleHorn) AndAlso getCurrentCharacter.IsInVehicle(Locomotive) Then

            Whistle = True
        End If

        If Game.IsControlJustReleased(Control.VehicleHorn) AndAlso Whistle Then

            Whistle = False
        End If

        If Game.IsControlJustPressed(Control.VehicleHandbrake) AndAlso Game.IsControlPressed(Control.CharacterWheel) = False AndAlso Bell = False AndAlso getCurrentCharacter.IsInVehicle(Locomotive) Then

            Bell = True
        End If

        If Game.IsControlJustPressed(Control.VehicleHeadlight) AndAlso getCurrentCharacter.IsInVehicle(Locomotive) Then

            IsLightOn = Not IsLightOn
        End If

        Select Case BellAnimation
            Case AnimationStep.First

                With aBell

                    .Play()

                    If .RotationIncreasing(Coordinate.X) <> BellAnimationChangedDirection Then

                        sBellSound.Play()
                        BellAnimationChangedDirection = Not BellAnimationChangedDirection
                    End If

                    If Game.IsControlPressed(Control.VehicleHandbrake) = False Then

                        .RotationMaxMinRatio(Coordinate.X) = 1 - ((1 / BellAnimationLength) * BellAnimationCounter)
                        .RotationStepRatio(Coordinate.X) = .RotationMaxMinRatio(Coordinate.X)

                        Try
                            sBellSound.Volume = .RotationMaxMinRatio(Coordinate.X)
                        Catch ex As Exception

                        End Try

                        BellAnimationCounter += 1 * Game.LastFrameTime

                        If BellAnimationCounter > BellAnimationLength Then

                            BellAnimationCounter = 0
                            BellAnimation = AnimationStep.Off
                        End If
                    ElseIf Game.IsControlJustPressed(Control.VehicleHandbrake) Then

                        .RotationMaxMinRatio(Coordinate.X) = 1
                        .RotationStepRatio(Coordinate.X) = 1

                        Try
                            sBellSound.Volume = 1
                        Catch ex As Exception

                        End Try

                        BellAnimationCounter = 0
                    End If
                End With
        End Select
    End Sub

    Private Sub SoundsTick()

        If Locomotive.Speed > 0 Then

            Dim newIndex As Integer

            Dim baseVal As Single = 8

            Select Case Locomotive.SpeedMPHf
                Case Is <= baseVal
                    newIndex = 0
                Case Is <= baseVal * 2
                    newIndex = 1
                Case Is <= baseVal * 3
                    newIndex = 2
                Case Is <= baseVal * 4
                    newIndex = 3
                Case Is <= baseVal * 5
                    newIndex = 4
                Case Else
                    newIndex = 5
            End Select

            If newIndex <> sTrainMovingIndex Then

                If sTrainMovingIndex > -1 AndAlso sTrainMoving(sTrainMovingIndex).IsAnyInstancePlaying Then

                    sTrainMoving(sTrainMovingIndex).Stop()
                End If

                sTrainMovingIndex = newIndex
                sTrainMoving(sTrainMovingIndex).Play()
            End If
        Else

            If sTrainMovingIndex > -1 AndAlso sTrainMoving(sTrainMovingIndex).IsAnyInstancePlaying Then

                sTrainMoving(sTrainMovingIndex).Stop()
                sTrainMovingIndex = -1
            End If
        End If

        If PistonSteam = False AndAlso PistonSteamVent = False Then

            If aPistons.Position(Coordinate.Y) > PistonOldPos AndAlso PistonGoingForward = False Then

                Dim tmp = Val(Game.GameTime.ToString.Last) 'RndGenerator.Next(1, 10)

                If tmp <= 4 Then

                    sTrainMove1.Play()
                Else

                    sTrainMove2.Play()
                End If

                PistonGoingForward = True
            End If

            If aPistons.Position(Coordinate.Y) < PistonOldPos AndAlso PistonGoingForward Then

                Dim tmp = Val(Game.GameTime.ToString.Last) 'RndGenerator.Next(1, 10)

                If tmp <= 4 Then

                    sTrainMove1.Play()
                Else

                    sTrainMove2.Play()
                End If

                PistonGoingForward = False
            End If

            PistonOldPos = aPistons.Position(Coordinate.Y)
        End If
    End Sub

    Private Sub TrainSpeedTick()

        If IsNothing(Locomotive.GetPedOnSeat(VehicleSeat.Driver)) = False Then

            If Locomotive.GetPedOnSeat(VehicleSeat.Driver) <> getCurrentCharacter() Then

                Exit Sub
            End If
        End If

        If getCurrentCharacter.IsInVehicle(Locomotive) Then

            If CurrentRogersSierra IsNot Me Then

                CurrentRogersSierra = Me
            End If

            If IsNothing(VisibleLocomotive.AttachedBlip) = False AndAlso VisibleLocomotive.AttachedBlip.Exists Then

                VisibleLocomotive.AttachedBlip.Delete()
            End If

            If Game.IsControlJustPressed(Control.VehicleExit) AndAlso IsVisible = False Then

                If Locomotive.SpeedMPH >= 10 Then

                    Native.Function.Call(Native.Hash.TASK_LEAVE_VEHICLE, getCurrentCharacter, Locomotive, 4160)
                Else

                    Native.Function.Call(Native.Hash.TASK_LEAVE_VEHICLE, getCurrentCharacter, Locomotive, 16)
                End If
            End If

            If isOnTrainMission = False And ForceHandbrake = False Then

                If Game.IsControlJustPressed(Control.VehicleDuck) Then

                    isCruiseControlOn = Not isCruiseControlOn

                    If isCruiseControlOn Then

                        ShowSubtitle("Enabled cruise control")
                    Else

                        ShowSubtitle("Disabled cruise control")
                    End If
                End If

                If Game.IsControlPressed(Control.VehicleAccelerate) Then

                    If LocomotiveSpeed < 0 Then

                        LocomotiveSpeed += 2 * Game.LastFrameTime
                    Else

                        LocomotiveSpeed += System.Math.Pow(Game.GetControlValueNormalized(Control.VehicleAccelerate) / 10, 1 / 3) * Game.LastFrameTime * LocomotiveAccModifier
                    End If
                ElseIf Game.IsControlPressed(Control.VehicleBrake) Then

                    If LocomotiveSpeed() > 0 Then

                        LocomotiveSpeed -= 2 * Game.LastFrameTime
                    Else

                        LocomotiveSpeed -= System.Math.Pow(Game.GetControlValueNormalized(Control.VehicleBrake) / 10, 1 / 3) * Game.LastFrameTime * LocomotiveAccModifier
                    End If
                End If
            End If
        ElseIf IsNothing(VisibleLocomotive.AttachedBlip) OrElse VisibleLocomotive.AttachedBlip.Exists = False Then

            VisibleLocomotive.AddBlip()

            VisibleLocomotive.AttachedBlip.Sprite = 120
        End If

        If getCurrentCharacter.IsInVehicle(Locomotive) = False OrElse (Game.IsControlPressed(Control.VehicleAccelerate) = False AndAlso Game.IsControlPressed(Control.VehicleBrake) = False) Then

            If isCruiseControlOn = False AndAlso isOnTrainMission = False Then

                If LocomotiveSpeed > 0 Then

                    LocomotiveSpeed -= 2 * Game.LastFrameTime

                    If LocomotiveSpeed < 0 Then

                        LocomotiveSpeed = 0
                    End If
                ElseIf LocomotiveSpeed < 0 Then

                    LocomotiveSpeed += 2 * Game.LastFrameTime

                    If LocomotiveSpeed > 0 Then

                        LocomotiveSpeed = 0
                    End If
                End If
            End If
        End If

        If ForceHandbrake Then

            If LocomotiveSpeed > 0 Then

                LocomotiveSpeed -= 10 * Game.LastFrameTime

                If LocomotiveSpeed < 0 Then

                    LocomotiveSpeed = 0
                    isCruiseControlOn = False
                    ForceHandbrake = False
                End If
            ElseIf LocomotiveSpeed < 0 Then

                LocomotiveSpeed += 10 * Game.LastFrameTime

                If LocomotiveSpeed > 0 Then

                    LocomotiveSpeed = 0
                    isCruiseControlOn = False
                    ForceHandbrake = False
                End If
            End If
        End If

        If LocomotiveSpeed > 0 Then

            Dim maxSpeed As Integer = If(isVehicleAttached, 90, 90)

            If MsToMph(LocomotiveSpeed) > maxSpeed Then

                LocomotiveSpeed = MphToMs(maxSpeed)
            End If
        Else

            If MsToMph(LocomotiveSpeed) < -40 Then

                LocomotiveSpeed = MphToMs(-40)
            End If
        End If

        Locomotive.setTrainSpeed(LocomotiveSpeed)
    End Sub

    Public Sub KeyDown(e As Windows.Forms.Keys)

        Select Case e
            Case Windows.Forms.Keys.L
                CustomCamera.ShowNext()
            Case Windows.Forms.Keys.K
                CycleCameras = Not CycleCameras
        End Select
    End Sub

    ''' <summary>
    ''' Where all the magic happens.
    ''' </summary>
    Friend Sub Tick()

        If Locomotive.Exists = False Then

            Delete()

            Exit Sub
        End If

        TrainSpeedTick()

        LightHandler.Draw(Me)

        CustomCamera.Process()

        If IsVisible Then

            AnimationTick()

            ParticlesTick()

            SoundsTick()
        End If

        VisibleLocomotive.Wash()

        VisibleLocomotive.Mods.PrimaryColor = Locomotive.Mods.PrimaryColor
        VisibleLocomotive.Mods.SecondaryColor = Locomotive.Mods.SecondaryColor

        If Type <> TrainType.NoTender AndAlso Type <> TrainType.OnlyLocomotive Then

            Tender.Wash()
        End If

        if Game.Player.Character.IsInVehicle(Locomotive) Then

            'World.DrawSpotLightWithShadow(1.470611, -0.6565136, 3.028564, -0.9940964, 0.02596331, -0.1053485, Color.White, 16, 6, 0, 100, 100);
        End If
    End Sub

    Public Property IsVisible As Boolean
        Get
            Return VisibleLocomotive.IsVisible
        End Get
        Set(value As Boolean)

            VisibleLocomotive.IsVisible = value
            Tender.IsVisible = value

            aAllProps.Visible = value

            ForceHandbrake = Not value

            If value = False Then

                LocomotiveSpeed = 0

                Locomotive.RemoveParticleEffects()
                VisibleLocomotive.RemoveParticleEffects()

                SoundsTick()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Deletes the train from the world.
    ''' </summary>
    Public Sub Delete(Optional deleteVeh As Boolean = True)

        aAllProps.DeleteAll()

        'aBrakePads.DeleteAll()
        'aBrakePistons.Delete()
        'aBrakeLevers.Delete()
        'aBrakeBars.Delete()

        Locomotive.RemoveParticleEffects()
        VisibleLocomotive.RemoveParticleEffects()

        AudioEngine.Dispose()

        VisibleLocomotive.Delete()

        If deleteVeh Then

            If Type <> TrainType.NoTender AndAlso Type <> TrainType.OnlyLocomotive Then

                Tender.Delete()
            End If

            Locomotive.Delete()

            If Type <> TrainType.NoColDelorean AndAlso Type <> TrainType.OnlyLocomotive Then

                ColDeLorean.Delete()
            End If
        End If

        CustomCamera.Abort()

        RemoveRogersSierra(Me)

        _Deleted = True
    End Sub

    Private Sub CheckPropsExists()

        aAllProps.CheckExists()

        'aBrakePads.CheckExists()
        'aBrakeBars.CheckExists()
        'aBrakeLevers.CheckExists()
        'aBrakePistons.CheckExists()
    End Sub

    Public Overrides Function ToString() As String

        Return RogersSierra.IndexOf(Me)
    End Function

    Public Shared Widening Operator CType(ByVal t As cRogersSierra) As Vehicle

        Return t.Locomotive
    End Operator

    Public Shared Operator =(ByVal t As cRogersSierra, ByVal v As Vehicle) As Boolean

        Return t.Locomotive = v
    End Operator

    Public Shared Operator <>(ByVal t As cRogersSierra, ByVal v As Vehicle) As Boolean

        Return t.Locomotive <> v
    End Operator
End Class
