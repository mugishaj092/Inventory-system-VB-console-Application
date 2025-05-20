Imports Npgsql
Imports System

Module Program
    Sub Main()
        Dim currentUsername As String = ""
        Console.Title = "Inventory Management System"
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine("╔════════════════════════════════════════╗")
        Console.WriteLine("║    INVENTORY MANAGEMENT SYSTEM         ║")
        Console.WriteLine("╚════════════════════════════════════════╝")
        Console.ResetColor()


        Dim authenticated As Boolean = False
        Do
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine(vbCrLf & "MAIN MENU")
            Console.WriteLine("═════════")
            Console.ResetColor()

            Console.WriteLine("1. Login")
            Console.WriteLine("2. Sign Up")
            Console.WriteLine("3. Exit")
            Console.WriteLine()

            Console.ForegroundColor = ConsoleColor.DarkCyan
            Console.Write("Enter your choice (1-3): ")
            Console.ResetColor()

            Dim optionChoice As Integer
            If Not Integer.TryParse(Console.ReadLine(), optionChoice) Then
                ShowError("Invalid input. Please enter a number.")
                Continue Do
            End If

            Select Case optionChoice
                Case 1
                    Console.WriteLine()
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Console.WriteLine("LOGIN")
                    Console.WriteLine("─────")
                    Console.ResetColor()

                    Console.Write("Username: ")
                    Dim username = Console.ReadLine()
                    Console.Write("Password: ")
                    Dim password = GetMaskedPassword()

                    authenticated = UserService.Authenticate(username, password)
                    If Not authenticated Then
                        ShowError("Authentication failed. Invalid username or password.")
                    Else
                        currentUsername = username
                        ShowSuccess($"Welcome back, {username}!")
                    End If

                Case 2
                    Console.WriteLine()
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Console.WriteLine("REGISTER NEW USER")
                    Console.WriteLine("─────────────────")
                    Console.ResetColor()

                    Console.Write("First name: ")
                    Dim fname = Console.ReadLine()
                    Console.Write("Last name: ")
                    Dim lname = Console.ReadLine()
                    Console.Write("Email: ")
                    Dim email = Console.ReadLine()
                    Console.Write("Username: ")
                    Dim username = Console.ReadLine()
                    Console.Write("Password: ")
                    Dim password = GetMaskedPassword()

                    Try
                        UserService.RegisterUser(fname, lname, email, username, password)
                    Catch ex As Exception
                        ShowError($"Registration failed: {ex.Message}")
                    End Try

                Case 3
                    Console.WriteLine()
                    ShowInfo("Thank you for using the system. Goodbye!")
                    Return

                Case Else
                    ShowError("Invalid choice. Please select 1-3.")
            End Select
        Loop Until authenticated

        ' Main application loop after authentication
        Dim choice As Integer
        Do
            Console.Clear()
            ShowHeader("INVENTORY MANAGEMENT SYSTEM")

            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("MAIN OPTIONS")
            Console.WriteLine("────────────")
            Console.ResetColor()

            Console.WriteLine("1. View All Items")
            Console.WriteLine("2. View Single Item")
            Console.WriteLine("3. Add New Item")
            Console.WriteLine("4. Update Item")
            Console.WriteLine("5. Delete Item")
            Console.WriteLine("6. Create Bill")
            Console.WriteLine("7. View Bills")
            Console.WriteLine("8. Delete Bill")
            Console.WriteLine("9. Account Settings")
            Console.WriteLine("10. Exit")
            Console.WriteLine()

            Console.ForegroundColor = ConsoleColor.DarkCyan
            Console.Write("Enter your choice (1-9): ")
            Console.ResetColor()

            If Not Integer.TryParse(Console.ReadLine(), choice) Then
                ShowError("Invalid input. Please enter a number.")
                Continue Do
            End If

            Select Case choice
                Case 1
                    Console.Clear()
                    ShowHeader("VIEW ALL ITEMS")
                    ItemService.ViewItems()
                    PressAnyKeyToContinue()

                Case 2
                    Console.Clear()
                    ShowHeader("VIEW ITEM DETAILS")
                    Console.Write("Enter Item ID: ")
                    Dim id As Integer
                    If Integer.TryParse(Console.ReadLine(), id) Then
                        ItemService.ViewSingleItem(id)
                    Else
                        ShowError("Invalid Item ID format.")
                    End If
                    PressAnyKeyToContinue()

                Case 3
                    Console.Clear()
                    ShowHeader("ADD NEW ITEM")
                    Try
                        Console.Write("Item name: ")
                        Dim name = Console.ReadLine()
                        Console.Write("Quantity: ")
                        Dim qty = Integer.Parse(Console.ReadLine())
                        Console.Write("Price: ")
                        Dim price = Decimal.Parse(Console.ReadLine())

                        ItemService.AddItem(name, qty, price)
                        ShowSuccess("Item added successfully!")
                    Catch ex As Exception
                        ShowError($"Invalid input: {ex.Message}")
                    End Try
                    PressAnyKeyToContinue()

                Case 4
                    Console.Clear()
                    ShowHeader("UPDATE ITEM")
                    Try
                        Console.Write("Enter Item ID to update: ")
                        Dim id = Integer.Parse(Console.ReadLine())
                        Console.Write("New name: ")
                        Dim name = Console.ReadLine()
                        Console.Write("New quantity: ")
                        Dim qty = Integer.Parse(Console.ReadLine())
                        Console.Write("New price: ")
                        Dim price = Decimal.Parse(Console.ReadLine())

                        ItemService.UpdateItem(id, name, qty, price)
                    Catch ex As Exception
                        ShowError($"Invalid input: {ex.Message}")
                    End Try
                    PressAnyKeyToContinue()

                Case 5
                    Console.Clear()
                    ShowHeader("DELETE ITEM")
                    Console.Write("Enter Item ID to delete: ")
                    Dim id As Integer
                    If Integer.TryParse(Console.ReadLine(), id) Then
                        ItemService.DeleteItem(id)
                    Else
                        ShowError("Invalid Item ID format.")
                    End If
                    PressAnyKeyToContinue()

                Case 6
                    Console.Clear()
                    ShowHeader("CREATE NEW BILL")
                    Try
                        Console.Write("Enter Customer Name: ")
                        Dim customerName As String = Console.ReadLine()
                        Dim billItems = UIHelper.GetBillItemsFromUser()

                        If billItems.Count > 0 Then
                            BillService.CreateBill(customerName, billItems)
                        Else
                            ShowWarning("No items were added to the bill.")
                        End If
                    Catch ex As Exception
                        ShowError($"Error creating bill: {ex.Message}")
                    End Try
                    PressAnyKeyToContinue()

                Case 7
                    Console.Clear()
                    ShowHeader("VIEW BILLS")
                    BillService.ViewBills()
                    PressAnyKeyToContinue()

                Case 8
                    Console.Clear()
                    ShowHeader("DELETE BILL")
                    Console.Write("Enter Bill ID to delete: ")
                    Dim billId As Integer
                    If Integer.TryParse(Console.ReadLine(), billId) Then
                        BillService.DeleteBill(billId)
                    Else
                        ShowError("Invalid Bill ID format.")
                    End If
                    PressAnyKeyToContinue()

                Case 9
                    Dim accountChoice As Integer
                    Do
                        Console.Clear()
                        ShowHeader("ACCOUNT SETTINGS")

                        Console.ForegroundColor = ConsoleColor.Yellow
                        Console.WriteLine($"Logged in as: {currentUsername}")
                        Console.WriteLine("──────────────────────────────")
                        Console.ResetColor()

                        Console.WriteLine("1. View Profile")
                        Console.WriteLine("2. Update Profile")
                        Console.WriteLine("3. Change Password")
                        Console.WriteLine("4. Back to Main Menu")
                        Console.WriteLine()

                        Console.Write("Enter your choice (1-4): ")

                        If Not Integer.TryParse(Console.ReadLine(), accountChoice) Then
                            ShowError("Invalid input")
                            PressAnyKeyToContinue()
                            Continue Do
                        End If

                        Try
                            Select Case accountChoice
                                Case 1
                                    UserService.ViewProfile(currentUsername)
                                    PressAnyKeyToContinue()

                                Case 2
                                    Console.Clear()
                                    ShowHeader("UPDATE PROFILE")

                                    Console.Write("Enter new first name: ")
                                    Dim newFname = Console.ReadLine()
                                    Console.Write("Enter new last name: ")
                                    Dim newLname = Console.ReadLine()
                                    Console.Write("Enter new email: ")
                                    Dim newEmail = Console.ReadLine()

                                    UserService.UpdateProfile(currentUsername, newFname, newLname, newEmail)
                                    ShowSuccess("Profile updated successfully!")
                                    PressAnyKeyToContinue()

                                Case 3
                                    Console.Clear()
                                    ShowHeader("CHANGE PASSWORD")

                                    Console.Write("Enter current password: ")
                                    Dim currentPass = GetMaskedPassword()
                                    Console.Write("Enter new password: ")
                                    Dim newPass = GetMaskedPassword()
                                    Console.Write("Confirm new password: ")
                                    Dim confirmPass = GetMaskedPassword()

                                    If newPass <> confirmPass Then
                                        ShowError("New passwords don't match!")
                                    Else
                                        UserService.ChangePassword(currentUsername, currentPass, newPass)
                                        ShowSuccess("Password changed successfully!")
                                    End If
                                    PressAnyKeyToContinue()

                                Case 4
                                    Exit Do

                                Case Else
                                    ShowError("Invalid choice")
                                    PressAnyKeyToContinue()
                            End Select
                        Catch ex As Exception
                            ShowError($"Error: {ex.Message}")
                            PressAnyKeyToContinue()
                        End Try
                    Loop While True

                Case 10 ' Exit
                    Console.WriteLine()
                    ShowInfo("Thank you for using the system. Goodbye!")
                    Exit Do

                Case Else
                    ShowError("Invalid choice. Please select 1-9.")
                    PressAnyKeyToContinue()
            End Select
        Loop
    End Sub

    ' Helper methods for UI
    Private Sub ShowHeader(title As String)
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine("╔════════════════════════════════════════╗")
        Console.WriteLine($"║    {title.PadRight(36)}║")
        Console.WriteLine("╚════════════════════════════════════════╝")
        Console.ResetColor()
        Console.WriteLine()
    End Sub

    Private Sub ShowSuccess(message As String)
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine(message)
        Console.ResetColor()
    End Sub

    Private Sub ShowError(message As String)
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine(message)
        Console.ResetColor()
    End Sub

    Private Sub ShowWarning(message As String)
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine(message)
        Console.ResetColor()
    End Sub

    Private Sub ShowInfo(message As String)
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine(message)
        Console.ResetColor()
    End Sub

    Private Sub PressAnyKeyToContinue()
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.DarkGray
        Console.Write("Press any key to continue...")
        Console.ResetColor()
        Console.ReadKey(True)
    End Sub

    Private Function GetMaskedPassword() As String
        Dim password As String = ""
        Dim key As ConsoleKeyInfo

        Do
            key = Console.ReadKey(True)

            If key.Key <> ConsoleKey.Backspace AndAlso key.Key <> ConsoleKey.Enter Then
                password += key.KeyChar
                Console.Write("*")
            Else
                If key.Key = ConsoleKey.Backspace AndAlso password.Length > 0 Then
                    password = password.Substring(0, (password.Length - 1))
                    Console.Write(vbBack & " " & vbBack)
                End If
            End If
        Loop Until key.Key = ConsoleKey.Enter

        Console.WriteLine()
        Return password
    End Function
End Module