using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
// 3rd party NuGet packages
using CommandLine;
using JsonFormatterPlus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCSAPI
{
    public partial class Form1 : Form
    {
        #region LUIS Strings
        static string appID = "cd59a1de-7d6e-4a3a-9b17-a4c6ffd36056";
        static string appVersion = "0.1";
        static string authoringKey = "533fe2634b0f42f68c996c0af991c9f8";
        static string host = "https://westus.api.cognitive.microsoft.com";
        static string path = "/luis/api/v2.0/apps/" + appID + "/versions/" + appVersion + "/";
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        // DoQuerey Button = pressing [Enter] will execute this method
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
                Make_Intent_Request(textBox1.Text);
        }

        private async void Make_Intent_Request(string querey)
        {
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

            //MessageBox.Show(endpointUri, "Querey");
            var strResponseContent = await response.Content.ReadAsStringAsync();

            // Display the JSON result from LUIS
            //Console.WriteLine(strResponseContent.ToString());
            //MessageBox.Show(strResponseContent, "Resposnse");
            HandleResponse(strResponseContent);
        }

        // LUIS Created methods used to programatically create and update a LUIS application from code
        // NOT USED IN THE CURRENT BUILD
        // TODO: Update to incorporate so that the application can differentiate between buttons
        //       and update them so that "the blue button" can be deleted instead of an arbitrary button

        #region LUIS Created
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

        async static Task<HttpResponseMessage> SendGet(string uri)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri);
                request.Headers.Add("Ocp-Apim-Subscription-Key", authoringKey);
                return await client.SendAsync(request);
            }
        }
        async static Task<HttpResponseMessage> SendPost(string uri, string requestBody)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);

                if (!String.IsNullOrEmpty(requestBody))
                {
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "text/json");
                }

                request.Headers.Add("Ocp-Apim-Subscription-Key", authoringKey);
                return await client.SendAsync(request);
            }
        }
        async static Task AddUtterances(string input_file)
        {
            string uri = host + path + "examples";
            string requestBody = File.ReadAllText(input_file);

            var response = await SendPost(uri, requestBody);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Added utterances.");
            Console.WriteLine(JsonFormatter.Format(result));
        }
        async static Task Train()
        {
            string uri = host + path + "train";

            var response = await SendPost(uri, null);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Sent training request.");
            Console.WriteLine(JsonFormatter.Format(result));
        }
        async static Task Status()
        {
            var response = await SendGet(host + path + "train");
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Requested training status.");
            Console.WriteLine(JsonFormatter.Format(result));
        }

        #endregion // NOT USED IN CURRENT BUILD

        // Chooses the operation to fufill based on the LUIS message that is returned
        private void HandleResponse(string response)
        {
            // Parse the JSON response from the LUIS applicaiton
            Data data = JsonConvert.DeserializeObject<Data>(response);

            #region DebugCode
            //MessageBox.Show(data.TopScoringIntent.ToString());
            //foreach (Entity entity in data.entities)
            //{
            //    MessageBox.Show(entity.ToString());
            //}
            #endregion

            Control control = ParseControl(data.entities);

            switch (data.TopScoringIntent.intent)
            {
                case "UI.Entity.Add":
                    if (control != null)
                        add_Control(control);
                    else
                        MessageBox.Show("No control was created. Could not parse Entity.", "Parse Failed.");
                    break;
                case "UI.Entity.Delete":
                    if (control != null)
                        delete_Contol(control);
                    else
                        MessageBox.Show("No control was deleted. Could not parse Entity.", "Parse Failed.");
                    break;
                case "UI.Entity.Select":
                    focus_Control(control);
                    break;
                case "None":
                    MessageBox.Show("Your querey of '" + data.Query + "' returned a response of None.", "Input not recognized.");
                    break;
                default:
                    MessageBox.Show("The LUIS application returned an unexpected response.", "Error!");
                    break;
            }
        }

        // Arbitrary button click method that all created buttons can use for a placeholder
        protected void buttonClick(object sender, EventArgs e)
        {
            MessageBox.Show("This button's name is " + ((Control)sender).Name);
        }

        // Parse the JSON objects and create From.Controls that the application can interact with
        private Control ParseControl(Entity[] entities)
        {
            // TODO: Once LUIS is Updated/Trained/Published when a new object is created in the application
            //       the returned object will contain more information instead of placeholder information
            switch (entities[0].type)
            {
                case "Button":
                    Button button = new Button();
                    button.Text = button.Name;
                    button.ForeColor = Color.Black;
                    button.Click += new EventHandler(buttonClick);
                    return button;
                case "TextBox":
                    TextBox textBox = new TextBox();
                    textBox.Text = textBox.Name;
                    return textBox;
                case "Image":
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Size = new System.Drawing.Size(100, 85);
                    pictureBox.Image = Image.FromFile("smiley.jpg");
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    return pictureBox;
                case "Label":
                    Label label = new Label();
                    label.Text = label.Name;
                    return label;
                default:
                    return null;
            }
        }

        private void add_Control(Control control)
        {
            DesignArea.Controls.Add(control);
        }

        // Because all controls have the same name in the current build
        // this method iterates over the collection, will find a match on the first item, delete it
        // causign the list to shrink, and then move to the next item, this causes half of selected object
        // type to get deleted from the form. Once LUIS is Updated/Trained/Published when a control is created
        // this method will need to be refactored to select the correct item from the DesignArea
        private void delete_Contol(Control control)
        {
            foreach (Control _control in DesignArea.Controls)
            {
                if (control.Name == _control.Name)
                    DesignArea.Controls.Remove(_control);
            }
        }

        private void focus_Control(Control control)
        {
            control.Focus();
        }
    }

    // JSON extracted LUIS objects
    public class Data
    {
        public string Query;
        public Intent TopScoringIntent;
        public Entity[] entities;
    }
    // LUIS object in JSON form that handles LUIS Intents
    public class Intent
    {
        public string intent;
        public double score;
        public override string ToString()
        {
            return intent + ", " + score.ToString();
        }
    }
    // LUIS object in JSON form that handles LUIS Entities
    public class Entity
    {
        public string entity;
        public string type;
        public int startIndex;
        public int endIndex;
        public double score;
        public override string ToString()
        {
            return entity + ", " + type + ", " + score.ToString();
        }
    }
}