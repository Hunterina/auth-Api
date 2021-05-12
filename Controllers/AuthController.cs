using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.String;
using SqlCommand = System.Data.SqlClient.SqlCommand;
using SqlConnection = System.Data.SqlClient.SqlConnection;
using SqlParameter = System.Data.SqlClient.SqlParameter;


namespace authAPI.Controllers{

    public class AuthPassport{
        [JsonProperty("Login")] public long Amount { get; set; }
        [JsonProperty("Password")] public string Currency { get; set; }
    }
    public static class Constantes{

        public const string ConnectionString = @"Server=localhost; User Id=login;Password=login;Encrypt=True;Trusted_Connection=False;";
    }
    public static class AuthController{
        [FunctionName("Auth")]
        public static bool RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req, ILogger log){
            
            string authInfo = req.Query["auth"];
            
            var authJson = JsonConvert.DeserializeObject<AuthPassport>(authInfo);
            return true;
        }

        private static bool SqlLoggingQuery(string login, string password){
            var con = new SqlConnection(Constantes.ConnectionString);
            con.Open();
            /*
            var query = "SELECT userspassport (login, password) VALUES (@login,@password);";
            var cmd = new SqlCommand(query, con);
            */
            var dataParameter = new SqlParameter("@login", login);
            var queryParameter = new SqlParameter("@password", password);
            /*
            cmd.Parameters.Add(dataParameter);
            cmd.Parameters.Add(queryParameter);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            */
            var query = "SELECT id FROM usespassport WHERE login = @login AND password = @password";
            var cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(dataParameter);
            cmd.Parameters.Add(queryParameter);
            var checkIdExist = cmd.ExecuteScalar().ToString();
            con.Close(); 
            return checkIdExist != "";
        }
    }
}