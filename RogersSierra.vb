﻿Imports FusionLibrary
Imports FusionLibrary.Extensions
Imports GTA
Imports GTA.Math
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
    Public Property LocomotiveAccModifier As Single = 1
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
    Private speedUnlocked As Boolean

    Public Sub New(mTrain As Vehicle, isRandom As Boolean)

        _ID = FusionUtils.Random.Next

        ColDeLorean = mTrain
        Locomotive = ColDeLorean.GetTrainCarriage(1)
        Locomotive.Decorator.DotNotDelete = True

        ColDeLorean.IsVisible = False
        ColDeLorean.IsCollisionEnabled = False
        ColDeLorean.Decorator().DotNotDelete = True

        Tender = ColDeLorean.GetTrainCarriage(2)
        Tender.Decorator.DotNotDelete = True

        Native.Function.Call(Native.Hash.SET_HORN_ENABLED, Locomotive.Handle, False)

        Locomotive.IsVisible = False

        VisibleLocomotive = World.CreateVehicle(TrainModels.RogersSierraModel, Locomotive.Position)
        VisibleLocomotive.IsCollisionEnabled = False
        VisibleLocomotive.AttachTo(Locomotive)
        VisibleLocomotive.Mods.InstallModKit()

        VisibleLocomotive.Mods.PrimaryColor = VehicleColor.MetallicStoneSilver
        VisibleLocomotive.Mods.SecondaryColor = VehicleColor.MetallicStoneSilver
        Tender.Mods.PrimaryColor = VehicleColor.MetallicStoneSilver
        Tender.Mods.SecondaryColor = VehicleColor.MetallicStoneSilver

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
    End Sub

