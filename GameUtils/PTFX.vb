Friend Class PTFX

    Private assetName As String
    Private ptfxName As String

    Private ptfxHandle As Integer = 0

    Public ReadOnly Property Handle As Integer
        Get
            Return ptfxHandle
        End Get
    End Property

    Sub New(ptfxHandle As Integer)

        Me.ptfxHandle = ptfxHandle
    End Sub

    Public Sub New(assetName As String, ptfxName As String, Optional request As Boolean = True)
        Me.assetName = assetName
        Me.ptfxName = ptfxName

        If request Then

            RequestAsset()
        End If
    End Sub

    Public Sub New(ptfx As String(), Optional request As Boolean = True)
        assetName = ptfx(0)
        ptfxName = ptfx(1)

        If request Then

            RequestAsset()
        End If
    End Sub

    Private Sub RequestAsset()

        GTA.Native.Function.Call(GTA.Native.Hash.REQUEST_NAMED_PTFX_ASSET, assetName)

        Do While isAssetLoaded() = False
            GTA.Script.Yield()
        Loop
    End Sub

    Public Function isAssetLoaded() As Boolean

        Return GTA.Native.Function.Call(Of Boolean)(GTA.Native.Hash.HAS_NAMED_PTFX_ASSET_LOADED, assetName)
    End Function

#Region "Looped"
    Public Function CreateLooped(pos As GTA.Math.Vector3, Optional rot As GTA.Math.Vector3 = Nothing, Optional scale As Single = 1.0F) As Integer

        RequestAsset()

        GTA.Native.Function.Call(GTA.Native.Hash.USE_PARTICLE_FX_ASSET, assetName)

        If IsNothing(rot) Then
            ptfxHandle = GTA.Native.Function.Call(Of Integer)(GTA.Native.Hash.START_PARTICLE_FX_LOOPED_AT_COORD, ptfxName, pos.X, pos.Y, pos.Z, 0.0F, 0.0F, 0.0F, scale, False, False, False, False)
        Else
            ptfxHandle = GTA.Native.Function.Call(Of Integer)(GTA.Native.Hash.START_PARTICLE_FX_LOOPED_AT_COORD, ptfxName, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, scale, False, False, False, False)
        End If

        Return ptfxHandle
    End Function

    Public Function CreateLooped(entity As GTA.Entity, offset As GTA.Math.Vector3, Optional rot As GTA.Math.Vector3 = Nothing, Optional scale As Single = 1.0F) As Integer

        RequestAsset()

        GTA.Native.Function.Call(GTA.Native.Hash.USE_PARTICLE_FX_ASSET, assetName)

        If IsNothing(rot) Then
            ptfxHandle = GTA.Native.Function.Call(Of Integer)(GTA.Native.Hash.START_PARTICLE_FX_LOOPED_ON_ENTITY, ptfxName, entity, offset.X, offset.Y, offset.Z, 0.0F, 0.0F, 0.0F, scale, False, False, False)
        Else
            ptfxHandle = GTA.Native.Function.Call(Of Integer)(GTA.Native.Hash.START_PARTICLE_FX_LOOPED_ON_ENTITY, ptfxName, entity, offset.X, offset.Y, offset.Z, rot.X, rot.Y, rot.Z, scale, False, False, False)
        End If

        Return ptfxHandle
    End Function

    Public Function CreateLoopedOnEntityBone(entity As GTA.Entity, boneName As String, offset As GTA.Math.Vector3, Optional rot As GTA.Math.Vector3 = Nothing, Optional scale As Single = 1.0F) As Integer

        RequestAsset()

        GTA.Native.Function.Call(GTA.Native.Hash.USE_PARTICLE_FX_ASSET, assetName)

        If IsNothing(rot) Then
            ptfxHandle = GTA.Native.Function.Call(Of Integer)(GTA.Native.Hash.START_PARTICLE_FX_LOOPED_ON_ENTITY_BONE, ptfxName, entity.Handle, offset.X, offset.Y, offset.Z, 0.0F, 0.0F, 0.0F, entity.Bones.Item(boneName).Index, scale, False, False, False)
        Else
            ptfxHandle = GTA.Native.Function.Call(Of Integer)(GTA.Native.Hash.START_PARTICLE_FX_LOOPED_ON_ENTITY_BONE, ptfxName, entity.Handle, offset.X, offset.Y, offset.Z, rot.X, rot.Y, rot.Z, entity.Bones.Item(boneName).Index, scale, False, False, False)
        End If

        Return ptfxHandle
    End Function

    Public Sub setLoopedEvolution(prop As String, value As Single)
        GTA.Native.Function.Call(GTA.Native.Hash.SET_PARTICLE_FX_LOOPED_EVOLUTION, ptfxHandle, prop, value, 0)
    End Sub

    Public Sub setLoopedScale(scale As Single)

        GTA.Native.Function.Call(GTA.Native.Hash.SET_PARTICLE_FX_LOOPED_SCALE, ptfxHandle, scale)
    End Sub

    Public Sub ColorLooped(r As Single, g As Single, b As Single)
        GTA.Native.Function.Call(GTA.Native.Hash.SET_PARTICLE_FX_LOOPED_COLOUR, ptfxHandle, r, g, b, 0)
    End Sub

    Public Sub AlphaLooped(pAlpha As Single)

        GTA.Native.Function.Call(GTA.Native.Hash.SET_PARTICLE_FX_LOOPED_ALPHA, ptfxHandle, pAlpha)
    End Sub

    Public Sub OffsetsLooped(oPosition As GTA.Math.Vector3, oRotation As GTA.Math.Vector3)

        GTA.Native.Function.Call(GTA.Native.Hash.SET_PARTICLE_FX_LOOPED_OFFSETS, ptfxHandle, oPosition.X, oPosition.Y, oPosition.Z, oRotation.X, oRotation.Y, oRotation.Z)
    End Sub

    Public Sub Delete()
        GTA.Native.Function.Call(GTA.Native.Hash.REMOVE_PARTICLE_FX, ptfxHandle, 1)
        ptfxHandle = 0
    End Sub

    Public Sub [Stop]()
        GTA.Native.Function.Call(GTA.Native.Hash.STOP_PARTICLE_FX_LOOPED, ptfxHandle, 0)
        ptfxHandle = 0
    End Sub

