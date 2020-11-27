Imports FusionLibrary.Enums
Imports FusionLibrary.Extensions
Imports GTA
Imports KlangRageAudioLibrary

Partial Public Class RogersSierra

    Private sTrainStart As AudioPlayer
    Private sTrainMove1 As AudioPlayer
    Private sTrainMove2 As AudioPlayer
    Private sWhistleSound As AudioPlayer
    Private sPistonSteamVentSound As AudioPlayer
    Private sBellSound As AudioPlayer
    Private sTrainMoving As New List(Of AudioPlayer)
    Private sTrainMovingIndex As Integer = -1
    Private sPrestoLogExpl As AudioPlayer

    Private Sub LoadSounds()

        AudioEngine.DefaultSourceEntity = Locomotive

        sTrainStart = AudioEngine.Create("trainstart" & _ID, My.Resources.TrainStart, Presets.Exterior)

        sTrainMove1 = AudioEngine.Create("trainmove1" & _ID, My.Resources.TrainMove1, Presets.Exterior)
        sTrainMove1.MinimumDistance = 10

        sTrainMove2 = AudioEngine.Create("trainmove2" & _ID, My.Resources.TrainMove2, Presets.Exterior)
        sTrainMove2.MinimumDistance = 10

        sWhistleSound = AudioEngine.Create("whistle" & _ID, My.Resources.Whistle, Presets.Exterior)
        sWhistleSound.MinimumDistance = 10

        sPistonSteamVentSound = AudioEngine.Create("pistonsteamvent" & _ID, My.Resources.PistonSteamVent, Presets.Exterior)

        sBellSound = AudioEngine.Create("bell" & _ID, My.Resources.Bell, Presets.Exterior)

        sPrestoLogExpl = AudioEngine.Create("prestologexpl" & _ID, My.Resources.funnelExplosion, Presets.Exterior)
        sPrestoLogExpl.SourceBone = TrainBones.sFunnel

        sTrainMoving.Clear()

        With sTrainMoving
            .Add(AudioEngine.Create("trainmoving1" & _ID, My.Resources.ambient_moving1, Presets.ExteriorLoop))
            .Last.StartFadeIn = True
            .Last.FadeInMultiplier = 0.7
            .Last.StopFadeOut = True
            .Last.FadeOutMultiplier = 0.7
            .Last.MinimumDistance = 0.5

            .Add(AudioEngine.Create("trainmoving2" & _ID, My.Resources.ambient_moving2, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving3" & _ID, My.Resources.ambient_moving3, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving5" & _ID, My.Resources.ambient_moving5, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving6" & _ID, My.Resources.ambient_moving6, Presets.ExteriorLoop))

            .Add(AudioEngine.Create("trainmoving7" & _ID, My.Resources.ambient_moving7, Presets.ExteriorLoop))
        End With
    End Sub

    Private Sub SoundsTick()

        If Locomotive.Speed > 0 Then

            Dim newIndex As Integer

            Dim baseVal As Single = 8

            Select Case Locomotive.GetMPHSpeed
                Case Is <= baseVal
                    newIndex = 0
                Case Is <= baseVal * 2
                    newIndex = 1
                Case Is <= baseVal * 3
                    newIndex = 2
                Case Is <= baseVal * 4
                    newIndex = 3
                Case Is <= baseVal * 5
                    newIndex = 4
                Case Else
                    newIndex = 5
            End Select

            If newIndex <> sTrainMovingIndex Then

                If sTrainMovingIndex > -1 AndAlso sTrainMoving(sTrainMovingIndex).IsAnyInstancePlaying Then

                    sTrainMoving(sTrainMovingIndex).Stop()
                End If

                sTrainMovingIndex = newIndex
                sTrainMoving(sTrainMovingIndex).Play()
            End If
        Else

            If sTrainMovingIndex > -1 AndAlso sTrainMoving(sTrainMovingIndex).IsAnyInstancePlaying Then

                sTrainMoving(sTrainMovingIndex).Stop()
                sTrainMovingIndex = -1
            End If
        End If

        If PistonSteam = False AndAlso PistonSteamVent = False Then

            If aPistons.Position(Coordinate.Y) > PistonOldPos AndAlso PistonGoingForward = False Then

                If Val(Game.GameTime.ToString.Last) <= 4 Then

                    sTrainMove1.Play()
                Else

                    sTrainMove2.Play()
                End If

                PistonGoingForward = True
            End If

            If aPistons.Position(Coordinate.Y) < PistonOldPos AndAlso PistonGoingForward Then

                If Val(Game.GameTime.ToString.Last) <= 4 Then

                    sTrainMove1.Play()
                Else

                    sTrainMove2.Play()
                End If

                PistonGoingForward = False
            End If

            PistonOldPos = aPistons.Position(Coordinate.Y)
        End If
    End Sub
End Class
