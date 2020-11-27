Imports GTA
Imports GTA.Math
Imports FusionLibrary
Imports FusionLibrary.Extensions
Imports FusionLibrary.Enums

Partial Public Class RogersSierra

    Private WheelRadius As Single
    Private SmallWheelRadius As Single

    Private PistonRelativePosY As Single
    Private PistonRelativePosZ As Single

    Private aAllProps As New AnimatePropHandler2

    Private aSmallWheelsTender As New AnimatePropHandler2

    Private aWheels As New AnimatePropHandler2
    Private aSmallWheels As New AnimatePropHandler2
    Private aRods As AnimateProp2
    Private aPRods As AnimateProp2

    Private aPistons As AnimateProp2
    Private PistonOldPos As Single
    Private PistonGoingForward As Boolean = False

    Private aLevValves As AnimateProp2
    Private aValves As AnimateProp2
    Private aValvesPist As AnimateProp2

    Private aBell As AnimateProp2
    Private BellAnimation As AnimationStep
    Private BellAnimationCounter As Single
    Private BellAnimationLength As Integer = 10
    Private BellAnimationChangedDirection As Boolean = True

    Private sLight As AnimateProp2
    Private sCabCols As AnimateProp2
    Private sFireboxDoor As AnimateProp2

    'Private aBrakePads As New AnimatePropHandler
    'Private aBrakeBars As AnimateProp
    'Private aBrakeLevers As AnimateProp
    'Private aBrakePistons As AnimateProp

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

    Private Sub LoadProps()

        PistonRelativePosY = Locomotive.Bones.Item(TrainBones.sPistons).RelativePosition.Y
        PistonRelativePosZ = Locomotive.Bones.Item(TrainBones.sPistons).RelativePosition.Z

        TrainProperties.connPointRadius = Locomotive.Bones.Item(TrainBones.sWheelDrive2).RelativePosition.DistanceTo(Locomotive.Bones.Item(TrainBones.sRods).RelativePosition) * -1
        TrainProperties.pRodsLength = Locomotive.Bones.Item(TrainBones.sRodsEnd).RelativePosition.DistanceTo(Locomotive.Bones.Item(TrainBones.sRods).RelativePosition)

        With aWheels
            WheelRadius = System.Math.Abs(TrainModels.sWheelDrive.Dimensions.frontTopRight.Z)

            .Props.Add(New AnimateProp2(TrainModels.sWheelDrive, Locomotive, TrainBones.sWheelDrive1, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp2(TrainModels.sWheelDrive, Locomotive, TrainBones.sWheelDrive2, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp2(TrainModels.sWheelDrive, Locomotive, TrainBones.sWheelDrive3, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
        End With

        With aSmallWheels
            SmallWheelRadius = System.Math.Abs(TrainModels.sWheelFront.Dimensions.frontTopRight.Z)

            .Props.Add(New AnimateProp2(TrainModels.sWheelFront, Locomotive, TrainBones.sWheelFront1, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp2(TrainModels.sWheelFront, Locomotive, TrainBones.sWheelFront2, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
        End With

        With aSmallWheelsTender

            .Props.Add(New AnimateProp2(TrainModels.tWheel, Tender, TrainBones.sWheelTender1, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp2(TrainModels.tWheel, Tender, TrainBones.sWheelTender2, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp2(TrainModels.tWheel, Tender, TrainBones.sWheelTender3, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp2(TrainModels.tWheel, Tender, TrainBones.sWheelTender4, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
        End With

        aRods = New AnimateProp2(TrainModels.sRods, Locomotive, TrainBones.sWheelDrive2, New Vector3(0, TrainProperties.connPointRadius, 0), Vector3.Zero)
        aAllProps.Props.Add(aRods)
        aPRods = New AnimateProp2(TrainModels.sPRods, Locomotive, TrainBones.sWheelDrive2, New Vector3(0, TrainProperties.connPointRadius, 0), Vector3.Zero)
        aAllProps.Props.Add(aPRods)
        aPistons = New AnimateProp2(TrainModels.sPistons, Locomotive, TrainBones.sPistons, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aPistons)

        aLevValves = New AnimateProp2(TrainModels.sLevValves, Locomotive, TrainBones.sLevValves, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aLevValves)

        aValves = New AnimateProp2(TrainModels.sValves, Locomotive, TrainBones.sValves, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aValves)

        aValvesPist = New AnimateProp2(TrainModels.sValvesPist, Locomotive, TrainBones.sValvesPist, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aValvesPist)

        aBell = New AnimateProp2(TrainModels.sBell, Locomotive, TrainBones.sBell, Vector3.Zero, Vector3.Zero, True)
        aBell.setRotationSettings(Coordinate.X, True, True, -70, 70, 3.5, False, 1, False, 1)
        BellAnimation = AnimationStep.Off
        aAllProps.Props.Add(aBell)

        sLight = New AnimateProp2(TrainModels.sLight, Locomotive, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(sLight)

        sCabCols = New AnimateProp2(TrainModels.sCabCols, Locomotive, Vector3.Zero, Vector3.Zero, True)
        sCabCols.Visible = False
        aAllProps.Props.Add(sCabCols)

        sFireboxDoor = New AnimateProp2(TrainModels.sFireboxDoor, Locomotive, TrainBones.sFireboxDoor, Vector3.Zero, Vector3.Zero, True)
        sFireboxDoor.setRotationSettings(Coordinate.Z, True, True, 10, 80, 7, False, 1, True, 1)
        sFireboxDoor.Play()
        sFireboxDoor.set_RotationUpdate(Coordinate.Z, False)
        aAllProps.Props.Add(sFireboxDoor)

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
        Dim wheelRot As Single = MathExtensions.AngularSpeed(Locomotive.Speed, WheelRadius, aWheels.get_Rotation(0).X, Locomotive.IsGoingForward, modifier)

        aWheels.set_AllRotation(Coordinate.X, wheelRot)

        aSmallWheels.set_AllRotation(Coordinate.X, MathExtensions.AngularSpeed(Locomotive.Speed, SmallWheelRadius, aSmallWheels.get_Rotation(0).X, Locomotive.IsGoingForward, modifier))

        aSmallWheelsTender.set_AllRotation(Coordinate.X, aSmallWheels.get_AllRotation(Coordinate.X))

        wheelRot = MathExtensions.PositiveAngle(wheelRot)

        Dim dY = System.Math.Cos(MathExtensions.ToRad(wheelRot)) * TrainProperties.connPointRadius
        Dim dZ = System.Math.Sin(MathExtensions.ToRad(wheelRot)) * TrainProperties.connPointRadius

        aRods.set_Position(Coordinate.Y, dY)
        aRods.set_Position(Coordinate.Z, dZ)

        aPRods.set_Position(Coordinate.Y, dY)
        aPRods.set_Position(Coordinate.Z, dZ)

        Dim dAngle = 90 - MathExtensions.ToDeg(MathExtensions.ArcCos((PistonRelativePosZ - aPRods.RelativePosition.Z) / TrainProperties.pRodsLength))

        aPRods.set_Rotation(Coordinate.X, dAngle)

        aPistons.set_Position(Coordinate.Y, TrainProperties.pRodsLength * System.Math.Cos(MathExtensions.ToRad(dAngle)) - (PistonRelativePosY - aPRods.RelativePosition.Y))

        aLevValves.set_Rotation(Coordinate.X, (TrainProperties.maxLevValvesRot / TrainProperties.maxPistonPos) * aPistons.Position(Coordinate.Y))

        aValvesPist.set_Position(Coordinate.Y, (TrainProperties.minValvesPistPos / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X)

        aValves.set_Position(Coordinate.Y, aValvesPist.Position(Coordinate.Y))
        aValves.set_Position(Coordinate.Z, (TrainProperties.maxValvesPos / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X)
        aValves.set_Rotation(Coordinate.X, (TrainProperties.minValesRot / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X)

        sFireboxDoor.Play()
    End Sub

    Private Sub AnimationTick()

        If VisibleLocomotive.Position.DistanceToSquared(Game.Player.Character.Position) > 100 * 100 AndAlso Locomotive.Speed = 0 Then

            aAllProps.CheckExists()
        Else

            AnimationProcess()
        End If

        If Game.IsControlJustPressed(Control.VehicleHandbrake) AndAlso Game.IsControlPressed(Control.CharacterWheel) = False AndAlso Bell = False AndAlso Utils.PlayerPed.IsInVehicle(Locomotive) Then

            Bell = True
        End If

        If Game.IsControlJustPressed(Control.VehicleHeadlight) AndAlso Utils.PlayerPed.IsInVehicle(Locomotive) Then

            IsLightOn = Not IsLightOn
        End If

        Select Case BellAnimation
            Case AnimationStep.First

                With aBell

                    .Play()

                    If .get_RotationIncreasing(Coordinate.X) <> BellAnimationChangedDirection Then

                        sBellSound.Play()
                        BellAnimationChangedDirection = Not BellAnimationChangedDirection
                    End If

                    If Game.IsControlPressed(Control.VehicleHandbrake) = False Then

                        .set_RotationMaxMinRatio(Coordinate.X, 1 - ((1 / BellAnimationLength) * BellAnimationCounter))
                        .set_RotationStepRatio(Coordinate.X, .get_RotationMaxMinRatio(Coordinate.X))

                        Try
                            sBellSound.Volume = .get_RotationMaxMinRatio(Coordinate.X)
                        Catch ex As Exception

                        End Try

                        BellAnimationCounter += 1 * Game.LastFrameTime

                        If BellAnimationCounter > BellAnimationLength Then

                            BellAnimationCounter = 0
                            BellAnimation = AnimationStep.Off
                        End If
                    ElseIf Game.IsControlJustPressed(Control.VehicleHandbrake) Then

                        .set_RotationMaxMinRatio(Coordinate.X, 1)
                        .set_RotationStepRatio(Coordinate.X, 1)

                        Try
                            sBellSound.Volume = 1
                        Catch ex As Exception

                        End Try

                        BellAnimationCounter = 0
                    End If
                End With
        End Select
    End Sub
End Class
