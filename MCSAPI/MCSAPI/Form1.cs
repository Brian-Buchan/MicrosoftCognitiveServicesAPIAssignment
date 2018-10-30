using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

// 3rd party NuGet packages
using CommandLine;

namespace MCSAPI
{
    // parse command line options 
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('t', "train", Required = false, HelpText = "Train model.")]
        public bool Train { get; set; }

        [Option('s', "status", Required = false, HelpText = "Get training status.")]
        public bool Status { get; set; }

        [Option('a', "add", Required = false, HelpText = "Add example utterances to model.")]
        public IEnumerable<string> Add { get; set; }
    }

    public partial class Form1 : Form
    {
        static string appID = "cd59a1de-7d6e-4a3a-9b17-a4c6ffd36056";
        static string appVersion = "0.1";
        static string authoringKey = "533fe2634b0f42f68c996c0af991c9f8";
        static string host = "https://westus.api.cognitive.microsoft.com";
        static string path = "/luis/api/v2.0/apps/" + appID + "/versions/" + appVersion + "/";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                Make_Intent_Request(textBox1.Text);
            }
        }

        private async void Make_Intent_Request(string querey)
        {
            //textBox1.Text = querey;
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            
            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", authoringKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["q"] = querey;

            // These optional request parameters are set to their default values
            queryString["timezoneOffset"] = "0";
            queryString["verbose"] = "false";
            queryString["spellCheck"] = "false";
            queryString["staging"] = "false";

            var endpointUri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + appID + "?" + queryString;
            var response = await client.GetAsync(endpointUri);

            var strResponseContent = await response.Content.ReadAsStringAsync();

            // Display the JSON result from LUIS
            //Console.WriteLine(strResponseContent.ToString());
            textBox1.Text = strResponseContent.ToString();
        }

        private void add_Control(Control control)
        {
            Controls.Add(control);
        }

        private void delete_Contol(Control control)
        {
            Controls.Remove(control);
        }

        private void focus_Control(Control control)
        {
            control.Focus();
        }
    }
}
