using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using KingShaggySFConnectionManager;

namespace KingShaggySalesforceSource
{
    [DtsPipelineComponent(DisplayName = "KS Salesforce Source"
       , ComponentType = ComponentType.SourceAdapter
       , Description = "Salesforce Source"
       , UITypeName = "KingShaggySalesforceSource.KingShaggySalesforceSourceUI, KingShaggySalesforceSource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=557c716fe3bc828b"
       , IconResource = "KingShaggySalesforceSource.Resources.KingShaggy_NewBl2.ico"
        )]
    public class KingShaggySalesforceSource : PipelineComponent
    {
        /// Remember the lifecycle!
        /// AcquireConnections()
        /// Validate()
        /// ReleaseConnections()
        /// PrepareForExecute()
        /// AcquireConnections()
        /// PreExecute()
        /// PrimeOutput()
        /// ProcessInput()
        /// PostExecute()
        /// ReleaseConnections()
        /// Cleanup()
        /// 

        public KingShaggySFConnectionManager.KingShaggySFConnectionManager.AuthConn sfConnection;
        private KingShaggySFConnectionManager.KingShaggySFConnectionManager ksConnectionManager;
        string subUrl = "/services/data/";

        public int[] mapOutputColsToBufferCols;

        public override void PerformUpgrade(int pipelineVersion)
        {

            ComponentMetaData.CustomPropertyCollection["UserComponentTypeName"].Value = this.GetType().AssemblyQualifiedName;
        }
        public override void AcquireConnections(object transaction)
        {
            if (ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager != null)
            {
                ConnectionManager connectionManager = Microsoft.SqlServer.Dts.Runtime.DtsConvert.GetWrapper(
                  ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager);

                this.ksConnectionManager = connectionManager.InnerObject as KingShaggySFConnectionManager.KingShaggySFConnectionManager;

                if (this.ksConnectionManager == null)
                    throw new Exception("Couldn't get the Salesforce connection manager, ");

                this.sfConnection = (KingShaggySFConnectionManager.KingShaggySFConnectionManager.AuthConn)this.ksConnectionManager.AcquireConnection(transaction);
            }
        }
        public override void ReleaseConnections()
        {
            if (ksConnectionManager != null)
            {
                this.ksConnectionManager.ReleaseConnection(sfConnection);
            }
        }
        public override void ReinitializeMetaData()
        {
            IDTSOutput100 output = ComponentMetaData.OutputCollection[0];

            output.OutputColumnCollection.RemoveAll();
            output.ExternalMetadataColumnCollection.RemoveAll();

            this.ComponentMetaData.RemoveInvalidInputColumns();

            var soaq_qry = this.ComponentMetaData.CustomPropertyCollection["SOAQ"].Value.ToString();
            if (!string.IsNullOrEmpty(soaq_qry))
            {
                AddOutputColumns(soaq_qry);
            }
            base.ReinitializeMetaData();
        }
        public override DTSValidationStatus Validate()
        {
            return base.Validate();
        }
        public override IDTSCustomProperty100 SetComponentProperty(string propertyName, object propertyValue)
        {
            if (propertyName == "SOAQ" && ComponentMetaData.OutputCollection[0].OutputColumnCollection.Count == 0)
            {
                AddOutputColumns(propertyValue.ToString());
            }

            return base.SetComponentProperty(propertyName, propertyValue);
        }
        public override void ProvideComponentProperties()
        {
            // Reset the component.
            base.ProvideComponentProperties();
            base.RemoveAllInputsOutputsAndCustomProperties();
            ComponentMetaData.RuntimeConnectionCollection.RemoveAll();

            IDTSCustomProperty100 sf_query = ComponentMetaData.CustomPropertyCollection.New();
            sf_query.Name = "SOAQ";
            sf_query.Description = "KingShaggy Salesforce Connector Query";

            IDTSCustomProperty100 uiAccessMode = ComponentMetaData.CustomPropertyCollection.New();
            uiAccessMode.Name = "AccessMode";
            uiAccessMode.Description = "AccessMode";

            IDTSCustomProperty100 uiTablesListItem = ComponentMetaData.CustomPropertyCollection.New();
            uiTablesListItem.Name = "SelectedObject";
            uiTablesListItem.Description = "SelectedObject";

            IDTSCustomProperty100 uiCustomQuery = ComponentMetaData.CustomPropertyCollection.New();
            uiCustomQuery.Name = "CustomQuery";
            uiCustomQuery.Description = "CustomQuery";
            uiCustomQuery.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            uiCustomQuery.Value = (object)string.Empty;

            IDTSOutput100 output = ComponentMetaData.OutputCollection.New();
            output.Name = "SFOutput";

            output.ExternalMetadataColumnCollection.IsUsed = true;
            // validate external metadata
            ComponentMetaData.ValidateExternalMetadata = true;

            IDTSRuntimeConnection100 connection = ComponentMetaData.RuntimeConnectionCollection.New();
            connection.Name = "KS Salesforce Connection";
            connection.ConnectionManagerID = "KS Salesforce Connection";
        }

