Imports System.Drawing
Imports GTA

MustInherit Class SierraMenu
    Inherits FusionLibrary.CustomNativeMenu

    Public Sub New(name As String)
        MyBase.New("")

        InternalName = name
        Subtitle = GetMenuTitle()
        Banner = New LemonUI.Elements.ScaledTexture(New PointF(0, 0), New SizeF(200, 200), "sierra_gui", "sierra_menu_logo")
    End Sub

    Public Overrides Function GetMenuTitle() As String

        Return Game.GetLocalizedString($"RogersSierra_Menu_{InternalName}_Title")
    End Function

    Public Overrides Function GetMenuDescription() As String

        Return Game.GetLocalizedString($"RogersSierra_Menu_{InternalName}_Description")
    End Function

    Public Overrides Function GetItemTitle(itemName As String) As String

        Return Game.GetLocalizedString($"RogersSierra_Menu_{InternalName}_Item_{itemName}_Title")
    End Function

    Public Overrides Function GetItemDescription(itemName As String) As String

        Return Game.GetLocalizedString($"RogersSierra_Menu_{InternalName}_Item_{itemName}_Description")
    End Function

    Public Overrides Function GetItemValueTitle(itemName As String, valueName As String) As String

        Return Game.GetLocalizedString($"RogersSierra_Menu_{InternalName}_Item_{itemName}_Value_{valueName}_Title")
    End Function

    Public Overrides Function GetItemValueDescription(itemName As String, valueName As String) As String

        Return Game.GetLocalizedString($"RogersSierra_Menu_{InternalName}_Item_{itemName}_Value_{valueName}_Description")
    End Function
End Class
