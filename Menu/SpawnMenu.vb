Imports System.ComponentModel
Imports GTA
Imports GTA.Math
Imports LemonUI.Menus
Public Class SpawnMenu
    Inherits NativeMenu

    Private LocationCamera As Camera
    Private LocationBlip As Blip

    Private WithEvents SelectLocation As New NativeListItem(Of Integer)(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Location"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Location_Description"), 1, 2, 3)
    Private WithEvents SpawnTrain As New NativeItem(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Spawn"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Spawn_Description"))
    Private WithEvents DeleteTrain As New NativeItem(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Delete"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Delete_Description"))

    Public Sub New()
        MyBase.New(Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Subtitle"), Game.GetLocalizedString("RogersSierra_Menu_SpawnMenu_Description"))

        Add(SelectLocation)
        Add(SpawnTrain)
        Add(DeleteTrain)
    End Sub

    Private Sub SpawnMenu_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        If LocationCamera <> Nothing AndAlso LocationCamera.Exists Then

            LocationCamera.Delete()
            World.RenderingCamera = Nothing
        End If

        If IsNothing(LocationBlip) = False AndAlso LocationBlip.Exists() Then

            LocationBlip.Delete()
        End If
    End Sub

    Private Sub SelectLocation_ItemChanged(sender As Object, e As ItemChangedEventArgs(Of Integer)) Handles SelectLocation.ItemChanged

        ShowLocation(e.Object)
    End Sub

    Public Sub ShowLocation(index As Integer)

        Dim position = Game.Player.Character.Position 'SpawnLocations(index)

        Native.Function.Call(Native.Hash.LOAD_SCENE, position.X, position.Y, position.Z)

        LocationCamera = World.CreateCamera(position.GetSingleOffset(Coordinate.Z, 50), Vector3.Zero, 75)
        LocationCamera.PointAt(position)

        World.RenderingCamera = LocationCamera

        If IsNothing(LocationBlip) = False AndAlso LocationBlip.Exists() Then

            LocationBlip.Delete()
        End If

        LocationBlip = World.CreateBlip(position)
        LocationBlip.Sprite = 120
    End Sub

    Private Sub SpawnTrain_Activated(sender As Object, e As EventArgs) Handles SpawnTrain.Activated

        CreateRogersSierra(Game.Player.Character.Position, True)

        DeleteTrain.Enabled = Not IsNothing(RogersSierra)
    End Sub

    Private Sub DeleteTrain_Activated(sender As Object, e As EventArgs) Handles DeleteTrain.Activated

        RogersSierra.Delete()

        DeleteTrain.Enabled = Not IsNothing(RogersSierra)
    End Sub

    Private Sub DeleteTrain_Selected(sender As Object, e As SelectedEventArgs) Handles DeleteTrain.Selected

        DeleteTrain.Enabled = Not IsNothing(RogersSierra)
    End Sub

    Private Sub SpawnMenu_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        DeleteTrain.Enabled = Not IsNothing(RogersSierra)
    End Sub
End Class
