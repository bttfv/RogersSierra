Imports FusionLibrary
Imports FusionLibrary.Enums
Imports GTA.Math

Partial Public Class RogersSierra

    Public CustomCamera As New CustomCameraHandler

    Public Property Camera As TrainCamera
        Get

            Return CustomCamera.CurrentCameraIndex
        End Get
        Set(value As TrainCamera)

            If value = TrainCamera.Off Then

                CustomCamera.Abort()
            Else

                CustomCamera.Show(value)
            End If
        End Set
    End Property

    Public Property CycleCameras As Boolean
        Get
            Return CustomCamera.CycleCameras
        End Get
        Set(value As Boolean)
            CustomCamera.CycleCameras = value
        End Set
    End Property

    Public Property CycleCamerasInterval As Integer
        Get
            Return CustomCamera.CycleInterval
        End Get
        Set(value As Integer)
            CustomCamera.CycleInterval = value
        End Set
    End Property

    Private Sub LoadCamera()

        'TowardsRail
        CustomCamera.Add(Locomotive, New Vector3(0, 6, 1), New Vector3(0, 16, 1), 75)

        'Pilot
        CustomCamera.Add(Locomotive, New Vector3(0, 8, 0.1), New Vector3(0, 6, 1.1), 75)

        'Front
        CustomCamera.Add(Locomotive, New Vector3(0, 11, 5), New Vector3(0, -4, 0), 75)

        'RightFunnel
        CustomCamera.Add(Locomotive, New Vector3(1, -0.5, 3.5), New Vector3(1, 2.5, 3.5), 50)

        'RightWheels
        CustomCamera.Add(Locomotive, New Vector3(2, 2.5, 1), New Vector3(2, -10.5, 1), 50)

        'RightFrontWheels
        CustomCamera.Add(Locomotive, New Vector3(2, -0.5, 1.25), New Vector3(2, 2.5, 1.25), 50)

        'RightFront2Wheels
        CustomCamera.Add(Locomotive, New Vector3(3, -8, 1), New Vector3(2, 2.5, 1), 50)

        'RightSide
        CustomCamera.Add(Locomotive, New Vector3(7.5, -4, 8), New Vector3(0, -4, 0), 75)

        'TopCabin
        CustomCamera.Add(Locomotive, New Vector3(0, -6, 7), New Vector3(0, 3, 5), 75)

        'LeftSide
        CustomCamera.Add(Locomotive, New Vector3(-7.5, -4, 8), New Vector3(0, -4, 0), 75)

        'LeftFunnel
        CustomCamera.Add(Locomotive, New Vector3(-1, -0.5, 3.5), New Vector3(-1, 2.5, 3.5), 50)

        'LeftWheels
        CustomCamera.Add(Locomotive, New Vector3(-2, 2.5, 1), New Vector3(-2, -10.5, 1), 50)

        'LeftFrontWheels
        CustomCamera.Add(Locomotive, New Vector3(-2, -0.5, 1.25), New Vector3(-2, 2.5, 1.25), 50)

        'LeftFront2Wheels
        CustomCamera.Add(Locomotive, New Vector3(-3, -8, 1), New Vector3(-2, 2.5, 1), 50)

        'Inside
        CustomCamera.Add(Locomotive, New Vector3(0, -6, 2.5), New Vector3(0, -3, 2.5), 75)

        'WheelieUp
        CustomCamera.Add(Locomotive, New Vector3(2.6, 11.3, 1.36), New Vector3(2, 10.5, 1.37), 50)

        'WheelieDown
        CustomCamera.Add(Locomotive, New Vector3(1.86, 9, 0.68), New Vector3(0.9, 8.75, 0.5), 50)
    End Sub
End Class
