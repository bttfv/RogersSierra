Imports System.IO
Imports GTA
Imports GTA.Math
Imports IrrKlang
Imports RogersSierra

Friend Class AudioPlayer

    Private Shared _soundEngine As New ISoundEngine()
    Private Shared _allSounds As New List(Of AudioPlayer)

    Public Shared Sub ProcessAllSounds()
        Dim listenerPos As Vector3 = If(IsCameraValid(World.RenderingCamera), World.RenderingCamera.Position, GameplayCamera.Position)
        Dim listenerDir As Vector3 = If(IsCameraValid(World.RenderingCamera), World.RenderingCamera.Direction, GameplayCamera.Direction)

        _soundEngine.SetListenerPosition(listenerPos.ToVector3D, listenerDir.ToVector3D)

        For Each player In _allSounds

            player.Process()
        Next
    End Sub

    Public Shared Sub MuteAll(mute As Boolean)
        If mute AndAlso _soundEngine.SoundVolume <> 0 Then
            _soundEngine.SoundVolume = 0
        End If
        If mute = False AndAlso _soundEngine.SoundVolume = 0 Then
            _soundEngine.SoundVolume = 1
        End If
    End Sub

    Public Shared Sub PauseAll(pause As Boolean)
        For Each player In _allSounds
            If player.Paused <> pause Then
                player.Paused = pause
            End If
        Next
    End Sub

    Public Shared Sub StopAndDispose()
        _soundEngine.StopAllSounds()
        For Each player In _allSounds
            player?.Dispose(False)
        Next
        _allSounds.Clear()
        _soundEngine.RemoveAllSoundSources()
    End Sub

    Private _sound As ISound
    Private _soundSource As ISoundSource
    Private _entityowner As Entity
    Private _mindistance As Single
    Private _looped As Boolean
    Private _volume As Single
    Private _isplaying As Boolean = False

    Sub New(fileRes As Stream, soundName As String, Optional Looped As Boolean = False, Optional Volume As Single = 1)

        _looped = Looped
        _volume = Volume
        _soundSource = _soundEngine.AddSoundSourceFromIOStream(fileRes, soundName)

        _allSounds.Add(Me)
    End Sub

    Sub New(EntityOwner As Entity, fileRes As Stream, soundName As String, Optional Looped As Boolean = False, Optional Volume As Single = 1, Optional minDistance As Single = 50.0)

        _looped = Looped
        _volume = Volume
        _mindistance = minDistance
        _entityowner = EntityOwner
        _soundSource = _soundEngine.AddSoundSourceFromIOStream(fileRes, soundName)

        _allSounds.Add(Me)
    End Sub

    Sub New(filePath As String, Optional Looped As Boolean = False, Optional Volume As Single = 1)

        _looped = Looped
        _volume = Volume
        _soundSource = _soundEngine.AddSoundSourceFromFile(filePath)

        _allSounds.Add(Me)
    End Sub

    Sub New(EntityOwner As Entity, filePath As String, Optional Looped As Boolean = False, Optional Volume As Single = 1, Optional minDistance As Single = 50.0)

        _looped = Looped
        _volume = Volume
        _mindistance = minDistance
        _entityowner = EntityOwner
        _soundSource = _soundEngine.AddSoundSourceFromFile(filePath)

        _allSounds.Add(Me)
    End Sub

    Private Sub Process()

        If IsNothing(_sound) Then

            Exit Sub
        End If

        If isPlaying Then

            _sound.PlaybackSpeed = Game.TimeScale
        End If

        If IsNothing(_sound) = False AndAlso Finished = False AndAlso IsNothing(_entityowner) = False Then

            If getCurrentVehicle() = _entityowner Then

                _sound.Position = GameplayCamera.Position.ToVector3D

                Exit Sub
            End If

            _sound.Position = _entityowner.Position.ToVector3D
        End If
    End Sub

    Public Sub Play()

        If IsNothing(_entityowner) Then

            _sound = _soundEngine.Play2D(_soundSource, _looped, False, False)
        Else

            _sound = _soundEngine.Play3D(_soundSource, _entityowner.Position.X, _entityowner.Position.Y, _entityowner.Position.Z, _looped, False, False)
            _sound.MinDistance = _mindistance
        End If

        _isplaying = True
    End Sub

    Public Sub [Stop]()
        
        _isplaying = False
        _sound?.Stop()
        _sound?.Dispose()
    End Sub

    Public Sub Dispose(Optional remove As Boolean = True)

        If remove Then
            _allSounds.Remove(Me)
        End If

        _sound?.Dispose()
        _sound = Nothing
    End Sub

    Public ReadOnly Property isPlaying As Boolean
        Get
            Return _isplaying
        End Get
    End Property

    Public ReadOnly Property Finished As Boolean
        Get
            Return _sound.Finished
        End Get
    End Property

    Public Property Volume As Single
        Get
            Return _sound.Volume
        End Get
        Set(value As Single)
            _volume = value
            _sound.Volume = value
        End Set
    End Property

    Public Property Looped As Boolean
        Get
            Return _sound.Looped
        End Get
        Set(value As Boolean)
            _looped = value
            _sound.Looped = value
        End Set
    End Property

    Public Property PlaybackSpeed As Single
        Get
            Return _sound.PlaybackSpeed
        End Get
        Set(value As Single)
            _sound.PlaybackSpeed = value
        End Set
    End Property

    Public Property PlayPosition As UInteger
        Get
            Return _sound.PlayPosition
        End Get
        Set(value As UInteger)
            _sound.PlayPosition = value
        End Set
    End Property

    Public Property Paused As Boolean
        Get
            Return If(IsNothing(_sound), False, _sound.Paused)
        End Get
        Set(value As Boolean)
            If IsNothing(_sound) = False Then
                _sound.Paused = value
            End If
        End Set
    End Property
End Class