﻿Imports System.Math
Imports System.Runtime.InteropServices
Imports OpenTK
Imports OpenTK.Platform.Windows
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.ComponentModel

Module modOpenGL
    Public PROJECTIONMATRIX As Matrix4
    Public VIEWMATRIX As Matrix4
    Public CAM_POSITION As Vector3
    Public PRESPECTIVE_NEAR As Single = 0.1F
    Public PRESPECTIVE_FAR As Single = 3000.0F

    Public defaultVao As Integer
    Public FieldOfView As Single = CSng(Math.PI) * (60 / 180.0F)

    Public Function GetMaxGLVersion() As Tuple(Of Integer, Integer)
        Dim tmpControl = New GLControl()
        tmpControl.MakeCurrent()

        Dim majorVersion = GL.GetInteger(GetPName.MajorVersion)
        Dim minorVersion = GL.GetInteger(GetPName.MinorVersion)

        Return New Tuple(Of Integer, Integer)(majorVersion, minorVersion)
    End Function

    Public Class GLCapabilities
        Public Shared maxTextureSize As Integer
        Public Shared maxArrayTextureLayers As Integer
        Public Shared maxUniformBufferBindings As Integer
        Public Shared maxColorAttachments As Integer
        Public Shared maxAniso As Single
        Public Shared maxVertexOutputComponents As Single

        Public Shared Sub init()
            maxTextureSize = GL.GetInteger(GetPName.MaxTextureSize)
            maxArrayTextureLayers = GL.GetInteger(GetPName.MaxArrayTextureLayers)
            maxUniformBufferBindings = GL.GetInteger(GetPName.MaxUniformBufferBindings)
            maxColorAttachments = GL.GetInteger(GetPName.MaxColorAttachments)
            maxAniso = GL.GetFloat(ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt)
            maxAniso = GL.GetFloat(ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt)
            maxVertexOutputComponents = GL.GetInteger(GetPName.MaxVertexOutputComponents)

            'LogThis(String.Format("Max Texture Size = {0}", maxTextureSize))
            'LogThis(String.Format("Max Array Texture Layers = {0}", maxArrayTextureLayers))
            'LogThis(String.Format("Max Uniform Buffer Bindings = {0}", maxUniformBufferBindings))
            'LogThis(String.Format("Max Color Attachments = {0}", maxColorAttachments))
            'LogThis(String.Format("Max Texture Max Anisotropy = {0}", maxAniso))
            'LogThis(String.Format("Max vertex output components = {0}", maxVertexOutputComponents))
        End Sub
    End Class

    <StructLayout(LayoutKind.Sequential)>
    Public Structure DrawElementsIndirectCommand
        Dim count As UInt32
        Dim instanceCount As UInt32
        Dim firstIndex As UInt32
        Dim baseVertex As UInt32
        Dim baseInstance As UInt32
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure ModelInstance
        Dim matrix As Matrix4
        Dim bmin As Vector3
        Dim lod_offset As UInt32
        Dim bmax As Vector3
        Dim lod_count As UInt32
        Dim batch_count As UInt32 ' hack!!!
        Dim reserverd1 As UInt32
        Dim reserverd2 As UInt32
        Dim reserverd3 As UInt32
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure ModelLoD
        Dim draw_offset As UInt32
        Dim draw_count As UInt32
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure CandidateDraw
        Dim model_id As UInt32
        Dim material_id As UInt32
        Dim count As UInt32
        Dim firstIndex As UInt32
        Dim baseVertex As UInt32
        Dim baseInstance As UInt32
        Dim lod_level As UInt32
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure GLMaterial
        Dim g_atlasIndexes As Vector4
        Dim g_atlasSizes As Vector4
        Dim g_colorTint As Vector4
        Dim dirtParams As Vector4
        Dim dirtColor As Vector4
        Dim g_tile0Tint As Vector4
        Dim g_tile1Tint As Vector4
        Dim g_tile2Tint As Vector4
        Dim g_tileUVScale As Vector4
        Dim g_detailInfluences As Vector4
        Dim map1Handle As UInt64
        Dim map2Handle As UInt64
        Dim map3Handle As UInt64
        Dim map4Handle As UInt64
        Dim map5Handle As UInt64
        Dim map6Handle As UInt64
        Dim shader_type As UInt32
        Dim texAddressMode As UInt32
        Dim alphaReference As Single
        Dim g_useNormalPackDXT1 As UInt32
        Dim alphaTestEnable As UInt32
        Dim g_enableAO As UInt32
        Dim double_sided As UInt32
        Dim pad0 As UInt32
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure TPerViewData
        Public view As Matrix4
        Public projection As Matrix4
        Public viewProj As Matrix4
        Public invViewProj As Matrix4
        Public cameraPos As Vector3
        Private pad As UInt32 'reserved
        Public resolution As Vector2
    End Structure
    Public PerViewData As New TPerViewData
    Public PerViewDataBuffer As New GLBuffer


    Public Sub Ortho_main()
        GL.Viewport(0, 0, frmMain.glControl_main.ClientSize.Width, frmMain.glControl_main.ClientSize.Height)
        PROJECTIONMATRIX = Matrix4.CreateOrthographicOffCenter(0.0F, frmMain.glControl_main.Width, -frmMain.glControl_main.Height, 0.0F, -30000.0F, 30000.0F)
        VIEWMATRIX = Matrix4.Identity
    End Sub



    Public Sub set_prespective_view()

        GL.Viewport(0, 0, frmMain.glControl_main.ClientSize.Width, frmMain.glControl_main.ClientSize.Height)

        PROJECTIONMATRIX = Matrix4.CreateOrthographicOffCenter(0.0F, frmMain.glControl_main.Width, -frmMain.glControl_main.Height, 0.0F, -300.0F, 300.0F)
        Dim sin_x, cos_x, cos_y, sin_y As Single
        Dim cam_x, cam_y, cam_z As Single

        sin_x = Sin(U_Cam_X_angle)
        cos_x = Cos(U_Cam_X_angle)
        cos_y = Cos(U_Cam_Y_angle)
        sin_y = Sin(U_Cam_Y_angle)
        cam_y = sin_y * view_radius
        cam_x = cos_y * sin_x * view_radius
        cam_z = cos_y * cos_x * view_radius

        Dim LOOK_Y = U_look_point_y
        CAM_POSITION.X = cam_x + U_look_point_x
        CAM_POSITION.Y = cam_y + LOOK_Y
        CAM_POSITION.Z = cam_z + U_look_point_z

        Dim target As New Vector3(U_look_point_x, LOOK_Y, U_look_point_z)

        PerViewData.projection = Matrix4.CreatePerspectiveFieldOfView(
                                   FieldOfView,
                                   frmMain.glControl_main.ClientSize.Width / CSng(frmMain.glControl_main.ClientSize.Height),
                                   PRESPECTIVE_NEAR, PRESPECTIVE_FAR)
