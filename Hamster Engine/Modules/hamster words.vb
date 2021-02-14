Imports HamsterEngine.Engine

Public Class Words

    Public version As New HamsterVersion("Words", 1, 0, 131102, 1)
    Public Shared ParseString = "<HE_PARSE>"


    Public Class hException
        Public Shared Unknown As String = "HE_EXP_UNKNOWN"
        Public Shared General As String = "HE_EXP_GENERAL"
        Public Shared IlegalArgument As String = "HE_EXP_ILEGALARGUMENT"
        Public Shared NoPermission As String = "HE_EXP_NOPERMISSION"

        Public Class File
            Public Shared NoEntry As String = "HE_EXP_HMF_NOENTRY"
            Public Shared IlegalEntry As String = "HE_EXP_HMF_ILEGALENTRY"
            Public Shared IO As String = "HE_EXP_HMF_IO"
        End Class
    End Class

    Public Class hQuestion
        Public Shared FuncExit As String = "HE_QES_EXIT"
        Public Shared Success As String = "HE_SUCCESS"
        Public Shared Fail As String = "HE_FAIL"
    End Class

    Public Class hString
        Public Shared NullString As String = "HE_STR_NULL"
    End Class

    Public Class hVersion
        Public Shared NotInclude = "HE_VER_NOTINCLUDE"
    End Class

    Public Class hNetwork

        Public Class Socket
            Public Shared AcceptMAXClient As Byte = 254
            Public Shared AcceptMINClient As Byte = 1

            Public Shared Client = "0"
            Public Shared Server = "0.0.0.0"
        End Class

    End Class

    Public Class hThread

        Public Class Threadname
            Public Shared ListenThread = "ListenThread"
        End Class

    End Class

    Public Class hName
        Public Class ModuleName
            Public Shared Console = "Console"
            Public Shared FileManager = "File"
            Public Shared NetworkManager = "Network"
        End Class
        Public Shared General = "General"
    End Class

End Class
