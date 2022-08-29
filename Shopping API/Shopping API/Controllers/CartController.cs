using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shopping_API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shopping_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        public IConfiguration _configuration;


        public CartController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Get cart items list
        [HttpGet]
        [Route("GetCartItems/{id:int}")]
        [Authorize(Roles = "customer")]
        public JsonResult GetCartItems(int id)
        {
            string query = @"select Cart.ID AS Id,Cart.ProductId As ProductId,Cart.Quantity as Quantity,Cart.CustomerId as CustomerId,Product.Price AS UnitPrice,Product.ProductName, Product.Image,Product.Quantity As MaximumAvailable  from Cart,Product where Cart.CustomerId=@customerId AND Cart.ProductId=Product.ProductId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@customerId", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }

            }

            return new JsonResult(table);
        }


        // Get cart items edit info
        [HttpGet]
        [Route("GetEditInfo/{cartItemId:int}")]
        [Authorize(Roles = "customer")]
        public JsonResult GetEditInfo(int cartItemId)
        {
            string query = @"select Cart.ID AS Id,Cart.ProductId As ProductId,Cart.Quantity as Quantity,Cart.CustomerId as CustomerId,Product.Price AS UnitPrice,Product.ProductName,Product.Quantity As MaximumAvailable  from Cart,Product where Cart.ID=@cartItemId AND Cart.ProductId=Product.ProductId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@cartItemId", cartItemId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }

            }

            return new JsonResult(table);
        }


        // Check Item Duplication
        [HttpPost]
        [Route("CheckItemDuplication")]
        [Authorize(Roles = "customer")]
        public JsonResult CheckItemDuplication([FromBody] Items item)
        {
            string query = @"select * from Cart where ProductId=@productId AND CustomerId=@customer";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@productId", item.ProductId);
                        myCommand.Parameters.AddWithValue("@customer", item.CustomerId);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();
                    }

                }
            }catch(Exception ex)
            {
                Console.Write(ex);
            }

            return new JsonResult(table);
        }

        // Add Cart Items
        [HttpPost]
        [Route("AddCartItems")]
        [Authorize(Roles = "customer")]
        public int AddCartItems([FromBody] Items item)
        {
            int result = 0;
            string query = @"insert into Cart(ProductId,Quantity,CustomerId) values (@productId,@quantity,@customerId)";

            
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@productId", item.ProductId);
                        myCommand.Parameters.AddWithValue("@quantity", item.Quantity);
                        myCommand.Parameters.AddWithValue("@customerId", item.CustomerId);
                        

                        result = myCommand.ExecuteNonQuery();
                        myCon.Close();
                    }

                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }




        // Update Cart Quantity
        [HttpPost]
        [Route("UpdateItem")]
        [Authorize(Roles = "customer")]
        public int UpdateItem([FromBody] Items item)
        {
            int result = 0;
            string query = @"update Cart set Quantity=@newQuantity where ID=@itemId";

            
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@itemId", item.ItemsId);
                        myCommand.Parameters.AddWithValue("@newQuantity", item.Quantity);

                        result = myCommand.ExecuteNonQuery();
                        myCon.Close();
                    }

                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }




        // Delete Item
        [HttpDelete]
        [Route("DeleteItem/{id:int}")]
        [Authorize(Roles = "customer")]
        public int DeleteItem(int id)
        {
            int result = 0;
            string query = @"delete from Cart where ID=@itemId";

            
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            try
            {

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@itemId", id);


                        result = myCommand.ExecuteNonQuery();
                        myCon.Close();
                    }

                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }


        //Purchase
        [HttpPost]
        [Route("Purchase")]
        [Authorize(Roles = "customer")]
        public int Purchase([FromBody] Purchases purchase)
        {
            int returnStatus = 0;
            double TotalPrice = purchase.TotalPrice;
            int lastId;
            var result = "-1";

            //Payment 

            ServicesContainer.ConfigureService(new GpEcomConfig
            {
                MerchantId = "dev266848426534915232",
                AccountId = "internet",
                SharedSecret = "UUV2RkpMfD",
                ServiceUrl = "https://api.sandbox.realexpayments.com/epage-remote.cgi"
            });

            try
            {
                var card = new CreditCardData
                {
                    Number = purchase.CardNumber,
                    ExpMonth = int.Parse(purchase.ExpMonth),
                    ExpYear = int.Parse(purchase.ExpYear),
                    Cvn = purchase.Cvn,
                    CardHolderName = purchase.CardHolderName
                };

                /*
                 card.setNumber("4263970000005262");
card.setExpMonth(12);
card.setExpYear(2025);
card.setCvn("131");
                 */

                // process an auto-capture authorization
                GlobalPayments.Api.Entities.Transaction response = card.Charge(decimal.Parse(purchase.TotalPrice.ToString()))
                       .WithCurrency("USD")
                       .Execute();

                result = response.ResponseCode; // 00 == Success
                var message = response.ResponseMessage; // [ test system ] AUTHORISED


            }
            catch (BuilderException ex)
            {
                Console.WriteLine(ex);
                returnStatus = 0;
            }
            catch (GatewayException ex)

            {
                Console.WriteLine(ex);
                returnStatus = 0;
            }
            catch (ApiException ex)
            {
                Console.WriteLine(ex);
                returnStatus = 0;
            }


            //If payment Success
            if (result == "00")
            {
                returnStatus = 1;
                //Get List of Purchasing products
                string query = @"select Cart.ProductId AS ProductId,Cart.Quantity AS Quantity,Product.Price AS Price from Cart,Product where CustomerId=@customerId AND Cart.ProductId=Product.ProductId";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
                SqlDataReader myReader;
                try
                {

                    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                    {
                        myCon.Open();

                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                            myCommand.Parameters.AddWithValue("@customerId", purchase.CustomerId);
                            myReader = myCommand.ExecuteReader();
                            table.Load(myReader);

                            myReader.Close();
                            myCon.Close();
                        }

                    }
                }catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }

                IList<Items> items = table.AsEnumerable().Select(row =>
        new Items
        {
            ProductId = row.Field<int>("ProductId"),
            Quantity = row.Field<int>("Quantity"),
            Price = row.Field<decimal>("Price"),
        }).ToList();


                //Add To Orders
                string orderQuery = @"insert into Orders(CustomerId,ItemsCount,DateOfPurchase,PaidAmount,Address,City,Zip) OUTPUT INSERTED.OrderId values (@customer,@items,@date,@amount,@address,@city,@zip)"; 
                          

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(orderQuery, myCon))
                    {

                        myCommand.Parameters.AddWithValue("@customer", purchase.CustomerId);
                        myCommand.Parameters.AddWithValue("@items", items.Count);
                        myCommand.Parameters.AddWithValue("@date", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
                        myCommand.Parameters.AddWithValue("@amount", Math.Round(TotalPrice, 2));
                        myCommand.Parameters.AddWithValue("@address", purchase.Address);
                        myCommand.Parameters.AddWithValue("@city", purchase.City);
                        myCommand.Parameters.AddWithValue("@zip", purchase.Zip);
                        lastId = (int)myCommand.ExecuteScalar();



                        myCon.Close();
                    }

                }

                //Record Items Information of the Order
                for (int a = 0; a < items.Count; a++)
                {
                    string itemQuery = @"insert into Item(OrderId,Quantity,Price,ProductId) values (@orderId,@quantity,@price,@productId)";

                    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                    {
                        myCon.Open();

                        using (SqlCommand myCommand = new SqlCommand(itemQuery, myCon))
                        {

                            myCommand.Parameters.AddWithValue("@orderId", lastId);
                            myCommand.Parameters.AddWithValue("@quantity", items[a].Quantity);
                            myCommand.Parameters.AddWithValue("@price", items[a].Price);
                            myCommand.Parameters.AddWithValue("@productId", items[a].ProductId);

                            myCommand.ExecuteNonQuery();



                            myCon.Close();
                        }

                    }
                }

                //Reduce Stocks
                for (int a = 0; a < items.Count; a++)
                {
                    string itemQuery = @"Update Product set Quantity=Quantity-@quantity where ProductId=@productId";

                    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                    {
                        myCon.Open();

                        using (SqlCommand myCommand = new SqlCommand(itemQuery, myCon))
                        {

                            
                            myCommand.Parameters.AddWithValue("@quantity", items[a].Quantity);
                           
                            myCommand.Parameters.AddWithValue("@productId", items[a].ProductId);

                            myCommand.ExecuteNonQuery();



                            myCon.Close();
                        }

                    }
                }

                //Clear Cart
                string clearCartQuery = @"delete from Cart where CustomerId=@customer";

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(clearCartQuery, myCon))
                    {

                        myCommand.Parameters.AddWithValue("@customer", purchase.CustomerId);


                        myCommand.ExecuteNonQuery();



                        myCon.Close();
                    }

                }


                //Send Email
                //Get List of Purchasing products
                string invoiceQuery = @"select Item.ItemId AS ItemId, Product.ProductName AS Product,Customer.Name As CustomerName,Customer.Email AS Email,Item.Quantity AS Quantity, Item.Price AS Price, Orders.DateOfPurchase AS OrderDate, Orders.Address As Address,Orders.City AS City,Orders.Zip As Zip from Product,Item,Orders,Customer  where Orders.OrderId=@orderId AND Item.OrderId=Orders.OrderId AND Product.ProductId=Item.ProductId AND Customer.CustomerId=Orders.CustomerId";

                DataTable invoicetable = new DataTable();
                string sqlDataSource2 = _configuration.GetConnectionString("ShoppingAppCon");
                SqlDataReader myReader2;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource2))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(invoiceQuery, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@orderId", lastId);
                        myReader2 = myCommand.ExecuteReader();
                        invoicetable.Load(myReader2);

                        myReader2.Close();
                        myCon.Close();
                    }

                }

                

                IList<Invoice> invoice = invoicetable.AsEnumerable().Select(row =>
        new Invoice
        {
           
            Quantity = row.Field<int>("Quantity"),
            Price = row.Field<decimal>("Price"),
            ProductName = row.Field<string>("Product"),
            Email = row.Field<string>("Email"),
        }).ToList();




                try { 
                string tablebody = "";
                string emailBody = "<html>\n    <head>\n<style>\n    img {\n  display: block;\n  margin-left: auto;\n  margin-right: auto;\n  \n}\nbody{\n    font-family: 'Montserrat';\n} table{margin-left: auto;margin-right: auto;border-collapse: collapse;border: 1px solid black;}\n</style>\n<link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC\" crossorigin=\"anonymous\">\n<script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js\" integrity=\"sha384-MrcW6ZMFYlzcLA8Nl+NtUVF0sA7MsXsP1UyJoMp4YLEuNSfAP+JcXn/tWtIaxVXM\" crossorigin=\"anonymous\"></script>\n    </head>\n    <body>\n        <div class=\"container\">\n        <img src=\"https://firebasestorage.googleapis.com/v0/b/learnersapp-90354.appspot.com/o/logo.png?alt=media&token=83041d8d-dab4-4fc8-87e5-8c1ce33da728\" style=\"text-align: center;\" height=\"150vh\">\n        <h1 style=\"text-align: center;color:rgb(4, 62, 145)\">Order Confirmation</h1>\n         <br>\n <h5>Order Number " + lastId.ToString()+ "</h5><br>       <table >\n            <thead>\n                <tr>\n                    <th style=\"text-align: center;border: 1px solid black;\">Product</th>\n                    <th style=\"text-align: center;border: 1px solid black;\">Quantity</th>\n                    <th style=\"text-align: center;border: 1px solid black;\">Unit Price (Rs.)</th>\n                    <th style=\"text-align: center;border: 1px solid black;\">Price (Rs.)</th>\n                </tr>\n            </thead>\n" +
                    " <tbody> \n ";
                for (int a = 0; a < invoice.Count; a++)
                {
                    tablebody = tablebody + "<tr style=\"text-align: center;border: 1px solid black;\">\n                <td style=\"text-align: center;border: 1px solid black;\">" + invoice[a].ProductName+ "</td>\n                <td style=\"text-align: center;border: 1px solid black;\">" + invoice[a].Quantity.ToString() + "</td>\n\n                <td style=\"text-align: center;border: 1px solid black;\">" + invoice[a].Price.ToString() + "</td>\n                <td style=\"text-align: center;border: 1px solid black;\">" + (invoice[a].Price * invoice[a].Quantity).ToString() + "</td>\n                </tr>\n ";
                }
                emailBody = emailBody + tablebody + "</tbody>\n        </table>\n\n        <br>\n        <div class=\"row\">\n   <div class=\"col-8\">\n    <h5>Delivery Address</h5>\n <br>"+purchase.Address+"<br>"+purchase.City+"<br>"+purchase.Zip+"  </div>\n   <div class=\"col-4\" style=\"text-align: right;\">\n    <h5>Total Paid</h5>\n    <h3 style=\"color:rgb(4, 62, 145)\">Rs."+TotalPrice.ToString()+"</h3>\n</div>\n        </div>\n        \n    </div>\n    </body>\n</html>";

                
                    MailMessage MailMessage = new MailMessage();
                    MailMessage.From = new MailAddress("senderemailaddress");
                    MailMessage.To.Add(invoice[0].Email);
                    MailMessage.Subject = "ShopperLK Order Recipt";
                    MailMessage.Body = emailBody;
                    MailMessage.IsBodyHtml = true;
                    SmtpClient SmtpClient = new SmtpClient();
                    SmtpClient.EnableSsl = true;
                    SmtpClient.Host = "smtp.zoho.com";
                    SmtpClient.Port = 587;
                    SmtpClient.UseDefaultCredentials = false;
                    SmtpClient.Credentials = new System.Net.NetworkCredential("senderemailaddress", "password");
                    SmtpClient.Send(MailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }

            




            return returnStatus;
        }

    }
}