        public void AddOutputColumns(string propValueQuery)
        {
            string objectName = GetObjectNameFromQuery(propValueQuery);
            if (string.IsNullOrEmpty(objectName))
            {
                //Throw exception here.
            }
            string modifiedQuery = propValueQuery.Contains("LIMIT") ? propValueQuery.Substring(0, propValueQuery.IndexOf("LIMIT")) + "LIMIT 1" : propValueQuery + " LIMIT 1";
            var fullBaseUrl = sfConnection.InstKSUrl + subUrl + sfConnection.ApiKSVersion + "/";

            var record = GetRecordForTypesAsync(fullBaseUrl, sfConnection.AccKSToken, objectName, modifiedQuery).GetAwaiter().GetResult();
            var fieldTypes = GetFieldTypes(fullBaseUrl, sfConnection.AccKSToken, objectName).GetAwaiter().GetResult();

            if (record.Count > 0)
            {
                foreach (var rec in record) //Only 1 record.
                {
                    foreach (var field in rec)
                    {
                        if (field.Key == "attributes")
                        {
                            continue;
                        }
                        var fieldName = field.Key;
                        var fieldValue = field.Value;
                        var fieldAttri = fieldTypes[fieldName];

                        Console.WriteLine($"  {fieldName} ({fieldAttri}): {fieldValue}");

                        IDTSOutputColumn100 outputCol = ComponentMetaData.OutputCollection[0].OutputColumnCollection.New();

                        //Convet to .Net type and then to SSIS type.
                        Type dotNetType = MapSalesforceTypeToDotNetType(fieldAttri.Type);
                        DataType dType = MapDotNetTypeToSSISType(dotNetType);
                        int length = fieldAttri.Length;
                        int precision = (int)fieldAttri.Precision;
                        int scale = (int)fieldAttri.Scale;
                        int codePage = (int)fieldAttri.CodePage;

                        switch (dType)
                        {
                            case DataType.DT_STR:
                            case DataType.DT_TEXT:
                                precision = 0;
                                scale = 0;
                                break;
                            case DataType.DT_NUMERIC:
                                length = 0;
                                codePage = 0;
                                if (precision > 38)
                                    precision = 38;
                                if (scale > precision)
                                    scale = precision;
                                break;
                            case DataType.DT_DECIMAL:
                                length = 0;
                                precision = 0;
                                codePage = 0;
                                if (scale > 28)
                                    scale = 28;
                                break;
                            case DataType.DT_WSTR:
                                length = (length == 0) ? 4000 : length;
                                precision = 0;
                                scale = 0;
                                codePage = 0;
                                break;
                            //case DataType.DT_R8:
                            //    precision = ;
                            //    scale = ;
                            //    codePage = ;
                            //    break;
                            default:
                                length = 0;
                                precision = 0;
                                scale = 0;
                                codePage = 0;
                                break;
                        }

                        length = (length > 4000) ? 4000 : length;

                        outputCol.Name = fieldName;
                        outputCol.SetDataTypeProperties(dType, length, precision, scale, codePage);

                        CreateExternalMetaDataColumn(ComponentMetaData.OutputCollection[0].ExternalMetadataColumnCollection, outputCol);
                    }
                    Console.WriteLine();
                }
            }
        }
        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            base.PrimeOutput(outputs, outputIDs, buffers);

            IDTSOutput100 output = ComponentMetaData.OutputCollection.FindObjectByID(outputIDs[0]);
            PipelineBuffer buffer = buffers[0];

            var propQuery = ComponentMetaData.CustomPropertyCollection["SOAQ"].Value.ToString();

            var fullBaseUrl = sfConnection.InstKSUrl + subUrl + sfConnection.ApiKSVersion + "/";
            string nextRecordsUrl = null;

