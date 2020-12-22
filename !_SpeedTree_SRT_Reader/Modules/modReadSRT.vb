#Region "imports"
Imports System.Text
Imports System.IO
Imports System.Math
Imports Tao.DevIl
'Imports Ionic.Zip
#End Region

Module modReadSRT
    Dim t_name As String = "scots_pine_0.srt"
    Public polys() As poly_
    Public stm_list As Integer
    Public Structure poly_
        Public vert As vect3
        Public uv As vect2
        Public normal As vect3
    End Structure

    Public Sub read_stm(ByVal stm_name As String)

        STM_LOADED = False

        Dim model = File.ReadAllText(stm_name).Replace(vbCr, "").Split(vbLf)
        Dim tri_count As Integer = model(0) * 3
        ReDim polys(tri_count - 1)
        Dim n_loc As Integer
        Dim uv_loc As Integer
        For i = 1 To tri_count
            polys(i - 1) = New poly_
            Dim d = model(i).Split(" ")

            If d.Length = 13 Then
                n_loc = 3
                uv_loc = 6
            End If
            If d.Length = 8 Then
                n_loc = 3
                uv_loc = 6
            End If
            polys(i - 1).vert.X = d(0)
            polys(i - 1).vert.Z = d(1)
            polys(i - 1).vert.Y = d(2)

            polys(i - 1).uv.X = d(uv_loc + 0)
            polys(i - 1).uv.Y = d(uv_loc + 1)

            polys(i - 1).normal.X = d(n_loc + 0)
            polys(i - 1).normal.Z = d(n_loc + 1)
            polys(i - 1).normal.Y = d(n_loc + 2)

            normalize(polys(i - 1).normal)
        Next
        'If stm_list > 0 Then
        '    Gl.glDeleteLists(stm_list, 1)
        'End If
        'stm_list = Gl.glGenLists(1)
        'Gl.glNewList(stm_list, Gl.GL_COMPILE)

        'Gl.glBegin(Gl.GL_TRIANGLES)
        'For i = 0 To tri_count - 1

        '    Gl.glTexCoord2f(polys(i).uv.X, polys(i).uv.Y)
        '    Gl.glNormal3f(polys(i).normal.X, polys(i).normal.Y, polys(i).normal.Z)
        '    Gl.glVertex3f(polys(i).vert.X, polys(i).vert.Y, polys(i).vert.Z)
        'Next
        'Gl.glEnd()

        'Gl.glEndList()

        STM_LOADED = True
    End Sub

    Public Sub normalize(ByRef v As vect3)
        Dim l = Sqrt(v.X ^ 2 + v.Y ^ 2 + v.Z ^ 2)
        If l = 0.0 Then l = 1.0
        v.X /= l
        v.Y /= l
        v.Z /= l

    End Sub

    Public Function load_tga_file(ByVal fs As String) As Integer
        Dim image_id As Integer = -1

        Dim texID As UInt32
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError
        Il.ilLoad(Il.IL_TGA, fs)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            ' Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)
            'Dim dds_format = Il.ilGetInteger(Il.IL_DXTC_DATA_FORMAT)
            'Debug.WriteLine(dds_format.ToString)

            Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE)

            ''success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE)

            'Gl.glGenTextures(1, image_id)
            'Gl.glEnable(Gl.GL_TEXTURE_2D)
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
            'If largestAnsio > 0 Then
            '    Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)
            'End If
            'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR)
            'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            'Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, width, height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE,
            'Il.ilGetData()) '  Texture specification 


            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            'Il.ilBindImage(0)
            'Ilu.iluDeleteImage(texID)
            'Gl.glFinish()
            'Return image_id
        Else
            Stop
        End If
        Return Nothing
    End Function

    Private Sub prase_wind(ByRef br As BinaryReader, ByRef t As tree_entry_)
        ' todo
        ' tons a data we dont need but may want to read it for fun.
        t.wind_1 = br.ReadSingle
        t.wind_2 = br.ReadSingle
        t.wind_3 = br.ReadSingle
        t.wind_4 = br.ReadSingle

    End Sub
    Private Sub prase_lods(ByRef br As BinaryReader, ByRef t As tree_entry_)

    End Sub
    Private Sub prase_BB(ByRef br As BinaryReader, ByRef t As tree_entry_)
        'read lbb
        t.LBB.X = br.ReadSingle
        t.LBB.Y = br.ReadSingle
        t.LBB.Z = br.ReadSingle
        'read ubb
        t.UBB.X = br.ReadSingle
        t.UBB.Y = br.ReadSingle
        t.UBB.Z = br.ReadSingle
    End Sub

    Private Sub prase_strings(br As BinaryReader, ByRef t As tree_entry_)

        t.NOCOLLIDE = False

        t.string_count = br.ReadInt32 - 1
        ReDim t.str_lens(TREE.string_count - 1)
        ReDim t.strings(TREE.string_count - 1)
        br.ReadInt32() 'blank int
        br.ReadInt32() 'always 4?
        br.ReadInt32() 'blank int
        For i = 0 To t.string_count - 1
            t.str_lens(i) = br.ReadInt32 ' read string lengths
            br.ReadInt32() 'blank int
        Next
        For i = 0 To t.string_count - 1
            Dim b(t.str_lens(i)) As Char
            b = br.ReadChars(b.Length - 1)
            For k = 0 To t.str_lens(i) - 1
                Dim ch As Char = b(k)
                If ch > vbNullChar Then
                    t.strings(i) += ch
                End If
            Next
            to_output(t.strings(i)) 'show in textbox

            If t.strings(i).Contains("NOCOL") Then
                t.NOCOLLIDE = True
            End If
        Next
        'Dim pad = 4 - (br.BaseStream.Position Mod 4)
        'br.BaseStream.Position += pad
    End Sub

    Public Sub prase_collision_objects(br As BinaryReader, ByRef t As tree_entry_)

        ' Not 100% sure about this
        'If t.NOCOLLIDE Then Return

        t.collision_object_count = br.ReadInt32

        If t.collision_object_count = 0 Then Return

        ReDim t.collsion_objects(t.collision_object_count)

        For i = 0 To t.collision_object_count - 1

            t.collsion_objects(i) = New collsion_obj_

            t.collsion_objects(i).m_pUserString = br.ReadUInt32

            t.collsion_objects(i).m_vCenter1 = read_vect3(br)

            t.collsion_objects(i).m_vCenter1 = read_vect3(br)

            t.collsion_objects(i).m_fRadius = br.ReadSingle
        Next

    End Sub

    Private Sub prase_Vertical_billboard(br As BinaryReader, ByRef t As tree_entry_)

        t.Vertical_Billboard = New Vertical_BillBoard_
        t.hasBillBoards = False

        'get diminsions
        t.Vertical_Billboard.billBoard_Width = br.ReadSingle
        t.Vertical_Billboard.billBoard_Top = br.ReadSingle
        t.Vertical_Billboard.billBoard_Bottom = br.ReadSingle

        ' Check if there is actually a billboard for this tree/grass/bush/ect.
        If t.Vertical_Billboard.billBoard_Width <> 0 Then

            t.hasBillBoards = True

            t.Vertical_Billboard.billBoard_Count = br.ReadInt32 ' number of 360 panels and UV data size

            ReDim t.Vertical_Billboard.billB_UVs(t.Vertical_Billboard.billBoard_Count - 1)

            Dim t_loc = br.BaseStream.Position
            For i = 0 To t.Vertical_Billboard.billBoard_Count - 1
                t.Vertical_Billboard.billB_UVs(i).uv_ul = read_vect2(br)
                t.Vertical_Billboard.billB_UVs(i).uv_wh = read_vect2(br)
            Next

            't.Vertical_Billboard.rotated = br.ReadByte <> 0 ' This right????
            Dim unknown1 = br.ReadInt32 ' flag?
            Dim unknown2 = br.ReadInt32 ' flag?

        End If

        'get cutout vert and indi counts
        t.Vertical_Billboard.cut_out_UV_count = br.ReadInt32
        t.Vertical_Billboard.cut_out_indi_count = br.ReadInt32

        ' It is possible there are no cut outs so we must check!
        If t.Vertical_Billboard.cut_out_UV_count > 0 And t.Vertical_Billboard.cut_out_indi_count > 0 Then

            ReDim t.Vertical_Billboard.cut_out_verts(t.Vertical_Billboard.cut_out_UV_count - 1)
            For i = 0 To t.Vertical_Billboard.cut_out_UV_count - 1
                t.Vertical_Billboard.cut_out_verts(i) = read_vect2(br)
            Next

            ReDim t.Vertical_Billboard.cut_out_indies(t.Vertical_Billboard.cut_out_indi_count - 1)
            For i = 0 To t.Vertical_Billboard.cut_out_indi_count - 1
                t.Vertical_Billboard.cut_out_indies(i) = br.ReadUInt16
            Next
        End If

        ReDim t.panels(t.Vertical_Billboard.billBoard_Count - 1)
        Dim w = t.Vertical_Billboard.billBoard_Width
        Dim top = t.Vertical_Billboard.billBoard_Top
        Dim bot = t.Vertical_Billboard.billBoard_Bottom

        'there is an int that = 0
        'followed by 4 uv parts.

        Dim unknown3 = br.ReadInt32

        Dim uv1 = read_vect2(br)
        Dim uv2 = read_vect2(br)
        Dim uv3 = read_vect2(br)
        Dim uv4 = read_vect2(br)

        'For i = 0 To t.Vertical_Billboard.billBoard_Count - 1
        '    With t.panels
        '        t.panels(0) = New panel_



        '    End With
        'Next

    End Sub

    Private Sub prase_horizontal_billBoard(br As BinaryReader, ByRef t As tree_entry_)

        t.Horizontal_Billboard = New Horizzonal_BillBoard_
        t.Horizontal_Billboard.present = br.ReadBoolean <> 0
        t.Horizontal_Billboard.uv1 = read_vect2(br)
        t.Horizontal_Billboard.uv2 = read_vect2(br)
        t.Horizontal_Billboard.uv3 = read_vect2(br)
        t.Horizontal_Billboard.uv4 = read_vect2(br)

        t.Horizontal_Billboard.p1 = read_vect3(br)
        t.Horizontal_Billboard.p2 = read_vect3(br)
        t.Horizontal_Billboard.p3 = read_vect3(br)
        t.Horizontal_Billboard.p4 = read_vect3(br)

    End Sub


    Public Sub prase_custom_Data(br As BinaryReader, ByRef t As tree_entry_)
        'there are 68 bytes here. I am unsure of their usage.
        ' For now, I am just reading them off as ints
        For i = 0 To (68 / 4) - 1
            br.ReadInt32()
        Next
    End Sub

    Public Sub prase_render_States(br As BinaryReader, ByRef t As tree_entry_)

        Dim caps = br.ReadInt32

        t.hasGrass = caps And 1 = 1
        t.hasBranch = caps And 8 = 8


    End Sub

    Public Sub prase_geometry(br As BinaryReader, ByRef t As tree_entry_)

    End Sub

    Public Function load_srt(tree_name As String) As Boolean

        SRT_LOADED = False

        Dim buffer = File.ReadAllBytes(tree_name)
        Dim ms As New MemoryStream(buffer)
        Dim br As New BinaryReader(ms)

        'read header
        Dim header(16) As Byte
        header = br.ReadBytes(16)
        Dim header_text = Encoding.Default.GetString(header)
        to_output(header_text) 'show in textbox
        'get file type info
        TREE.bigEndian = (br.ReadByte <> 0) ' 1 means bigEndian
        TREE.CoordSystem = br.ReadByte
        TREE.UVflip = (br.ReadByte <> 0)
        br.ReadByte() ' reserved

        prase_BB(br, TREE)

        assure_BB_Correct(TREE)
        assign_corners()
        '
        'are there LOD sections?
        Dim LOD_EXIST = br.ReadUInt32() <> 0

        'lod distances
        prase_lods(br, TREE)

        'wind information
        'this section has lots of data.
        prase_wind(br, TREE)

        ms.Position = &H58C ' point to string entry count
        prase_strings(br, TREE)

        Dim indexPos = ms.Position
        'ms.Position += 4

        prase_collision_objects(br, TREE)

        prase_Vertical_billboard(br, TREE)

        'this does not exist in version 6?
        prase_horizontal_billBoard(br, TREE)

        'todo
        prase_custom_Data(br, TREE)

        'todo
        prase_render_States(br, TREE)

        'todo
        prase_geometry(br, TREE)

        ms.Close()
        ms.Dispose()
        SRT_LOADED = True

        Return True
    End Function
    Private Sub assign_corners()
        '1 back
        TREE.BB_Cube.lbl.X = TREE.LBB.X
        TREE.BB_Cube.lbl.Y = TREE.LBB.Y
        TREE.BB_Cube.lbl.Z = TREE.LBB.Z
        '2
        TREE.BB_Cube.lbr.X = TREE.UBB.X
        TREE.BB_Cube.lbr.Y = TREE.LBB.Y
        TREE.BB_Cube.lbr.Z = TREE.LBB.Z
        '3
        TREE.BB_Cube.ltr.X = TREE.UBB.X
        TREE.BB_Cube.ltr.Y = TREE.UBB.Y
        TREE.BB_Cube.ltr.Z = TREE.LBB.Z
        '4
        TREE.BB_Cube.ltl.X = TREE.LBB.X
        TREE.BB_Cube.ltl.Y = TREE.UBB.Y
        TREE.BB_Cube.ltl.Z = TREE.LBB.Z
        '5 front
        TREE.BB_Cube.rtr.X = TREE.UBB.X
        TREE.BB_Cube.rtr.Y = TREE.UBB.Y
        TREE.BB_Cube.rtr.Z = TREE.UBB.Z
        '6
        TREE.BB_Cube.rtl.X = TREE.LBB.X
        TREE.BB_Cube.rtl.Y = TREE.UBB.Y
        TREE.BB_Cube.rtl.Z = TREE.UBB.Z
        '7
        TREE.BB_Cube.rbl.X = TREE.LBB.X
        TREE.BB_Cube.rbl.Y = TREE.LBB.Y
        TREE.BB_Cube.rbl.Z = TREE.UBB.Z
        '7
        TREE.BB_Cube.rbr.X = TREE.UBB.X
        TREE.BB_Cube.rbr.Y = TREE.LBB.Y
        TREE.BB_Cube.rbr.Z = TREE.UBB.Z

        TREE.BB_Cube.make_bb_box()

    End Sub
    Private Sub to_output(s As String)
        frmMain.out_tb.Text += s
        frmMain.out_tb.Text += vbCrLf
        Application.DoEvents()
    End Sub
    Private Sub assure_BB_Correct(ByRef t As tree_entry_)
        If t.LBB.X < t.UBB.X Then
            Dim s = t.LBB.X
            t.LBB.X = t.UBB.X
            t.UBB.X = s
        End If
        If t.LBB.Y < t.UBB.Y Then
            Dim s = t.LBB.Y
            t.LBB.Y = t.UBB.Y
            t.UBB.Y = s
        End If
        If t.LBB.Z < t.UBB.Z Then
            Dim s = t.LBB.Z
            t.LBB.Z = t.UBB.Z
            t.UBB.Z = s
        End If

    End Sub

    Public Function read_vect3(ByRef br As BinaryReader) As vect3
        Dim v As New vect3
        v.X = br.ReadSingle
        v.Y = br.ReadSingle
        v.Z = br.ReadSingle
        Return v
    End Function

    Public Function read_vect2(ByRef br As BinaryReader) As vect2
        Dim v As New vect2
        v.X = br.ReadSingle
        v.Y = br.ReadSingle
        Return v
    End Function


End Module
