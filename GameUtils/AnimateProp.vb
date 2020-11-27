Imports BTTFVLibrary.Enums
Imports GTA
Imports GTA.Math

Friend Class AnimateProp

    Private aProp As Prop
    Private pModel As Model
    Private boneName As String
    Private eBone As EntityBone
    Private pEntity As Entity
    Private pOffset As Math.Vector3
    Private pRotation As Math.Vector3

    Public isAnimationOn As Boolean

    Private cPos(3) As CoordinateSetting

    Private cRot(3) As CoordinateSetting

    Private toBone As Boolean

    Private isDetached As Boolean = False

    ''' <summary>
    ''' Spawns a new prop with <paramref name="pModel"/> attached to <paramref name="boneName"/> of <paramref name="pEntity"/> with <paramref name="pOffset"/> and <paramref name="pRotation"/>
    ''' </summary>
    ''' <param name="pModel"><seealso cref="GTA.Model"/> of the prop.</param>
    ''' <param name="pEntity"><seealso cref="GTA.Entity"/> owner of the <paramref name="boneName"/>.</param>
    ''' <param name="boneName">Bone's name of the <paramref name="pEntity"/>.</param>
    ''' <param name="pOffset">A <seealso cref="GTA.Math.Vector3"/> indicating offset of the prop relative to the <paramref name="boneName"/>'s position.</param>
    ''' <param name="pRotation">A <seealso cref="GTA.Math.Vector3"/> indicating rotation of the prop.</param>
    Sub New(pModel As Model, pEntity As Entity, boneName As String, pOffset As Math.Vector3, pRotation As Math.Vector3, Optional cAnimationOn As Boolean = False)

        Me.pModel = pModel
        Me.pEntity = pEntity
        Me.boneName = boneName
        eBone = pEntity.Bones.Item(boneName)
        Me.pOffset = pOffset
        Me.pRotation = pRotation

        toBone = True
        isAnimationOn = cAnimationOn

        aProp = World.CreateProp(pModel, pEntity.Position, False, False)

        aProp.IsPersistent = True

        Attach()
    End Sub

    Sub New(pModel As Model, pEntity As Entity, pOffset As Math.Vector3, pRotation As Math.Vector3, Optional cAnimationOn As Boolean = False)

        Me.pModel = pModel
        Me.pEntity = pEntity
        'Me.boneName = boneName
        'eBone = pEntity.Bones.Item(boneName)
        Me.pOffset = pOffset
        Me.pRotation = pRotation

        toBone = False
        isAnimationOn = cAnimationOn

        aProp = World.CreateProp(pModel, pEntity.Position, False, False)

        aProp.IsPersistent = True

        Attach()
    End Sub

    Private Sub Attach()

        If isDetached Then Exit Sub

        If toBone Then

            Native.Function.Call(Native.Hash.ATTACH_ENTITY_TO_ENTITY, aProp.Handle, pEntity.Handle, eBone.Index, pOffset.X, pOffset.Y, pOffset.Z, pRotation.X, pRotation.Y, pRotation.Z, False, False, True, False, 2, True)
        Else

            Native.Function.Call(Native.Hash.ATTACH_ENTITY_TO_ENTITY, aProp.Handle, pEntity.Handle, 0, pOffset.X, pOffset.Y, pOffset.Z, pRotation.X, pRotation.Y, pRotation.Z, False, False, True, False, 2, True)
        End If
    End Sub

    Public Sub Detach()

        aProp.Detach()

        aProp.IsPositionFrozen = False

        isDetached = True
    End Sub

    Public Sub ScatterProp(Optional ForceMultiplier As Single = 1)

        Detach()

        aProp.ApplyForce(Vector3.RandomXYZ * ForceMultiplier, Vector3.RandomXYZ * ForceMultiplier)
    End Sub

    Public Sub TransferTo(newEntity As Entity, boneName As String)

        pEntity = newEntity
        Me.boneName = boneName

        Attach()
    End Sub

    Public Sub TransferTo(newEntity As Entity)

        pEntity = newEntity

        Attach()
    End Sub

    ''' <summary>
    ''' Deletes prop.
    ''' </summary>
    Public Sub Delete()

        aProp.Delete()
    End Sub

    Public ReadOnly Property Prop As Prop
        Get
            Return aProp
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the visbile status of the prop.
    ''' </summary>
    ''' <returns></returns>
    Public Property Visible As Boolean
        Get
            Return aProp.IsVisible
        End Get
        Set(value As Boolean)
            aProp.IsVisible = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets offset of the prop.
    ''' </summary>
    ''' <returns></returns>
    Public Property Position As Math.Vector3
        Get
            Return pOffset
        End Get
        Set(value As Math.Vector3)
            pOffset = value
            Attach()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets rotation of the prop.
    ''' </summary>
    ''' <returns></returns>
    Public Property Rotation As Math.Vector3
        Get
            Return pRotation
        End Get
        Set(value As Math.Vector3)
            pRotation = value
            Attach()
        End Set
    End Property

    ''' <summary>
    ''' Sets given position's coordinate settings.
    ''' </summary>
    ''' <param name="pCord">Coordinate (X, Y, Z)</param>
    ''' <param name="cUpdate">If true, the coordinate value will update.</param>
    ''' <param name="cIncreasing">If the coordinate value should increase or not.</param>
    ''' <param name="cMinimum">Minimum value should reach.</param>
    ''' <param name="cMaximum">Maximum value should reach.</param>
    ''' <param name="cStep">Delta value that is added or substract.</param>
    ''' <param name="cStepRatio">From 0.0 to 1.0. Ratio of maximum and minimum values.</param>
    Public Sub setPositionSettings(pCord As Coordinate, cUpdate As Boolean, cIncreasing As Boolean, cMinimum As Single, cMaximum As Single, cStep As Single, Optional cStepRatio As Single = 1, Optional [Stop] As Boolean = False, Optional cMaxMinRatio As Single = 1)

        With cPos(pCord)

            .Update = cUpdate
            .isIncreasing = cIncreasing
            .Minimum = cMinimum
            .Maximum = cMaximum
            .Step = cStep
            .StepRatio = cStepRatio
            .isFullCircle = False
            .Stop = [Stop]
            .MaxMinRatio = cMaxMinRatio
        End With
    End Sub

    ''' <summary>
    ''' Sets given rotation's coordinate settings.
    ''' </summary>
    ''' <param name="pCord">Coordinate (X, Y, Z)</param>
    ''' <param name="cUpdate">If true, the coordinate value will update.</param>
    ''' <param name="cIncreasing">If the coordinate value should increase or not.</param>
    ''' <param name="cMinimum">Minimum value should reach.</param>
    ''' <param name="cMaximum">Maximum value should reach.</param>
    ''' <param name="cStep">Delta value that is added or substract.</param>
    ''' <param name="cFullCircle">If true disables maximum\minimum and the prop will continue to rotate indefinitely.</param>
    ''' <param name="cStepRatio">From 0.0 to 1.0. Ratio of maximum and minimum values.</param>
    Public Sub setRotationSettings(pCord As Coordinate, cUpdate As Boolean, cIncreasing As Boolean, cMinimum As Single, cMaximum As Single, cStep As Single, cFullCircle As Boolean, Optional cStepRatio As Single = 1, Optional [Stop] As Boolean = False, Optional cMaxMinRatio As Single = 1)

        With cRot(pCord)

            .Update = cUpdate
            .isIncreasing = cIncreasing
            .Minimum = cMinimum
            .Maximum = cMaximum
            .Step = cStep
            .StepRatio = cStepRatio
            .isFullCircle = cFullCircle
            .Stop = [Stop]
            .MaxMinRatio = cMaxMinRatio
        End With
    End Sub

