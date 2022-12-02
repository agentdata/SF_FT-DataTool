using System;
using System.Net.Http.Json;
using System.Text.Json;

namespace SF_FT_DataTool
{
	public static class JsonResponses
	{
		public static string BuildError(string ErrorMessage)
		{
			var jsonError = JsonSerializer.Serialize(new { ErrorMessage });
            return jsonError;
		}

        public static string BuildGoodResponse(string SuccessMessage)
        {
            var jsonError = JsonSerializer.Serialize(new { SuccessMessage });
            return jsonError;
        }
    }
}