#End Region

#Region "Non looped"
    Public Function Create(pos As GTA.Math.Vector3, Optional rot As GTA.Math.Vector3 = Nothing, Optional scale As Single = 1.0F) As Boolean

        RequestAsset()

        GTA.Native.Function.Call(GTA.Native.Hash.USE_PARTICLE_FX_ASSET, assetName)

        Dim ret As Boolean

        If IsNothing(rot) Then
            ret = GTA.Native.Function.Call(Of Boolean)(GTA.Native.Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, ptfxName, pos.X, pos.Y, pos.Z, 0.0F, 0.0F, 0.0F, scale, False, False, False)
        Else
            ret = GTA.Native.Function.Call(Of Boolean)(GTA.Native.Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, ptfxName, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, scale, False, False, False)
        End If

        Return ret
    End Function

    Public Function Create(entity As GTA.Entity, offset As GTA.Math.Vector3, Optional rot As GTA.Math.Vector3 = Nothing, Optional scale As Single = 1.0F) As Boolean

        RequestAsset()

        GTA.Native.Function.Call(GTA.Native.Hash.USE_PARTICLE_FX_ASSET, assetName)

        Dim ret As Boolean

        If IsNothing(rot) Then
            ret = GTA.Native.Function.Call(Of Boolean)(GTA.Native.Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, ptfxName, entity, offset.X, offset.Y, offset.Z, 0.0F, 0.0F, 0.0F, scale, False, False, False)
        Else
            ret = GTA.Native.Function.Call(Of Boolean)(GTA.Native.Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, ptfxName, entity, offset.X, offset.Y, offset.Z, rot.X, rot.Y, rot.Z, scale, False, False, False)
        End If

        Return ret
    End Function

    Public Function CreateOnEntityBone(entity As GTA.Entity, boneName As String, offset As GTA.Math.Vector3, Optional rot As GTA.Math.Vector3 = Nothing, Optional scale As Single = 1.0F) As Boolean

        RequestAsset()

        GTA.Native.Function.Call(GTA.Native.Hash.USE_PARTICLE_FX_ASSET, assetName)

        offset = entity.Bones(boneName).GetRelativeOffsetPosition(offset)

        Dim ret As Boolean

        If IsNothing(rot) Then
            ret = GTA.Native.Function.Call(Of Boolean)(GTA.Native.Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, ptfxName, entity.Handle, offset.X, offset.Y, offset.Z, 0.0F, 0.0F, 0.0F, scale, False, False, False)
        Else
            ret = GTA.Native.Function.Call(Of Boolean)(GTA.Native.Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, ptfxName, entity.Handle, offset.X, offset.Y, offset.Z, rot.X, rot.Y, rot.Z, scale, False, False, False)
        End If

        Return ret
    End Function

    Public Sub Color(r As Single, g As Single, b As Single)
        GTA.Native.Function.Call(GTA.Native.Hash.SET_PARTICLE_FX_NON_LOOPED_COLOUR, r, g, b)
    End Sub
#End Region

End Class
