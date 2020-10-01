Imports GTA
Imports GTA.Math

Public Class CustomerCameraManager

    Private Camera As Camera

    Public ReadOnly Property Cameras As New List(Of CustomCamera)
    Public ReadOnly Property CurrentCameraIndex As Integer = -1

    Public ReadOnly Property CurrentCamera As CustomCamera
        Get

            Return IIf(CurrentCameraIndex = -1, Nothing, Cameras(CurrentCameraIndex))
        End Get
    End Property

    Public Function Add(entity As Entity, positionOffset As Vector3, pointAtOffset As Vector3, fieldOfView As Single) As CustomCamera

        Dim ret As New CustomCamera(entity, positionOffset, pointAtOffset, fieldOfView)

        Cameras.Add(ret)

        Return ret
    End Function

    Public Sub Show(index As Integer)

        If Cameras.Count = 0 Then

            Exit Sub
        End If

        If index > Cameras.Count - 1 OrElse index < 0 Then

            Exit Sub
        End If

        Cameras(index).Show(Camera)
        _CurrentCameraIndex = index
    End Sub

    Public Sub ShowNext()

        If Cameras.Count = 0 Then

            Exit Sub
        End If

        If CurrentCameraIndex = Cameras.Count - 1 Then

            [Stop]()
        Else

            _CurrentCameraIndex += 1
            CurrentCamera.Show(Camera)
        End If
    End Sub

    Public Sub ShowPrevious()

        If Cameras.Count = 0 Then

            Exit Sub
        End If

        If CurrentCameraIndex <= 0 Then

            [Stop]()
        Else

            _CurrentCameraIndex -= 1
            CurrentCamera.Show(Camera)
        End If
    End Sub

    Public Sub [Stop]()

        If CurrentCameraIndex > -1 Then

            CurrentCamera.Stop(Camera)

            _CurrentCameraIndex = -1
        End If
    End Sub

    Public Sub Check()

        If CurrentCameraIndex > 0 Then

            If IsNothing(Camera) OrElse Camera.Exists = False Or Camera <> World.RenderingCamera Then

                Camera = Nothing
                World.RenderingCamera = Nothing
                _CurrentCameraIndex = -1
            End If
        End If
    End Sub
End Class
