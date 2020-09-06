Imports System.Windows.Forms
Imports LemonUI

Friend Class MenuManager

    Public ObjectPool As New ObjectPool

    Public SpawnMenu As New SpawnMenu

    Public Sub New()

        ObjectPool.Add(SpawnMenu)
    End Sub

    Public Sub KeyDown(Key As KeyEventArgs)

        Select Case Key.KeyData
            Case Keys.F9
                SpawnMenu.Visible = True
        End Select
    End Sub

    Public Sub Process()

        ObjectPool.Process()
    End Sub
End Class
