using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;

namespace Shopping_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        public IConfiguration _configuration;


        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JsonResult Get()

        {
            DataTable table = new DataTable();

            try
            {
                string query = @"select * from Customer";

                
                string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
                SqlDataReader myReader;

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


                
            }catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return new JsonResult(table);
        }


        //Login Admin

        [HttpPost]
        [Route("AdminLogin")]
        public IActionResult AdminLogin([FromBody] Admin admin)
        {
            

            string passwordEn = Encryption.EncryptString(admin.Password);
            string query = @"select * from Admin where username = @username";

            DataTable table = new DataTable();

            try
            {
                string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@username", admin.Username);
                        myReader = myCommand.ExecuteReader();

                        if (myReader.HasRows)
                        {
                            while (myReader.Read())
                            {
                                if (myReader["Password"].ToString() == passwordEn)
                                {
                                    var token = GenerateJSONWebToken(admin.Username, "admin");
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
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Ok("User Do Not Exist");


        }


        //Token
        private string GenerateJSONWebToken(string uname, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,uname),
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
        [Route("AdminInformation")]
        [Authorize(Roles = "admin")]
        public JsonResult AdminInformation([FromBody] Admin admin)
        {
            string query = @"select * from Admin where username = @uname";

            DataTable table = new DataTable();
            try
            {
                string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@uname", admin.Username);
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


        //Get All Products     
        [HttpGet]
        [Route("AllProducts")]
        [Authorize(Roles = "admin")]
        public JsonResult AllProducts()
        {
            string query = @"select Product.ProductId AS ProductId, Product.ProductName As ProductName, Product.Quantity AS Quantity, Product.Category AS Category, Product.Price AS Price, Product.Image AS Image, Product.Description AS Description, Seller.SellerName As SellerName, Seller.SellerContact AS SellerContact from Product,Seller where Product.SellerId=Seller.SellerId";

            DataTable table = new DataTable();

            try
            {
                string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
                SqlDataReader myReader;

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

        //Get Seller List 
        [HttpGet]
        [Route("AllSellers")]
        [Authorize(Roles = "admin")]
        public JsonResult AllSellers()
        {
            string query = @"select* from Seller";

            DataTable table = new DataTable();

            try
            {

            
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

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


        [HttpPost]
        [Route("AddNewProduct")]
        [Authorize(Roles = "admin")]
        public int AddNewProduct([FromBody] Products product)
        {

            int result = 0;

            
            string query = @"insert into Product(ProductName,SellerId,Quantity,Category,Price,Image,Description) OUTPUT INSERTED.ProductId values (@name,@seller,@quantity,@category,@price,@image,@description) ";

            try
            {
                string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");


                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {

                        myCommand.Parameters.AddWithValue("@name", product.ProductName);
                        myCommand.Parameters.AddWithValue("@seller", product.SellerId);
                        myCommand.Parameters.AddWithValue("@quantity", product.Quantity);
                        myCommand.Parameters.AddWithValue("@category", product.Category);
                        myCommand.Parameters.AddWithValue("@price", product.Price);
                        myCommand.Parameters.AddWithValue("@image", "defaultimage");
                        myCommand.Parameters.AddWithValue("@description", product.Description);
                        result = (int)myCommand.ExecuteScalar();



                        myCon.Close();
                    }

                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;


        }


        //Update Image
        [HttpPost, DisableRequestSizeLimit]
        [Route("UpdateImage/{id:int}")]
        [Authorize(Roles = "admin")]
        public int UpdateImage(int id)
        {
            int result = 0;


            try
            {
                var file = Request.Form.Files[0];

                var pathToSave = "C:\\E\\Virtusa\\Assignment\\Website\\ShoppingApplication\\src\\assets\\products";
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fileExt =Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(pathToSave, id.ToString()+fileExt);
                    //var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    
                    //Update SQL
                    string query = @"Update Product set Image=@image where ProductId=@productId ";


                    string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
                    string imgPath = "..//assets//products//" + id.ToString() + fileExt;

                    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                    {
                        myCon.Open();

                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {

                            myCommand.Parameters.AddWithValue("@image", fullPath);
                            myCommand.Parameters.AddWithValue("@productId", id);
                            
                            result = myCommand.ExecuteNonQuery();



                            myCon.Close();
                        }

                    }

                    return result;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 200;
            }

            


        }


        //Update Image
        [HttpPost]
        [Route("UpdateStock")]
        [Authorize(Roles = "admin")]
        public int UpdateStock([FromBody] Products product)
        {
            int result = 0;


            try
            {
                


                    //Update SQL
                    string query = @"Update Product set Quantity=@newQuantity where ProductId=@productId ";


                    string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
                    

                    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                    {
                        myCon.Open();

                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {

                            myCommand.Parameters.AddWithValue("@newQuantity", product.Quantity);
                            myCommand.Parameters.AddWithValue("@productId", product.ProductId);

                            result = myCommand.ExecuteNonQuery();



                            myCon.Close();
                        }

                    }

                    return result;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 200;
            }




        }

    }
}
