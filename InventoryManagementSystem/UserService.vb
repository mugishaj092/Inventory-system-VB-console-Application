Imports Npgsql

Public Module UserService
    Public Function Authenticate(username As String, password As String) As Boolean
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()
            Dim cmd As New NpgsqlCommand("SELECT * FROM users WHERE username=@u AND password=@p", conn)
            cmd.Parameters.AddWithValue("u", username)
            cmd.Parameters.AddWithValue("p", password)
            Dim reader = cmd.ExecuteReader()
            If reader.Read() Then
                Console.WriteLine($"Welcome {reader("first_name")} {reader("last_name")} ({reader("email")})")
                Return True
            End If
            Return False
        End Using
    End Function

    Public Sub RegisterUser(fname As String, lname As String, email As String, username As String, password As String)
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()

            ' Clear screen for clean registration display
            Console.Clear()
            Console.ForegroundColor = ConsoleColor.Cyan
            Console.WriteLine("╔════════════════════════════════════════╗")
            Console.WriteLine("║          USER REGISTRATION             ║")
            Console.WriteLine("╚════════════════════════════════════════╝")
            Console.ResetColor()
            Console.WriteLine()

            ' First check if username exists
            Dim checkCmd As New NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @u", conn)
            checkCmd.Parameters.AddWithValue("u", username)
            Dim userCount = Convert.ToInt32(checkCmd.ExecuteScalar())

            If userCount > 0 Then
                ' Username exists - show error
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine("│        REGISTRATION FAILED             │")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()
                Console.WriteLine()
                Console.WriteLine($" The username '{username}' is already taken.")
                Console.WriteLine(" Please choose a different username.")

                ' Show decorative footer
                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine("*".PadRight(40, "*"))
                Console.ResetColor()

                Throw New ArgumentException("Username already exists")
            End If

            ' Check if email exists
            Dim emailCheckCmd As New NpgsqlCommand("SELECT COUNT(*) FROM users WHERE email = @e", conn)
            emailCheckCmd.Parameters.AddWithValue("e", email)
            Dim emailCount = Convert.ToInt32(emailCheckCmd.ExecuteScalar())

            If emailCount > 0 Then
                ' Email exists - show error
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine("│        REGISTRATION FAILED             │")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()
                Console.WriteLine()
                Console.WriteLine($" The email '{email}' is already registered.")
                Console.WriteLine(" Please use a different email address.")

                ' Show decorative footer
                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine("*".PadRight(40, "*"))
                Console.ResetColor()

                Throw New ArgumentException("Email already registered")
            End If

            ' Insert new user
            Dim insertCmd As New NpgsqlCommand(
            "INSERT INTO users (username, password, first_name, last_name, email) " &
            "VALUES (@u, @p, @f, @l, @e) RETURNING id", conn)
            insertCmd.Parameters.AddWithValue("u", username)
            insertCmd.Parameters.AddWithValue("p", password)
            insertCmd.Parameters.AddWithValue("f", fname)
            insertCmd.Parameters.AddWithValue("l", lname)
            insertCmd.Parameters.AddWithValue("e", email)

            Dim newUserId = insertCmd.ExecuteScalar()

            If newUserId IsNot Nothing Then
                ' Success UI
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine("│       REGISTRATION SUCCESSFUL          │")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()
                Console.WriteLine()

                ' Show user details
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine("│          ACCOUNT DETAILS               │")
                Console.WriteLine("├────────────────────────────────────────┤")
                Console.WriteLine($"│ {"Name:".PadRight(15)} {fname} {lname}".PadRight(41) & "│")
                Console.WriteLine("├────────────────────────────────────────┤")
                Console.WriteLine($"│ {"Username:".PadRight(15)} {username}".PadRight(41) & "│")
                Console.WriteLine("├────────────────────────────────────────┤")
                Console.WriteLine($"│ {"Email:".PadRight(15)} {email}".PadRight(41) & "│")
                Console.WriteLine("├────────────────────────────────────────┤")
                Console.WriteLine($"│ {"User ID:".PadRight(15)} {newUserId}".PadRight(41) & "│")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()

                ' Success message and timestamp
                Console.WriteLine()
                Console.WriteLine(" Your account has been created successfully!")
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine($"* Registered at {DateTime.Now.ToString("hh:mm tt, dd MMM yyyy")} *".PadLeft(44))
                Console.ResetColor()
            Else
                ' Registration failed
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine("│        REGISTRATION FAILED             │")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()
                Console.WriteLine()
                Console.WriteLine(" An unexpected error occurred during registration.")
                Console.WriteLine(" Please try again or contact support.")

                Throw New Exception("User registration failed")
            End If

            ' Decorative footer
            Console.WriteLine()
            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("*".PadRight(40, "*"))
            Console.ResetColor()
        End Using
    End Sub

    Public Sub ViewProfile(username As String)
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()
            Dim cmd As New NpgsqlCommand("SELECT first_name, last_name, email, username FROM users WHERE username=@u", conn)
            cmd.Parameters.AddWithValue("u", username)

            Dim reader = cmd.ExecuteReader()
            If reader.Read() Then
                Console.Clear()
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.WriteLine("╔════════════════════════════════════════╗")
                Console.WriteLine("║            USER PROFILE                ║")
                Console.WriteLine("╚════════════════════════════════════════╝")
                Console.ResetColor()
                Console.WriteLine()

                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine("│          ACCOUNT INFORMATION           │")
                Console.WriteLine("├────────────────────────────────────────┤")
                Console.WriteLine($"│ {"Name:".PadRight(15)} {reader("first_name")} {reader("last_name")}".PadRight(41) & "│")
                Console.WriteLine("├────────────────────────────────────────┤")
                Console.WriteLine($"│ {"Username:".PadRight(15)} {reader("username")}".PadRight(41) & "│")
                Console.WriteLine("├────────────────────────────────────────┤")
                Console.WriteLine($"│ {"Email:".PadRight(15)} {reader("email")}".PadRight(41) & "│")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()
            Else
                Throw New Exception("User not found")
            End If
        End Using
    End Sub

    Public Sub UpdateProfile(username As String, newFname As String, newLname As String, newEmail As String)
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()
            Dim cmd As New NpgsqlCommand(
                "UPDATE users SET first_name=@f, last_name=@l, email=@e WHERE username=@u",
                conn)
            cmd.Parameters.AddWithValue("f", newFname)
            cmd.Parameters.AddWithValue("l", newLname)
            cmd.Parameters.AddWithValue("e", newEmail)
            cmd.Parameters.AddWithValue("u", username)

            Dim rowsAffected = cmd.ExecuteNonQuery()
            If rowsAffected = 0 Then
                Throw New Exception("Profile update failed")
            End If
        End Using
    End Sub

    Public Sub ChangePassword(username As String, currentPassword As String, newPassword As String)
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()

            ' First verify current password
            Dim verifyCmd As New NpgsqlCommand(
                "SELECT COUNT(*) FROM users WHERE username=@u AND password=@p",
                conn)
            verifyCmd.Parameters.AddWithValue("u", username)
            verifyCmd.Parameters.AddWithValue("p", currentPassword)

            If Convert.ToInt32(verifyCmd.ExecuteScalar()) = 0 Then
                Throw New ArgumentException("Current password is incorrect")
            End If

            ' Update password
            Dim updateCmd As New NpgsqlCommand(
                "UPDATE users SET password=@p WHERE username=@u",
                conn)
            updateCmd.Parameters.AddWithValue("p", newPassword)
            updateCmd.Parameters.AddWithValue("u", username)

            If updateCmd.ExecuteNonQuery() = 0 Then
                Throw New Exception("Password change failed")
            End If
        End Using
    End Sub
End Module