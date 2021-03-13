Imports FusionLibrary
Imports FusionLibrary.Enums
Imports FusionLibrary.Extensions
Imports GTA
Imports GTA.Math

Partial Public Class RogersSierra

    Private WheelRadius As Single
    Private SmallWheelRadius As Single

    Private PistonRelativePosY As Single
    Private PistonRelativePosZ As Single

    Private aAllProps As New AnimatePropsHandler

    Private aSmallWheelsTender As New AnimatePropsHandler

    Private aWheels As New AnimatePropsHandler
    Private aSmallWheels As New AnimatePropsHandler
    Private aRods As AnimateProp
    Private aPRods As AnimateProp

    Private aPistons As AnimateProp
    Private PistonGoingForward As Boolean = False

    Private aLevValves As AnimateProp
    Private aValves As AnimateProp
    Private aValvesPist As AnimateProp

    Private aBell As AnimateProp
    Private BellAnimationCounter As Single
    Private BellAnimationLength As Integer = 10
    Private BellAnimationChangedDirection As Boolean = True

    Private sLight As AnimateProp
    Private sCabCols As AnimateProp
    Private sFireboxDoor As AnimateProp

    'Private aBrakePads As New AnimatePropHandler
    'Private aBrakeBars As AnimateProp
    'Private aBrakeLevers As AnimateProp
    'Private aBrakePistons As AnimateProp

    Public Property Bell As Boolean
        Get
            Return aBell.IsPlaying
        End Get
        Set(value As Boolean)
            If value AndAlso aBell.IsPlaying = False Then

                aBell.Play()
            End If
        End Set
    End Property

    Private Sub LoadProps()

        PistonRelativePosY = Locomotive.Bones.Item(TrainBones.sPistons).RelativePosition.Y
        PistonRelativePosZ = Locomotive.Bones.Item(TrainBones.sPistons).RelativePosition.Z

        TrainProperties.connPointRadius = Locomotive.Bones.Item(TrainBones.sWheelDrive2).RelativePosition.DistanceTo(Locomotive.Bones.Item(TrainBones.sRods).RelativePosition) * -1
        TrainProperties.pRodsLength = Locomotive.Bones.Item(TrainBones.sRodsEnd).RelativePosition.DistanceTo(Locomotive.Bones.Item(TrainBones.sRods).RelativePosition)

        With aWheels
            WheelRadius = System.Math.Abs(TrainModels.sWheelDrive.Model.Dimensions.frontTopRight.Z)

            .Add(New AnimateProp(TrainModels.sWheelDrive, Locomotive, TrainBones.sWheelDrive1, Vector3.Zero, Vector3.Zero))
            aAllProps.Add(.Props.Last)
            .Add(New AnimateProp(TrainModels.sWheelDrive, Locomotive, TrainBones.sWheelDrive2, Vector3.Zero, Vector3.Zero))
            aAllProps.Add(.Props.Last)
            .Add(New AnimateProp(TrainModels.sWheelDrive, Locomotive, TrainBones.sWheelDrive3, Vector3.Zero, Vector3.Zero))
            aAllProps.Add(.Props.Last)
        End With

        With aSmallWheels
            SmallWheelRadius = System.Math.Abs(TrainModels.sWheelFront.Model.Dimensions.frontTopRight.Z)

            .Add(New AnimateProp(TrainModels.sWheelFront, Locomotive, TrainBones.sWheelFront1, Vector3.Zero, Vector3.Zero))
            aAllProps.Add(.Props.Last)
            .Add(New AnimateProp(TrainModels.sWheelFront, Locomotive, TrainBones.sWheelFront2, Vector3.Zero, Vector3.Zero))
            aAllProps.Add(.Props.Last)
        End With

        With aSmallWheelsTender

            .Add(New AnimateProp(TrainModels.tWheel, Tender, TrainBones.sWheelTender1, Vector3.Zero, Vector3.Zero))
            aAllProps.Add(.Props.Last)
            .Add(New AnimateProp(TrainModels.tWheel, Tender, TrainBones.sWheelTender2, Vector3.Zero, Vector3.Zero))
            aAllProps.Add(.Props.Last)
            .Add(New AnimateProp(TrainModels.tWheel, Tender, TrainBones.sWheelTender3, Vector3.Zero, Vector3.Zero))
            aAllProps.Add(.Props.Last)
            .Add(New AnimateProp(TrainModels.tWheel, Tender, TrainBones.sWheelTender4, Vector3.Zero, Vector3.Zero))
            aAllProps.Add(.Props.Last)
        End With

        aRods = New AnimateProp(TrainModels.sRods, Locomotive, TrainBones.sWheelDrive2, New Vector3(0, TrainProperties.connPointRadius, 0), Vector3.Zero)
        aAllProps.Add(aRods)
        aPRods = New AnimateProp(TrainModels.sPRods, Locomotive, TrainBones.sWheelDrive2, New Vector3(0, TrainProperties.connPointRadius, 0), Vector3.Zero)
        aAllProps.Add(aPRods)
        aPistons = New AnimateProp(TrainModels.sPistons, Locomotive, TrainBones.sPistons, Vector3.Zero, Vector3.Zero)
        aAllProps.Add(aPistons)

        aLevValves = New AnimateProp(TrainModels.sLevValves, Locomotive, TrainBones.sLevValves, Vector3.Zero, Vector3.Zero)
        aAllProps.Add(aLevValves)

        aValves = New AnimateProp(TrainModels.sValves, Locomotive, TrainBones.sValves, Vector3.Zero, Vector3.Zero)
        aAllProps.Add(aValves)

        aValvesPist = New AnimateProp(TrainModels.sValvesPist, Locomotive, TrainBones.sValvesPist, Vector3.Zero, Vector3.Zero)
        aAllProps.Add(aValvesPist)

        aBell = New AnimateProp(TrainModels.sBell, Locomotive, TrainBones.sBell, Vector3.Zero, Vector3.Zero)
        aBell(AnimationType.Rotation)(AnimationStep.First)(Coordinate.X).Setup(False, True, -70, 70, 1, 140, 1)
        aAllProps.Add(aBell)

        sLight = New AnimateProp(TrainModels.sLight, Locomotive, Vector3.Zero, Vector3.Zero)
        aAllProps.Add(sLight)

        sCabCols = New AnimateProp(TrainModels.sCabCols, Locomotive, Vector3.Zero, Vector3.Zero)
        aAllProps.Add(sCabCols)

        sFireboxDoor = New AnimateProp(TrainModels.sFireboxDoor, Locomotive, TrainBones.sFireboxDoor, Vector3.Zero, Vector3.Zero)
        sFireboxDoor(AnimationType.Rotation)(AnimationStep.First)(Coordinate.Z).Setup(True, True, 10, 80, 1, 140, 1)
        aAllProps.Add(sFireboxDoor)

        aAllProps.SpawnProp()

        sFireboxDoor.setRotation(Coordinate.Z, 10)
        sCabCols.Visible = False

        'With aBrakePads

        '    .Props.Add(New AnimateProp(Models.sBrakePadsFront, Locomotive, Bones.sBrakePadsFront, Vector3.Zero, Vector3.Zero))
        '    .Props.Add(New AnimateProp(Models.sBrakePadsMiddle, Locomotive, Bones.sBrakePadsMiddle, Vector3.Zero, Vector3.Zero))
        '    .Props.Add(New AnimateProp(Models.sBrakePadsRear, Locomotive, Bones.sBrakePadsRear, Vector3.Zero, Vector3.Zero))
        'End With

        'aBrakeBars = New AnimateProp(Models.sBrakeBars, Locomotive, Bones.sBrakeBars, Vector3.Zero, Vector3.Zero)
        'aBrakeLevers = New AnimateProp(Models.sBrakeLevers, Locomotive, Bones.sBrakeLevers, Vector3.Zero, Vector3.Zero)
        'aBrakePistons = New AnimateProp(Models.sBrakePistons, Locomotive, Bones.sBrakePistons, Vector3.Zero, Vector3.Zero)

        AnimationProcess()
    End Sub

    Private Sub AnimationProcess()

        Dim modifier As Single = If(Locomotive.GetMPHSpeed <= 10, 1 + (2.5 / 10) * Locomotive.GetMPHSpeed, 2.5)
        Dim wheelRot As Single = MathExtensions.AngularSpeed(Locomotive.Speed, WheelRadius, aWheels(0).CurrentRotation.X, Locomotive.IsGoingForward, modifier)

        aWheels.setRotation(Coordinate.X, wheelRot, True)

        aSmallWheels.setRotation(Coordinate.X, MathExtensions.AngularSpeed(Locomotive.Speed, SmallWheelRadius, aSmallWheels(0).CurrentRotation.X, Locomotive.IsGoingForward, modifier), True)

        aSmallWheelsTender.setRotation(Coordinate.X, aSmallWheels(0).CurrentRotation.X, True)

        wheelRot = MathExtensions.PositiveAngle(wheelRot)

        Dim dY = System.Math.Cos(MathExtensions.ToRad(wheelRot)) * TrainProperties.connPointRadius
        Dim dZ = System.Math.Sin(MathExtensions.ToRad(wheelRot)) * TrainProperties.connPointRadius

        aRods.setOffset(Coordinate.Y, dY, True)
        aRods.setOffset(Coordinate.Z, dZ, True)

        aPRods.setOffset(Coordinate.Y, dY, True)
        aPRods.setOffset(Coordinate.Z, dZ, True)

        Dim dAngle = 90 - MathExtensions.ToDeg(MathExtensions.ArcCos((PistonRelativePosZ - aPRods.RelativePosition.Z) / TrainProperties.pRodsLength))

        aPRods.setRotation(Coordinate.X, dAngle, True)

        aPistons.setOffset(Coordinate.Y, TrainProperties.pRodsLength * System.Math.Cos(MathExtensions.ToRad(dAngle)) - (PistonRelativePosY - aPRods.RelativePosition.Y), True)

        aLevValves.setRotation(Coordinate.X, (TrainProperties.maxLevValvesRot / TrainProperties.maxPistonPos) * aPistons.CurrentOffset.Y, True)

        aValvesPist.setOffset(Coordinate.Y, (TrainProperties.minValvesPistPos / TrainProperties.maxLevValvesRot) * aLevValves.CurrentRotation.X, True)

        aValves.setOffset(Coordinate.Y, aValvesPist.CurrentOffset.Y, True)
        aValves.setOffset(Coordinate.Z, (TrainProperties.maxValvesPos / TrainProperties.maxLevValvesRot) * aLevValves.CurrentRotation.X, True)
        aValves.setRotation(Coordinate.X, (TrainProperties.minValesRot / TrainProperties.maxLevValvesRot) * aLevValves.CurrentRotation.X, True)
    End Sub

    Private Sub AnimationTick()

        If VisibleLocomotive.Position.DistanceToSquared(Game.Player.Character.Position) > 100 * 100 AndAlso Locomotive.Speed = 0 Then

        Else

            AnimationProcess()
        End If

        If Game.IsControlJustPressed(Control.VehicleHandbrake) AndAlso Game.IsControlPressed(Control.CharacterWheel) = False AndAlso Bell = False AndAlso Utils.PlayerPed.IsInVehicle(Locomotive) Then

            Bell = True
        End If

        If Game.IsControlJustPressed(Control.VehicleHeadlight) AndAlso Utils.PlayerPed.IsInVehicle(Locomotive) Then

            IsLightOn = Not IsLightOn
        End If

        If aBell.IsPlaying Then

            With aBell

                If .Animation(AnimationType.Rotation)(AnimationStep.First)(Coordinate.X).IsIncreasing <> BellAnimationChangedDirection Then

                    sBellSound.Play()
                    BellAnimationChangedDirection = Not BellAnimationChangedDirection
                End If

                If Game.IsControlPressed(Control.VehicleHandbrake) = False Then

                    .Animation(AnimationType.Rotation)(AnimationStep.First)(Coordinate.X).MaxMinRatio = 1 - ((1 / BellAnimationLength) * BellAnimationCounter)
                    .Animation(AnimationType.Rotation)(AnimationStep.First)(Coordinate.X).StepRatio = .Animation(AnimationType.Rotation)(AnimationStep.First)(Coordinate.X).MaxMinRatio

                    Try
                        sBellSound.Volume = .Animation(AnimationType.Rotation)(AnimationStep.First)(Coordinate.X).MaxMinRatio
                    Catch ex As Exception

                    End Try

                    BellAnimationCounter += 1 * Game.LastFrameTime

                    If BellAnimationCounter > BellAnimationLength Then

                        BellAnimationCounter = 0
                        aBell.Stop()
                    End If
                ElseIf Game.IsControlJustPressed(Control.VehicleHandbrake) Then

                    .Animation(AnimationType.Rotation)(AnimationStep.First)(Coordinate.X).MaxMinRatio = 1
                    .Animation(AnimationType.Rotation)(AnimationStep.First)(Coordinate.X).StepRatio = 1

                    Try
                        sBellSound.Volume = 1
                    Catch ex As Exception

                    End Try

                    BellAnimationCounter = 0
                End If
            End With
        End If
    End Sub
End Class
