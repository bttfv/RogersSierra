Imports GTA
Imports GTA.Math

Public Class Light
    Public Sub New(position As Vector3, rotation As Vector3, distance As Single, brightness As Single, roundness As Single, radius As Single, fadeout As Single)
        Me.Position = position
        Me.Rotation = rotation
        Me.Distance = distance
        Me.Brightness = brightness
        Me.Roundness = roundness
        Me.Radius = radius
        Me.Fadeout = fadeout
    End Sub

    Public Property Position As Vector3
    Public Property Rotation As Vector3
    Public Property Distance As Single
    Public Property Brightness As Single
    Public Property Roundness As Single
    Public Property Radius As Single
    Public Property Fadeout As Single

    Public Sub Draw(Entity As Entity)

        World.DrawSpotLightWithShadow(Entity.GetOffsetPosition(Position), Rotation, Drawing.Color.White, Distance, Brightness, Roundness, Radius, Fadeout)
    End Sub
End Class
