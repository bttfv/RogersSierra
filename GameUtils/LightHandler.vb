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

    Public Sub Add(
                   sourceBone As String,
                   directionBone As String,
                   color As Color,
                   distance As Single, brightness As Single, roundness As Single, radius As Single, fadeout As Single)

        Lights.Add(New Light(sourceBone, directionBone, color, distance, brightness, roundness, radius, fadeout))
    End Sub

    Public Sub Add(
                   positionX As Single, positionY As Single, positionZ As Single,
                   directionX As Single, directionY As Single, directionZ As Single,
                   color As Color,
                   distance As Single, brightness As Single, roundness As Single, radius As Single, fadeout As Single)

        Lights.Add(New Light(positionX, positionY, positionZ, directionX, directionY, directionZ, color, distance, brightness, roundness, radius, fadeout))
    End Sub

    Public Sub Draw()

        'Dim hour = World.CurrentDate.TimeOfDay.Hours
        'Dim mills = World.CurrentDate.TimeOfDay.TotalMilliseconds

        'Dim brightness As Single

        'Select Case hour
        '    Case < 8, >= 20
        '        brightness = 0
        '    Case < 12
        '        brightness = ((mills - 28740000) / 15140000) * 100
        '    Case < 20
        '        brightness = ((72000000 - mills) / 28800000) * 100
        'End Select

        Lights.ForEach(Sub(x)

                           x.Draw(Entity, (Lights.IndexOf(x) + 1) * ShadowMulti)
                       End Sub)
    End Sub
End Class
