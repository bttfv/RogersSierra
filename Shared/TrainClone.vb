Imports FusionLibrary
Imports GTA
Imports GTA.Math

Friend Class TrainClone

    Public ReadOnly ID As Integer
    Public Position As Vector3
    Public ReadOnly Direction As Boolean
    Public ReadOnly SpawnTime As Date
    Public CurrentTime As Date

    Public Sub New(id As Integer, direction As Boolean)

        Me.ID = id
        Me.Direction = direction
        SpawnTime = FusionUtils.CurrentTime
    End Sub

    Public Sub Update(position As Vector3)

        Me.Position = position
        CurrentTime = FusionUtils.CurrentTime

        UI.Screen.ShowSubtitle($"{ID} {SpawnTime} {CurrentTime} {position}")
    End Sub
End Class