using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MyBaseOnC
{
    class Program
    {
        private static string _connectionString = @"Data Source=OSV01\SQLEXPRESS; Initial Catalog=customer_order; Pooling=true; Integrated Security=SSPI";
        static void Main( string[] args )
        {
            if ( args.Length < 1 )
            {
                Console.WriteLine( "Not enough arguments" );
                return;
            }

            string command = args[0];

            if ( command == "readorder" )
            {
                Console.WriteLine( "command: readorder" );
                List<Order> orders = ReadOrder();
                foreach ( Order order in orders )
                {
                    Console.WriteLine( order.OrderId );
                    Console.WriteLine( order.ProductName );
                    Console.WriteLine( order.Price );
                    Console.WriteLine( order.CustomerId );
                }
            }

            if ( command == "readcustomer" )
            {
                Console.WriteLine( "command: readcustomer" );
                List<Customer> customers = ReadCustomer();
                foreach ( Customer customer in customers )
                {
                    Console.WriteLine( customer.CustomerId );
                    Console.WriteLine( customer.Name );
                    Console.WriteLine( customer.City );
                }
            }

            if ( command == "insertorder" )
            {
                int createdOrderId = InsertOrder( 4, "Булки", 60 );
                Console.WriteLine( "Created order: " + createdOrderId );
            }

            if ( command == "insertcustomer" )
            {
                int createdCustomerId = InsertCustomer( "Артем Корнилов", "Медведево" );
                Console.WriteLine( "Created customer: " + createdCustomerId );
            }

            if ( command == "updateorder" )
            {
                UpdateOrder( 8, "Беляш", 70 );
                Console.WriteLine( "Update order complete" );
            }

            if ( command == "updatecustomer" )
            {
                UpdateCustomer( 4, "Артем Сергеевич Корнилов", "Йошкар-Ола" );
                Console.WriteLine( "Update customer complete" );
            }

            if ( command == "deleteorder" )
            {
                DeleteOrder( 8 );
                Console.WriteLine( "Delete order complete" );
            }

            if ( command == "deletecustomer" )
            {
                DeleteCustomer( 4 );
                Console.WriteLine( "Delete customer complete" );
            }

            if ( command == "countcustomers" )
            {
                int count = CustomersCount();
                Console.WriteLine( "Count customers: " + count );
            }

            if ( command == "countorders" )
            {
                int count = OrdersCount();
                Console.WriteLine( "Count orders: " + count );
            }

            if ( command == "sumorders" )
            {
                int sum = SumOrders();
                Console.WriteLine( "Sum orders: " + sum );
            }
        }

        private static List<Order> ReadOrder()
        {
            List<Order> orders = new List<Order>();
            using ( SqlConnection connection = new SqlConnection( _connectionString ) )
            {
                connection.Open();
                using ( SqlCommand command = new SqlCommand() )
                {
                    command.Connection = connection;
                    command.CommandText =
                        @"SELECT
                            [OrderId],
                            [ProductName],
                            [Price],
                            [CustomerId]
                        FROM [Order]";

                    using ( SqlDataReader reader = command.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            var order = new Order
                            {
                                OrderId = Convert.ToInt32( reader[ "OrderId" ] ),
                                ProductName = Convert.ToString( reader[ "ProductName" ] ),
                                Price = Convert.ToInt32( reader[ "Price" ] ),
                                CustomerId = Convert.ToInt32( reader[ "CustomerId" ] ),
                            };
                            orders.Add( order );
                        }
                    }
                }
            }
            return orders;
        }

        private static List<Customer> ReadCustomer()
        {
            List<Customer> customers = new List<Customer>();
            using ( SqlConnection connection = new SqlConnection( _connectionString ) )
            {
                connection.Open();
                using ( SqlCommand command = new SqlCommand() )
                {
                    command.Connection = connection;
                    command.CommandText =
                        @"SELECT
                            [CustomerId],
                            [Name],
                            [City]
                        FROM [Customer]";

                    using ( SqlDataReader reader = command.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            var customer = new Customer
                            {
                                CustomerId = Convert.ToInt32( reader[ "CustomerId"] ),
                                Name = Convert.ToString( reader[ "Name" ] ),
                                City = Convert.ToString( reader[ "City" ] ),
                            };
                            customers.Add( customer );
                        }
                    }
                }
            }
            return customers;
        }

        private static int InsertOrder( int customerId, string productName, int price )
        {

            using ( SqlConnection connection = new SqlConnection( _connectionString ) )
            {
                connection.Open();
                using ( SqlCommand cmd = connection.CreateCommand() )
                {
                    cmd.CommandText = @"
                    INSERT INTO [Order]
                        ([ProductName], 
                         [Price], 
                         [CustomerId])
                    VALUES 
                       (@productName,
                        @price,
                        @customerId)
                    SELECT SCOPE_IDENTITY()";

                    cmd.Parameters.Add( "@productName", SqlDbType.NVarChar ).Value = productName;
                    cmd.Parameters.Add( "@price", SqlDbType.Int ).Value = price;
                    cmd.Parameters.Add( "@customerId", SqlDbType.Int ).Value = customerId;

                    return Convert.ToInt32( cmd.ExecuteScalar() );
                }
            }
        }

        private static int InsertCustomer( string name, string city )
        {

            using ( SqlConnection connection = new SqlConnection( _connectionString ) )
            {
                connection.Open();
                using ( SqlCommand cmd = connection.CreateCommand() )
                {
                    cmd.CommandText = @"
                    INSERT INTO [Customer]
                        ([Name], 
                         [City])
                    VALUES 
                       (@name,
                        @city)
                    SELECT SCOPE_IDENTITY()";

                    cmd.Parameters.Add( "@name", SqlDbType.NVarChar ).Value = name;
                    cmd.Parameters.Add( "@city", SqlDbType.NVarChar ).Value = city;

                    return Convert.ToInt32( cmd.ExecuteScalar() );
                }
            }
        }

        private static void UpdateOrder( int orderId, string productName, int price )
        {
            using ( SqlConnection connection = new SqlConnection( _connectionString ) )
            {
                connection.Open();
                using ( SqlCommand command = connection.CreateCommand() )
                {
                    command.CommandText = @"
                        UPDATE [Order]
                        SET 
                        [ProductName] = @productName,
                        [Price] = @price
                        WHERE OrderId = @orderId";

                    command.Parameters.Add( "@orderId", SqlDbType.Int ).Value = orderId;
                    command.Parameters.Add( "@productName", SqlDbType.NVarChar ).Value = productName;
                    command.Parameters.Add( "@price", SqlDbType.Int ).Value = price;

                    command.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateCustomer( int customerId, string name, string city )
        {
            using ( SqlConnection connection = new SqlConnection( _connectionString ) )
            {
                connection.Open();
                using ( SqlCommand command = connection.CreateCommand() )
                {
                    command.CommandText = @"
                        UPDATE [Customer]
                        SET 
                        [Name] = @name,
                        [City] = @city
                        WHERE CustomerId = @customerId";

                    command.Parameters.Add( "@customerId", SqlDbType.Int ).Value = customerId;
                    command.Parameters.Add( "@name", SqlDbType.NVarChar ).Value = name;
                    command.Parameters.Add( "@city", SqlDbType.NVarChar ).Value = city;

                    command.ExecuteNonQuery();
                }
            }
        }

        private static void DeleteOrder( int orderId )
        {
            using ( SqlConnection connection = new SqlConnection( _connectionString ) )
            {
                connection.Open();
                using ( SqlCommand command = connection.CreateCommand() )
                {
                    command.CommandText = @"
                        DELETE FROM [Order]
                        WHERE OrderId = @orderId";

                    command.Parameters.Add( "@orderId", SqlDbType.Int ).Value = orderId;

                    command.ExecuteNonQuery();
                }
            }
        }

        private static void DeleteCustomer( int customerId )
        {
            using ( SqlConnection connection = new SqlConnection( _connectionString ) )
            {
                connection.Open();
                using ( SqlCommand command = connection.CreateCommand() )
                {
                    command.CommandText = @"
                        DELETE FROM [Customer]
                        WHERE CustomerId = @customerId";

                    command.Parameters.Add( "@customerId", SqlDbType.Int ).Value = customerId;

                    command.ExecuteNonQuery();
                }
            }
        }

        private static int CustomersCount()
        {
            int count = 0;
            using ( SqlConnection connection = new SqlConnection(_connectionString) )
            {
                connection.Open();
                using ( SqlCommand command = connection.CreateCommand() )
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT COUNT(*) as count FROM [Customer]";

                    using ( SqlDataReader reader = command.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            count = Convert.ToInt32( reader["count"] );
                        }
                    }
                }
            }
            return count;
        }

        private static int OrdersCount()
        {
            int count = 0;
            using ( SqlConnection connection = new SqlConnection(_connectionString) )
            {
                connection.Open();
                using ( SqlCommand command = connection.CreateCommand() )
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT COUNT(*) as count FROM [Order]";

                    using ( SqlDataReader reader = command.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            count = Convert.ToInt32( reader["count"] );
                        }
                    }
                }
            }
            return count;
        }

        private static int SumOrders()
        {
            int sum = 0;
            using ( SqlConnection connection = new SqlConnection( _connectionString ) )
            {
                connection.Open();
                using ( SqlCommand command = connection.CreateCommand() )
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT SUM(Price) as sum FROM [Order]";

                    using ( SqlDataReader reader = command.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            sum = Convert.ToInt32( reader["sum"] );
                        }
                    }
                }
            }
            return sum;
        }
    }
}
