Imports Npgsql

Public Module UIHelper
    Public Function GetBillItemsFromUser() As List(Of Tuple(Of Integer, Integer, Decimal))
        Dim billItems As New List(Of Tuple(Of Integer, Integer, Decimal))
        Dim continueAdding As Boolean = True

        While continueAdding
            Console.Write("Enter Item ID: ")
            Dim itemId = Convert.ToInt32(Console.ReadLine())

            Using conn = DatabaseHelper.GetConnection()
                conn.Open()
                Dim cmd As New NpgsqlCommand("SELECT * FROM items WHERE id=@id", conn)
                cmd.Parameters.AddWithValue("id", itemId)
                Dim reader = cmd.ExecuteReader()
                If reader.Read() Then
                    Dim itemName = reader("name").ToString()
                    Dim price = Convert.ToDecimal(reader("price"))
                    Dim availableQty = Convert.ToInt32(reader("quantity"))

                    Console.WriteLine($"Item: {itemName}, Price: {price}, Available: {availableQty}")
                    Console.Write("Enter Quantity: ")
                    Dim qty = Convert.ToInt32(Console.ReadLine())

                    If qty > availableQty Then
                        Console.WriteLine("Not enough stock.")
                    Else
                        billItems.Add(Tuple.Create(itemId, qty, price))
                    End If
                Else
                    Console.WriteLine("Item not found.")
                End If
            End Using

            Console.Write("Add more items? (y/n): ")
            continueAdding = Console.ReadLine().ToLower() = "y"
        End While

        Return billItems
    End Function
End Module