            do {
                var results = GetRecordsAsyncWithCont(sfConnection.AccKSToken, fullBaseUrl, propQuery, nextRecordsUrl).GetAwaiter().GetResult();

                nextRecordsUrl = results.NextRecordsUrl;

                if (results.dt.Rows.Count > 0)
                {
                    foreach (DataRow row in results.dt.Rows)
                    {
                        buffer.AddRow();

                        for (int x = 0; x < mapOutputColsToBufferCols.Length; x++)
                        {
                            if (row.IsNull(x))
                                buffer.SetNull(mapOutputColsToBufferCols[x]);
                            else
                                buffer[mapOutputColsToBufferCols[x]] = row[x];
                        }
                    }
                }
            } while (!string.IsNullOrEmpty(nextRecordsUrl));

            buffer.SetEndOfRowset();
        }
        public override void PreExecute()
        {
            base.PreExecute();

            IDTSOutput100 output = ComponentMetaData.OutputCollection[0];
            mapOutputColsToBufferCols = new int[output.OutputColumnCollection.Count];

            for (int i = 0; i < ComponentMetaData.OutputCollection[0].OutputColumnCollection.Count; i++)
            {
                mapOutputColsToBufferCols[i] = BufferManager.FindColumnByLineageID(output.Buffer, output.OutputColumnCollection[i].LineageID);
            }
        }

        /// <summary>
        /// Create an external metadata column for each output. Map the two
        /// by setting the ExternalMetaDataColumnID property of the output to
        /// the external metadata column.
        /// </summary>
        /// <param name="output">The output the columns are added to.</param>
        private static void CreateExternalMetaDataColumn(IDTSExternalMetadataColumnCollection100 externalCollection, IDTSOutputColumn100 column)
        {
            // For each output column create an external meta data columns.
            IDTSExternalMetadataColumn100 eColumn = externalCollection.New();
            eColumn.Name = column.Name;
            eColumn.DataType = column.DataType;
            eColumn.Precision = column.Precision;
            eColumn.Length = column.Length;
            eColumn.Scale = column.Scale;

            // wire the output column to the external metadata
            column.ExternalMetadataColumnID = eColumn.ID;
        }

        #region Overloaded insert and delete methods ( insert/delete inputs, outputs and columns )

        //public override void PerformUpgrade(int pipelineVersion)
        //{
        //    base.PerformUpgrade(pipelineVersion);
        //}

        public override IDTSInput100 InsertInput(DTSInsertPlacement insertPlacement, int inputID)
        {
            throw new PipelineComponentHResultException("You cannot insert an input", HResults.DTS_W_GENERICWARNING);
        }

        public override IDTSOutput100 InsertOutput(DTSInsertPlacement insertPlacement, int outputID)
        {
            throw new PipelineComponentHResultException("Output insertion has been disabled!", HResults.DTS_W_GENERICWARNING);
        }

        public override void DeleteInput(int inputID)
        {
            throw new PipelineComponentHResultException("You cannot delete an input", HResults.DTS_W_GENERICWARNING);
        }

        public override void DeleteOutput(int outputID)
        {
            throw new PipelineComponentHResultException("Ouput deletion has been disabled!", HResults.DTS_W_GENERICWARNING);
        }

        public override void DeleteExternalMetadataColumn(int iID, int iExternalMetadataColumnID)
        {
            throw new PipelineComponentHResultException("You cannot delete external metadata column", HResults.DTS_W_GENERICWARNING);
        }

        public override IDTSOutputColumn100 InsertOutputColumnAt(int outputID, int outputColumnIndex, string name, string description)
        {
            throw new PipelineComponentHResultException("Output column insertion has been disabled!", HResults.DTS_W_GENERICWARNING);
        }

        public override IDTSExternalMetadataColumn100 InsertExternalMetadataColumnAt(int iID, int iExternalMetadataColumnIndex, string strName, string strDescription)
        {
            throw new PipelineComponentHResultException("External metadata column insertion has been disabled!", HResults.DTS_W_GENERICWARNING);
        }

        //=================================================================================================

        #endregion Overloaded insert and delete methods ( insert/delete inputs, outputs and columns )

