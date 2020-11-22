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

    Public Sub Show(ByRef OldCamera As CustomCamera, Optional cameraSwitchType As CameraSwitchType = CameraSwitchType.Instant)

        If IsNothing(Camera) OrElse Camera.Exists = False Then

            _Camera = World.CreateCamera(Entity.Position, Entity.Rotation, FieldOfView)

            Camera.AttachTo(Entity, PositionOffset) '.GetSingleOffset(Coordinate.Y, -4.15))
            Camera.PointAt(Entity, PointAtOffset) '.GetSingleOffset(Coordinate.Y, -4.15))
        End If

        If IsNothing(OldCamera) OrElse IsNothing(OldCamera.Camera) OrElse OldCamera.Camera.Exists = False Then

            World.RenderingCamera = Camera
        Else

            Camera.IsActive = True
            OldCamera.Camera.IsActive = False

            If cameraSwitchType = CameraSwitchType.Animated Then

                OldCamera.Camera.InterpTo(Camera, 900, 1, 1)
            Else

                World.RenderingCamera = Camera
            End If
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
