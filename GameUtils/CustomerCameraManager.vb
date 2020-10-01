Imports GTA
Imports GTA.Math

Public Class CustomerCameraManager

    Public ReadOnly Property Cameras As New List(Of CustomCamera)
    Public ReadOnly Property CurrentCameraIndex As Integer = -1

    Public Property CycleCameras As Boolean = False

    Private _CycleInterval As Integer = 10000

    Public Property CycleInterval As Integer
        Get
            Return _CycleInterval
        End Get
        Set(value As Integer)

            If CycleCameras Then

                nextChange -= _CycleInterval
                nextChange += value
            End If

            _CycleInterval = value
        End Set
    End Property

    Private nextChange As Integer = 0

    Public ReadOnly Property CurrentCamera As CustomCamera
        Get

            If CurrentCameraIndex = -1 Then

                Return Nothing
            Else

                Return Cameras(CurrentCameraIndex)
            End If
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

        Cameras(index).Show(CurrentCamera)
        _CurrentCameraIndex = index
    End Sub

    Public Sub ShowNext()

        If Cameras.Count = 0 Then

            Exit Sub
        End If

        If CurrentCameraIndex = Cameras.Count - 1 Then

            Abort()
        Else

            Cameras(CurrentCameraIndex + 1).Show(CurrentCamera)
            _CurrentCameraIndex += 1
        End If
    End Sub

    Public Sub ShowPrevious()

        If Cameras.Count = 0 Then

            Exit Sub
        End If

        If CurrentCameraIndex <= 0 Then

            Abort()
        Else

            Cameras(CurrentCameraIndex - 1).Show(CurrentCamera)
            _CurrentCameraIndex -= 1
        End If
    End Sub

    Public Sub [Stop]()

        If CurrentCameraIndex > -1 Then

            CurrentCamera.Stop()
            _CurrentCameraIndex = -1
        End If
    End Sub

    Public Sub Abort()

        [Stop]()

        Cameras.ForEach(Sub(x)

                            x.Abort()
                        End Sub)

        World.DestroyAllCameras()
    End Sub

    Public Sub Process()

        If CycleCameras Then

            If nextChange < Game.GameTime Then

                ShowNext()
                nextChange = Game.GameTime + CycleInterval
            End If
        End If
    End Sub
End Class
