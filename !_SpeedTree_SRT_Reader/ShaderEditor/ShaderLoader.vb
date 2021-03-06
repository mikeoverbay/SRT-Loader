﻿Imports System.IO
Imports OpenTK.Graphics.OpenGL

Module ShaderLoader
    Private incPaths() As String = {"/"}
    Private incPathLengths() As Integer = {1}

    Public SHADER_PATHS() As String
#Region "shader_storage"

    Private Function get_shader(ByRef name As String) As String
        For Each n In SHADER_PATHS
            If Path.GetFileName(n).ToLower = name.ToLower Then
                Return n
            End If
        Next
        Return ""
    End Function

    Public Class Shader
        Private is_used As Boolean
        Private loaded As Boolean
        Public program As Integer
        Private uniforms As Dictionary(Of String, Integer)

        Public name As String
        Public fragment As String
        Public vertex As String
        Public geo As String
        Public compute As String

        Default ReadOnly Property Item(uniformName As String) As Integer
            Get
#If DEBUG Then
                If Not loaded Then
                    'Stop ' For debugging
                End If
                If Not is_used Then
                    'Stop ' For debugging
                End If
#End If
                If Not uniforms.ContainsKey(uniformName) Then
                    ' Some glsl compilers can optimize some uniforms if they are not used internally in the shader
                    ' Stop ' For debugging
                    Return -1
                End If
                Return uniforms(uniformName)
            End Get
        End Property

        Sub Use()
#If DEBUG Then
            If Not loaded Then
                'Stop ' For debugging
            End If
            If is_used Then
                'Stop ' For debugging
            End If
            is_used = True
#End If
            GL.UseProgram(program)
        End Sub

        Sub StopUse()
#If DEBUG Then
            If Not loaded Then
                'Stop ' For debugging
            End If
            If Not is_used Then
                'Stop ' For debugging
            End If
            is_used = False
#End If
            GL.UseProgram(0)
        End Sub

        Sub UpdateShader()
            uniforms = New Dictionary(Of String, Integer)
            loaded = False
            is_used = False

            If program > 0 Then
                GL.UseProgram(0)
                GL.DeleteProgram(program)
                Dim status_code As Integer
                GL.GetShader(program, ShaderParameter.DeleteStatus, status_code)
                Debug.Assert(status_code = 0)
                GL.Finish()
            End If

            program = assemble_shader(vertex, geo, compute, fragment, name)

            If program = 0 Then
                ' Stop ' For debugging
                Return
            End If

            Dim numActiveUniforms As Integer
            GL.GetProgram(program, GetProgramParameterName.ActiveUniforms, numActiveUniforms)

            For i = 0 To numActiveUniforms - 1
                Dim uniformName As String = GL.GetActiveUniformName(program, i)
                If uniformName.StartsWith("gl_") Then
                    ' Skip internal glsl uniforms
                    Continue For
                End If
                Me.uniforms(uniformName) = GL.GetUniformLocation(program, uniformName)
            Next
            loaded = True
        End Sub

        Sub New(name As String)
            Me.name = name
            is_used = False
            loaded = False
            Dim failed_1, failed_2, failed_3, failed_4 As Boolean
            vertex = get_shader(String.Format("{0}.vert", name))
            If Not File.Exists(vertex) Then
                failed_1 = True
                vertex = Nothing
            End If

            geo = get_shader(String.Format("{0}.geom", name))
            If Not File.Exists(geo) Then
                failed_2 = True
                geo = Nothing
            End If

            compute = get_shader(String.Format("{0}.comp", name))
            If Not File.Exists(compute) Then
                failed_3 = True
                compute = Nothing
            End If

            fragment = get_shader(String.Format("{0}.frag", name))
            If Not File.Exists(fragment) Then
                failed_4 = True
                fragment = Nothing
            End If

            'check if our shader was found
            If failed_1 And failed_2 And failed_3 And failed_4 Then
                MsgBox(name + " was not found.", MsgBoxStyle.Exclamation, "Oh No!!")
                Return
            End If
            UpdateShader()
        End Sub
    End Class

    Public shaders As New List(Of Shader)
    Public TextRenderShader As Shader

#End Region