#Region "Position properties"
    Public ReadOnly Property RelativePosition As Math.Vector3
        Get
            Return eBone.GetRelativeOffsetPosition(pOffset)
        End Get
    End Property

    Public ReadOnly Property WorldPosition As Math.Vector3
        Get
            Return eBone.GetOffsetPosition(pOffset)
        End Get
    End Property

    Public Property Position(pCord As Coordinate) As Single
        Get
            Select Case pCord
                Case Coordinate.X
                    Return pOffset.X
                Case Coordinate.Y
                    Return pOffset.Y
                Case Else
                    Return pOffset.Z
            End Select
        End Get
        Set(value As Single)
            Select Case pCord
                Case Coordinate.X
                    pOffset.X = value
                Case Coordinate.Y
                    pOffset.Y = value
                Case Else
                    pOffset.Z = value
            End Select
            Attach()
        End Set
    End Property

    Public Property PositionUpdate(pCord As Coordinate) As Boolean
        Get
            Return cPos(pCord).Update
        End Get
        Set(value As Boolean)
            cPos(pCord).Update = value
        End Set
    End Property

    Public Property PositionMaximum(pCord As Coordinate) As Single
        Get
            Return cPos(pCord).Maximum
        End Get
        Set(value As Single)
            cPos(pCord).Maximum = value
        End Set
    End Property

    Public Property PositionMinimum(pCord As Coordinate) As Single
        Get
            Return cPos(pCord).Minimum
        End Get
        Set(value As Single)
            cPos(pCord).Minimum = value
        End Set
    End Property

    Public Property PositionStep(pCord As Coordinate) As Single
        Get
            Return cPos(pCord).Step
        End Get
        Set(value As Single)
            cPos(pCord).Step = value
        End Set
    End Property

    Public Property PositionStepRatio(pCord As Coordinate) As Single
        Get
            Return cPos(pCord).StepRatio
        End Get
        Set(value As Single)
            cPos(pCord).StepRatio = value
        End Set
    End Property

    Public Property PositionIncreasing(pCord As Coordinate) As Boolean
        Get
            Return cPos(pCord).isIncreasing
        End Get
        Set(value As Boolean)
            cPos(pCord).isIncreasing = value
        End Set
    End Property

    Public Property PositionStop(pCord As Coordinate) As Boolean
        Get
            Return cPos(pCord).Stop
        End Get
        Set(value As Boolean)
            cPos(pCord).Stop = value
        End Set
    End Property

    Public Property PositionMaxMinRatio(pCord As Coordinate) As Single
        Get
            Return cPos(pCord).MaxMinRatio
        End Get
        Set(value As Single)
            cPos(pCord).MaxMinRatio = value
        End Set
    End Property
