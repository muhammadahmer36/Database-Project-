using DbUp;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog;
using System.Reflection;
using DbUp.Engine;
using System.Data;
using System.Text;

internal class Program
{
    public static void Main(string[] args)
    {

        try
        {

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug) // restricted... is Optional
            .CreateLogger();

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json") // Specify the configuration file name
                .Build();

            //var connectionString = "Server=10.50.32.131;Database=HBS_EcoMobile;Trusted_Connection=False;MultipleActiveResultSets=true;User ID=EcoMobile;Password=8!D7n4@s!;TrustServerCertificate=True;";
            var connectionString = config.GetConnectionString("HBSData");

            string deleteScriptIds = config["DeleteScriptIds"] as string;
            

            if (!string.IsNullOrEmpty(deleteScriptIds))
            {
               string guid = $"temp-{Guid.NewGuid()}.sql";
                string script = ProvideScript(deleteScriptIds.Split(","));
                //Console.WriteLine(script);
               var updgraderForDeleteScripts = DeployChanges.To.SqlDatabase(connectionString)
                    //.WithScripts(new Script0006UpdateInCode(deleteScriptIds.Split(',')))
                    .WithScript(guid, script)
                    .WithExecutionTimeout(TimeSpan.FromSeconds(10))
                    .LogToConsole()
                    .Build();
                    ;
                EnsureDatabase.For.SqlDatabase(connectionString);
                var resultForDeleteScripts = updgraderForDeleteScripts.PerformUpgrade();
            }
             
            var builder = DeployChanges.To.SqlDatabase(connectionString);
            builder.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .WithTransactionPerScript()
                    .WithExecutionTimeout(TimeSpan.FromSeconds(10))
                    .LogToConsole();
           

            var updgrader = builder.Build();

#if DEBUG
            EnsureDatabase.For.SqlDatabase(connectionString);
#endif
            var result = updgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();

            Log.Logger.Information("changes executed");

        }
        catch (Exception ex)
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error!");
            Console.WriteLine("Exception! ex {0}", ex.Message);
            Console.ResetColor();

            Log.Logger.Error("Exception => {@ex}", ex);
        }


    }

    public static string ProvideScript(string[] _scriptNames)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("DELETE FROM SchemaVersions WHERE ScriptName IN (");
        for (int i = 0; i < _scriptNames.Length; i++)
        {
            sb.Append($"'{_scriptNames[i]}'");
            if (i != _scriptNames.Length - 1)
            {
                sb.Append(",");
            }
        }

        sb.Append(")");
      
        return sb.ToString();
    }

//    public class Script0006UpdateInCode : IScript
//    {
//        private readonly string[] _scriptNames;
//        public Script0006UpdateInCode(string[] scriptNames)
//        {
//            _scriptNames = scriptNames;
//        }

//        public string ProvideScript(Func<IDbCommand> commandFactory)
//        {
//            var command = commandFactory();

//            StringBuilder sb = new StringBuilder();
//            sb.Append("DELETE FROM SchemaVersions WHERE ScriptName IN (");
//            for (int i = 0; i < _scriptNames.Length; i++)
//            {
//                sb.Append($"'{_scriptNames[i]}'");
//                if (i != _scriptNames.Length - 1)
//                {
//                    sb.Append(",");
//                }
//            }
//            sb.Append(")");
//            string query = sb.ToString();
//            command.CommandText = query;
//            command.ExecuteNonQuery();

//            return sb.ToString();
//        }
//    }

}