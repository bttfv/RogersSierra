Imports System.Drawing
Imports GTA
Imports GTA.Math
Imports GTA.UI
Imports KlangRageAudioLibrary

Partial Public Class RogersSierra
    ''' <summary>
    ''' Train's audio engine.
    ''' </summary>
    Public ReadOnly AudioEngine As New AudioEngine()
    ''' <summary>
    ''' Train's unique identifier.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ID As Integer
    ''' <summary>
    ''' Points to the Rogers Sierra vehicle.
    ''' </summary>
    Public ReadOnly Locomotive As Vehicle
    ''' <summary>
    ''' Points to the tender.
    ''' </summary>
    Public ReadOnly Tender As Vehicle
    ''' <summary>
    ''' Points to the fake DeLorean on the front.
    ''' </summary>
    Public ReadOnly ColDeLorean As Vehicle
    ''' <summary>
    ''' Points to the visible Rogers Sierra vehicle.
    ''' </summary>
    Public ReadOnly VisibleLocomotive As Vehicle
    ''' <summary>
    ''' Returns <c>true</c> if train is exploded.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public ReadOnly Property IsExploded As Boolean
    ''' <summary>
    ''' Returns the current attached <seealso cref="GTA.Vehicle"/>, if any.
    ''' </summary>
    ''' <returns><seealso cref="GTA.Vehicle"/></returns>
    Public Property AttachedVehicle As Vehicle
    ''' <summary>
    ''' Gets or sets whether train should reject new attach to it.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property RejectAttach As Boolean = True
    ''' <summary>
    ''' Gets or sets whether train should go full speed.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property UnlockSpeed As Boolean
    ''' <summary>
    ''' Gets or sets if train is on a missison.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property IsOnTrainMission As Boolean = False
    ''' <summary>
    ''' Modifier for the train's acceleration.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Private Property LocomotiveAccModifier As Single = 1
    ''' <summary>
    ''' Setted speed of the train.
    ''' </summary>
    ''' <returns><seealso cref="Single"/></returns>
    Public Property LocomotiveSpeed As Single = 0
    ''' <summary>
    ''' Gets or sets current status of handbrake.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property ForceHandbrake As Boolean = False
    ''' <summary>
    ''' Gets or sets current status of cruise control.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public Property IsCruiseControlOn As Boolean = False
    ''' <summary>
    ''' Returns <c>true</c> if train is deleted.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
    Public ReadOnly Property Deleted As Boolean
    ''' <summary>
    ''' Gets or sets current status of wheels on pilot.
    ''' </summary>
    ''' <returns></returns>
    Public Property WheelsOnPilot As Boolean
        Get
            Return VisibleLocomotive.Mods(VehicleModType.Aerials).Index = 0
        End Get
        Set(value As Boolean)
            VisibleLocomotive.Mods(VehicleModType.Aerials).Index = If(value, 0, 1)
        End Set
    End Property

    Public Property RandomTrain As Boolean = True

    Private rejectTimer As Integer = -1

    Public Sub New(mTrain As Vehicle, isRandom As Boolean)

        _ID = RndGenerator.Next

        ColDeLorean = mTrain
        Locomotive = ColDeLorean.GetTrainCarriage(1)

        ColDeLorean.IsVisible = False
        ColDeLorean.IsCollisionEnabled = False

        Tender = ColDeLorean.GetTrainCarriage(2)

        Native.Function.Call(Native.Hash.SET_HORN_ENABLED, Locomotive.Handle, False)

        Locomotive.IsVisible = False

        Locomotive.Mods.PrimaryColor = VehicleColor.MetallicStoneSilver
        Locomotive.Mods.SecondaryColor = VehicleColor.MetallicStoneSilver
        Tender.Mods.PrimaryColor = VehicleColor.MetallicStoneSilver

        VisibleLocomotive = World.CreateVehicle(TrainModels.RogersSierraModel, Locomotive.Position)
        VisibleLocomotive.IsCollisionEnabled = False
        VisibleLocomotive.AttachTo(Locomotive)
        VisibleLocomotive.Mods.InstallModKit()

        Locomotive.IsInvincible = False
        Tender.IsInvincible = False

        RandomTrain = isRandom

        WheelsOnPilot = False

        LoadProps()

        LoadParticles()

        LoadSounds()

        LoadCamera()

        If Not IsNothing(Locomotive.GetPedOnSeat(VehicleSeat.Driver)) Then

            Locomotive.GetPedOnSeat(VehicleSeat.Driver).IsVisible = True
        End If

        If Locomotive.Health = 0 OrElse Tender.Health = 0 Then

            Derail()
        End If
    End Sub

#Region "Public Methods"
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

            Tender.Delete()
            Locomotive.Delete()
            ColDeLorean.Delete()

            'Native.Function.Call(Native.Hash.DELETE_MISSION_TRAIN, ColDeLorean)
        End If

        CustomCamera.Abort()

        RemoveRogersSierra(Me)

        _Deleted = True
    End Sub
    ''' <summary>
    ''' Train derails instantly.
    ''' </summary>
    Public Sub Derail()

        Locomotive.MakeTrainDerail()

        ForceHandbrake = True
        _IsExploded = True
    End Sub
    ''' <summary>
    ''' Train explodes instantly.
    ''' </summary>
    Public Sub Explode()

        pTrainExpl.Create(Locomotive, Vector3.Zero)

        pTrainExpl.Create(Locomotive, Vector3.Zero, New Vector3(0, 0, 180))

        aAllProps.ScatterProps(25)

        VisibleLocomotive.Explode()

        Derail()
    End Sub
    ''' <summary>
    ''' Starts a presto log explosion of <paramref name="Smoke"/> type.
    ''' </summary>
    ''' <param name="Smoke"><seealso cref="SmokeColor"/> of presto log.</param>
    Public Sub PrestoLogExplosion(Smoke As SmokeColor)

        pPrestoLogExpl.CreateOnEntityBone(Locomotive, TrainBones.sFunnel, Vector3.Zero)
        sPrestoLogExpl.Play()

        FunnelSmoke = Smoke

        If Smoke = SmokeColor.Red Then

            FunnelFire = True
            FireboxFireSize = 1
            sFireboxDoor.RotationUpdate(Coordinate.Z) = True
        End If
    End Sub
    ''' <summary>
    ''' Returns the world position of <paramref name="boneName"/>.
    ''' </summary>
    ''' <param name="boneName"><seealso cref="TrainBones"/></param>
    ''' <returns></returns>
    Public Function GetBonePosition(boneName As String) As Vector3

        Return Locomotive.Bones(boneName).Position
    End Function
    ''' <summary>
    ''' Returns the squared distance of <paramref name="entity"/> from world position of <paramref name="boneName"/>.
    ''' </summary>
    ''' <param name="boneName"><seealso cref="TrainBones"/></param>
    ''' <param name="entity"><seealso cref="GTA.Entity"/></param>
    ''' <returns><seealso cref="GTA.Math.Vector3"/></returns>
    Public Function GetBoneDistanceSquared(boneName As String, entity As Entity) As Single

        Return Locomotive.Bones(boneName).Position.DistanceToSquared(entity.Position)
    End Function
    ''' <summary>
    ''' Returns the squared distance of <paramref name="entity"/> from position <paramref name="pos"/>.
    ''' </summary>
    ''' <param name="boneName"><seealso cref="TrainBones"/></param>
    ''' <param name="pos"><seealso cref="GTA.Math.Vector3"/></param>
    ''' <returns><seealso cref="GTA.Math.Vector3"/></returns>
    Public Function GetBoneDistanceSquared(boneName As String, pos As Vector3) As Single

        Return Locomotive.Bones(boneName).Position.DistanceToSquared(pos)
    End Function

    Public Sub SetRejectDelay(Optional delay As Integer = 0)

        rejectTimer = Game.GameTime + delay
        RejectAttach = True
    End Sub
