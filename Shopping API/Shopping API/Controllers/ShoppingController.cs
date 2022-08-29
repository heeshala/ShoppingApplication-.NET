using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
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

    public class ShoppingController : ControllerBase
    {
        public IConfiguration _configuration;


        public ShoppingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // Get all items
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select * from Product";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            try { 

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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            return new JsonResult(table);
        }

        // Get distinct categories
        [HttpGet]
        [Route("GetCategoryList")]
        public JsonResult GetCategoryList()
        {
            string query = @"select Distinct(Category) from Product";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            try { 
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return new JsonResult(table);
        }




        //Filter By Category
        [HttpGet]
        [Route("GetByCategory/{category}")]
        public JsonResult GetByCategory(string category)
        {
            string query = @"select * from Product where Category=@category";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            try { 
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@category", category);
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


        //Filter By Search
        [HttpGet]
        [Route("SearchProduct/{search}")]
        public JsonResult SearchProduct(string search)
        {
            string query = @"select * from Product where Category LIKE @search OR  Description LIKE @search OR  ProductName LIKE @search";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            try { 
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@search", "%"+search+"%");
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


        //Filter By Product Id
        [HttpGet]
        [Route("GetByProductId/{productId:int}")]
        public JsonResult GetByProductId(int productId)
        {
            string query = @"select * from Product where ProductId=@productId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ShoppingAppCon");
            SqlDataReader myReader;

            try { 

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@productId", productId);
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



     //Test Sending Email
        [HttpPost]
        [Route("SendEmail")]
        public void SendEmail()
        {
            string tablebody = "";
            string emailBody = "<html>\n    <head>\n<style>\ntable{margin-left: auto;margin-right: auto;border-collapse: collapse;border: 1px solid black;}    img {\n  display: block;\n  margin-left: auto;\n  margin-right: auto;\n  \n}\nbody{\n    font-family: 'Montserrat';\n}\n</style>\n<link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC\" crossorigin=\"anonymous\">\n<script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js\" integrity=\"sha384-MrcW6ZMFYlzcLA8Nl+NtUVF0sA7MsXsP1UyJoMp4YLEuNSfAP+JcXn/tWtIaxVXM\" crossorigin=\"anonymous\"></script>\n    </head>\n    <body>\n        <div class=\"container\">\n        <img src=\"https://firebasestorage.googleapis.com/v0/b/learnersapp-90354.appspot.com/o/logo.png?alt=media&token=83041d8d-dab4-4fc8-87e5-8c1ce33da728\" style=\"text-align: center;\" height=\"150vh\">\n        <h1 style=\"text-align: center;color:rgb(4, 62, 145)\">Order Confirmation</h1>\n         <br>\n        <table >\n            <thead>\n                <tr>\n                    <th style=\"text-align: center;border: 1px solid black;\">Product</th>\n                    <th style=\"text-align: center;border: 1px solid black;\">Quantity</th>\n                    <th style=\"text-align: center;border: 1px solid black;\">Unit Price (Rs.)</th>\n                    <th style=\"text-align: center;border: 1px solid black;\">Price (Rs.)</th>\n                </tr>\n            </thead>\n" +
                " <tbody> \n ";
                for(int a = 0; a < 7; a++)
            {
                tablebody = tablebody + "<tr style=\"text-align: center;border: 1px solid black;\">\n                <td style=\"text-align: center;border: 1px solid black;\" >{{dataItem.Product}}</td>\n                <td style=\"text-align: center;border: 1px solid black;\">{{dataItem.Quantity}}</td>\n\n                <td style=\"text-align: center;border: 1px solid black;\">{{dataItem.Price| number}}</td>\n                <td style=\"text-align: center;border: 1px solid black;\">{{dataItem.Price*dataItem.Quantity| number}}</td>\n                </tr>\n ";
            }
            emailBody=emailBody+tablebody+             "</tbody>\n        </table>\n\n        <br>\n        <div class=\"row\">\n   <div class=\"col-8\">\n    <h5>Delivery Address</h5>\n   </div>\n   <div class=\"col-4\" style=\"text-align: right;\">\n    <h5>Total Paid</h5>\n    <h3 style=\"color:rgb(4, 62, 145)\">Rs.6000</h3>\n</div>\n        </div>\n        \n    </div>\n    </body>\n</html>";


            try
            {
                MailMessage MailMessage = new MailMessage();
                MailMessage.From = new MailAddress("sendingemailaddress");
                MailMessage.To.Add("recepientaddress");
                MailMessage.Subject = "ShopperLK Order Recipt";
                MailMessage.Body = emailBody;
                MailMessage.IsBodyHtml = true;
                SmtpClient SmtpClient = new SmtpClient();
                SmtpClient.EnableSsl = true;
                SmtpClient.Host = "smtp.zoho.com";
                SmtpClient.Port = 587;
                SmtpClient.UseDefaultCredentials = false;
                SmtpClient.Credentials = new System.Net.NetworkCredential("sendingaddress", "password");
                SmtpClient.Send(MailMessage);
            }
            
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

}

    }
}
