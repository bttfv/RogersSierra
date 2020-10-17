Imports System.Runtime.InteropServices
Imports GTA
Imports GTA.Math

Public Module Commons

    Friend RndGenerator As New Random(Game.GameTime)

    Friend SpawnLocations As New List(Of SpawnLocation) From 
    {
        New SpawnLocation(New Vector3(2611, 1681, 27), New Vector3(2601.0, 1700.2, 29.9), New Vector3(0.8, -0.6, -0.1)), 
        New SpawnLocation(New Vector3(2462, -289, 93), New Vector3(2455.4, -276.4, 96.2), New Vector3(0.4, -0.9, 0.1)), 
        New SpawnLocation(New Vector3(2014, 2493, 58), New Vector3(2028.6, 2482.9, 67.6), New Vector3(-1.0, 0.2, 0.0)),
        New SpawnLocation(New Vector3(2994, 3990, 57), New Vector3(3004.8, 3983.3, 60.0), New Vector3(-0.5, 0.9, 0.0)),
        New SpawnLocation(New Vector3(1807, 3510, 39), New Vector3(1828.1, 3535.5, 45.2), New Vector3(-0.5, -0.9, 0.1)),
        New SpawnLocation(New Vector3(-478, 5253, 88), New Vector3(-476.1, 5215.4, 98.9), New Vector3(-0.5, 0.8, -0.2)),
        New SpawnLocation(New Vector3(749, 6433, 30), New Vector3(765.6, 6449.3, 34.3), New Vector3(-0.9, -0.4, 0.2)),
        New SpawnLocation(New Vector3(2486, 5743, 64), New Vector3(2510.5, 5716.2, 68.4), New Vector3(-1.0, 0.3, 0.1))
    }

    Friend MenuManager As New MenuManager

    Friend Class SpawnLocation

        Public Position As Vector3
        Public CameraPos As Vector3 = Vector3.Zero
        Public CameraDir As Vector3 = Vector3.Zero
        Public Direction As Boolean
        Public Name As String

        Public Sub New(position As Vector3, direction As Boolean)
            Me.Position = position
            Me.Direction = direction
            Name = World.GetZoneLocalizedName(position)
        End Sub

        Public Sub New(position As Vector3, cameraPos As Vector3, cameraDir As Vector3)
            Me.Position = position
            Me.Direction = True
            Name = World.GetZoneLocalizedName(position)
            Me.CameraPos = cameraPos
            Me.CameraDir = cameraDir
        End Sub

        Public Overrides Function ToString() As String

            Return Name
        End Function
    End Class

    Public Enum SmokeColor
        Off
        [Default]
        Green
        Yellow
        Red
    End Enum

    Public Enum TrainCamera
        Off = -1
        TowardsRail
        Pilot
        Front
        RightFunnel
        RightWheels
        RightFrontWheels
        RightFront2Wheels
        RightSide
        TopCabin
        LeftSide
        LeftFunnel
        LeftWheels
        LeftFrontWheels
        LeftFront2Wheels
        Inside
    End Enum

    Friend Enum Coordinate
        X
        Y
        Z
    End Enum

    Friend Structure CoordinateSetting

        Dim Update As Boolean
        Dim isIncreasing As Boolean
        Dim Minimum As Single
        Dim Maximum As Single
        Dim MaxMinRatio As Single
        Dim [Step] As Single
        Dim StepRatio As Single
        Dim isFullCircle As Boolean
        Dim [Stop] As Boolean
    End Structure

    Public Enum AnimationStep
        Off
        First
        Second
        Third
        Fourth
        Fifth
    End Enum

    Friend Sub ShowSubtitle(txt As String)

        UI.Screen.ShowSubtitle(txt)
    End Sub

    Friend Function LoadAndRequestModel(modelName As String) As Model
        Dim ret As New Model(modelName)

        ret.Request()

        Do While ret.IsLoaded = False
            Script.Yield()
        Loop

        Return ret
    End Function

    Friend Function IsNight() As Boolean

        Return World.CurrentTimeOfDay.Hours >= 20 OrElse World.CurrentTimeOfDay.Hours <= 5
    End Function

    Friend Function CreateMissionTrain(var As Integer, pos As Math.Vector3, direction As Boolean) As Vehicle

        Return Native.Function.Call(Of Vehicle)(Native.Hash.CREATE_MISSION_TRAIN, var, pos.X, pos.Y, pos.Z, direction)
    End Function

    Friend Function IsCameraValid(cam As Camera) As Boolean

        Return IsNothing(cam) = False AndAlso cam.Position <> Math.Vector3.Zero
    End Function

    Friend Function PlayerPed() As Ped

        Return Game.Player.Character
    End Function

    Friend Function PlayerVehicle() As Vehicle

        Return PlayerPed.CurrentVehicle
    End Function

    <Runtime.CompilerServices.Extension>
    Friend Function GetSingleOffset(vect3 As Math.Vector3, coord As Coordinate, value As Single) As Math.Vector3

        Select Case coord
            Case Coordinate.X
                vect3.X += value
            Case Coordinate.Y
                vect3.Y += value
            Case Coordinate.Z
                vect3.Z += value
        End Select

        Return vect3
    End Function

    <Runtime.CompilerServices.Extension>
    Friend Sub AttachToPhisically(ent1 As Entity, ent2 As Entity, pOffset As Math.Vector3, vRotation As Math.Vector3, breakForce As Single)

        Native.Function.Call(Native.Hash.ATTACH_ENTITY_TO_ENTITY_PHYSICALLY, ent1, ent2, 0, 0, pOffset.X, pOffset.Y, pOffset.Z, 0, 0, 0, vRotation.X, vRotation.Y, vRotation.Z, breakForce, True, True, False, False, 2)
    End Sub

    <Runtime.CompilerServices.Extension>
    Friend Sub setTrainCruiseSpeed(veh As Vehicle, speed As Single)

        Native.Function.Call(Native.Hash.SET_TRAIN_CRUISE_SPEED, veh.Handle, speed)
    End Sub

    <Runtime.CompilerServices.Extension>
    Friend Sub setTrainPosition(veh As Vehicle, pos As Math.Vector3)

        Native.Function.Call(Native.Hash.SET_MISSION_TRAIN_COORDS, veh.Handle, pos.X, pos.Y, pos.Z)
    End Sub

    <Runtime.CompilerServices.Extension>
    Friend Sub setTrainCruiseSpeedMPH(veh As Vehicle, speed As Integer)

        Native.Function.Call(Native.Hash.SET_TRAIN_CRUISE_SPEED, veh.Handle, MphToMs(speed))
    End Sub

    <Runtime.CompilerServices.Extension>
    Friend Sub setTrainSpeed(veh As Vehicle, speed As Single)

        Native.Function.Call(Native.Hash.SET_TRAIN_SPEED, veh.Handle, speed)
    End Sub

    <Runtime.CompilerServices.Extension>
    Friend Sub setTrainSpeedMPH(veh As Vehicle, speed As Integer)

        Native.Function.Call(Native.Hash.SET_TRAIN_SPEED, veh.Handle, MsToMph(speed))
    End Sub

    <Runtime.CompilerServices.Extension>
    Friend Sub setSpeedMPH(veh As GTA.Vehicle, speed As Integer)

        veh.ForwardSpeed = MphToMs(speed)
    End Sub

    <Runtime.CompilerServices.Extension>
    Friend Sub MakeTrainDerail(veh As Vehicle)

        Native.Function.Call(Native.Hash.SET_RENDER_TRAIN_AS_DERAILED, veh.Handle, True)
    End Sub

    <Runtime.CompilerServices.Extension>
    Friend Function IsATrain(veh As Vehicle) As Boolean

        Return Native.Function.Call(Of Boolean)(Native.Hash.IS_THIS_MODEL_A_TRAIN, veh.Model.Hash)
    End Function

    <Runtime.CompilerServices.Extension>
    Friend Function GetTrainCarriage(veh As Vehicle, index As Integer) As Vehicle

        Return Native.Function.Call(Of Vehicle)(Native.Hash.GET_TRAIN_CARRIAGE, veh.Handle, index)
    End Function

    <Runtime.CompilerServices.Extension>
    Friend Function SpeedMPHf(veh As Vehicle) As Single

        Return SpeedKMHf(veh) * 0.62137119
    End Function

    <Runtime.CompilerServices.Extension>
    Friend Function SpeedKMHf(veh As Vehicle) As Single

        Return veh.Speed * 3.6
    End Function

    <Runtime.CompilerServices.Extension>
    Friend Function SpeedMPH(veh As Vehicle) As Integer

        Return System.Math.Round(SpeedMPHf(veh))
    End Function

    <Runtime.CompilerServices.Extension>
    Friend Function SpeedKMH(veh As Vehicle) As Integer

        Return System.Math.Round(SpeedKMHf(veh))
    End Function

    <Runtime.CompilerServices.Extension>
    Friend Function isGoingForward(veh As Vehicle) As Boolean

        Return Native.Function.Call(Of Math.Vector3)(Native.Hash.GET_ENTITY_SPEED_VECTOR, veh.Handle, True).Y > 0
    End Function
End Module
