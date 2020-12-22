

Module modTreeStructures
    Public Enum EInstanceType As Integer
        INSTANCES_3D_TREES
        INSTANCES_GRASS
        INSTANCES_BILLBOARDS
        INSTANCES_NONE
    End Enum

    Public Structure vect3
        Public X, Y, Z As Single
    End Structure
    Public Structure vect2
        Public X, Y As Single
    End Structure

    Public Structure bb_cube_
        Public displayList As Integer
        Public lbl As vect3
        Public lbr As vect3
        Public ltl As vect3
        Public ltr As vect3
        Public rbl As vect3
        Public rbr As vect3
        Public rtl As vect3
        Public rtr As vect3
        Public Sub make_bb_box()
            'Me.displayList = Gl.glGenLists(1)
            'Gl.glNewList(Me.displayList, Gl.GL_COMPILE)

            'Gl.glBegin(Gl.GL_QUADS)
            ''1 right
            'Gl.glNormal3f(1.0, 0.0, 0.0)
            'Gl.glVertex3f(Me.lbr.X, Me.lbr.Y, Me.lbr.Z)
            'Gl.glVertex3f(Me.ltr.X, Me.ltr.Y, Me.ltr.Z)
            'Gl.glVertex3f(Me.rtr.X, Me.rtr.Y, Me.rtr.Z)
            'Gl.glVertex3f(Me.rbr.X, Me.rbr.Y, Me.rbr.Z)
            ''2 back
            'Gl.glNormal3f(0.0, 0.0, -1.0)
            'Gl.glVertex3f(Me.lbl.X, Me.lbl.Y, Me.lbl.Z)
            'Gl.glVertex3f(Me.ltl.X, Me.ltl.Y, Me.ltl.Z)
            'Gl.glVertex3f(Me.ltr.X, Me.ltr.Y, Me.ltr.Z)
            'Gl.glVertex3f(Me.lbr.X, Me.lbr.Y, Me.lbr.Z)
            ''3 left
            'Gl.glNormal3f(-1.0, 0.0, 0.0)
            'Gl.glVertex3f(Me.rbl.X, Me.rbl.Y, Me.rbl.Z)
            'Gl.glVertex3f(Me.rtl.X, Me.rtl.Y, Me.rtl.Z)
            'Gl.glVertex3f(Me.ltl.X, Me.ltl.Y, Me.ltl.Z)
            'Gl.glVertex3f(Me.lbl.X, Me.lbl.Y, Me.lbl.Z)
            ''4 front
            'Gl.glNormal3f(0.0, 0.0, 1.0)
            'Gl.glVertex3f(Me.rbr.X, Me.rbr.Y, Me.rbr.Z)
            'Gl.glVertex3f(Me.rtr.X, Me.rtr.Y, Me.rtr.Z)
            'Gl.glVertex3f(Me.rtl.X, Me.rtl.Y, Me.rtl.Z)
            'Gl.glVertex3f(Me.rbl.X, Me.rbl.Y, Me.rbl.Z)
            ''5 top
            'Gl.glNormal3f(0.0, 1.0, 0.0)
            'Gl.glVertex3f(Me.rtr.X, Me.rtr.Y, Me.rtr.Z)
            'Gl.glVertex3f(Me.ltr.X, Me.ltr.Y, Me.ltr.Z)
            'Gl.glVertex3f(Me.ltl.X, Me.ltl.Y, Me.ltl.Z)
            'Gl.glVertex3f(Me.rtl.X, Me.rtl.Y, Me.rtl.Z)
            ''6 bottom
            'Gl.glNormal3f(0.0, -1.0, 0.0)
            'Gl.glVertex3f(Me.rbl.X, Me.rbl.Y, Me.rbl.Z)
            'Gl.glVertex3f(Me.lbl.X, Me.lbl.Y, Me.lbl.Z)
            'Gl.glVertex3f(Me.lbr.X, Me.lbr.Y, Me.lbr.Z)
            'Gl.glVertex3f(Me.rbr.X, Me.rbr.Y, Me.rbr.Z)
            'Gl.glEnd()

            'Gl.glEndList()
        End Sub

    End Structure

    Public TREE As tree_entry_

    Public Structure tree_entry_

        Public hasBranch As Boolean
        Public hasFrondsCaps As Boolean
        Public hasLeafs As Boolean
        Public hasForwardFacingLeafs As Boolean
        Public hasRigidGeo As Boolean
        Public hasGrass As Boolean

        Public hasBillBoards As Boolean

        Public UBB As vect3
        Public LBB As vect3
        Public BB_Cube As bb_cube_
        'lod
        Public LOD_0_distance As Single
        Public LOD_1_distance As Single
        Public LOD_2_distance As Single
        Public LOD_3_distance As Single
        'wind... this has to change
        Public wind_1 As Single
        Public wind_2 As Single
        Public wind_3 As Single
        Public wind_4 As Single
        'strings
        Public string_count As Integer
        Public str_lens() As Integer
        Public strings() As String
        'flags
        Public bigEndian As Boolean
        Public isGrass As Boolean
        '0 COORD_SYS_RIGHT_HANDED_Z_UP
        '1 COORD_SYS_RIGHT_HANDED_Y_UP
        '2 COORD_SYS_LEFT_HANDED_Z_UP
        '3 COORD_SYS_LEFT_HANDED_Y_UP
        '4 COORD_SYS_CUSTOM
        Public CoordSystem As Integer
        Public UVflip As Boolean

        'bill board info
        Public b_width As Single
        Public b_top As Single
        Public b_bot As Single
        Public b_count As Integer
        Public b_uvs() As vect2
        Public LOD_EXIST As Boolean
        Public NOCOLLIDE As Boolean

        Public collision_object_count As Integer
        Public collsion_objects() As collsion_obj_

        Public Vertical_Billboard As Vertical_BillBoard_
        Public Horizontal_Billboard As Horizzonal_BillBoard_

        Public panel_count As Integer
        Public panels() As panel_

    End Structure

    Public Structure panel_
        Public panel_verts() As panel_vert_
    End Structure

    Public Structure panel_vert_
        Public v As vect3
        Public n As vect3
        Public uv As vect2

    End Structure
    Public Structure collsion_obj_
        Dim m_pUserString As UInt32         ' any data entered by the artist In the Modeler app
        Dim m_vCenter1 As vect3             ' center Of sphere Or one End Of a capsule
        Dim m_vCenter2 As vect3             ' other End Of capsule Or same As m_vCenter1 If sphere
        Dim m_fRadius As Single
    End Structure

    Public Structure Vertical_BillBoard_

        Public billBoard_Width As Single        '  width of the billboard, governed by tree extents	
        Public billBoard_Top As Single          ' top-most point Of the billboard, governed by tree height
        Public billBoard_Bottom As Single       ' bottom-most point, can be below zero for trees with roots, etc.

        Public billBoard_Count As Integer       ' number of 360-degree billboards generated by Compiler app

        Public texCoord_loc_Left_u As Single    ' 4 entries per image (left u, bottom v, width u, height v)
        Public texCoord_loc_Bottom_v As Single
        Public texCoord_u_Width As Single
        Public texCoord_v_height As Single

        Public rotated As Boolean                ' one entry per image, true = rotated, false = standard

        'There seems to be vertex data 
        Public billB_UVs() As bb_panel_uvs_

        Public cut_out_UV_count As Integer
        ' the Compiler app can generate non-rectangular cutouts, reducing the fill requirements at the
        ' cost of added vertices; these vertices are 
        ' # elements = 2 * m_nNumCutoutVertices [ (x,y) pairs ]
        ' [x,y] values are range [0,1] as percent across height & width
        Public cut_out_verts() As vect2

        Public cut_out_indi_count As Integer

        ' # elements = m_nNumCutoutIndices, indexed triangles
        Public cut_out_indies() As UInt16
    End Structure

    Public Structure bb_panel_uvs_
        Public uv_ul As vect2
        Public uv_wh As vect2
    End Structure


    Public Structure Horizzonal_BillBoard_
        Public present As Boolean               ' true if an overhead billboard was exported using Compiler
        Public p1, p2, p3, p4 As vect3          ' four sets of (xyz) to render the overhead square
        Public uv1, uv2, uv3, uv4 As vect2      ' 4 * (s,t) pairs of diffuse/normal texcoords
    End Structure

    Enum LightingModel
        LIGHTING_MODEL_PER_VERTEX
        LIGHTING_MODEL_PER_PIXEL
        LIGHTING_MODEL_PER_VERTEX_X_PER_PIXEL ' transitional state, forward rendering only
        LIGHTING_MODEL_DEFERRED
    End Enum
    '    Public Structure renderState_
    '        ' lighting model
    '        Public Shared m_eLightingModel = LightingModel.LIGHTING_MODEL_PER_VERTEX
    '	' ambient
    '	m_vAmbientColor(1.0f, 1.0f, 1.0f),
    '	m_eAmbientContrast(EFFECT_OFF),
    '	m_fAmbientContrastFactor(0.0f),
    '	m_bAmbientOcclusion(false),
    '	' diffuse
    '	m_vDiffuseColor(1.0f, 1.0f, 1.0f),
    '	m_fDiffuseScalar(1.0f),
    '	m_bDiffuseAlphaMaskIsOpaque(false),
    '	' detail
    '	m_eDetailLayer(EFFECT_OFF),
    '	' specular
    '	m_eSpecular(EFFECT_OFF),
    '	m_fShininess(30.0f),
    '	m_vSpecularColor(1.0f, 1.0f, 1.0f),
    '	' transmission
    '	m_eTransmission(EFFECT_OFF),
    '	m_vTransmissionColor(1.0f, 1.0f, 0.0f),
    '	m_fTransmissionShadowBrightness(0.2f),
    '	m_fTransmissionViewDependency(0.5f),
    '	' branch seam smoothing
    '	m_eBranchSeamSmoothing(EFFECT_OFF),
    '	m_fBranchSeamWeight(1.0f),
    '	' LOD parameters
    '	m_eLodMethod(LOD_METHOD_POP),
    '	m_bFadeToBillboard(true),
    '	m_bVertBillboard(false),
    '	m_bHorzBillboard(false),
    '	' render states
    '	m_eShaderGenerationMode(SHADER_GEN_MODE_STANDARD),
    '	m_bUsedAsGrass(false),
    '	m_eFaceCulling(CULLTYPE_NONE),
    '	m_bBlending(false),
    '	' image-based ambient lighting
    '	m_eAmbientImageLighting(EFFECT_OFF),
    '	m_eHueVariation(EFFECT_OFF),
    '	' fog
    '	m_eFogCurve(FOG_CURVE_NONE),
    '	m_eFogColorStyle(FOG_COLOR_TYPE_CONSTANT),
    '	' shadows
    '	m_bCastsShadows(false),
    '	m_bReceivesShadows(false),
    '	m_bShadowSmoothing(false),
    '	' alpha effects
    '	m_fAlphaScalar(1.4f),
    '	' wind
    '	m_eWindLod(WIND_LOD_NONE),
    '	' non-lighting shader
    '	m_eRenderPass(RENDER_PASS_MAIN),
    '	' geometry
    '	m_bBranchesPresent(false),
    '	m_bFrondsPresent(false),
    '	m_bLeavesPresent(false),
    '	m_bFacingLeavesPresent(false),
    '	m_bRigidMeshesPresent(false),
    '	' misc
    '	m_pDescription(NULL),
    '	m_pUserData(NULL)
    '{
    '	For (st_int32 i = 0; i < TL_NUM_TEX_LAYERS; ++i)
    '		m_apTextures[i] = NULL;
    '}
    '    End Structure
End Module
