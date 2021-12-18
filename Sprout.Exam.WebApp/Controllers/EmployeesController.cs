using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Sprout.Exam.Business.DataTransferObjects;
using Sprout.Exam.Common.Enums;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace Sprout.Exam.WebApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public EmployeesController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }


        /// <summary>
        /// Refactor this method to go through proper layers and fetch from the DB.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string query = @"SELECT 
                            [Id]
                          ,[FullName]
                          ,[Birthdate]
                          ,[TIN]
                          ,[EmployeeTypeId]
                          ,[IsDeleted]
                          ,[Rate]
                        FROM [dbo].[Employee] where [IsDeleted] = 0
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
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
            List<EmployeeDto> ResultList = new List<EmployeeDto>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                EmployeeDto emp = new EmployeeDto();
                emp.Id = Convert.ToInt32(table.Rows[i]["Id"]);
                emp.FullName = table.Rows[i]["FullName"].ToString();
                emp.Birthdate = Convert.ToDateTime(table.Rows[i]["Birthdate"].ToString()).ToShortDateString();
                emp.Tin = table.Rows[i]["TIN"].ToString();
                emp.TypeId = Convert.ToInt32(table.Rows[i]["EmployeeTypeId"]);
                emp.Rate = float.Parse(table.Rows[i]["Rate"].ToString());
                ResultList.Add(emp);
            }
            var result = await Task.FromResult(ResultList);
            return Ok(result);
        }

        /// <summary>
        /// Refactor this method to go through proper layers and fetch from the DB.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            string query = @"SELECT 
                            [Id]
                          ,[FullName]
                          ,[Birthdate]
                          ,[TIN]
                          ,[EmployeeTypeId]
                          ,[IsDeleted]
                          ,[Rate]
                        FROM [dbo].[Employee] where [Id] = @Identifier
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Identifier", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

           EmployeeDto emp = new EmployeeDto();
            if (table.Rows.Count > 0)
            {
                emp.Id = Convert.ToInt32(table.Rows[0]["Id"]);
                emp.FullName = table.Rows[0]["FullName"].ToString();
                emp.Birthdate = String.Format("{0:MM/dd/yyyy}", table.Rows[0]["Birthdate"].ToString());  
                emp.Tin = table.Rows[0]["TIN"].ToString();
                emp.TypeId = Convert.ToInt32(table.Rows[0]["EmployeeTypeId"]);
                emp.Rate = float.Parse(table.Rows[0]["Rate"].ToString());
                return Ok(emp);
            }
            else
            {
                return Ok(null);
            }
          
           // var result = await Task.FromResult(emp);
            //var result = await Task.FromResult(StaticEmployees.ResultList.FirstOrDefault(m => m.Id == id));
            
        }

        /// <summary>
        /// Refactor this method to go through proper layers and update changes to the DB.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(EditEmployeeDto input)
        {
            string query = @"UPDATE [dbo].[Employee]
                            SET [FullName] = @FullName,
                            [Tin] = @Tin,
                            [Birthdate] = @Birthdate,
                            [EmployeeTypeId] = @EmployeeTypeId,
                            [Rate] = @Rate
                            WHERE [Id] = @Identifier";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Identifier", input.Id);
                    myCommand.Parameters.AddWithValue("@FullName", input.FullName);
                    myCommand.Parameters.AddWithValue("@Birthdate", input.Birthdate);
                    myCommand.Parameters.AddWithValue("@Tin", input.Tin);
                    myCommand.Parameters.AddWithValue("@EmployeeTypeId", input.TypeId);
                    myCommand.Parameters.AddWithValue("@Rate", input.Rate);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            var item = await Task.FromResult(StaticEmployees.ResultList.FirstOrDefault(m => m.Id == input.Id));
            if (item == null) return NotFound();
            item.FullName = input.FullName;
            item.Tin = input.Tin;
            item.Birthdate = input.Birthdate.ToString("yyyy-MM-dd");
            item.TypeId = input.TypeId;
            item.Rate = input.Rate;
            return Ok(item);
        }

        /// <summary>
        /// Refactor this method to go through proper layers and insert employees to the DB.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(CreateEmployeeDto input)
        {

           var id = await Task.FromResult(StaticEmployees.ResultList.Max(m => m.Id) + 1);

            StaticEmployees.ResultList.Add(new EmployeeDto
            {
                Birthdate = input.Birthdate.ToString("yyyy-MM-dd"),
                FullName = input.FullName,
                Id = id,
                Tin = input.Tin,
                TypeId = input.TypeId
            });

            string query = @"
                           INSERT INTO [dbo].[Employee]
                           ([FullName]
                           ,[Birthdate]
                           ,[TIN]
                           ,[EmployeeTypeId]
                           ,[IsDeleted]
                           ,[Rate])
                         VALUES
                             (@FullName,
                              @Birthdate,
                              @TIN,
                              @EmployeeTypeId,
                              @IsDeleted, 
                              @Rate)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@FullName", input.FullName);
                    myCommand.Parameters.AddWithValue("@Birthdate", input.Birthdate);
                    myCommand.Parameters.AddWithValue("@TIN", input.Tin);
                    myCommand.Parameters.AddWithValue("@EmployeeTypeId", input.TypeId);
                    myCommand.Parameters.AddWithValue("@IsDeleted", false);
                    myCommand.Parameters.AddWithValue("@Rate", input.Rate);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return Created($"/api/employees/{id}", id);
        }


        /// <summary>
        /// Refactor this method to go through proper layers and perform soft deletion of an employee to the DB.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string query = @"UPDATE [dbo].[Employee]
                            SET [IsDeleted] = @IsDeleted
                            WHERE [Id] = @Identifier";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Identifier", id);
                    myCommand.Parameters.AddWithValue("@IsDeleted", 1);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            
            return Ok(id);
        }



        /// <summary>
        /// Refactor this method to go through proper layers and use Factory pattern
        /// </summary>
        /// <param name="id"></param>
        /// <param name="absentDays"></param>
        /// <param name="workedDays"></param>
        /// <returns></returns>
        [HttpPost("{id}/calculate")]
        public async Task<IActionResult> Calculate(Calculate cal)
        {
            Services sc = new Services();
            double salary = Convert.ToDouble(cal.rate);
            double tax = Convert.ToDouble(cal.tax);

            var result = await Task.FromResult(StaticEmployees.ResultList.FirstOrDefault(m => m.Id == cal.Id));

            if (result == null) return NotFound();
            var type = (EmployeeType) result.TypeId;

            return type switch
            {
                EmployeeType.Regular =>
                    //create computation for regular.
                Ok(sc.ComputeRegularSalary(salary, Convert.ToDouble(cal.absentDays), Convert.ToDouble(tax))),
                EmployeeType.Contractual =>
                    //create computation for contractual.
                    Ok(sc.ComputeContractualSalary(salary, Convert.ToDouble(cal.workedDays))),
                _ => NotFound("Employee Type not found")
            };

        }

    }
}
