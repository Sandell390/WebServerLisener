using System.Net;
using System.Text;

namespace WebServer
{
    internal class Program
    {
        static HttpListener listener = new HttpListener();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            StartListeninger();
            Console.ReadLine();
        }

        public static void HttpListenerCallBack(IAsyncResult result)
        {
            Console.WriteLine();

            Console.WriteLine("Thread id: " + Thread.CurrentThread.ManagedThreadId);
            HttpListener listener = (HttpListener)result.AsyncState;

            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;

            IAsyncResult asyncResult = listener.BeginGetContext(new AsyncCallback(HttpListenerCallBack), listener);

            Console.WriteLine("METHOD: "+request.HttpMethod);
            Console.WriteLine("CONTENT TYPE: "+request.ContentType);
            Console.WriteLine("URL: " + request.RawUrl);

            foreach (var item in request.QueryString)
            {
                Console.WriteLine(item.ToString());
            }

            if (request.HasEntityBody)
            {
                byte[] bytes = new byte[request.ContentLength64];
                Console.Write("BODY: ");
                using (Stream bodyInput = request.InputStream)
                {
                    bodyInput.Read(bytes, 0 , bytes.Length);
                }
                Console.WriteLine(Encoding.UTF8.GetString(bytes));
            }

            HttpListenerResponse response = context.Response;

            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using (Stream output = response.OutputStream)
                output.Write(buffer, 0, buffer.Length);
            Console.WriteLine("Completed client request!!!!!");
        }

        public static void StartListeninger()
        {
            listener.Prefixes.Add("http://localhost/");
            Console.WriteLine("Thread id: " + Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("Starts listening");

            IAsyncResult asyncResult = listener.BeginGetContext(new AsyncCallback(HttpListenerCallBack), listener);

            Console.WriteLine("Starts getting context async");
        }

    }
}