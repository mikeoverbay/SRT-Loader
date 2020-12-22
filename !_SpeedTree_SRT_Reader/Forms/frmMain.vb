#Region "imports"
Imports System.Globalization
Imports System.IO
Imports System.Math
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports Tao.DevIl
#End Region

Public Class frmMain
    Public update_thread As New Thread(AddressOf update_mouse)


#Region "frmMain events"

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Show()
        Application.DoEvents()
        Me.KeyPreview = True    'so i catch keyboard before despatching it
        Application.DoEvents()
        Dim flags As GraphicsContextFlags
#If DEBUG Then
        flags = GraphicsContextFlags.ForwardCompatible Or GraphicsContextFlags.Debug
#Else
        flags = GraphicsContextFlags.ForwardCompatible
#End If
        Dim maxSupportedGL = GetMaxGLVersion()
        Me.glControl_main = New OpenTK.GLControl(New GraphicsMode(ColorFormat.Empty, 0), maxSupportedGL.Item1, maxSupportedGL.Item2, flags)
        Me.glControl_main.VSync = False
        Me.glControl_main.Parent = PB1
        Me.glControl_main.Dock = DockStyle.Fill

        Il.ilInit()
        Ilu.iluInit()
        Ilut.ilutInit()
        '-----------------------------------------------------------------------------------------
        'So numbers work in any nation I'm running in.
        Dim nonInvariantCulture As CultureInfo = New CultureInfo("en-US")
        nonInvariantCulture.NumberFormat.NumberDecimalSeparator = "."
        Thread.CurrentThread.CurrentCulture = nonInvariantCulture
        '-----------------------------------------------------------------------------------------
        get_GLSL_filter_strings()
        '-----------------------------------------------------------------------------------------
        'get directory of all shader files
        SHADER_PATHS = Directory.GetFiles(Application.StartupPath + "\shaders\", "*.*", SearchOption.AllDirectories)
        '-----------------------------------------------------------------------------------------
        build_shaders()
        '-----------------------------------------------------------------------------------------
        'make_xy_grid()

        cam_x = 0
        cam_y = 0
        cam_z = 10
        Cam_X_angle = PI * 0.25
        Cam_Y_angle = -PI * 0.25
        view_radius = -10.0


        _STARTED = True
        Timer1.Start()

    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        _STARTED = False
        While update_thread.IsAlive
            Application.DoEvents()
        End While
        'DisableOpenGL()
    End Sub

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.E
                frmProgramEditor.Show()
            Case 16
                move_mod = True
            Case 17
                z_move = True
        End Select

        'If e.KeyCode = 16 Then
        'End If
        'If e.KeyCode = 17 Then
        'End If

    End Sub

    Private Sub frmMain_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        move_mod = False
        z_move = False
    End Sub

#End Region


    Public Sub draw_scene()
        If stopGL Then Return
        If gl_busy Then Return
        gl_busy = True

        Me.glControl_main.Context.MakeCurrent(Me.glControl_main.WindowInfo)

        set_prespective_view()

        GL.ClearColor(0.0, 0.0, 0.0, 0.0)
        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)
        GL.LineWidth(1)
        GL.DepthFunc(DepthFunction.Less)
        GL.Disable(EnableCap.Blend)
        GL.Enable(EnableCap.DepthTest)
        GL.Enable(EnableCap.Lighting)

        GL.Disable(EnableCap.CullFace)
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)

        GL.Enable(EnableCap.PolygonSmooth)
        GL.Enable(EnableCap.Normalize)

        'GL.CallList(GRID_id)

        If SRT_LOADED Then

        End If
        'If TREE.BB_Cube.displayList > 0 Then
        '    Gl.glFrontFace(Gl.GL_CW)
        '    Gl.glColor4f(0.0, 0.0, 0.5, 1.0)
        '    Gl.glCallList(TREE.BB_Cube.displayList)

        'End If
        'If STM_LOADED Then
        '    If texture_id > 0 Then
        '        Gl.glEnable(Gl.GL_TEXTURE_2D)
        '        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        '        Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture_id)
        '    End If
        '    Gl.glFrontFace(Gl.GL_CW)
        '    Gl.glColor4f(0.4, 0.4, 0.4, 1.0)
        '    Gl.glCallList(stm_list)

        'End If
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        'Gl.glDisable(Gl.GL_TEXTURE_2D)

        ''-----------------------------------
        'If move_mod Or z_move Then    'draw reference lines to eye center
        '    Gl.glColor3f(1.0, 1.0, 1.0)
        '    Gl.glLineWidth(1)
        '    Gl.glBegin(Gl.GL_LINES)
        '    Gl.glVertex3f(U_look_point_x, U_look_point_y + 1000, U_look_point_z)
        '    Gl.glVertex3f(U_look_point_x, U_look_point_y - 1000, U_look_point_z)

        '    Gl.glVertex3f(U_look_point_x + 1000, U_look_point_y, U_look_point_z)
        '    Gl.glVertex3f(U_look_point_x - 1000, U_look_point_y, U_look_point_z)

        '    Gl.glVertex3f(U_look_point_x, U_look_point_y, U_look_point_z + 1000)
        '    Gl.glVertex3f(U_look_point_x, U_look_point_y, U_look_point_z - 1000)
        '    Gl.glEnd()
        'End If

        glControl_main.SwapBuffers()

        gl_busy = False

    End Sub

    Private Sub get_GLSL_filter_strings()
        Dim ts = IO.File.ReadAllText(Application.StartupPath + "\data\glsl_filtered_strings.txt")
        Dim f_list = ts.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        set_GLSL_keywords(f_list)
    End Sub
    Private Sub set_GLSL_keywords(ByRef f_list() As String)
        GLSL_KEYWORDS = "\b("
        For Each s In f_list
            If InStr(s, "#") = 0 Then
                If s.Length > 2 Then
                    GLSL_KEYWORDS += s + "|"
                End If
            End If
        Next
        'this is needed because of the last | in the load loop!
        GLSL_KEYWORDS += "float)\b"
    End Sub