#End Region

#Region "Rotation properties"
    Public Property Rotation(pCord As Coordinate) As Single
        Get
            Select Case pCord
                Case Coordinate.X
                    Return pRotation.X
                Case Coordinate.Y
                    Return pRotation.Y
                Case Else
                    Return pRotation.Z
            End Select
        End Get
        Set(value As Single)
            Select Case pCord
                Case Coordinate.X
                    pRotation.X = value
                Case Coordinate.Y
                    pRotation.Y = value
                Case Else
                    pRotation.Z = value
            End Select
            Attach()
        End Set
    End Property

    Public Property RotationUpdate(pCord As Coordinate) As Boolean
        Get
            Return cRot(pCord).Update
        End Get
        Set(value As Boolean)
            cRot(pCord).Update = value
        End Set
    End Property

    Public Property RotationMaximum(pCord As Coordinate) As Single
        Get
            Return cRot(pCord).Maximum
        End Get
        Set(value As Single)
            cRot(pCord).Maximum = value
        End Set
    End Property

    Public Property RotationMinimum(pCord As Coordinate) As Single
        Get
            Return cRot(pCord).Minimum
        End Get
        Set(value As Single)
            cRot(pCord).Minimum = value
        End Set
    End Property

    Public Property RotationStep(pCord As Coordinate) As Single
        Get
            Return cRot(pCord).Step
        End Get
        Set(value As Single)
            cRot(pCord).Step = value
        End Set
    End Property

    Public Property RotationStepRatio(pCord As Coordinate) As Single
        Get
            Return cRot(pCord).StepRatio
        End Get
        Set(value As Single)
            cRot(pCord).StepRatio = value
        End Set
    End Property

    Public Property RotationIncreasing(pCord As Coordinate) As Boolean
        Get
            Return cRot(pCord).isIncreasing
        End Get
        Set(value As Boolean)
            cRot(pCord).isIncreasing = value
        End Set
    End Property

    Public Property RotationFullCircle(pCord As Coordinate) As Boolean
        Get
            Return cRot(pCord).isFullCircle
        End Get
        Set(value As Boolean)
            cRot(pCord).isFullCircle = value
        End Set
    End Property

    Public Property RotationStop(pCord As Coordinate) As Boolean
        Get
            Return cRot(pCord).Stop
        End Get
        Set(value As Boolean)
            cRot(pCord).Stop = value
        End Set
    End Property

    Public Property RotationMaxMinRatio(pCord As Coordinate) As Single
        Get
            Return cRot(pCord).MaxMinRatio
        End Get
        Set(value As Single)
            cRot(pCord).MaxMinRatio = value
        End Set
    End Property
