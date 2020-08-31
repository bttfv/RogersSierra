Imports GTA
Imports GTA.Math
Imports RogersSierra
Imports KlangRageAudioLibrary

Public Class cRogersSierra

    Private SmokeScript As cSmokeScript

    Public AudioEngine As New AudioEngine()

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

    Private _isExploded As Boolean = False

    Public Property isExploded As Boolean
        Get
            Return _isExploded
        End Get
        Friend Set(value As Boolean)
            _isExploded = value
        End Set
    End Property

    ''' <summary>
    ''' Points to the real DeLorean, if it is attached.
    ''' </summary>
    Public AttachedDeLorean As Vehicle

    ''' <summary>
    ''' If true, there is a DeLorean attached on the front.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property isDeLoreanAttached As Boolean = AttachedDeLorean <> Nothing

    ''' <summary>
    ''' If true, the train is in Rocket mode.
    ''' </summary>
    ''' <returns></returns>
    Public Property isOnTrainMission As Boolean = False

    ''' <summary>
    ''' Modifier for the train's acceleration.
    ''' </summary>
    ''' <returns></returns>
    Friend Property LocomotiveAccModifier As Single = 4

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

    Private WheelRadius As Single
    Private SmallWheelRadius As Single

    Private PistonRelativePosY As Single
    Private PistonRelativePosZ As Single

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

        ColDeLorean = mTrain
        ColDeLorean.IsVisible = False
        ColDeLorean.IsCollisionEnabled = False

        Locomotive = ColDeLorean.GetTrainCarriage(1)
        Native.Function.Call(Native.Hash.SET_HORN_ENABLED, Locomotive.Handle, False)
        Locomotive.IsVisible = False

        Tender = ColDeLorean.GetTrainCarriage(2)

        VisibleLocomotive = World.CreateVehicle(Models.RogersSierraModel, Locomotive.Position)
        VisibleLocomotive.IsCollisionEnabled = False
        VisibleLocomotive.AttachTo(Locomotive)
        VisibleLocomotive.ToggleExtra(1, False)

        PistonRelativePosY = Locomotive.Bones.Item(Bones.sPistons).RelativePosition.Y

        PistonRelativePosZ = Locomotive.Bones.Item(Bones.sPistons).RelativePosition.Z

        With aWheels
            WheelRadius = System.Math.Abs(Models.sWheelDrive.Dimensions.frontTopRight.Z)

            .Props.Add(New AnimateProp(Models.sWheelDrive, Locomotive, Bones.sWheelDrive1, Math.Vector3.Zero, Math.Vector3.Zero))
            .Props.Add(New AnimateProp(Models.sWheelDrive, Locomotive, Bones.sWheelDrive2, Math.Vector3.Zero, Math.Vector3.Zero))
            .Props.Add(New AnimateProp(Models.sWheelDrive, Locomotive, Bones.sWheelDrive3, Math.Vector3.Zero, Math.Vector3.Zero))

            .Props.Add(New AnimateProp(Models.sWheelDrive, Locomotive, Bones.sWheelDrive1, Math.Vector3.Zero, Math.Vector3.Zero))
            .Props.Add(New AnimateProp(Models.sWheelDrive, Locomotive, Bones.sWheelDrive2, Math.Vector3.Zero, Math.Vector3.Zero))
            .Props.Add(New AnimateProp(Models.sWheelDrive, Locomotive, Bones.sWheelDrive3, Math.Vector3.Zero, Math.Vector3.Zero))
        End With

        With aSmallWheels
            SmallWheelRadius = System.Math.Abs(Models.sWheelFront.Dimensions.frontTopRight.Z)

            .Props.Add(New AnimateProp(Models.sWheelFront, Locomotive, Bones.sWheelFront1, Math.Vector3.Zero, Math.Vector3.Zero))
            .Props.Add(New AnimateProp(Models.sWheelFront, Locomotive, Bones.sWheelFront2, Math.Vector3.Zero, Math.Vector3.Zero))

            .Props.Add(New AnimateProp(Models.sWheelFront, Locomotive, Bones.sWheelFront1, Math.Vector3.Zero, Math.Vector3.Zero))
            .Props.Add(New AnimateProp(Models.sWheelFront, Locomotive, Bones.sWheelFront2, Math.Vector3.Zero, Math.Vector3.Zero))
        End With

        'With aSmallWheelsTender
        '    .Props.Add(New AnimateProp(Models.sWheelTenderLeft, Tender, Bones.sWheelTenderFront1Left, Math.Vector3.Zero, Math.Vector3.Zero))
        '    .Props.Add(New AnimateProp(Models.sWheelTenderLeft, Tender, Bones.sWheelTenderFront2Left, Math.Vector3.Zero, Math.Vector3.Zero))

        '    .Props.Add(New AnimateProp(Models.sWheelTenderLeft, Tender, Bones.sWheelTenderRear1Left, Math.Vector3.Zero, Math.Vector3.Zero))
        '    .Props.Add(New AnimateProp(Models.sWheelTenderLeft, Tender, Bones.sWheelTenderRear2Left, Math.Vector3.Zero, Math.Vector3.Zero))

        '    .Props.Add(New AnimateProp(Models.sWheelTenderRight, Tender, Bones.sWheelTenderFront1Right, Math.Vector3.Zero, Math.Vector3.Zero))
        '    .Props.Add(New AnimateProp(Models.sWheelTenderRight, Tender, Bones.sWheelTenderFront2Right, Math.Vector3.Zero, Math.Vector3.Zero))

        '    .Props.Add(New AnimateProp(Models.sWheelTenderRight, Tender, Bones.sWheelTenderRear1Right, Math.Vector3.Zero, Math.Vector3.Zero))
        '    .Props.Add(New AnimateProp(Models.sWheelTenderRight, Tender, Bones.sWheelTenderRear2Right, Math.Vector3.Zero, Math.Vector3.Zero))
        'End With

        aRods = New AnimateProp(Models.sRods, Locomotive, Bones.sRods, New Math.Vector3(0, TrainProperties.connPointRadius, 0), New Math.Vector3(90, 0, 0))
        aPRods = New AnimateProp(Models.sPRods, Locomotive, Bones.sRods, New Math.Vector3(0, TrainProperties.connPointRadius, 0), New Math.Vector3(0, 0, 0))

        aPistons = New AnimateProp(Models.sPistons, Locomotive, Bones.sPistons, Math.Vector3.Zero, Math.Vector3.Zero)

        aLevValves = New AnimateProp(Models.sLevValves, Locomotive, Bones.sLevValves, Math.Vector3.Zero, Math.Vector3.Zero)

        aValves = New AnimateProp(Models.sValves, Locomotive, Bones.sValves, Math.Vector3.Zero, Vector3.Zero)

        aValvesPist = New AnimateProp(Models.sValvesPist, Locomotive, Bones.sValvesPist, Math.Vector3.Zero, Math.Vector3.Zero)

        aBell = New AnimateProp(Models.sBell, Locomotive, Bones.sBell, Math.Vector3.Zero, Math.Vector3.Zero, True)
        aBell.setRotationSettings(Coordinate.X, True, True, -70, 70, 2, False, 1, False, 1)
        BellAnimation = AnimationStep.Off

        'With aBrakePads

        '    .Props.Add(New AnimateProp(Models.sBrakePadsFront, Locomotive, Bones.sBrakePadsFront, Math.Vector3.Zero, Math.Vector3.Zero))
        '    .Props.Add(New AnimateProp(Models.sBrakePadsMiddle, Locomotive, Bones.sBrakePadsMiddle, Math.Vector3.Zero, Math.Vector3.Zero))
        '    .Props.Add(New AnimateProp(Models.sBrakePadsRear, Locomotive, Bones.sBrakePadsRear, Math.Vector3.Zero, Math.Vector3.Zero))
        'End With

        'aBrakeBars = New AnimateProp(Models.sBrakeBars, Locomotive, Bones.sBrakeBars, Math.Vector3.Zero, Math.Vector3.Zero)
        'aBrakeLevers = New AnimateProp(Models.sBrakeLevers, Locomotive, Bones.sBrakeLevers, Math.Vector3.Zero, Math.Vector3.Zero)
        'aBrakePistons = New AnimateProp(Models.sBrakePistons, Locomotive, Bones.sBrakePistons, Math.Vector3.Zero, Math.Vector3.Zero)

        AnimationProcess()

        Dim dRopeLength = Locomotive.Bones(Bones.sBellRopeStart).Position.DistanceTo(Locomotive.Bones(Bones.sBellRopeEnd).Position)

        Dim num = dRopeLength + 0.1
        BellRope = World.AddRope(RopeType.ThickRope, Locomotive.Bones(Bones.sBellRopeStart).Position, DirectionToRotation(Locomotive.Bones(Bones.sBellRopeEnd).Position - Locomotive.Bones(Bones.sBellRopeStart).Position, 0.0F), num, System.Math.Min(dRopeLength, num), False)
        BellRope.Connect(Locomotive, Locomotive.Bones(Bones.sBellRopeStart).Position, Locomotive, Locomotive.Bones(Bones.sBellRopeEnd).Position, num)
        BellRope.ActivatePhysics()

        PistonSteam = True

        SmokeScript = Script.InstantiateScript(Of cSmokeScript)

        LoadSounds()
    End Sub

    Private Sub LoadSounds()

        AudioEngine.DefaultSourceEntity = Locomotive

        sTrainStart = AudioEngine.Create("trainstart", My.Resources.TrainStart, Presets.Exterior)

        sTrainMove1 = AudioEngine.Create("trainmove1", My.Resources.TrainMove1, Presets.Exterior)

        sTrainMove2 = AudioEngine.Create("trainmove2", My.Resources.TrainMove2, Presets.Exterior)

        sWhistleSound = AudioEngine.Create("whistle", My.Resources.Whistle, Presets.Exterior)

        sPistonSteamVentSound = AudioEngine.Create("pistonsteamvent", My.Resources.PistonSteamVent, Presets.Exterior)

        sBellSound = AudioEngine.Create("bell", My.Resources.Bell, Presets.Exterior)

        sTrainMoving.Clear()

        With sTrainMoving
            .Add(AudioEngine.Create("trainmoving1", My.Resources.ambient_moving1, Presets.ExteriorLoop))
            .Last.StartFadeIn = True
            .Last.FadeInMultiplier = 0.7
            .Last.StopFadeOut = True
            .Last.FadeOutMultiplier = 0.7

            .Add(AudioEngine.Create("trainmoving2", My.Resources.ambient_moving2, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving3", My.Resources.ambient_moving3, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving5", My.Resources.ambient_moving5, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving6", My.Resources.ambient_moving6, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving7", My.Resources.ambient_moving7, Presets.ExteriorLoop))
        End With
    End Sub

    Public Sub Explode()

        pTrainExpl.Create(Locomotive, Math.Vector3.Zero)

        pTrainExpl.Create(Locomotive, Math.Vector3.Zero, New Math.Vector3(0, 0, 180))

        ForceHandbrake = True
        VisibleLocomotive.Explode()
        Locomotive.MakeTrainDerail()
        BellRope.Delete()
        isExploded = True
    End Sub

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
                pWhistle.CreateLoopedOnEntityBone(Locomotive, Bones.sWhistle, Math.Vector3.Zero)
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

                pFunnelFire.CreateLoopedOnEntityBone(Locomotive, Bones.sFunnel, New Math.Vector3(0, -0.5, -0.25),, 0.75)
                pFunnelFire.setLoopedEvolution("fade", 1)
            Else

                pFunnelFire.Stop()
            End If
        End Set
    End Property

    Friend Property FunnelInterval As Integer
        Get
            Return SmokeScript.Interval
        End Get
        Set(value As Integer)
            SmokeScript.Interval = value
        End Set
    End Property

    Friend Property PistonSteam As Boolean
        Get
            Return pSteam.Count > 0
        End Get
        Set(value As Boolean)
            If value Then

                For i = 0 To 3

                    With pSteam

                        .Add(New PTFX(Particles.sSteam))
                        .Last.CreateLoopedOnEntityBone(Locomotive, Bones.sSteam(i), Math.Vector3.Zero, New Math.Vector3(90, 0, 0))
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

                            .Last.CreateLoopedOnEntityBone(Locomotive, Bones.sSteam(i), New Math.Vector3(-0.285, -0, -0.39), New Math.Vector3(0, 90, 0))
                        Else

                            .Last.CreateLoopedOnEntityBone(Locomotive, Bones.sSteam(i), New Math.Vector3(0.285, 0, -0.39), New Math.Vector3(0, -90, 0))
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

        Dim modifier As Single = If(Locomotive.Speedmph <= 10, 1 + (2.5 / 10) * Locomotive.SpeedMPH, 2.5)
        Dim wheelRot As Single = GetAngularSpeedRotation(Locomotive.Speed, WheelRadius, aWheels.Rotation(0).X, Locomotive.isGoingForward, modifier)

        aWheels.AllRotation(Coordinate.X) = wheelRot

        aSmallWheels.AllRotation(Coordinate.X) = GetAngularSpeedRotation(Locomotive.Speed, SmallWheelRadius, aSmallWheels.Rotation(0).X, Locomotive.isGoingForward, modifier)

        aSmallWheelsTender.AllRotation(Coordinate.X) = aSmallWheels.AllRotation(Coordinate.X)

        wheelRot = PositiveAngle(wheelRot)

        Dim dY = System.Math.Cos(DegToRad(wheelRot)) * TrainProperties.connPointRadius
        Dim dZ = System.Math.Sin(DegToRad(wheelRot)) * TrainProperties.connPointRadius

        aRods.Position(Coordinate.Y) = dY
        aRods.Position(Coordinate.Z) = dZ

        aPRods.Position(Coordinate.Y) = dY
        aPRods.Position(Coordinate.Z) = dZ

        Dim dAngle = 175 - RadToDeg(ArcCos((PistonRelativePosZ - aPRods.RelativePosition.Z) / TrainProperties.pRodsLength))

        aPRods.Rotation(Coordinate.X) = dAngle

        aPistons.Position(Coordinate.Y) = TrainProperties.pRodsLength * System.Math.Cos(DegToRad(dAngle)) - (PistonRelativePosY - aPRods.RelativePosition.Y)

        aLevValves.Rotation(Coordinate.X) = 155 + (TrainProperties.maxLevValvesRot / TrainProperties.maxPistonPos) * aPistons.Position(Coordinate.Y)

        aValvesPist.Position(Coordinate.Y) = (TrainProperties.minValvesPistPos / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X

        aValves.Position(Coordinate.Y) = aValvesPist.Position(Coordinate.Y) + 5
        aValves.Position(Coordinate.Z) = (TrainProperties.maxValvesPos / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X
        aValves.Rotation(Coordinate.X) = (TrainProperties.minValesRot / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X
    End Sub

    Private Sub AnimationTick()

        If Locomotive.Speed = 0 Then

            CheckPropsExists()
        Else

            AnimationProcess()
        End If
    End Sub

    Private Sub ParticlesTick()

        Dim tmpInterval = 1500 - (1450 / 51) * Locomotive.SpeedMPH

        If tmpInterval < 50 Then tmpInterval = 50

        FunnelInterval = tmpInterval

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

    Friend Sub SoundsTick()

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

        If getCurrentCharacter.IsInVehicle(Locomotive) Then

            If Game.IsControlJustPressed(Control.VehicleExit) Then

                If Locomotive.SpeedMPH >= 10 Then

                    Native.Function.Call(Native.Hash.TASK_LEAVE_VEHICLE, getCurrentCharacter, Locomotive, 4160)
                Else

                    Native.Function.Call(Native.Hash.TASK_LEAVE_VEHICLE, getCurrentCharacter, Locomotive, 0)
                End If
            End If

            If isOnTrainMission = False Then

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

            Dim maxSpeed As Integer = If(isDeLoreanAttached, 90, 51)

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

    ''' <summary>
    ''' Where all the magic happens.
    ''' </summary>
    Friend Sub Tick()

        If Locomotive.Exists = False Then

            Delete()

            Exit Sub
        End If

        TrainSpeedTick()

        AnimationTick()

        ParticlesTick()

        SoundsTick()
    End Sub

    ''' <summary>
    ''' Deletes the train from the world.
    ''' </summary>
    Public Sub Delete()

        aWheels.DeleteAll()
        aSmallWheels.DeleteAll()
        aSmallWheelsTender.DeleteAll()
        aRods.Delete()
        aPRods.Delete()
        aPistons.Delete()
        aLevValves.Delete()
        aValves.Delete()
        aValvesPist.Delete()
        aBell.Delete()

        aBrakePads.DeleteAll()
        aBrakePistons.Delete()
        aBrakeLevers.Delete()
        aBrakeBars.Delete()

        BellRope.Delete()

        Locomotive.RemoveParticleEffects()
        VisibleLocomotive.RemoveParticleEffects()

        AudioEngine.Dispose()

        Tender.Delete()
        Locomotive.Delete()
        ColDeLorean.Delete()
        VisibleLocomotive.Delete()

        RogersSierra = Nothing
    End Sub

    Private Sub CheckPropsExists()

        aWheels.CheckExists()
        aSmallWheels.CheckExists()
        aSmallWheelsTender.CheckExists()

        aRods.CheckExists()
        aPRods.CheckExists()
        aPistons.CheckExists()
        aLevValves.CheckExists()
        aValves.CheckExists()
        aValvesPist.CheckExists()
        aBell.CheckExists()

        aBrakePads.CheckExists()
        aBrakeBars.CheckExists()
        aBrakeLevers.CheckExists()
        aBrakePistons.CheckExists()
    End Sub
End Class
