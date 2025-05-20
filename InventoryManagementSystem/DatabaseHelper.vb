Imports Npgsql

Public Module DatabaseHelper
    Public ReadOnly ConnectionString As String = "Host=localhost;Port=5432;Username=postgres;Password=Walmond@123;Database=inventory_db"

    Public Function GetConnection() As NpgsqlConnection
        Return New NpgsqlConnection(ConnectionString)
    End Function
End Module