Imports BTTFVLibrary.Enums

Friend Class AnimatePropHandler

    Public Props As New List(Of AnimateProp)

    Public Property isAnimationOn As Boolean
        Get
            Return Props(0).isAnimationOn
        End Get
        Set(value As Boolean)
            Props.ForEach(Sub(x)
                              x.isAnimationOn = value
                          End Sub)
        End Set
    End Property

    Public Sub CheckExists()

        Props.ForEach(Sub(x)
                          x.CheckExists()
                      End Sub)
    End Sub

    Public Sub Detach()

        Props.ForEach(Sub(x)
                          x.Detach()
                      End Sub)
    End Sub

    Public Sub ScatterProps(Optional ForceMultiplier As Single = 1)

        Props.ForEach(Sub(x)
                          x.ScatterProp(ForceMultiplier)
                      End Sub)
    End Sub

    Public Sub Play()

        Props.ForEach(Sub(x)
                          x.Play()
                      End Sub)
    End Sub

    Public Sub DeleteAll()

        Props.ForEach(Sub(x)

                          x.Delete()
                      End Sub)

        Props.Clear()
    End Sub

    Public Sub TransferAllTo(newEntity As GTA.Entity)

        Props.ForEach(Sub(x)

                          x.TransferTo(newEntity)
                      End Sub)
    End Sub

    Public Sub TransferAllTo(newEntity As GTA.Entity, boneName As String)

        Props.ForEach(Sub(x)

                          x.TransferTo(newEntity, boneName)
                      End Sub)
    End Sub

    Public Sub setPositionSettings(pCord As Coordinate, cUpdate As Boolean, cIncreasing As Boolean, cMinimum As Single, cMaximum As Single, cStep As Single, Optional cStepRatio As Single = 1)

        Props.ForEach(Sub(x)
                          x.setPositionSettings(pCord, cUpdate, cIncreasing, cMinimum, cMaximum, cStep, cStepRatio)
                      End Sub)
    End Sub

    Public Sub setRotationSettings(pCord As Coordinate, cUpdate As Boolean, cIncreasing As Boolean, cMinimum As Single, cMaximum As Single, cStep As Single, cFullCircle As Boolean, Optional cStepRatio As Single = 1)

        Props.ForEach(Sub(x)
                          x.setRotationSettings(pCord, cUpdate, cIncreasing, cMinimum, cMaximum, cStep, cFullCircle, cStepRatio)
                      End Sub)
    End Sub

    Public Property Visible() As Boolean
        Get
            Return Props.First.Visible
        End Get
        Set(value As Boolean)
            Props.ForEach(Sub(x)
                              x.Visible = value
                          End Sub)
        End Set
    End Property

#Region "Position properties"
    Public Property Position(index As Integer) As GTA.Math.Vector3
        Get
            Return Props(index).Position
        End Get
        Set(value As GTA.Math.Vector3)
            Props(index).Position = value
        End Set
    End Property

    Public Property PositionUpdate(pCord As Coordinate) As Boolean
        Get
            Return Props(0).PositionUpdate(pCord)
        End Get
        Set(value As Boolean)
            Props.ForEach(Sub(x)
                              x.PositionUpdate(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property PositionMaximum(pCord As Coordinate) As Single
        Get
            Return Props(0).PositionMaximum(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.PositionMaximum(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property PositionMinimum(pCord As Coordinate) As Single
        Get
            Return Props(0).PositionMinimum(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.PositionMinimum(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property PositionStep(pCord As Coordinate) As Single
        Get
            Return Props(0).PositionStep(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.PositionStep(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property PositionStepRatio(pCord As Coordinate) As Single
        Get
            Return Props(0).PositionStepRatio(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.PositionStepRatio(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property PositionIncreasing(pCord As Coordinate) As Boolean
        Get
            Return Props(0).PositionIncreasing(pCord)
        End Get
        Set(value As Boolean)
            Props.ForEach(Sub(x)
                              x.PositionIncreasing(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property PositionMaxMinRatio(pCord As Coordinate) As Single
        Get
            Return Props(0).PositionMaxMinRatio(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.PositionMaxMinRatio(pCord) = value
                          End Sub)
        End Set
    End Property
#End Region

#Region "Rotation properties"
    Public Property AllRotation(pCord As Coordinate) As Single
        Get
            Return Props(0).Rotation(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.Rotation(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property Rotation(index As Integer) As GTA.Math.Vector3
        Get
            Return Props(index).Rotation
        End Get
        Set(value As GTA.Math.Vector3)
            Props(index).Rotation = value
        End Set
    End Property

    Public Property RotationUpdate(pCord As Coordinate) As Boolean
        Get
            Return Props(0).RotationUpdate(pCord)
        End Get
        Set(value As Boolean)
            Props.ForEach(Sub(x)
                              x.RotationUpdate(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property RotationMaximum(pCord As Coordinate) As Single
        Get
            Return Props(0).RotationMaximum(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.RotationMaximum(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property RotationMinimum(pCord As Coordinate) As Single
        Get
            Return Props(0).RotationMinimum(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.RotationMinimum(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property RotationStep(pCord As Coordinate) As Single
        Get
            Return Props(0).RotationStep(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.RotationStep(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property RotationStepRatio(pCord As Coordinate) As Single
        Get
            Return Props(0).RotationStepRatio(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.RotationStepRatio(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property RotationIncreasing(pCord As Coordinate) As Boolean
        Get
            Return Props(0).RotationIncreasing(pCord)
        End Get
        Set(value As Boolean)
            Props.ForEach(Sub(x)
                              x.RotationIncreasing(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property RotationFullCircle(pCord As Coordinate) As Boolean
        Get
            Return Props(0).RotationFullCircle(pCord)
        End Get
        Set(value As Boolean)
            Props.ForEach(Sub(x)
                              x.RotationFullCircle(pCord) = value
                          End Sub)
        End Set
    End Property

    Public Property RotationMaxMinRatio(pCord As Coordinate) As Single
        Get
            Return Props(0).RotationMaxMinRatio(pCord)
        End Get
        Set(value As Single)
            Props.ForEach(Sub(x)
                              x.RotationMaxMinRatio(pCord) = value
                          End Sub)
        End Set
    End Property
#End Region

End Class
