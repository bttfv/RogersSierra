Imports GTA

Public Class LightHandler

    Public Shared Lights As New List(Of Light)

    Shared Sub New()

        'Lights.Add(New Light(New Math.Vector3(1.954406, -1.011467, 3.10197), New Math.Vector3(-7.559062, 0, 20.53542), 6, 49, 0, 1000, 100))

        'Lights.Add(New Light(New Math.Vector3(-1.857859, -0.8503271, 2.956698), New Math.Vector3(-7.937012, 0, -93.60631), 6, 49, 0, 1000, 100))

        'Lights.Add(New Light(New Math.Vector3(0.06846464, -2.795146, 3.148575), New Math.Vector3(-10.96063, 0, -40.12598), 6, 49, 0, 1000, 100))

        'Lights.Add(New Light(New Math.Vector3(10.46642, 3.308652, 2.527255), New Math.Vector3(3.086617, 0, 63.81097), 100, 32, 0, 1000, 100))

        'Lights.Add(New Light(New Math.Vector3(-9.813047, -0.06620856, 2.344684), New Math.Vector3(9.637792, 0, -102.1104), 100, 32, 0, 1000, 100))
    End Sub

    Public Shared Sub Draw(Entity As Entity)

        Lights.ForEach(Sub(x)
                           x.Draw(Entity)
                       End Sub)
    End Sub
End Class
