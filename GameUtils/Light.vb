Imports System.Drawing
Imports GTA
Imports GTA.Math
Imports GTA.Native

Public Class Light
    Public Sub New(
                   positionX As Single, positionY As Single, positionZ As Single,
                   directionX As Single, directionY As Single, directionZ As Single,
                   color As Color,
                   distance As Single, brightness As Single, roundness As Single, radius As Single, fadeout As Single, optional timeDep As Boolean = true)
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

        Me.TimeDep = timeDep
    End Sub

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
    Public Property TimeDep As Boolean

    Public Sub Draw(Entity As Entity, shadowId As Single, brightness As Single)
        Dim pos = New Vector3(PositionX, PositionY, PositionZ)
        Dim dir = New Vector3(DirectionX, DirectionY, DirectionZ)

        pos = Entity.GetOffsetPosition(pos)

        If TimeDep Then
            Distance = brightness - 30
            Brightness = brightness - 30
        End If

        [Function].Call(Hash._DRAW_SPOT_LIGHT_WITH_SHADOW, 
                        pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, Color.R, Color.G, Color.B, 
                        Distance, Brightness, Roundness, Radius, Fadeout, shadowId)
    End Sub
End Class
