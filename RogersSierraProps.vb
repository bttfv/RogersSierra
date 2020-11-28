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

    Private aAllProps As New AnimatePropsHandler

    Private aSmallWheelsTender As New AnimatePropsHandler

    Private aWheels As New AnimatePropsHandler
    Private aSmallWheels As New AnimatePropsHandler
    Private aRods As AnimateProp
    Private aPRods As AnimateProp

    Private aPistons As AnimateProp
    Private PistonOldPos As Single
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
            Return aBell.IsAnimationOn
        End Get
        Set(value As Boolean)
            If value Then

                If aBell.IsAnimationOn = False Then

                    aBell.setRotationIncreasing(Coordinate.X, True)
                End If

                aBell.IsAnimationOn = True
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

            .Props.Add(New AnimateProp(TrainModels.sWheelDrive, Locomotive, TrainBones.sWheelDrive1, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp(TrainModels.sWheelDrive, Locomotive, TrainBones.sWheelDrive2, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp(TrainModels.sWheelDrive, Locomotive, TrainBones.sWheelDrive3, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
        End With

        With aSmallWheels
            SmallWheelRadius = System.Math.Abs(TrainModels.sWheelFront.Dimensions.frontTopRight.Z)

            .Props.Add(New AnimateProp(TrainModels.sWheelFront, Locomotive, TrainBones.sWheelFront1, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp(TrainModels.sWheelFront, Locomotive, TrainBones.sWheelFront2, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
        End With

        With aSmallWheelsTender

            .Props.Add(New AnimateProp(TrainModels.tWheel, Tender, TrainBones.sWheelTender1, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp(TrainModels.tWheel, Tender, TrainBones.sWheelTender2, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp(TrainModels.tWheel, Tender, TrainBones.sWheelTender3, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
            .Props.Add(New AnimateProp(TrainModels.tWheel, Tender, TrainBones.sWheelTender4, Vector3.Zero, Vector3.Zero))
            aAllProps.Props.Add(.Props.Last)
        End With

        aRods = New AnimateProp(TrainModels.sRods, Locomotive, TrainBones.sWheelDrive2, New Vector3(0, TrainProperties.connPointRadius, 0), Vector3.Zero)
        aAllProps.Props.Add(aRods)
        aPRods = New AnimateProp(TrainModels.sPRods, Locomotive, TrainBones.sWheelDrive2, New Vector3(0, TrainProperties.connPointRadius, 0), Vector3.Zero)
        aAllProps.Props.Add(aPRods)
        aPistons = New AnimateProp(TrainModels.sPistons, Locomotive, TrainBones.sPistons, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aPistons)

        aLevValves = New AnimateProp(TrainModels.sLevValves, Locomotive, TrainBones.sLevValves, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aLevValves)

        aValves = New AnimateProp(TrainModels.sValves, Locomotive, TrainBones.sValves, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aValves)

        aValvesPist = New AnimateProp(TrainModels.sValvesPist, Locomotive, TrainBones.sValvesPist, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(aValvesPist)

        aBell = New AnimateProp(TrainModels.sBell, Locomotive, TrainBones.sBell, Vector3.Zero, Vector3.Zero)
        aBell.setRotationSettings(Coordinate.X, True, True, -70, 70, 1, 70, 1, False, True)
        aAllProps.Props.Add(aBell)

        sLight = New AnimateProp(TrainModels.sLight, Locomotive, Vector3.Zero, Vector3.Zero)
        aAllProps.Props.Add(sLight)

        sCabCols = New AnimateProp(TrainModels.sCabCols, Locomotive, Vector3.Zero, Vector3.Zero, True)
        sCabCols.Visible = False
        aAllProps.Props.Add(sCabCols)

        sFireboxDoor = New AnimateProp(TrainModels.sFireboxDoor, Locomotive, TrainBones.sFireboxDoor, Vector3.Zero, Vector3.Zero, True)
        sFireboxDoor.setRotationSettings(Coordinate.Z, True, True, 10, 80, 1, 7, 1, True)
        sFireboxDoor.Play()
        sFireboxDoor.IsAnimationOn = False
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
        Dim wheelRot As Single = MathExtensions.AngularSpeed(Locomotive.Speed, WheelRadius, aWheels.getRotation(0).X, Locomotive.IsGoingForward, modifier)

        aWheels.setAllRotation(Coordinate.X, wheelRot)

        aSmallWheels.setAllRotation(Coordinate.X, MathExtensions.AngularSpeed(Locomotive.Speed, SmallWheelRadius, aSmallWheels.getRotation(0).X, Locomotive.IsGoingForward, modifier))

        aSmallWheelsTender.setAllRotation(Coordinate.X, aSmallWheels.getAllRotation(Coordinate.X))

        wheelRot = MathExtensions.PositiveAngle(wheelRot)

        Dim dY = System.Math.Cos(MathExtensions.ToRad(wheelRot)) * TrainProperties.connPointRadius
        Dim dZ = System.Math.Sin(MathExtensions.ToRad(wheelRot)) * TrainProperties.connPointRadius

        aRods.setOffset(Coordinate.Y, dY)
        aRods.setOffset(Coordinate.Z, dZ)

        aPRods.setOffset(Coordinate.Y, dY)
        aPRods.setOffset(Coordinate.Z, dZ)

        Dim dAngle = 90 - MathExtensions.ToDeg(MathExtensions.ArcCos((PistonRelativePosZ - aPRods.RelativePosition.Z) / TrainProperties.pRodsLength))

        aPRods.setRotation(Coordinate.X, dAngle)

        aPistons.setOffset(Coordinate.Y, TrainProperties.pRodsLength * System.Math.Cos(MathExtensions.ToRad(dAngle)) - (PistonRelativePosY - aPRods.RelativePosition.Y))

        aLevValves.setRotation(Coordinate.X, (TrainProperties.maxLevValvesRot / TrainProperties.maxPistonPos) * aPistons.Offset(Coordinate.Y))

        aValvesPist.setOffset(Coordinate.Y, (TrainProperties.minValvesPistPos / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X)

        aValves.setOffset(Coordinate.Y, aValvesPist.Offset(Coordinate.Y))
        aValves.setOffset(Coordinate.Z, (TrainProperties.maxValvesPos / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X)
        aValves.setRotation(Coordinate.X, (TrainProperties.minValesRot / TrainProperties.maxLevValvesRot) * aLevValves.Rotation.X)
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

        If aBell.IsAnimationOn Then

            With aBell

                If .getRotationIncreasing(Coordinate.X) <> BellAnimationChangedDirection Then

                    sBellSound.Play()
                    BellAnimationChangedDirection = Not BellAnimationChangedDirection
                End If

                If Game.IsControlPressed(Control.VehicleHandbrake) = False Then

                    .setRotationMaxMinRatio(Coordinate.X, 1 - ((1 / BellAnimationLength) * BellAnimationCounter))
                    .setRotationStepRatio(Coordinate.X, .getRotationMaxMinRatio(Coordinate.X))

                    Try
                        sBellSound.Volume = .getRotationMaxMinRatio(Coordinate.X)
                    Catch ex As Exception

                    End Try

                    BellAnimationCounter += 1 * Game.LastFrameTime

                    If BellAnimationCounter > BellAnimationLength Then

                        BellAnimationCounter = 0
                        aBell.IsAnimationOn = False
                    End If
                ElseIf Game.IsControlJustPressed(Control.VehicleHandbrake) Then

                    .setRotationMaxMinRatio(Coordinate.X, 1)
                    .setRotationStepRatio(Coordinate.X, 1)

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
