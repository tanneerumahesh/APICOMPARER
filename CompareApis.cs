using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ApiComparer
{
    public class CompareApis
    {
         string legacy_api = "https://jsonplaceholder.typicode.com/";
         string new_api = "https://jsonplaceholder.typicode.com/";

        List<string> endpoints = new List<string>()
        {
            "todos/1",
            "todos/2"
        };

       public Dictionary<string, List<string>> requests() {

           Dictionary<string, List<string>> prequests = new Dictionary<string, List<string>>();

           prequests.Add("todos/1", new List<string>() {"payload1", "payload2"});
           prequests.Add("todos/2", new List<string>() {"payload3", "payload4"});

           return prequests;

       }

        public void Compare()
        {
            var legacy_client = new RestClient(legacy_api);

            var new_api_client = new RestClient(new_api);

            List<Tuple<string, string, string, string>> errors = new List<Tuple<string, string, string, string>>();

            var crequests = requests(); 

            foreach (var xrequest in crequests)
            {

                var request1 = new RestRequest(xrequest.Key, Method.POST);

                var request2 = new RestRequest(xrequest.Key, Method.POST);

                foreach(var pload in xrequest.Value) {

                request1.AddParameter("application/x-www-form-urlencoded", pload, ParameterType.RequestBody);

                request2.AddParameter("application/x-www-form-urlencoded", pload, ParameterType.RequestBody);

                var legacy_response = JObject.Parse(legacy_client.Execute(request1).Content);

                var new_response = JObject.Parse(new_api_client.Execute(request2).Content);

                bool result = JToken.DeepEquals(legacy_response, new_response);

                if(!result)
                {
                    errors.Add(Tuple.Create(xrequest.Key, pload, legacy_response.ToString(), new_response.ToString()));
                }

                }   

            }

            if(errors.Count>0)
            {
                Console.Write("********* Migrations Failed :( Below are the failures details ***********");

                foreach(var error in errors)
                {
                    Console.Write("**********");
                    Console.Write("Request : "+error.Item1);
                    Console.Write("PayLoad : "+error.Item2);
                    Console.Write("Old Response : "+error.Item3);
                    Console.Write("New Response : "+error.Item4);
                }


            } else
            {
                Console.Write("WOW....All migrations seems to be working fine :)");
            }


        } 
    }
}