#Region "update timing"

    Private Delegate Sub update_screen_delegate()
    Private Sub update_screen()
        Try
            If Me.InvokeRequired Then
                Me.Invoke(New update_screen_delegate(AddressOf update_screen))
            Else
                draw_scene()
            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Function need_update() As Boolean
        'This updates the display if the mouse has changed the view angles, locations or distance.
        Dim update As Boolean = False

        If look_point_x <> U_look_point_x Then
            U_look_point_x = look_point_x
            update = True
        End If
        If look_point_y <> U_look_point_y Then
            U_look_point_y = look_point_y
            update = True
        End If
        If look_point_z <> U_look_point_z Then
            U_look_point_z = look_point_z
            update = True
        End If
        If Cam_X_angle <> U_Cam_X_angle Then
            U_Cam_X_angle = Cam_X_angle
            update = True
        End If
        If Cam_Y_angle <> U_Cam_Y_angle Then
            U_Cam_Y_angle = Cam_Y_angle
            update = True
        End If
        If view_radius <> u_View_Radius Then
            u_View_Radius = view_radius
            update = True
        End If

        Return update
    End Function
    Public Sub update_mouse()
        'Dim l_rot As Single
        Dim sun_angle As Single = 0
        Dim sun_radius As Single = 5
        'This will run for the duration that Terra! is open.
        'Its in a closed loop
        screen_totaled_draw_time = 10.0
        Dim swat As New Stopwatch
        While _STARTED
            need_update()
            angle_offset = 0

            '	Application.DoEvents()
            If Not gl_busy And Not Me.WindowState = FormWindowState.Minimized Then

                'If spin_light Then
                '    Dim x, z As Single
                '    l_rot += 0.01
                '    If l_rot > 2 * PI Then
                '        l_rot -= (2 * PI)
                '    End If
                '    If sun_radius > 0 Then
                '        'sun_radius *= -1.0
                '    End If
                '    Dim s As Single = 2.0
                '    sun_angle = l_rot
                '    x = Cos(l_rot) * (sun_radius * s)
                '    z = Sin(l_rot) * (sun_radius * s)
                '    '                    position0(0) = x
                '    ' position0(1) = sun_radius * s * 0.75
                '    '                   position0(1) = 2.5

                '    '                    position0(2) = z

                'End If


                'If Not w_changing Then
                update_screen()
                'End If
                screen_draw_time = CInt(swat.ElapsedMilliseconds)
                Dim freq = Stopwatch.Frequency
                'screen_draw_time = screen_draw_time / freq
                'screen_draw_time *= 0.001
                swat.Reset()
                swat.Start()
                If screen_avg_counter > 15 Then
                    screen_totaled_draw_time = screen_avg_draw_time / screen_avg_counter
                    screen_avg_counter = 0
                    screen_avg_draw_time = 0
                Else
                    If screen_draw_time < 1 Then
                        'screen_draw_time = 5
                    End If
                    screen_avg_counter += 1
                    screen_avg_draw_time += screen_draw_time
                End If
            End If

            Thread.Sleep(10)
        End While
        'Thread.CurrentThread.Abort()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False
        update_thread.IsBackground = True
        update_thread.Name = "mouse updater"
        update_thread.Priority = ThreadPriority.Normal
        update_thread.Start()
    End Sub
#End Region