#Region "Compiler code"
    ''' <summary>
    ''' Finds the shaders in the folder and
    ''' calls the assemble_shader fucntion to build them
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub build_shaders()
        If File.Exists("shaders\common.h") Then
            Dim code = File.ReadAllText("shaders\common.h")
            Dim name = "/common.h"
            GL.Arb.NamedString(ArbShadingLanguageInclude.ShaderIncludeArb, name.Length, name, code.Length, code)
        End If

        'Try and keep these in alphabetical order 
        TextRenderShader = New Shader("TextRender")


        shaders.Add(TextRenderShader)
    End Sub

    Public Function assemble_shader(v As String,
                                    g As String,
                                    c As String,
                                    f As String,
                                    name As String) As Integer
        Dim status_code As Integer

        Dim program As Integer = GL.CreateProgram()
        LabelObject(ObjectLabelIdentifier.Program, program, name)

        If program = 0 Then
            Return 0
        End If

        ' Compile vertex shader
        Dim vertexObject As Integer = 0
        If v IsNot Nothing Then
            vertexObject = GL.CreateShader(ShaderType.VertexShader)

            If USE_SPIRV_SHADERS And File.Exists(v + ".spv") Then
                Dim vs_buf = File.ReadAllBytes(v + ".spv")
                GL.ShaderBinary(1, vertexObject, DirectCast(&H9551, BinaryFormat), vs_buf, vs_buf.Length)
                GL.SpecializeShader(vertexObject, "main", 0, 0, 0)
            Else
                Using vs_s As New StreamReader(v)
                    Dim vs As String = vs_s.ReadToEnd()
                    GL.ShaderSource(vertexObject, vs)
                End Using

                GL.Arb.CompileShaderInclude(vertexObject, incPaths.Length, incPaths, incPathLengths)
            End If

            ' Get & check status after compile
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, status_code)
            If status_code = 0 Then
                Dim info = GL.GetShaderInfoLog(vertexObject)
                GL.DeleteShader(vertexObject)
                GL.DeleteProgram(program)
                gl_error(name + "_vertex didn't compile!" + vbCrLf + info.ToString)
                Return 0
            End If
        End If

        ' Compile fragment shader
        Dim fragmentObject As Integer = 0
        If f IsNot Nothing Then
            fragmentObject = GL.CreateShader(ShaderType.FragmentShader)

            If USE_SPIRV_SHADERS And File.Exists(f + ".spv") Then
                Dim fs_buf = File.ReadAllBytes(f + ".spv")
                GL.ShaderBinary(1, fragmentObject, DirectCast(&H9551, BinaryFormat), fs_buf, fs_buf.Length)
                GL.SpecializeShader(fragmentObject, "main", 0, 0, 0)
            Else
                Using fs_s As New StreamReader(f)
                    Dim fs As String = fs_s.ReadToEnd
                    GL.ShaderSource(fragmentObject, fs)
                End Using

                GL.Arb.CompileShaderInclude(fragmentObject, incPaths.Length, incPaths, incPathLengths)
            End If

            ' Get & check status after compile
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, status_code)
            If status_code = 0 Then
                Dim info = GL.GetShaderInfoLog(fragmentObject)
                GL.DeleteShader(vertexObject)
                GL.DeleteShader(fragmentObject)
                GL.DeleteProgram(program)
                gl_error(name + "_fragment didn't compile!" + vbCrLf + info.ToString)
                Return 0
            End If
        End If

        ' Compile geom shader
        Dim geomObject As Integer = 0
        If g IsNot Nothing Then
            geomObject = GL.CreateShader(ShaderType.GeometryShader)

            Using gs_s As New StreamReader(g)
                Dim gs As String = gs_s.ReadToEnd()
                GL.ShaderSource(geomObject, gs)
            End Using

            GL.Arb.CompileShaderInclude(geomObject, incPaths.Length, incPaths, incPathLengths)

            ' Get & check status after compile
            GL.GetShader(geomObject, ShaderParameter.CompileStatus, status_code)
            If status_code = 0 Then
                Dim info = GL.GetShaderInfoLog(geomObject)
                GL.DeleteShader(vertexObject)
                GL.DeleteShader(fragmentObject)
                GL.DeleteShader(geomObject)
                GL.DeleteProgram(program)
                gl_error(name + "_geo didn't compile!" + vbCrLf + info.ToString)
                Return 0
            End If


            'If name.Contains("raytrace") Then
            '    GL.Ext.ProgramParameter(program, DirectCast(ExtGeometryShader4.GeometryInputTypeExt, AssemblyProgramParameterArb), All.Triangles)
            '    GL.Ext.ProgramParameter(program, DirectCast(ExtGeometryShader4.GeometryOutputTypeExt, AssemblyProgramParameterArb), All.LineStrip)
            '    GL.Ext.ProgramParameter(program, DirectCast(ExtGeometryShader4.GeometryVerticesOutExt, AssemblyProgramParameterArb), 6)
            'End If

            If name = "normal" Then
                GL.Ext.ProgramParameter(program, DirectCast(ExtGeometryShader4.GeometryInputTypeExt, AssemblyProgramParameterArb), All.Triangles)
                GL.Ext.ProgramParameter(program, DirectCast(ExtGeometryShader4.GeometryOutputTypeExt, AssemblyProgramParameterArb), All.LineStrip)
                GL.Ext.ProgramParameter(program, DirectCast(ExtGeometryShader4.GeometryVerticesOutExt, AssemblyProgramParameterArb), 21)
            End If

            If name = "TerrainNormals" Then
                GL.Ext.ProgramParameter(program, DirectCast(ExtGeometryShader4.GeometryInputTypeExt, AssemblyProgramParameterArb), All.Triangles)
                GL.Ext.ProgramParameter(program, DirectCast(ExtGeometryShader4.GeometryOutputTypeExt, AssemblyProgramParameterArb), All.LineStrip)
                GL.Ext.ProgramParameter(program, DirectCast(ExtGeometryShader4.GeometryVerticesOutExt, AssemblyProgramParameterArb), 9)
            End If
        End If
        ' Compile Compute shader
        Dim computeObject As Integer = 0
        If c IsNot Nothing Then
            computeObject = GL.CreateShader(ShaderType.ComputeShader)

            If USE_SPIRV_SHADERS And File.Exists(c + ".spv") Then
                Dim cs_buf = File.ReadAllBytes(c + ".spv")
                GL.ShaderBinary(1, computeObject, DirectCast(&H9551, BinaryFormat), cs_buf, cs_buf.Length)
                GL.SpecializeShader(computeObject, "main", 0, 0, 0)
            Else
                Using cs_s As New StreamReader(c)
                    Dim cs As String = cs_s.ReadToEnd()
                    GL.ShaderSource(computeObject, cs)
                End Using

                GL.Arb.CompileShaderInclude(computeObject, incPaths.Length, incPaths, incPathLengths)
            End If

            ' Get & check status after compile
            GL.GetShader(computeObject, ShaderParameter.CompileStatus, status_code)
            If status_code = 0 Then
                Dim info = GL.GetShaderInfoLog(computeObject)
                GL.DeleteShader(vertexObject)
                GL.DeleteShader(fragmentObject)
                GL.DeleteShader(geomObject)
                GL.DeleteShader(computeObject)
                GL.DeleteProgram(program)
                gl_error(name + "_compute didn't compile!" + vbCrLf + info.ToString)
                Return 0
            End If

        End If
        ' attach shader objects
        If vertexObject Then
            GL.AttachShader(program, vertexObject)
        End If

        If geomObject Then
            GL.AttachShader(program, geomObject)
        End If

        If computeObject Then
            GL.AttachShader(program, computeObject)
        End If

        If fragmentObject Then
            GL.AttachShader(program, fragmentObject)
        End If

        ' link program
        GL.LinkProgram(program)

        ' Get & check status after link
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, status_code)
        If status_code = 0 Then
            Dim info = GL.GetProgramInfoLog(program)
            gl_error(name + " Would not link!" + vbCrLf + info.ToString)
        End If

        ' detach shader objects
        If vertexObject Then
            GL.DetachShader(program, vertexObject)
        End If
        If geomObject Then
            GL.DetachShader(program, geomObject)
        End If
        If computeObject Then
            GL.DetachShader(program, computeObject)
        End If
        If fragmentObject Then
            GL.DetachShader(program, fragmentObject)
        End If

        ' delete shader objects
        If vertexObject Then
            GL.DeleteShader(vertexObject)
        End If
        If geomObject Then
            GL.DeleteShader(geomObject)
        End If
        If computeObject Then
            GL.DeleteShader(computeObject)
        End If
        If fragmentObject Then
            GL.DeleteShader(fragmentObject)
        End If

        Return program
    End Function

    Public Sub gl_error(s As String)
        s = s.Replace(vbLf, vbCrLf)
        s.Replace("0(", vbCrLf + "(")
        frmShaderError.Show()
        frmShaderError.er_tb.Text += s
        frmShaderError.er_tb.SelectionLength = 0
        frmShaderError.er_tb.SelectionStart = 0
    End Sub

#End Region
End Module