#If False Then ' reverse depth
        PerViewData.projection.M33 *= -1
        PerViewData.projection.M33 -= 1
        PerViewData.projection.M43 *= -1
#End If
        PerViewData.cameraPos = CAM_POSITION
        PerViewData.view = Matrix4.LookAt(CAM_POSITION, target, Vector3.UnitY)
        PerViewData.viewProj = PerViewData.view * PerViewData.projection
        PerViewData.invViewProj = Matrix4.Invert(PerViewData.viewProj)
        PerViewData.resolution.X = frmMain.glControl_main.ClientSize.Width
        PerViewData.resolution.Y = frmMain.glControl_main.ClientSize.Height
        GL.NamedBufferSubData(PerViewDataBuffer.buffer_id, IntPtr.Zero, Marshal.SizeOf(PerViewData), PerViewData)
    End Sub

    Private Function pack_10(x As Single) As UInt32
        Dim qx As Int32 = MathHelper.Clamp(CType(x * 511.0F, Int32), -512, 511)
        If qx < 0 Then
            Return (1 << 9) Or ((CType(-1 - qx, UInt32) Xor ((1 << 9) - 1)))
        Else
            Return qx
        End If
    End Function

    Public Function pack_2_10_10_10(unpacked As Vector3, Optional w As UInt32 = 0) As UInt32
        unpacked.Normalize()

        Dim packed_x As UInt32 = pack_10(unpacked.X)
        Dim packed_y As UInt32 = pack_10(unpacked.Y)
        Dim packed_z As UInt32 = pack_10(unpacked.Z)
        Return packed_x Or (packed_y << 10) Or (packed_z << 20) Or (w << 30)
    End Function

    Private debugOutputCallbackProc As DebugProc
    Private Sub DebugOutputCallback(source As DebugSource,
                                   type As DebugType,
                                   id As UInteger,
                                   severity As DebugSeverity,
                                   length As Integer,
                                   messagePtr As IntPtr,
                                   userParam As IntPtr)
        If source = DebugSource.DebugSourceApplication Then Return
        If id = 131185 Then Return
        If id = 1281 Then Return
        'If id = 1282 Then Return

        Dim message = Marshal.PtrToStringAnsi(messagePtr)

        Application.DoEvents()
        'LogThis(String.Format("OpenGL error #{0}: {1}", id, message))
    End Sub

    Private stack_pos As Integer = 0

    <Conditional("DEBUG")>
    Public Sub GL_PUSH_GROUP(name As String)
        stack_pos += 1
        GL.PushDebugGroup(DebugSourceExternal.DebugSourceApplication, stack_pos + 10, -1, name)
    End Sub

    <Conditional("DEBUG")>
    Public Sub GL_POP_GROUP()
        stack_pos -= 1
        GL.PopDebugGroup()
        If stack_pos < 0 Or stack_pos > 5 Then Stop
    End Sub

    Public Sub SetupDebugOutputCallback()
        GL.Enable(EnableCap.DebugOutput)
        GL.Enable(EnableCap.DebugOutputSynchronous)
        debugOutputCallbackProc = New DebugProc(AddressOf DebugOutputCallback)
        GL.DebugMessageCallback(debugOutputCallbackProc, IntPtr.Zero)
        GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DebugTypeError, DebugSeverityControl.DontCare, 0, 0, True)
    End Sub

    Public Function get_GL_error_string(ByVal e As ErrorCode) As String
        Return [Enum].GetName(GetType(ErrorCode), e)
    End Function
End Module