#End Region

    ''' <summary>
    ''' Gets or sets a value indicating whether train is visible.
    ''' </summary>
    ''' <returns><seealso cref="Boolean"/></returns>
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

    Friend Sub Tick()

        If Locomotive.Exists = False Then

            Delete()

            Exit Sub
        End If

        If (VisibleLocomotive.Health = 0 Or Locomotive.Health = 0 Or Tender.Health = 0) AndAlso Not IsExploded Then

            Explode()
        End If

        If rejectTimer > 0 AndAlso rejectTimer <= Game.GameTime Then

            rejectTimer = -1
            RejectAttach = False
        End If

        TrainSpeedTick()

        CustomLights.Draw()

        CustomCamera.Process()

        If IsVisible Then

            AnimationTick()

            ParticlesTick()

            SoundsTick()
        End If

        VisibleLocomotive.DirtLevel = 0

        VisibleLocomotive.Mods.PrimaryColor = Locomotive.Mods.PrimaryColor
        VisibleLocomotive.Mods.SecondaryColor = Locomotive.Mods.SecondaryColor

        Tender.Mods.PrimaryColor = Locomotive.Mods.PrimaryColor
        Tender.DirtLevel = 0

        'If Not PlayerPed.IsInVehicle AndAlso Not WheelsOnPilot Then

        '    Dim tmpPos = Locomotive.GetOffsetPosition(New Vector3(0, TrainModels.RogersSierraModel.Dimensions.frontTopRight.Y, 0.5))

        '    If PlayerPed.Position.DistanceToSquared(tmpPos) < 1.5 * 1.5 Then

        '        UI.Screen.ShowHelpTextThisFrame(Game.GetLocalizedString("RogersSierra_Help_InstallWheelsOnPilot"))

        '        If Game.IsControlJustPressed(Control.Context) Then

        '            WheelsOnPilot = True
        '            RejectAttach = False
        '        End If
        '    End If
        'End If
    End Sub

    Public Sub KeyDown(e As Windows.Forms.Keys)

        Select Case e
            Case Windows.Forms.Keys.L
                CustomCamera.ShowNext()
            Case Windows.Forms.Keys.K
                Camera = TrainCamera.Off
        End Select
    End Sub

    Private Sub TrainSpeedTick()

        If RandomTrain Then

            If PlayerPed.IsInVehicle(Locomotive) Then

                Locomotive.setTrainCruiseSpeed(0)
                LocomotiveSpeed = Locomotive.Speed

                RandomTrain = False
            Else

                Exit Sub
            End If
        End If

        If PlayerPed.IsInVehicle(Locomotive) Then

            If CurrentRogersSierra IsNot Me Then

                CurrentRogersSierra = Me
            End If

            If IsNothing(VisibleLocomotive.AttachedBlip) = False AndAlso VisibleLocomotive.AttachedBlip.Exists Then

                VisibleLocomotive.AttachedBlip.Delete()
            End If

            If Game.IsControlJustPressed(Control.VehicleExit) AndAlso IsVisible Then

                PlayerPed.Task.LeaveVehicle()
            End If

            If IsOnTrainMission = False And ForceHandbrake = False Then

                If Game.IsControlJustPressed(Control.VehicleDuck) Then

                    IsCruiseControlOn = Not IsCruiseControlOn

                    If IsCruiseControlOn Then

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
            VisibleLocomotive.AttachedBlip.Name = "Rogers Sierra No. 3"
        End If

        If PlayerPed.IsInVehicle(Locomotive) = False OrElse (Game.IsControlPressed(Control.VehicleAccelerate) = False AndAlso Game.IsControlPressed(Control.VehicleBrake) = False) Then

            If IsCruiseControlOn = False AndAlso IsOnTrainMission = False Then

                If LocomotiveSpeed > 0 Then

                    LocomotiveSpeed -= 0.2 * Game.LastFrameTime

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
                    IsCruiseControlOn = False
                    ForceHandbrake = False
                End If
            ElseIf LocomotiveSpeed < 0 Then

                LocomotiveSpeed += 10 * Game.LastFrameTime

                If LocomotiveSpeed > 0 Then

                    LocomotiveSpeed = 0
                    IsCruiseControlOn = False
                    ForceHandbrake = False
                End If
            End If
        End If

        If LocomotiveSpeed > 0 Then

            Dim maxSpeed As Integer = If(UnlockSpeed, 90, 51)

            If MsToMph(LocomotiveSpeed) > maxSpeed Then

                LocomotiveSpeed = MphToMs(maxSpeed)
            End If
        Else

            If MsToMph(LocomotiveSpeed) < -40 Then

                LocomotiveSpeed = MphToMs(-40)
            End If
        End If

        Locomotive.setTrainSpeed(LocomotiveSpeed)

        If LocomotiveSpeed = 0 AndAlso Locomotive.Speed > 0 Then

            Locomotive.setTrainCruiseSpeed(0)
        End If
    End Sub

#Region "Overrides and Operators"
    Public Overrides Function ToString() As String

        Return RogersSierraList.IndexOf(Me)
    End Function

    Public Shared Widening Operator CType(ByVal t As RogersSierra) As Vehicle

        Return t.Locomotive
    End Operator

    Public Shared Widening Operator CType(ByVal t As RogersSierra) As Entity

        Return t.Locomotive
    End Operator
#End Region
End Class