        static async Task<List<Dictionary<string, object>>> GetRecordForTypesAsync(string instanceUrl, string accessToken, string objectName, string query)
        {
            using (var client = new HttpClient())
            {
                var queryEndpoint = $"{instanceUrl}query?q={Uri.EscapeDataString(query)}";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync(queryEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(content);

                    var records = new List<Dictionary<string, object>>();

                    foreach (var record in jsonResponse.records)
                    {
                        var recordDict = new Dictionary<string, object>();
                        foreach (var field in record)
                        {
                            recordDict[field.Name] = field.Value;
                        }
                        records.Add(recordDict);
                    }

                    return records;
                }

                Console.WriteLine($"Error querying records: {response.ReasonPhrase}");
                return null;
            }
        }
        static async Task<Dictionary<string, (string Type, int Length, int? Precision, int? Scale, int? CodePage)>> GetFieldTypes(string instanceUrl, string accessToken, string objectName)
        {
            using (var client = new HttpClient())
            {
                var describeEndpoint = $"{instanceUrl}sobjects/{objectName}/describe";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync(describeEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(content);

                    var fieldTypes = new Dictionary<string, (string Type, int Length, int? Precision, int? Scale, int? CodePage)>();

                    foreach (var field in jsonResponse.fields)
                    {
                        var fieldName = field.name.ToString();
                        var fieldType = (string)field.type.ToString();
                        var fieldLength = 0;

                        //if (fieldType == "string" && field.length != null)
                        if (field.length != null)
                        {
                            fieldLength = int.Parse(field.length.ToString());
                        }

                        // Extract field precision, scale, and codePage if available
                        var fieldPrecision = field.precision != null ? (int)field.precision : (int?)null;
                        var fieldScale = field.scale != null ? (int)field.scale : (int?)null;
                        var fieldCodePage = field.byteLength != null ? (int)field.byteLength : (int?)null;

                        fieldTypes[fieldName] = (Type: fieldType, Length: fieldLength, Precision: fieldPrecision, Scale: fieldScale, CodePage: fieldCodePage);
                    }

                    return fieldTypes;
                }

                Console.WriteLine($"Error getting field types: {response.ReasonPhrase}");
                return null;
            }
        }
        public async Task<(DataTable dt, string NextRecordsUrl)> GetRecordsAsyncWithCont(string accessToken, string apiEndpoint, string query, string nextRecordsUrl)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiEndpoint);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                string queryEndpoint = string.IsNullOrEmpty(nextRecordsUrl) ? $"query?q={Uri.EscapeDataString(query)}" : nextRecordsUrl;
                //var queryEndpoint = $"query?q={Uri.EscapeDataString(query)}";
                var dataTable = new DataTable();

                var response = await client.GetAsync(queryEndpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response to a DataTable
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                string objectName = GetObjectNameFromQuery(query);
                var fieldTypes = GetFieldTypes(apiEndpoint, accessToken, objectName).GetAwaiter().GetResult();

                // Create columns based on the first record's fields
                if (dataTable.Columns.Count == 0 && jsonResponse.records.Count > 0)
                {
                    foreach (JProperty property in jsonResponse.records[0])
                    {
                        if (property.Name == "attributes")
                        {
                            //Put in one liner.
                            continue;
                        }
                        var fieldType = MapSalesforceTypeToDotNetType(fieldTypes[property.Name].Type);

                        dataTable.Columns.Add(property.Name, fieldType); // Adjust the data type as needed
                    }
                }

                // Add rows to the DataTable
                foreach (var record in jsonResponse.records)
                {
                    var row = dataTable.NewRow();

                    foreach (JProperty property in record)
                    {
                        if (property != null)
                        {
                            if (property.Name == "attributes")
                            {
                                continue;
                            }

                            string originalValue = property.Value.ToString();
                            if (!string.IsNullOrEmpty(originalValue))
                            {
                                if (originalValue.Length > 4000) //MaxValue SSIS length
                                {
                                    string truncatedValue = originalValue.Substring(0, 4000);
                                    property.Value = truncatedValue;
                                }
                            }
                            //Apply substring for values more than 4000 in length.
                            row[property.Name] = string.IsNullOrEmpty(property.Value.ToString()) ? (object)DBNull.Value : property.Value.ToString(); // Adjust the data type as needed
                        }
                }

                    dataTable.Rows.Add(row);
                }

                return (dataTable, jsonResponse.nextRecordsUrl);
            }
        }

        public async Task<DataTable> GetRecordsAsync(string accessToken, string apiEndpoint, string query)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiEndpoint);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var queryEndpoint = $"query?q={Uri.EscapeDataString(query)}";
                var dataTable = new DataTable();

                do
                {
                    var response = await client.GetAsync(queryEndpoint);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize JSON response to a DataTable
                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    // Create columns based on the first record's fields
                    if (dataTable.Columns.Count == 0 && jsonResponse.records.Count > 0)
                    {
                        foreach (JProperty property in jsonResponse.records[0])
                        {
                            if (property.Name == "attributes")
                            {
                                continue;
                            }
                            dataTable.Columns.Add(property.Name);
                        }
                    }

                    // Add rows to the DataTable
                    foreach (var record in jsonResponse.records)
                    {
                        var row = dataTable.NewRow();

                        foreach (JProperty property in record)
                        {
                            if (property.Name == "attributes")
                            {
                                continue;
                            }

                            row[property.Name] = property.Value; // Adjust the data type as needed
                        }

                        dataTable.Rows.Add(row);
                    }
                    // Check if there are more records to fetch
                    queryEndpoint = jsonResponse.nextRecordsUrl;
                } while (!string.IsNullOrEmpty(queryEndpoint));

                return dataTable;
            }
        }
        public async Task<string> GetObjectFields(string instanceUrl, string accessToken, string objectApiName)
        {
            List<string> objectDescription = new List<string>();

            using (HttpClient client = new HttpClient())
            {
                // Set up the Salesforce REST API endpoint for object metadata
                string endpoint = $"{instanceUrl}sobjects/{objectApiName}/describe/";
                //string endpoint = $"{instanceUrl}/services/data/v52.0/sobjects/{objectApiName}/describe/";

                // Set the authorization header with the access token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Make the request to Salesforce
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response and extract the fields
                    string responseBody = response.Content.ReadAsStringAsync().ContinueWith(task => task.GetAwaiter()).Result.GetResult();
                    //string responseBody = await response.Content.ReadAsStringAsync();

                    // You can further process the JSON to extract the field information as needed
                    JObject json = JObject.Parse(responseBody);
                    // Navigate through the JSON structure to access field information
                    JArray fields = (JArray)json["fields"];

                    foreach (var field in fields)
                    {
                        //string fieldPrecision = field["precision"].ToString();
                        //string fieldLength = field["length"].ToString();
                        //Console.WriteLine($"Field Name: {fieldName} \tField Label: {fieldLabel} \tField Type: {fieldType} \tField Scale: {fieldScale} \tField Precision: {fieldPrecision} \tField Length: {fieldLength}");
                        ////Console.WriteLine($"Field Name: {fieldName}");

                        objectDescription.Add(field["name"].ToString());
                    }

                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }

            return (string.Join(", ", objectDescription));
        }
        static string GetObjectNameFromQuery(string query)
        {
            // Define a regular expression pattern to match object names in a SOQL query
            string pattern = @"FROM\s+([a-zA-Z0-9_]+)";

            // Use Regex.Match() to find the first match in the query string
            Match match = Regex.Match(query, pattern, RegexOptions.IgnoreCase);

            // Check if a match is found
            if (match.Success)
            {
                // Extract and return the object name
                return match.Groups[1].Value;
            }
            else
            {
                // Return null if no match is found
                return null;
            }
        }
        // Map Salesforce data type to .NET data type
        static Type MapSalesforceTypeToDotNetType(string sfType)
        {
            switch (sfType.ToLower())
            {
                case "text":
                case "textarea":
                case "longtextarea":
                case "richtextarea":
                case "email":
                case "phone":
                case "url":
                case "picklist":
                case "reference":
                    return typeof(string);
                case "checkbox":
                case "boolean":
                    return typeof(bool);
                case "date":
                case "datetime":
                    return typeof(DateTime);
                case "time":
                    return typeof(TimeSpan);
                case "multipicklist":
                    return typeof(string);
                    //return typeof(List<string>);
                case "currency":
                case "double":
                case "percent":
                    return typeof(double);
                case "int":
                    return typeof(int);
                case "decimal":
                    return typeof(decimal);
                case "long":
                    return typeof(long);
                case "id":
                    return typeof(string);
                default:
                    //return typeof(string); // Default to object if the mapping is not found
                    return typeof(object); // Default to object if the mapping is not found
            }
        }
        // Map .NET data type to SSIS data type
        static DataType MapDotNetTypeToSSISType(Type dotNetType)
        {
            if (dotNetType == typeof(string))
                return DataType.DT_WSTR;
            else if (dotNetType == typeof(bool))
                return DataType.DT_BOOL;
            else if (dotNetType == typeof(Boolean))
                return DataType.DT_BOOL;
            else if (dotNetType == typeof(DateTime))
                return DataType.DT_DBTIMESTAMP;
            else if (dotNetType == typeof(TimeSpan))
                return DataType.DT_DBTIME2;
            else if (dotNetType == typeof(List<string>))
                return DataType.DT_WSTR; // Use Object and handle the conversion in SSIS
            else if (dotNetType == typeof(int))
                return DataType.DT_I4;
            else if (dotNetType == typeof(decimal))
                return DataType.DT_NUMERIC;
            else if (dotNetType == typeof(double))
                return DataType.DT_R8;
            else if (dotNetType == typeof(long))
                return DataType.DT_I8;
            else
                return DataType.DT_WSTR; // Default to empty if the mapping is not found
                //return DataType.DT_EMPTY; // Default to empty if the mapping is not found
        }
}

}
