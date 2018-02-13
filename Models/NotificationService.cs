using SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SignalR.Models
{
    public class NotificationService
    {
        static readonly string ConnString = @"data source=.;Initial Catalog=SignalRDemo;user id=sa;password=123;";

        internal static SqlCommand command=null;
        internal static SqlDependency dependency = null;

        public static string GetNotification()
        {
            try
            {
                var messages = new List<tNotification>();
                using (var connection = new SqlConnection(ConnString))
                {
                    connection.Open();
                    using (command = new SqlCommand(@"Select [NotificationId],[Status],[Message],[Extracolum] from [dbo].[tNotification]", connection))
                    {
                        command.Notification = null;
                        if(dependency==null)
                        {
                            dependency = new SqlDependency(command);
                            dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                        }
                        if (connection.State == ConnectionState.Closed)
                            connection.Open();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            messages.Add(item: new tNotification
                            {
                                NotificationId=(int)reader["NotificationId"],
                                Status = reader["Status"] != DBNull.Value ? (string)reader["Status"] : "",
                                Message = reader["Message"] != DBNull.Value ? (string)reader["Message"] : "",
                                ExtraColum = reader["ExtraColum"] != DBNull.Value ? (string)reader["ExtraColum"] : ""
                            });
                        }
                    }
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(messages);
                return json;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private static void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if(dependency!=null)
            {
                dependency.OnChange -= dependency_OnChange;
                dependency = null;
            }
            if (e.Type == SqlNotificationType.Change)
            {
                MyNewHub.Send();
            }
        }
    }
}