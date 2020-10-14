Imports System.Drawing
Imports GTA

Public Class LightHandler

    Public Lights As New List(Of Light)

    Private Entity As Entity

    Private ShadowMulti As Integer

    Sub New(entity As Entity, shadowMulti As Integer)

        Me.Entity = entity
        Me.ShadowMulti = shadowMulti * 10
    End Sub

    Public Function Add(
                   sourceBone As String,
                   directionBone As String,
                   color As Color,
                   distance As Single, brightness As Single, roundness As Single, radius As Single, fadeout As Single) As Light

        Lights.Add(New Light(sourceBone, directionBone, color, distance, brightness, roundness, radius, fadeout))

        Return Lights.Last
    End Function

    Public Function Add(
                   positionX As Single, positionY As Single, positionZ As Single,
                   directionX As Single, directionY As Single, directionZ As Single,
                   color As Color,
                   distance As Single, brightness As Single, roundness As Single, radius As Single, fadeout As Single) As Light

        Lights.Add(New Light(positionX, positionY, positionZ, directionX, directionY, directionZ, color, distance, brightness, roundness, radius, fadeout))

        Return Lights.Last
    End Function

    Public Sub Draw()

        Lights.ForEach(Sub(x)

                           x.Draw(Entity, (Lights.IndexOf(x) + 1) * ShadowMulti)
                       End Sub)
    End Sub
End Class
