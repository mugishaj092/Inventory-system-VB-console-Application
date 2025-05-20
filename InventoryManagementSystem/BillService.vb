Imports Npgsql
Imports System.Collections.Generic

Public Module BillService
    Public Sub CreateBill(customerName As String, billItems As List(Of Tuple(Of Integer, Integer, Decimal)))
        Dim totalAmount As Decimal = billItems.Sum(Function(item) item.Item2 * item.Item3)

        Using conn = DatabaseHelper.GetConnection()
            conn.Open()
            Using tran = conn.BeginTransaction()
                Try
                    Dim billCmd As New NpgsqlCommand("INSERT INTO bills (customer_name, total_amount) VALUES (@c, @t) RETURNING id", conn)
                    billCmd.Parameters.AddWithValue("c", customerName)
                    billCmd.Parameters.AddWithValue("t", totalAmount)
                    billCmd.Transaction = tran
                    Dim billId = Convert.ToInt32(billCmd.ExecuteScalar())

                    For Each item In billItems
                        Dim itemId = item.Item1
                        Dim qty = item.Item2
                        Dim price = item.Item3

                        Dim detailCmd As New NpgsqlCommand("INSERT INTO bill_items (bill_id, item_id, quantity, unit_price) VALUES (@b, @i, @q, @p)", conn)
                        detailCmd.Parameters.AddWithValue("b", billId)
                        detailCmd.Parameters.AddWithValue("i", itemId)
                        detailCmd.Parameters.AddWithValue("q", qty)
                        detailCmd.Parameters.AddWithValue("p", price)
                        detailCmd.Transaction = tran
                        detailCmd.ExecuteNonQuery()
                        Dim updateCmd As New NpgsqlCommand("UPDATE items SET quantity = quantity - @q WHERE id = @i", conn)
                        updateCmd.Parameters.AddWithValue("q", qty)
                        updateCmd.Parameters.AddWithValue("i", itemId)
                        updateCmd.Transaction = tran
                        updateCmd.ExecuteNonQuery()
                    Next

                    tran.Commit()
                    Console.WriteLine($"Bill created successfully. Total amount: {totalAmount}")
                Catch ex As Exception
                    tran.Rollback()
                    Console.WriteLine("Error creating bill: " & ex.Message)
                End Try
            End Using
        End Using
    End Sub

    Public Sub ViewBills()
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()
            Dim cmd As New NpgsqlCommand("SELECT * FROM bills ORDER BY id DESC", conn)
            Dim reader = cmd.ExecuteReader()

            Console.WriteLine()
            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗")
            Console.WriteLine("║                          BILL RECORDS                        ║")
            Console.WriteLine("╠══════╦══════════════════════╦═══════════════╦════════════════╣")
            Console.WriteLine("║  ID  ║    Customer Name     ║ Total Amount  ║     Date       ║")
            Console.WriteLine("╠══════╬══════════════════════╬═══════════════╬════════════════╣")
            Console.ResetColor()

            While reader.Read()
                Dim id = reader("id").ToString().PadRight(4)
                Dim customer = TruncateString(reader("customer_name").ToString(), 20).PadRight(20)
                Dim amount = FormatCurrency(reader("total_amount")).PadRight(13)
                Dim dateStr = FormatDateTime(reader("created_at"), DateFormat.ShortDate).PadRight(14)
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.WriteLine($"║ {id} ║ {customer} ║ {amount} ║ {dateStr} ║")
            End While
            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("╚══════╩══════════════════════╩═══════════════╩════════════════╝")
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
    Public Sub DeleteBill(billId As Integer)
        Using conn = DatabaseHelper.GetConnection()
            conn.Open()
            Using tran = conn.BeginTransaction()
                Try
                    ' First get bill details for confirmation message
                    Dim getBillCmd As New NpgsqlCommand(
                    "SELECT customer_name, total_amount, created_at FROM bills WHERE id = @billId", conn)
                    getBillCmd.Parameters.AddWithValue("billId", billId)
                    getBillCmd.Transaction = tran
                    Dim billReader = getBillCmd.ExecuteReader()

                    Dim customerName As String = ""
                    Dim totalAmount As Decimal = 0
                    Dim createdAt As DateTime = DateTime.Now

                    If billReader.Read() Then
                        customerName = billReader("customer_name").ToString()
                        totalAmount = Convert.ToDecimal(billReader("total_amount"))
                        createdAt = Convert.ToDateTime(billReader("created_at"))
                    End If
                    billReader.Close()

                    ' Delete bill_items linked to the bill first
                    Dim deleteItemsCmd As New NpgsqlCommand(
                    "DELETE FROM bill_items WHERE bill_id = @billId", conn)
                    deleteItemsCmd.Parameters.AddWithValue("billId", billId)
                    deleteItemsCmd.Transaction = tran
                    deleteItemsCmd.ExecuteNonQuery()

                    ' Delete the bill itself
                    Dim deleteBillCmd As New NpgsqlCommand(
                    "DELETE FROM bills WHERE id = @billId", conn)
                    deleteBillCmd.Parameters.AddWithValue("billId", billId)
                    deleteBillCmd.Transaction = tran
                    Dim rowsAffected = deleteBillCmd.ExecuteNonQuery()

                    Console.Clear()
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.WriteLine("╔════════════════════════════════════════╗")
                    Console.WriteLine("║          BILL DELETION RESULT          ║")
                    Console.WriteLine("╚════════════════════════════════════════╝")
                    Console.ResetColor()
                    Console.WriteLine()

                    If rowsAffected > 0 Then
                        tran.Commit()

                        ' Success UI
                        Console.ForegroundColor = ConsoleColor.Green
                        Console.WriteLine("┌────────────────────────────────────────┐")
                        Console.WriteLine("│          BILL DELETED SUCCESSFULLY     │")
                        Console.WriteLine("└────────────────────────────────────────┘")
                        Console.ResetColor()

                        ' Show deleted bill details
                        Console.WriteLine()
                        Console.ForegroundColor = ConsoleColor.Yellow
                        Console.WriteLine(" Deleted Bill Details:")
                        Console.WriteLine("───────────────────────")
                        Console.ResetColor()
                        Console.WriteLine($" Bill ID:    {billId}")
                        Console.WriteLine($" Customer:   {customerName}")
                        Console.WriteLine($" Total:      {totalAmount:C}")
                        Console.WriteLine($" Date:       {createdAt:dd-MMM-yyyy}")

                        ' Warning message
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                        Console.WriteLine()
                        Console.WriteLine(" All associated bill items were also deleted.")
                        Console.WriteLine(" This action cannot be undone.")
                        Console.ResetColor()
                    Else
                        tran.Rollback()

                        ' Error UI
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("┌────────────────────────────────────────┐")
                        Console.WriteLine("│            BILL NOT FOUND              │")
                        Console.WriteLine("└────────────────────────────────────────┘")
                        Console.ResetColor()

                        Console.WriteLine()
                        Console.WriteLine($" No bill found with ID: {billId}")
                        Console.WriteLine(" Possible reasons:")
                        Console.WriteLine(" - The bill was already deleted")
                        Console.WriteLine(" - You entered an incorrect ID")
                    End If

                    ' Decorative footer
                    Console.WriteLine()
                    Console.ForegroundColor = ConsoleColor.DarkGray
                    Console.WriteLine("*".PadRight(40, "*"))
                    Console.ResetColor()

                Catch ex As Exception
                    tran.Rollback()

                    ' Error UI
                    Console.Clear()
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("╔════════════════════════════════════════╗")
                    Console.WriteLine("║          DELETION FAILED               ║")
                    Console.WriteLine("╚════════════════════════════════════════╝")
                    Console.ResetColor()

                    Console.WriteLine()
                    Console.WriteLine(" An error occurred while deleting the bill:")
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Console.WriteLine($" {ex.Message}")
                    Console.ResetColor()

                    Console.WriteLine()
                    Console.WriteLine(" Please try again or contact support.")

                    ' Decorative footer
                    Console.WriteLine()
                    Console.ForegroundColor = ConsoleColor.DarkGray
                    Console.WriteLine("*".PadRight(40, "*"))
                    Console.ResetColor()
                End Try
            End Using
        End Using
    End Sub
End Module