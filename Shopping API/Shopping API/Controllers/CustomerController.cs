using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shopping_API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shopping_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        public  IConfiguration _configuration;

        
        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        
        //Get All Users
        [HttpGet]
        
        public JsonResult Get()
        {
            string query = @"select * from Customer";

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

            return new JsonResult(table);
        }

       

        //Register New User
        [HttpPost]
        [Route("CustomerRegistration")]
        public int CustomerRegistration([FromBody] Customers customer)
        {
            int result = 0;

            string passwordEn = Encryption.EncryptString(customer.Password);
            string query = @"insert into Customer(Name,Address,Contact,Email,DOB,Password) values (@name,@address,@contact,@email,@dob,@password) ";

            
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {

                        myCommand.Parameters.AddWithValue("@name", customer.Name);
                        myCommand.Parameters.AddWithValue("@address", customer.Address);
                        myCommand.Parameters.AddWithValue("@contact", customer.Contact);
                        myCommand.Parameters.AddWithValue("@email", customer.Email);
                        myCommand.Parameters.AddWithValue("@dob", customer.DOB);
                        myCommand.Parameters.AddWithValue("@password", passwordEn);
                        result = myCommand.ExecuteNonQuery();



                        myCon.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



            return result;
        }


        //Login
        [HttpPost]
        [Route("CustomerLogin")]
        public IActionResult CustomerLogin([FromBody] Customers customer)
        {
            

            string passwordEn = Encryption.EncryptString(customer.Password);
            string query = @"select * from Customer where Email = @email";

           
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            try
            {

            
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@email", customer.Email);
                    myReader = myCommand.ExecuteReader();

                    if (myReader.HasRows)
                    {
                        while (myReader.Read())
                        {
                            if (myReader["Password"].ToString() == passwordEn)
                            {
                                var token = GenerateJSONWebToken(customer.Email, "customer");
                                return Ok(token);
                            }
                            else
                            {
                                return Ok("User Credentials Do Not Match"); 
                            }
                        }
                    }
                    

                    myReader.Close();
                    myCon.Close();
                }

            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            return Ok("User Do Not Exist");
        
            
        }


        //Token
        private string GenerateJSONWebToken(string email,string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,email),
                new Claim(ClaimTypes.Role,role),
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //Get User Information
        [HttpPost]
        [Route("UserInformation")]
        [Authorize(Roles = "customer")]
        public JsonResult UserInformation([FromBody] Customers customers)
        {
            string query = @"select * from Customer where Email = @email";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;
            try { 
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@email", customers.Email);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }

            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            return new JsonResult(table);
        }



        //Get Customer Orders       
        [HttpGet]
        [Route("CustomerOrders/{id:int}")]
        [Authorize(Roles = "customer")]
        public JsonResult CustomerOrders(int id)
        {
            string query = @"select * from Orders where CustomerId=@customerId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            try { 

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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            return new JsonResult(table);
        }


        //Get Order Invoice Orders
        [HttpGet]
        [Route("OrderInvoice/{id:int}")]
        [Authorize(Roles="customer")]
        public JsonResult OrderInvoice(int id)
        {
            string query = @"select Product.ProductName AS Product,Item.Quantity AS Quantity, Item.Price AS Price, Orders.DateOfPurchase AS OrderDate, Orders.Address As Address,Orders.City AS City,Orders.Zip As Zip from Product,Item,Orders  where Orders.OrderId=@orderId AND Item.OrderId=Orders.OrderId AND Product.ProductId=Item.ProductId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            try { 
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@orderId", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }

            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            return new JsonResult(table);
        }


        //Customer Info
        [HttpGet]
        [Route("CustomerInformation/{customerId:int}")]
        [Authorize(Roles = "customer")]
        public JsonResult CustomerInformation(int customerId)
        {
            string query = @"select * from Customer where CustomerId = @id";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            try { 

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", customerId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }

            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            return new JsonResult(table);
        }


        //ChangePassword
        [HttpPost]
        [Route("ChangePassword")]
        [Authorize(Roles = "customer")]
        public int ChangePassword([FromBody] Customers customer)
        {
            int status = 0;

            string passwordEn = Encryption.EncryptString(customer.Password);
            string query = @"update Customer Set Password=@password where CustomerId = @id";

            
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            try { 
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", customer.CustomerId);
                    myCommand.Parameters.AddWithValue("@password", passwordEn);
                    status = myCommand.ExecuteNonQuery();
                    

                    
                    myCon.Close();
                }

            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return status;
        }

    }
}
