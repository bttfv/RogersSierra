Imports System.Drawing
Imports GTA
Imports GTA.Math
Imports GTA.Native

Public Class Light
    Public Sub New(
                   positionX As Single, positionY As Single, positionZ As Single,
                   directionX As Single, directionY As Single, directionZ As Single,
                   color As Color,
                   distance As Single, brightness As Single, roundness As Single, radius As Single, fadeout As Single)

        Me.PositionX = positionX
        Me.PositionY = positionY
        Me.PositionZ = positionZ

        Me.DirectionX = directionX
        Me.DirectionY = directionY
        Me.DirectionZ = directionZ

        Me.Distance = distance
        Me.Brightness = brightness
        Me.Roundness = roundness
        Me.Radius = radius
        Me.Fadeout = fadeout

        Me.Color = color
    End Sub

    Public Sub New(
                   sourceBone As String,
                   directionBone As String,
                   color As Color,
                   distance As Single, brightness As Single, roundness As Single, radius As Single, fadeout As Single)

        Me.SourceBone = sourceBone
        Me.DirectionBone = directionBone

        Me.Distance = distance
        Me.Brightness = brightness
        Me.Roundness = roundness
        Me.Radius = radius
        Me.Fadeout = fadeout

        Me.Color = color

        useBones = True
    End Sub

    Private useBones As Boolean

    Public Property SourceBone As String
    Public Property DirectionBone As String

    Public Property PositionX As Single
    Public Property PositionY As Single
    Public Property PositionZ As Single
    Public Property DirectionX As Single
    Public Property DirectionY As Single
    Public Property DirectionZ As Single
    Public Property Distance As Single
    Public Property Brightness As Single
    Public Property Roundness As Single
    Public Property Radius As Single
    Public Property Fadeout As Single
    Public Property Color As Color
    Public Property IsEnabled As Boolean = True

    Public Sub Draw(Entity As Entity, shadowId As Single)

        If Not IsEnabled Then
            Return
        End If

        Dim pos As Vector3
        Dim dir As Vector3

        If useBones Then

            pos = Entity.Bones(SourceBone).Position

            dir = Vector3.Subtract(Entity.Bones(DirectionBone).Position, Entity.Bones(SourceBone).Position)
            dir.Normalize()
        Else

            pos = Entity.GetOffsetPosition(New Vector3(PositionX, PositionY, PositionZ))
            dir = New Vector3(DirectionX, DirectionY, DirectionZ)
        End If

        [Function].Call(Hash._DRAW_SPOT_LIGHT_WITH_SHADOW,
                        pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, Color.R, Color.G, Color.B,
                        Distance, Brightness, Roundness, Radius, Fadeout, shadowId)
    End Sub
End Class
