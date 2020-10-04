Imports GTA
Imports GTA.Math

Public Class CustomCamera

    Public ReadOnly Entity As Entity
    Public ReadOnly PositionOffset As Vector3
    Public ReadOnly PointAtOffset As Vector3
    Public ReadOnly FieldOfView As Single

    Public ReadOnly Property Camera As Camera

    Public Sub New(entity As Entity, positionOffset As Vector3, pointAtOffset As Vector3, fieldOfView As Single)

        Me.Entity = entity
        Me.PositionOffset = positionOffset
        Me.PointAtOffset = pointAtOffset
        Me.FieldOfView = fieldOfView
    End Sub

    Public Sub Show(ByRef OldCamera As CustomCamera)

        If IsNothing(Camera) OrElse Camera.Exists = False Then

            _Camera = World.CreateCamera(Entity.Position, Entity.Rotation, FieldOfView)

            Camera.AttachTo(Entity, PositionOffset)
            Camera.PointAt(Entity, PointAtOffset)
        End If

        If IsNothing(OldCamera) OrElse IsNothing(OldCamera.Camera) OrElse OldCamera.Camera.Exists = False Then

            World.RenderingCamera = Camera
        Else

            Camera.IsActive = True
            OldCamera.Camera.IsActive = False

            OldCamera.Camera.InterpTo(Camera, 900, 1, 1)
        End If
    End Sub

    Public Sub [Stop]()

        Camera.IsActive = False

        World.RenderingCamera = Nothing
    End Sub

    Public Sub Abort()

        If Not IsNothing(Camera) Then

            Camera.Delete()
            _Camera = Nothing
        End If
    End Sub
End Class