#Region "PB1 Mouse"

    Private Sub PB1_MouseDown(sender As Object, e As MouseEventArgs) Handles PB1.MouseDown
        If e.Button = Forms.MouseButtons.Left Then
            mouse.X = e.X
            mouse.Y = e.Y
            M_DOWN = True
        End If
        If e.Button = Forms.MouseButtons.Right Then
            'Timer1.Enabled = False
            move_cam_z = True
            mouse.X = e.X
            mouse.Y = e.Y
        End If

    End Sub

    Private Sub PB1_MouseEnter(sender As Object, e As EventArgs) Handles PB1.MouseEnter
        PB1.Focus()
    End Sub

    Private Sub PB1_MouseMove(sender As Object, e As MouseEventArgs) Handles PB1.MouseMove
        Dim dead As Integer = 5
        Dim t As Single
        Dim M_Speed As Single = 0.8
        Dim ms As Single = 0.2F * view_radius ' distance away changes speed.. THIS WORKS WELL!
        If M_DOWN Then
            If e.X > (mouse.X + dead) Then
                If e.X - mouse.X > 100 Then t = (1.0F * M_Speed)
            Else : t = CSng(Sin((e.X - mouse.X) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        look_point_x -= ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_z -= ((t * ms) * (-Sin(Cam_X_angle)))
                    Else
                        Cam_X_angle -= t
                    End If
                    If Cam_X_angle > (2 * PI) Then Cam_X_angle -= (2 * PI)
                    mouse.X = e.X
                End If
            End If
            If e.X < (mouse.X - dead) Then
                If mouse.X - e.X > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.X - e.X) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        look_point_x += ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_z += ((t * ms) * (-Sin(Cam_X_angle)))
                    Else
                        Cam_X_angle += t
                    End If
                    If Cam_X_angle < 0 Then Cam_X_angle += (2 * PI)
                    mouse.X = e.X
                End If
            End If
            ' ------- Y moves ----------------------------------
            If e.Y > (mouse.Y + dead) Then
                If e.Y - mouse.Y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((e.Y - mouse.Y) / 100)) * M_Speed
                If z_move Then
                    look_point_y -= (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        look_point_z -= ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_x -= ((t * ms) * (Sin(Cam_X_angle)))
                    Else
                        If Cam_Y_angle - t < -PI / 2.0 Then
                            Cam_Y_angle = (-PI / 2.0) + 0.001
                        Else
                            Cam_Y_angle -= t
                        End If
                    End If
                End If
                mouse.Y = e.Y
            End If
            If e.Y < (mouse.Y - dead) Then
                If mouse.Y - e.Y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.Y - e.Y) / 100)) * M_Speed
                If z_move Then
                    look_point_y += (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        look_point_z += ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_x += ((t * ms) * (Sin(Cam_X_angle)))
                    Else
                        Cam_Y_angle += t
                    End If
                    If Cam_Y_angle > 1.3 Then Cam_Y_angle = 1.3
                End If
                mouse.Y = e.Y
            End If
            Return
        End If
        If move_cam_z Then
            If e.Y > (mouse.Y + dead) Then
                If e.Y - mouse.Y > 100 Then t = (10)
            Else : t = CSng(Sin((e.Y - mouse.Y) / 100)) * 12
                view_radius += (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                mouse.Y = e.Y
            End If
            If e.Y < (mouse.Y - dead) Then
                If mouse.Y - e.Y > 100 Then t = (10)
            Else : t = CSng(Sin((mouse.Y - e.Y) / 100)) * 12
                view_radius -= (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                If view_radius > -0.01 Then view_radius = -0.01
                mouse.Y = e.Y
            End If
            If view_radius > -0.1 Then view_radius = -0.1
            Return
        End If
        mouse.X = e.X
        mouse.Y = e.Y
    End Sub

    Private Sub PB1_MouseUp(sender As Object, e As MouseEventArgs) Handles PB1.MouseUp
        M_DOWN = False
        move_cam_z = False
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        OpenFileDialog1.Filter = "STM (.stm)|*.stm"
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            read_stm(OpenFileDialog1.FileName)
        End If
    End Sub

    Private Sub LoadTgaTextureToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadTgaTextureToolStripMenuItem.Click
        OpenFileDialog1.Filter = "TGA (.tga)|*.tga"
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            Dim t_path = OpenFileDialog1.FileName
            If texture_id > 0 Then
                GL.DeleteTextures(1, texture_id)
            End If
            texture_id = load_tga_file(t_path)
        End If
    End Sub

    Private Sub LOADSRTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LOADSRTToolStripMenuItem.Click
        OpenFileDialog1.Filter = "SRT (.srt)|*.srt"
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            load_srt(OpenFileDialog1.FileName)
        End If
    End Sub

#End Region
End Class
