Imports Npgsql

Public Module ItemService
    Public Sub ViewItems()
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()
            Dim cmd As New NpgsqlCommand("SELECT * FROM items ORDER BY id", conn)
            Dim reader = cmd.ExecuteReader()
            Console.WriteLine()
            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗")
            Console.WriteLine("║                     INVENTORY ITEMS                      ║")
            Console.WriteLine("╠══════╦══════════════════════╦═══════════╦════════════════╣")
            Console.WriteLine("║  ID  ║        Name          ║ Quantity  ║     Price      ║")
            Console.WriteLine("╠══════╬══════════════════════╬═══════════╬════════════════╣")
            Console.ResetColor()

            ' Table rows
            While reader.Read()
                Dim id = reader("id").ToString().PadRight(4)
                Dim name = TruncateString(reader("name").ToString(), 20).PadRight(20)
                Dim quantity = reader("quantity").ToString().PadRight(9)
                Dim price = FormatCurrency(reader("price")).PadRight(14)
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine($"║ {id} ║ {name} ║ {quantity} ║ {price} ║")
            End While

            ' Table footer
            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("╚══════╩══════════════════════╩═══════════╩════════════════╝")
            Console.ResetColor()
            Console.WriteLine()
        End Using
    End Sub

    Private Function TruncateString(value As String, maxLength As Integer) As String
        If value.Length <= maxLength Then
            Return value
        Else
            Return value.Substring(0, maxLength - 3) & "..."
        End If
    End Function

    Private Function FormatCurrency(amount As Object) As String
        Return String.Format("{0:N2} RWF", amount)
    End Function

    Public Sub ViewSingleItem(id As Integer)
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()
            Dim cmd As New NpgsqlCommand("SELECT * FROM items WHERE id = @id", conn)
            cmd.Parameters.AddWithValue("id", id)
            Dim reader = cmd.ExecuteReader()

            If reader.Read() Then
                ' Clear screen and show header
                Console.Clear()
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.WriteLine("╔════════════════════════════════════════╗")
                Console.WriteLine("║          ITEM DETAILS                  ║")
                Console.WriteLine("╚════════════════════════════════════════╝")
                Console.ResetColor()
                Console.WriteLine()

                ' Display item information in a box
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine($"│ {"ID:".PadRight(15)} {reader("id").ToString().PadRight(22)} │")
                Console.WriteLine("├────────────────────────────────────────┤")
                Console.WriteLine($"│ {"Name:".PadRight(15)} {TruncateString(reader("name").ToString(), 23).PadRight(22)} │")
                Console.WriteLine("├────────────────────────────────────────┤")
                Console.WriteLine($"│ {"Quantity:".PadRight(15)} {reader("quantity").ToString().PadRight(22)} │")
                Console.WriteLine("├────────────────────────────────────────┤")
                Console.WriteLine($"│ {"Price:".PadRight(15)} {FormatCurrency(reader("price")).PadRight(22)} │")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()

                ' Add some decorative elements
                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine("*".PadRight(40, "*"))
                Console.ResetColor()
            Else
                ' Error message for not found
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine()
                Console.WriteLine("╔════════════════════════════════════════╗")
                Console.WriteLine("║          ITEM NOT FOUND                ║")
                Console.WriteLine("╚════════════════════════════════════════╝")
                Console.WriteLine($" No item found with ID: {id}")
                Console.ResetColor()
            End If
        End Using
    End Sub

    Public Sub AddItem(name As String, qty As Integer, price As Decimal)
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()
            Dim cmd As New NpgsqlCommand("INSERT INTO items (name, quantity, price) VALUES (@n, @q, @p) RETURNING id", conn)
            cmd.Parameters.AddWithValue("n", name)
            cmd.Parameters.AddWithValue("q", qty)
            cmd.Parameters.AddWithValue("p", price)
            Dim newId = cmd.ExecuteScalar()
            Console.Clear()
            Console.ForegroundColor = ConsoleColor.Cyan
            Console.WriteLine("╔════════════════════════════════════════╗")
            Console.WriteLine("║          ITEM ADDED SUCCESSFULLY       ║")
            Console.WriteLine("╚════════════════════════════════════════╝")
            Console.ResetColor()
            Console.WriteLine()
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("┌────────────────────────────────────────┐")
            Console.WriteLine("│          NEW ITEM DETAILS              │")
            Console.WriteLine("├────────────────────────────────────────┤")
            Console.WriteLine($"│ {"ID:".PadRight(15)} {newId.ToString().PadRight(22)} │")
            Console.WriteLine("├────────────────────────────────────────┤")
            Console.WriteLine($"│ {"Name:".PadRight(15)} {TruncateString(name, 23).PadRight(22)} │")
            Console.WriteLine("├────────────────────────────────────────┤")
            Console.WriteLine($"│ {"Quantity:".PadRight(15)} {qty.ToString().PadRight(22)} │")
            Console.WriteLine("├────────────────────────────────────────┤")
            Console.WriteLine($"│ {"Price:".PadRight(15)} {price.ToString("C2").PadRight(22)} │")
            Console.WriteLine("└────────────────────────────────────────┘")
            Console.ResetColor()
            Console.WriteLine()
            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("*".PadRight(40, "*"))
            Console.ResetColor()
        End Using
    End Sub

    Public Sub UpdateItem(id As Integer, name As String, qty As Integer, price As Decimal)
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()
            Dim cmd As New NpgsqlCommand("UPDATE items SET name=@n, quantity=@q, price=@p WHERE id=@id", conn)
            cmd.Parameters.AddWithValue("id", id)
            cmd.Parameters.AddWithValue("n", name)
            cmd.Parameters.AddWithValue("q", qty)
            cmd.Parameters.AddWithValue("p", price)

            Dim rowsAffected = cmd.ExecuteNonQuery()

            Console.Clear()
            Console.ForegroundColor = ConsoleColor.Cyan
            Console.WriteLine("╔════════════════════════════════════════╗")
            Console.WriteLine("║          UPDATE ITEM RESULT            ║")
            Console.WriteLine("╚════════════════════════════════════════╝")
            Console.ResetColor()
            Console.WriteLine()

            If rowsAffected > 0 Then
                ' Success UI
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine("│          ITEM UPDATED SUCCESSFULLY     │")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()

                ' Show updated values
                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine(" Updated Item Details:")
                Console.WriteLine("───────────────────────")
                Console.ResetColor()
                Console.WriteLine($" ID:       {id}")
                Console.WriteLine($" Name:     {name}")
                Console.WriteLine($" Quantity: {qty}")
                Console.WriteLine($" Price: {price:N2} RWF")

                ' Decorative footer
                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine("*".PadRight(40, "*"))
                Console.ResetColor()
            Else
                ' Error UI
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine("│          ITEM NOT FOUND                │")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()

                Console.WriteLine()
                Console.WriteLine($" No item found with ID: {id}")
                Console.WriteLine(" Please verify the item ID and try again.")

                ' Decorative footer
                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine("*".PadRight(40, "*"))
                Console.ResetColor()
            End If
        End Using
    End Sub

    Public Sub DeleteItem(id As Integer)
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()

            ' First check if item exists in any bills
            Dim checkCmd As New NpgsqlCommand(
            "SELECT COUNT(*) FROM bill_items WHERE item_id = @id", conn)
            checkCmd.Parameters.AddWithValue("id", id)
            Dim billCount = Convert.ToInt32(checkCmd.ExecuteScalar())

            If billCount > 0 Then
                Console.Clear()
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("╔════════════════════════════════════════╗")
                Console.WriteLine("║         DELETE OPERATION FAILED         ║")
                Console.WriteLine("╚════════════════════════════════════════╝")
                Console.ResetColor()

                Console.WriteLine()
                Console.WriteLine("This item cannot be deleted because it appears in:")
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine($" {billCount} bill(s)")
                Console.ResetColor()

                Console.WriteLine()
                Console.WriteLine("Options:")
                Console.WriteLine("1. First delete the related bill items")
                Console.WriteLine("2. Keep this item in the system")

                Return
            End If

            Dim deleteCmd As New NpgsqlCommand("DELETE FROM items WHERE id=@id", conn)
            deleteCmd.Parameters.AddWithValue("id", id)

            Dim rowsAffected = deleteCmd.ExecuteNonQuery()
            Console.Clear()
            Console.ForegroundColor = ConsoleColor.Cyan
            Console.WriteLine("╔════════════════════════════════════════╗")
            Console.WriteLine("║          DELETE ITEM RESULT            ║")
            Console.WriteLine("╚════════════════════════════════════════╝")
            Console.ResetColor()

            If rowsAffected > 0 Then
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine("│          ITEM DELETED SUCCESSFULLY     │")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("┌────────────────────────────────────────┐")
                Console.WriteLine("│          ITEM NOT FOUND                │")
                Console.WriteLine("└────────────────────────────────────────┘")
                Console.ResetColor()
            End If
        End Using
    End Sub
End Module