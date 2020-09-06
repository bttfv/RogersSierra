Imports System.Runtime.InteropServices
Imports GTA
Imports GTA.Math

Public Module Commons

    Friend RndGenerator As New Random(Game.GameTime)

    Friend SpawnLocations As New List(Of SpawnLocation) From {New SpawnLocation(New Vector3(2416, -346, 94), True), New SpawnLocation(New Vector3(2348, 1182, 79), True), New SpawnLocation(New Vector3(2615, 2945, 39), True)}

    Friend MenuManager As New MenuManager

    Friend Class SpawnLocation

        Public Position As Vector3
        Public Direction As Boolean

        Public Sub New(position As Vector3, direction As Boolean)
            Me.Position = position
            Me.Direction = direction
        End Sub

        Public Overrides Function ToString() As String

            Return SpawnLocations.IndexOf(Me)
        End Function
    End Class

    Public Enum SmokeColor
        Off
        [Default]
        Green
        Yellow
        Red
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

    Friend Function CreateMissionTrain(var As Integer, pos As Math.Vector3, direction As Boolean) As Vehicle

        Return Native.Function.Call(Of Vehicle)(Native.Hash.CREATE_MISSION_TRAIN, var, pos.X, pos.Y, pos.Z, direction)
    End Function

    Friend Function IsCameraValid(cam As Camera) As Boolean

        Return IsNothing(cam) = False AndAlso cam.Position <> Math.Vector3.Zero
    End Function

    Friend Function getCurrentCharacter() As Ped

        Return Game.Player.Character
    End Function

    Friend Function getCurrentVehicle() As Vehicle

        Return getCurrentCharacter.CurrentVehicle
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
