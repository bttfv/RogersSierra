Imports System.ComponentModel
Imports FusionLibrary
Imports GTA
Imports GTA.Math
Imports LemonUI.Menus

Friend Class SpawnMenu
    Inherits SierraMenu

    Private SpawnMode As Boolean

    Private LocationCamera As Camera
    Private LocationBlip As Blip

    Private SpawnLocations As New SpawnLocationHandler

    Private WithEvents SelectLocation As NativeListItem(Of SpawnLocation)
    Private WithEvents WarpPlayer As NativeCheckboxItem
    Private WithEvents SpawnTrain As NativeItem
    Private WithEvents DeleteTrain As NativeListItem(Of RogersSierra)
    Private WithEvents DeleteAllTrains As NativeItem
    Private WithEvents RandomTrains As NativeCheckboxItem

    Public Sub New()
        MyBase.New("Spawn")

        SpawnLocations.Add(New Vector3(2611, 1681, 27), New Vector3(2601.0, 1700.2, 29.9), New Vector3(0.8, -0.6, -0.1), False)
        SpawnLocations.Add(New Vector3(2462, -289, 93), New Vector3(2455.4, -276.4, 96.2), New Vector3(0.4, -0.9, 0.1))
        SpawnLocations.Add(New Vector3(2014, 2493, 58), New Vector3(2028.6, 2482.9, 67.6), New Vector3(-1.0, 0.2, 0.0))
        SpawnLocations.Add(New Vector3(2994, 3990, 57), New Vector3(3004.8, 3983.3, 60.0), New Vector3(-0.5, 0.9, 0.0))
        SpawnLocations.Add(New Vector3(1807, 3510, 39), New Vector3(1828.1, 3535.5, 45.2), New Vector3(-0.5, -0.9, 0.1))
        SpawnLocations.Add(New Vector3(-478, 5253, 88), New Vector3(-476.1, 5215.4, 98.9), New Vector3(-0.5, 0.8, -0.2))
        SpawnLocations.Add(New Vector3(749, 6433, 30), New Vector3(765.6, 6449.3, 34.3), New Vector3(-0.9, -0.4, 0.2))
        SpawnLocations.Add(New Vector3(2486, 5743, 64), New Vector3(2510.5, 5716.2, 68.4), New Vector3(-1.0, 0.3, 0.1))

        SelectLocation = NewListItem("SelectLocation", SpawnLocations.Locations.ToArray)
        WarpPlayer = NewCheckboxItem("WarpPlayer", True)
        SpawnTrain = NewItem("SpawnTrain")
        RandomTrains = NewCheckboxItem("RandomTrains", False)
        DeleteTrain = NewListItem("Delete", RogersSierraList.ToArray)
        DeleteAllTrains = NewItem("DeleteAll")
    End Sub

    Public Overrides Sub Menu_Closing(sender As Object, e As CancelEventArgs)

        ResetCamera()
    End Sub

    Public Sub ResetCamera()

        SpawnLocation.ResetCamera()

        If IsNothing(LocationBlip) = False AndAlso LocationBlip.Exists() Then

            LocationBlip.Delete()
        End If
    End Sub

    Private Sub SelectLocation_ItemChanged(sender As Object, e As ItemChangedEventArgs(Of SpawnLocation)) Handles SelectLocation.ItemChanged

        ShowLocation()
    End Sub

    Public Sub ShowLocation()

        SelectLocation.SelectedItem.ShowLocation()

        If IsNothing(LocationBlip) = False AndAlso LocationBlip.Exists() Then

            LocationBlip.Delete()
        End If

        LocationBlip = World.CreateBlip(SelectLocation.SelectedItem.Position)
        LocationBlip.Sprite = 120

        SelectLocation.Description = SelectLocation.SelectedItem.Name
        Recalculate()
    End Sub

    Private Sub SpawnTrain_Activated(sender As Object, e As EventArgs) Handles SpawnTrain.Activated, SelectLocation.Activated

        CreateRogersSierra(SelectLocation.SelectedItem.Position, WarpPlayer.Checked, SelectLocation.SelectedItem.Direction)

        Close()
    End Sub

    Public Overrides Sub Menu_Shown(sender As Object, e As EventArgs)

        ShowLocation()

        DeleteTrain.Enabled = RogersSierraList.Count > 0

        DeleteTrain.Items = RogersSierraList

        RandomTrains.Checked = Utils.RandomTrains
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

        If Not Game.IsControlJustPressed(Control.PhoneSelect) Then

            Exit Sub
        End If

        Utils.RandomTrains = RandomTrains.Checked
    End Sub

    Public Overrides Sub Tick()

    End Sub

    Public Overrides Sub Menu_OnItemValueChanged(sender As NativeSliderItem, e As EventArgs)

    End Sub

    Public Overrides Sub Menu_OnItemCheckboxChanged(sender As NativeCheckboxItem, e As EventArgs, Checked As Boolean)

    End Sub

    Public Overrides Sub Menu_OnItemSelected(sender As NativeItem, e As SelectedEventArgs)

    End Sub

    Public Overrides Sub Menu_OnItemActivated(sender As NativeItem, e As EventArgs)

    End Sub
End Class