#End Region

    Public Sub CheckExists()

        If aProp.Exists = False Then

            aProp = World.CreateProp(pModel, pEntity.Position, False, False)
            Attach()
        End If
    End Sub

    Public Sub Play()

        If isDetached Then Exit Sub

        If isAnimationOn = False Then Exit Sub

        CheckExists()

        With cPos(0)
            If .Update Then

                If .isIncreasing Then

                    pOffset.X += .Step * .StepRatio

                    If pOffset.X > .Maximum * .MaxMinRatio Then

                        If .Stop Then .Update = False

                        .isIncreasing = False
                        pOffset.X = .Maximum * .MaxMinRatio
                    End If
                Else

                    pOffset.X -= .Step * .StepRatio

                    If pOffset.X < .Minimum * .MaxMinRatio Then

                        If .Stop Then .Update = False

                        .isIncreasing = True
                        pOffset.X = .Minimum * .MaxMinRatio
                    End If
                End If
            End If
        End With

        With cPos(1)
            If .Update Then

                If .isIncreasing Then

                    pOffset.Y += .Step * .StepRatio

                    If pOffset.Y > .Maximum * .MaxMinRatio Then

                        If .Stop Then .Update = False

                        .isIncreasing = False
                        pOffset.Y = .Maximum * .MaxMinRatio
                    End If
                Else

                    pOffset.Y -= .Step * .StepRatio

                    If pOffset.Y < .Minimum * .MaxMinRatio Then

                        If .Stop Then .Update = False

                        .isIncreasing = True
                        pOffset.Y = .Minimum * .MaxMinRatio
                    End If
                End If
            End If
        End With

        With cPos(2)
            If .Update Then

                If .isIncreasing Then

                    pOffset.Z += .Step * .StepRatio

                    If pOffset.Z > .Maximum * .MaxMinRatio Then

                        If .Stop Then .Update = False

                        .isIncreasing = False
                        pOffset.Z = .Maximum * .MaxMinRatio
                    End If
                Else

                    pOffset.Z -= .Step * .StepRatio

                    If pOffset.Z < .Minimum * .MaxMinRatio Then

                        If .Stop Then .Update = False

                        .isIncreasing = True
                        pOffset.Z = .Minimum * .MaxMinRatio
                    End If
                End If
            End If
        End With

        With cRot(0)
            If .Update Then

                If .isIncreasing Then

                    pRotation.X += .Step * .StepRatio

                    If pRotation.X > .Maximum * .MaxMinRatio Then

                        If .isFullCircle Then

                            pRotation.X -= 360
                        Else

                            If .Stop Then .Update = False

                            .isIncreasing = False
                            pRotation.X = .Maximum * .MaxMinRatio
                        End If
                    End If
                Else

                    pRotation.X -= .Step * .StepRatio

                    If pRotation.X < .Minimum * .MaxMinRatio Then

                        If .isFullCircle Then

                            pRotation.X += 360
                        Else

                            If .Stop Then .Update = False

                            .isIncreasing = True
                            pRotation.X = .Minimum * .MaxMinRatio
                        End If
                    End If
                End If
            End If
        End With

        With cRot(1)
            If .Update Then

                If .isIncreasing Then

                    pRotation.Y += .Step * .StepRatio

                    If pRotation.Y > .Maximum * .MaxMinRatio Then

                        If .isFullCircle Then

                            pRotation.Y -= 360
                        Else

                            If .Stop Then .Update = False

                            .isIncreasing = False
                            pRotation.Y = .Maximum * .MaxMinRatio
                        End If
                    End If
                Else

                    pRotation.Y -= .Step * .StepRatio

                    If pRotation.Y < .Minimum * .MaxMinRatio Then

                        If .isFullCircle Then

                            pRotation.Y += 360
                        Else

                            If .Stop Then .Update = False

                            .isIncreasing = True
                            pRotation.Y = .Minimum * .MaxMinRatio
                        End If
                    End If
                End If
            End If
        End With

        With cRot(2)
            If .Update Then

                If .isIncreasing Then

                    pRotation.Z += .Step * .StepRatio

                    If pRotation.Z > .Maximum * .MaxMinRatio Then

                        If .isFullCircle Then

                            pRotation.Z -= 360
                        Else

                            If .Stop Then .Update = False

                            .isIncreasing = False
                            pRotation.Z = .Maximum * .MaxMinRatio
                        End If
                    End If
                Else

                    pRotation.Z -= .Step * .StepRatio

                    If pRotation.Z < .Minimum * .MaxMinRatio Then

                        If .isFullCircle Then

                            pRotation.Z += 360
                        Else

                            If .Stop Then .Update = False

                            .isIncreasing = True
                            pRotation.Z = .Minimum * .MaxMinRatio
                        End If
                    End If
                End If
            End If
        End With

        Attach()
    End Sub
End Class
