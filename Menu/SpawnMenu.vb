Imports System.ComponentModel
Imports System.Drawing
Imports GTA
Imports GTA.Math
Imports LemonUI.Menus
Imports FusionLibrary
Imports FusionLibrary.Extensions
Imports FusionLibrary.Enums

Friend Class SpawnMenu
    Inherits CustomNativeMenu

    Private SpawnMode As Boolean

    Private LocationCamera As Camera
    Private LocationBlip As Blip

    Private WithEvents SelectLocation As New NativeListItem(Of SpawnLocation)(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Location"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Location_Description"))
    Private WithEvents SpawnTrain As New NativeItem(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Spawn"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Spawn_Description"))
    Private WithEvents DeleteTrain As New NativeListItem(Of RogersSierra)(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Delete"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Delete_Description"))
    Private WithEvents DeleteAllTrains As New NativeItem(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_DeleteAll"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_DeleteAll_Description"))
    Private WithEvents WarpPlayer As New NativeCheckboxItem(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_WarpPlayer"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_WarpPlayer_Description"), True)
    Private WithEvents RandomTrains As New NativeCheckboxItem(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_RandomTrains"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_RandomTrains_Description"), False)

    Public Sub New()
        MyBase.New("", Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Subtitle"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Description"))

        SpawnLocation.SpawnLocations = New List(Of SpawnLocation) From {
                    New SpawnLocation(New Vector3(2611, 1681, 27), New Vector3(2601.0, 1700.2, 29.9), New Vector3(0.8, -0.6, -0.1), False),
                    New SpawnLocation(New Vector3(2462, -289, 93), New Vector3(2455.4, -276.4, 96.2), New Vector3(0.4, -0.9, 0.1)),
                    New SpawnLocation(New Vector3(2014, 2493, 58), New Vector3(2028.6, 2482.9, 67.6), New Vector3(-1.0, 0.2, 0.0)),
                    New SpawnLocation(New Vector3(2994, 3990, 57), New Vector3(3004.8, 3983.3, 60.0), New Vector3(-0.5, 0.9, 0.0)),
                    New SpawnLocation(New Vector3(1807, 3510, 39), New Vector3(1828.1, 3535.5, 45.2), New Vector3(-0.5, -0.9, 0.1)),
                    New SpawnLocation(New Vector3(-478, 5253, 88), New Vector3(-476.1, 5215.4, 98.9), New Vector3(-0.5, 0.8, -0.2)),
                    New SpawnLocation(New Vector3(749, 6433, 30), New Vector3(765.6, 6449.3, 34.3), New Vector3(-0.9, -0.4, 0.2)),
                    New SpawnLocation(New Vector3(2486, 5743, 64), New Vector3(2510.5, 5716.2, 68.4), New Vector3(-1.0, 0.3, 0.1))}

        Banner = New LemonUI.Elements.ScaledTexture(New PointF(0, 0), New SizeF(200, 200), "sierra_gui", "sierra_menu_logo")

        SelectLocation.Items = SpawnLocation.SpawnLocations
        DeleteTrain.Items = RogersSierraList

        Add(SelectLocation)
        Add(WarpPlayer)
        Add(SpawnTrain)
        Add(RandomTrains)
        Add(DeleteTrain)
        Add(DeleteAllTrains)
    End Sub

    Private Sub SpawnMenu_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        ResetCamera()
    End Sub

    Public Sub ResetCamera()

        If LocationCamera <> Nothing AndAlso LocationCamera.Exists Then

            LocationCamera.Delete()
        End If

        Native.Function.Call(Native.Hash.UNLOCK_MINIMAP_POSITION)

        World.DestroyAllCameras()
        World.RenderingCamera = Nothing

        If IsNothing(LocationBlip) = False AndAlso LocationBlip.Exists() Then

            LocationBlip.Delete()
        End If
    End Sub

    Private Sub SelectLocation_ItemChanged(sender As Object, e As ItemChangedEventArgs(Of SpawnLocation)) Handles SelectLocation.ItemChanged

        ShowLocation()
    End Sub

    Public Sub ShowLocation()

        Dim position = SelectLocation.SelectedItem.Position

        Native.Function.Call(Native.Hash.NEW_LOAD_SCENE_START_SPHERE, position.X, position.Y, position.Z, 100, 0)

        If LocationCamera <> Nothing AndAlso LocationCamera.Exists Then

            LocationCamera.Delete()
        End If

        Dim cameraPos = SelectLocation.SelectedItem.CameraPos
        Dim cameraDir = SelectLocation.SelectedItem.CameraDir

        If cameraPos <> Vector3.Zero AndAlso cameraDir <> Vector3.Zero Then

            LocationCamera = World.CreateCamera(cameraPos, Vector3.Zero, 75)
            LocationCamera.Direction = cameraDir
        Else

            LocationCamera = World.CreateCamera(position.GetSingleOffset(Coordinate.Z, 10).GetSingleOffset(Coordinate.Y, 10), Vector3.Zero, 75)
            LocationCamera.PointAt(position)
        End If

        World.RenderingCamera = LocationCamera

        Native.Function.Call(Native.Hash.LOCK_MINIMAP_POSITION, position.X, position.Y)

        If IsNothing(LocationBlip) = False AndAlso LocationBlip.Exists() Then

            LocationBlip.Delete()
        End If

        LocationBlip = World.CreateBlip(position)
        LocationBlip.Sprite = 120

        SelectLocation.Description = SelectLocation.SelectedItem.Name
        Recalculate()
    End Sub

    Private Sub SpawnTrain_Activated(sender As Object, e As EventArgs) Handles SpawnTrain.Activated, SelectLocation.Activated

        CreateRogersSierra(SelectLocation.SelectedItem.Position, WarpPlayer.Checked, SelectLocation.SelectedItem.Direction)

        Close()
    End Sub

    Private Sub SpawnMenu_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        ShowLocation()

        DeleteTrain.Enabled = RogersSierraList.Count > 0

        DeleteTrain.Items = RogersSierraList
    End Sub

    Private Sub DeleteTrain_Activated(sender As Object, e As EventArgs) Handles DeleteTrain.Activated

        DeleteTrain.SelectedItem.Delete()

        RogersSierraToRemove.Remove(DeleteTrain.SelectedItem)

        RogersSierraList.Remove(DeleteTrain.SelectedItem)

        Close()
    End Sub

    Private Sub DeleteTrain_Selected(sender As Object, e As SelectedEventArgs) Handles DeleteTrain.Selected

        If DeleteTrain.Enabled Then

            If SpawnMode = False Then

                Exit Sub
            Else

                SpawnMode = False
            End If

            ResetCamera()

            ShowTrain()
        End If
    End Sub

    Public Sub ShowTrain()

        Dim position = DeleteTrain.SelectedItem.Locomotive.Position

        Native.Function.Call(Native.Hash.NEW_LOAD_SCENE_START_SPHERE, position.X, position.Y, position.Z, 100, 0)

        If LocationCamera <> Nothing AndAlso LocationCamera.Exists Then

            LocationCamera.Delete()
        End If

        LocationCamera = World.CreateCamera(position, Vector3.Zero, 75)
        LocationCamera.AttachTo(DeleteTrain.SelectedItem.Locomotive, New Vector3(10, 0, 10))
        LocationCamera.PointAt(DeleteTrain.SelectedItem.Locomotive)

        World.RenderingCamera = LocationCamera

        Native.Function.Call(Native.Hash.LOCK_MINIMAP_POSITION, position.X, position.Y)
    End Sub

    Private Sub DeleteTrain_ItemChanged(sender As Object, e As ItemChangedEventArgs(Of RogersSierra)) Handles DeleteTrain.ItemChanged

        ShowTrain()
    End Sub

    Private Sub SelectLocation_Selected(sender As Object, e As SelectedEventArgs) Handles SelectLocation.Selected, SpawnTrain.Selected

        If SpawnMode Then

            Exit Sub
        Else

            SpawnMode = True
        End If

        ResetCamera()

        ShowLocation()
    End Sub

    Private Sub DeleteAllTrains_Activated(sender As Object, e As EventArgs) Handles DeleteAllTrains.Activated

        RogersSierraList.ForEach(Sub(x)
                                     x.Delete()
                                 End Sub)

        Close()
    End Sub

    Private Sub RandomTrains_CheckboxChanged(sender As Object, e As EventArgs) Handles RandomTrains.CheckboxChanged

        Native.Function.Call(Native.Hash.SET_RANDOM_TRAINS, RandomTrains.Checked)

        If Not RandomTrains.Checked Then

            Native.Function.Call(Native.Hash.DELETE_ALL_TRAINS)
        End If
    End Sub

    Public Overrides Sub Tick()

    End Sub
End Class