#Region "Public Methods"
    ''' <summary>
    ''' Deletes the train from the world.
    ''' </summary>
    Public Sub Delete(Optional deleteVeh As Boolean = True)

        aAllProps.Dispose()

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
        End If

        CustomCamera.Abort()

        RemoveRogersSierra(Me)

        _Deleted = True
    End Sub
    ''' <summary>
    ''' Train derails instantly.
    ''' </summary>
    Public Sub Derail()

        Locomotive.Derail()

        ForceHandbrake = True
        _IsExploded = True
    End Sub
    ''' <summary>
    ''' Train explodes instantly.
    ''' </summary>
    Public Sub Explode()

        pTrainExpl.Create(Locomotive, Vector3.Zero)

        pTrainExpl.Create(Locomotive, Vector3.Zero, New Vector3(0, 0, 180))

        aAllProps.ScatterProp(25)

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
        End If
    End Sub
    ''' <summary>
    ''' Opens firebox door and increase fire size.
    ''' </summary>
    Public Sub ExplodeFirebox()

        FireboxFireSize = 1
        sFireboxDoor.Play()
    End Sub
    ''' <summary>
    ''' Set a <paramref name="delay"/> for permit attach to train again.
    ''' </summary>
    ''' <param name="delay"><seealso cref="Integer"/></param>
    Public Sub SetRejectDelay(delay As Integer)

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

            For Each ped In Locomotive.Occupants

                ped.IsVisible = value
            Next

            If value = False Then

                LocomotiveSpeed = 0

                Locomotive.RemoveParticleEffects()
                VisibleLocomotive.RemoveParticleEffects()

                StopAllSounds()
            End If
        End Set
    End Property

    Friend Sub Tick()

        If Locomotive.Exists = False Then

            Delete()

            Exit Sub
        End If

        If (VisibleLocomotive.Health = 0 Or Locomotive.Health = 0 Or Tender.Health = 0) AndAlso Not IsExploded Then

            If IsVisible Then

                Explode()
            Else

                VisibleLocomotive.Health = VisibleLocomotive.MaxHealth
                Locomotive.Health = Locomotive.MaxHealth
                Tender.Health = Tender.MaxHealth
            End If
        End If

        If rejectTimer > 0 AndAlso rejectTimer <= Game.GameTime Then

            rejectTimer = -1
            RejectAttach = False
        End If

        TrainSpeedTick()

        If IsVisible Then

            CustomLights.Draw()
        End If

        CustomCamera.Tick()

        If IsVisible Then

            AnimationTick()

            ParticlesTick()

            SoundsTick()
        End If

        VisibleLocomotive.DirtLevel = 0

        ' TEMPORARY DISABLED BECAUSE IT DOESNT FUCKING WORK IT ASSIGNS RANDOM COLORS!!!
        'VisibleLocomotive.Mods.PrimaryColor = Locomotive.Mods.PrimaryColor
        'VisibleLocomotive.Mods.SecondaryColor = Locomotive.Mods.SecondaryColor

        Tender.DirtLevel = 0

        If Not FusionUtils.PlayerPed.IsInVehicle AndAlso Not WheelsOnPilot AndAlso Not IsExploded Then

            Dim tmpPos = Locomotive.GetOffsetPosition(New Vector3(0, TrainModels.RogersSierraModel.Model.Dimensions.frontTopRight.Y, 0.5))

            If FusionUtils.PlayerPed.DistanceToSquared2D(tmpPos, 1.5) Then

                TextHandler.ShowHelp("WheelsOnPilot")

                If Game.IsControlJustPressed(Control.Context) Then

                    WheelsOnPilot = True
                    RejectAttach = False
                End If
            End If
        End If
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

            If FusionUtils.PlayerPed.IsInVehicle(Locomotive) Then

                Locomotive.SetTrainCruiseSpeed(0)
                LocomotiveSpeed = Locomotive.Speed

                RandomTrain = False
            Else

                Exit Sub
            End If
        End If

        If FusionUtils.PlayerPed.IsInVehicle(Locomotive) Then

            If CurrentRogersSierra IsNot Me Then

                CurrentRogersSierra = Me

                FusionUtils.PlayerPed.Task.PlayAnimation("amb@code_human_in_bus_passenger_idles@female@sit@base", "base", 900, -1, AnimationFlags.Loop)
            End If

            If IsNothing(VisibleLocomotive.AttachedBlip) = False AndAlso VisibleLocomotive.AttachedBlip.Exists Then

                VisibleLocomotive.AttachedBlip.Delete()
            End If

            If Game.IsControlJustPressed(Control.VehicleExit) AndAlso IsVisible Then

                Dim exitTask As New TaskSequence()

                exitTask.AddTask().ClearAnimation("amb@code_human_in_bus_passenger_idles@female@sit@base", "base")
                exitTask.AddTask().LeaveVehicle()

                FusionUtils.PlayerPed.Task.PerformSequence(exitTask)
            End If

            If IsOnTrainMission = False And ForceHandbrake = False Then

                If Game.IsControlJustPressed(Control.VehicleDuck) Then

                    IsCruiseControlOn = Not IsCruiseControlOn

                    TextHandler.ShowHelp("CruiseControl", True, If(IsCruiseControlOn, "Enabled", "Disabled"))
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

        If FusionUtils.PlayerPed.IsInVehicle(Locomotive) = False OrElse (Game.IsControlPressed(Control.VehicleAccelerate) = False AndAlso Game.IsControlPressed(Control.VehicleBrake) = False) Then

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

        If IsOnTrainMission = False Then

            If UnlockSpeed AndAlso Not speedUnlocked AndAlso Locomotive.GetMPHSpeed > 51 Then

                speedUnlocked = True
            End If

            If UnlockSpeed AndAlso speedUnlocked AndAlso Locomotive.GetMPHSpeed < 49 Then

                UnlockSpeed = False
                speedUnlocked = False
            End If
        End If

        If LocomotiveSpeed > 0 Then

            Dim maxSpeed As Integer = If(UnlockSpeed, 90, 51)

            If MathExtensions.ToMPH(LocomotiveSpeed) > maxSpeed Then

                LocomotiveSpeed = MathExtensions.ToMS(maxSpeed)
            End If
        Else

            If MathExtensions.ToMPH(LocomotiveSpeed) < -40 Then

                LocomotiveSpeed = MathExtensions.ToMS(-40)
            End If
        End If

        If LocomotiveSpeed > 0 Then

            If sCabCols.SecondOffset = Vector3.Zero AndAlso sCabCols.IsPlaying = False Then

                sCabCols.SecondOffset = New Vector3(0, 0.75, 0)
                sCabCols.Play()
            End If
        ElseIf sCabCols.SecondOffset <> Vector3.Zero Then

            sCabCols.Stop()
            sCabCols.MoveProp(Vector3.Zero, Vector3.Zero, False)
        End If

        Locomotive.SetTrainSpeed(LocomotiveSpeed)

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
