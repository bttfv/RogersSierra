Imports GTA

Public Class LightHandler

    Public Shared Lights As New List(Of Light)

    Public Shared Sub Draw(Entity As Entity)
        Dim hour = World.CurrentDate.TimeOfDay.Hours
        Dim mills = World.CurrentDate.TimeOfDay.TotalMilliseconds
        Dim brightness As Single

        'If hour < 12 Then
        '    brightness = mills / 43200000 * 100
        'Else
        '    brightness = 100 - ((mills-43200000) / 43200000 * 100)
        'End If
        brightness = 1.6418462 * hour +12.3018462

        GTA.UI.Screen.ShowSubtitle(brightness)

        Lights.ForEach(Sub(x)
                           x.Draw(Entity, Lights.IndexOf(x), brightness)
                       End Sub)
    End Sub
End Class
