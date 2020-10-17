Imports System.ComponentModel
Imports System.Drawing
Imports GTA
Imports GTA.Math
Imports LemonUI.Menus
Friend Class SpawnMenu
    Inherits NativeMenu

    Private SpawnMode As Boolean

    Private LocationCamera As Camera
    Private LocationBlip As Blip

    Private WithEvents SelectLocation As New NativeListItem(Of SpawnLocation)(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Location"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Location_Description"))
    Private WithEvents SpawnTrain As New NativeItem(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Spawn"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Spawn_Description"))
    Private WithEvents DeleteTrain As New NativeListItem(Of RogersSierra)(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Delete"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Delete_Description"))
    Private WithEvents DeleteAllTrains As New NativeItem(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_DeleteAll"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_DeleteAll_Description"))
    Private WithEvents WarpPlayer As New NativeCheckboxItem(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_WarpPlayer"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_WarpPlayer_Description"), True)

    Public Sub New()
        MyBase.New("", Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Subtitle"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Description"))

        Banner = New LemonUI.Elements.ScaledTexture(New PointF(0, 0), New SizeF(200, 200), "sierra", "sierra_livery")

        SelectLocation.Items = SpawnLocations
        DeleteTrain.Items = RogersSierraList

        Add(SelectLocation)
        Add(WarpPlayer)
        Add(SpawnTrain)
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
End Class
