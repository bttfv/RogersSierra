Imports GTA

Public Class LightHandler

    Public Shared Lights As New List(Of Light)

    Shared Sub New()

        Lights.Add(New Light(New Math.Vector3(0, 0, 0), New Math.Vector3(0, 0, 0), 0, 0, 0, 0, 0))
    End Sub

    Public Shared Sub Draw(Entity As Entity)

        Lights.ForEach(Sub(x)
                           x.Draw(Entity)
                       End Sub)
    End Sub
End Class
