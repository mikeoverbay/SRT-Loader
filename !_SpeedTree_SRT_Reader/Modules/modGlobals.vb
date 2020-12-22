Module modGlobals
    Public USE_SPIRV_SHADERS As Boolean

    'GLSL highlighting string used in the editor
    Public GLSL_KEYWORDS As String

    Public _STARTED As Boolean = False
    Public srt_path As String = "C:\Users\theco\OneDrive\Desktop\Extracted\vegetation\Hangars\Scots_Pine"
    Public srt_path2 As String = "C:\Users\theco\OneDrive\Desktop\Extracted\vegetation\Hangars\Scots_Pine"
    '=========================================================
    'Mouse view update screen variables
    Public mouse As New Point
    Public M_DOWN As Boolean = False
    Public move_cam_z As Boolean = False
    Public move_mod As Boolean = False
    Public U_Cam_X_angle, U_Cam_Y_angle, Cam_X_angle, Cam_Y_angle As Single
    Public look_point_x, look_point_y, look_point_z As Single
    Public U_look_point_x, U_look_point_y, U_look_point_z As Single
    Public angle_offset, u_View_Radius As Single
    Public view_radius As Single
    Public cam_x, cam_y, cam_z As Single
    Public eyeX, eyeY, eyeZ As Single
    Public z_move As Boolean = False
    Public gl_busy As Boolean = False
    Public screen_avg_counter, screen_avg_draw_time, screen_draw_time, screen_totaled_draw_time As Double
    Public stopGL As Boolean = False
    Public thePath As String
    Public STM_LOADED As Boolean
    Public SRT_LOADED As Boolean
    Public texture_id As Integer
End Module
