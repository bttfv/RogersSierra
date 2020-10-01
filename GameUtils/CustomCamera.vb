Imports GTA
Imports GTA.Math

Public Class CustomCamera

    Public ReadOnly Entity As Entity
    Public ReadOnly PositionOffset As Vector3
    Public ReadOnly PointAtOffset As Vector3
    Public ReadOnly FieldOfView As Single

    Public Sub New(entity As Entity, positionOffset As Vector3, pointAtOffset As Vector3, fieldOfView As Single)

        Me.Entity = entity
        Me.PositionOffset = positionOffset
        Me.PointAtOffset = pointAtOffset
        Me.FieldOfView = fieldOfView
    End Sub

    Public Sub Show(ByRef Camera As Camera)

        If IsNothing(Camera) OrElse Camera.Exists = False Then

            Camera = World.CreateCamera(Entity.Position, Entity.Rotation, FieldOfView)
        Else

            Camera.FieldOfView = FieldOfView
        End If

        Camera.AttachTo(Entity, PositionOffset)
        Camera.PointAt(Entity, PointAtOffset)

        World.RenderingCamera = Camera
    End Sub

    Public Sub [Stop](ByRef Camera As Camera)

        Camera.Delete()
        Camera = Nothing

        World.RenderingCamera = Nothing
    End Sub
End Class
