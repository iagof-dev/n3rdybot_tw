using System;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using System.Net;
using System.Diagnostics;
using System.IO;
using Tweetinvi.Parameters;
using HtmlAgilityPack;
using System.Linq;
using MySql.Data;
using MySql;
using System.Windows.Forms;

namespace N3rdyBot
{
    internal class Program
    {

        ///
        /// VARIÁVEIS 
        ///

        public static string user_resp = string.Empty;
        public static string msg = string.Empty;
        public static WebClient web = new WebClient();
        public static bool bot_on = true;
        public static string frase = string.Empty;
        public static bool inte_dm = false;
        public static int verify = 0;


        ///
        /// Twitter API config.
        ///

        public static string tw_acesskey = "";
        public static string tw_acesskeysec = "";
        public static string tw_apikey = "";
        public static string tw_apikeysec = "";
        public static TwitterCredentials userCredentials = new TwitterCredentials(tw_apikey, tw_apikeysec, tw_acesskey, tw_acesskeysec);
        public static TwitterClient userClient = new TwitterClient(userCredentials);


        ///
        /// Banco de Dados Flexível
        /// 
        public static string db_server = "";
        public static string db_user = "";
        public static string db_pass = "";
        public static string db_table = "bot_resp";
        public static string db_database = "nrdydesi_n3rdybot";
        public static string db_dados = "server=" + db_server + ";uid=" + db_user + ";pwd=" + db_pass + ";database=" + db_database + "";
        


        public static async Task Main(string[] args)
        {
            ///
            ///Principal Função do BOT (Corredor Principal)
            ///


            // ========= Status =========
            // SC = Starting and Configuring
            // ON = Online/Up And Running
            // EC = Error | Crashed!
            // ==========================

            Console.Title = "[SC] github.com/n3rdydzn | n3rdydzn.software";
            Console.WriteLine("Twitter API | Fazendo Login...");


            var info_accname = await userClient.Users.GetAuthenticatedUserAsync();
            var info_followers = "TwitterAPI.User.GetFollowers()";
            var info_id = "[undefined]";
            string logo = web.DownloadString("https://pastebin.com/raw/MBkFZ417");
            



            Console.WriteLine("Twitter API | Conectado!");
            Console.WriteLine("Banco de Dados | Conectando...");
            try
            {
                MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = db_dados;
                conn.Open();
                conn.Close();
                Console.WriteLine("Banco de Dados | Conectado!");
                Console.WriteLine("Bot | Verificando...");

                System.Threading.Thread.Sleep(500);

            }
            catch (Exception e)
            {
                Console.Title = "[EC!] github.com/n3rdydzn | n3rdydzn.software";
                Console.WriteLine("Banco De Dados | Erro! Mais Detalhes abaixo ↓");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
                Console.WriteLine("Deseja tentar novamente? (S/N)");
                string ec_resp = Console.ReadLine();
                if (ec_resp == "S")
                {
                    Application.Restart();
                    bot_on = false;
                }
                else
                {
                    bot_on = false;
                }
                
            }
            Console.Title = "[ON] github.com/n3rdydzn | n3rdydzn.software";

            while (bot_on == true)
            {
                Console.Clear();
                Console.WriteLine(logo);
                Console.WriteLine("> Usuário: @" + info_accname );
                Console.WriteLine("> API key: " + (tw_apikey.Remove(tw_apikey.Length - 9) + "#########"));
                Console.WriteLine("> Banco de Dados: " + db_server);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Status: Online");
                Console.ResetColor();
                Console.WriteLine("\n");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("──────────────────────");
                Console.WriteLine("Escolha uma opção:");
                Console.WriteLine("[1] Criar Tweet      | [6] Retweet");
                Console.WriteLine("[2] Criar Post c/IMG | [7] Curtir/Favoritar");
                Console.WriteLine("[3] Enviar DM        | [9] Ver Mensagens Diretas");
                Console.WriteLine("[4] Seguir");
                Console.WriteLine("[5] Parar de Seguir");
                if (inte_dm == true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[8] DM I.A (ON)");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.WriteLine("[8] DM I.A (OFF)");
                }
                
                Console.WriteLine("[0] Desligar Bot");
                Console.WriteLine("──────────────────────");
                Console.ResetColor();
                user_resp = Console.ReadLine();


                switch (user_resp)
                {
                    case "1":
                        await criartweet();
                        break;
                    case "2":
                        await criartweetmedia();
                        break;
                    case "3":
                        enviardm();
                        break;
                    case "4":
                        await seguiracc();
                        break;
                    case "5":
                        await deixarseguir();
                        break;
                    case "6":
                        await retweet();
                        break;
                    case "7":
                        await curtir();
                        break;
                    case "8":
                        
                        if (inte_dm == true)
                        {
                            //positivo
                            
                            inte_dm = false;

                        }
                        else
                        {
                            //negativo
                            inte_dm = true;
                            autobot();
                        }
                        break;

                    case "9":

                        break;

                    case "0":
                        bot_on = false;
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Opção inválida!");
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
            Console.WriteLine("Bot foi encerrado!");
        }

        public static async Task curtir()
        {
            Console.WriteLine("Coloque o ID da postagem para CURTIR (Exemplo: 1544223251026661378):");
            long user_retweet = long.Parse(Console.ReadLine());
            // reply to a tweet
            await userClient.Tweets.FavoriteTweetAsync(user_retweet);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sucesso!");
            Console.ResetColor();
        }

        public static async Task enviardm()
        {
            Console.WriteLine("Digite o @ da pessoa:");
            string pessoa_ar = Console.ReadLine();

            
            Console.WriteLine("Digite a mensagem para enviar:");
            string msg = Console.ReadLine();

            var pessoa = await  userClient.Users.GetUserAsync(pessoa_ar);
            var dm_send = userClient.Messages.PublishMessageAsync(msg, pessoa);
            var publishedMessage = await userClient.Messages.GetMessageAsync(dm_send.Id);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Dm Enviado ;)");
            Console.ResetColor();

            System.Threading.Thread.Sleep(1500);


        }


        public static async Task retweet()
        {
            Console.WriteLine("Coloque o ID da postagem para retweetar (Exemplo: 1544223251026661378):");
            long user_retweet = long.Parse(Console.ReadLine());
            // reply to a tweet
            var retweet = await userClient.Tweets.PublishRetweetAsync(user_retweet);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sucesso!");
            Console.ResetColor();
        }

        public static async Task criartweet()
        {
            Console.WriteLine("Digite para criar um tweet:");
            msg = Console.ReadLine();
            var tweet = await userClient.Tweets.PublishTweetAsync(msg);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sucesso!");
            Console.ResetColor();
        }


        public static async Task seguiracc()
        {
            Console.WriteLine("Digite o Usuário para seguir:");
            string user_follow = Console.ReadLine();
            await userClient.Users.FollowUserAsync(user_follow);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sucesso!");
            Console.ResetColor();
        }

        public static async Task deixarseguir()
        {
            Console.WriteLine("Digite o Usuário para deixar de seguir:");
            string user_follow = Console.ReadLine();
            await userClient.Users.UnfollowUserAsync(user_follow);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sucesso!");
            Console.ResetColor();


        }

        public static async Task criartweetmedia()
        {
            Console.WriteLine("Digite texto para criar com a foto:");
            string user_legenda = Console.ReadLine();

            Console.WriteLine("Cole o link da foto:");
            string user_linkfoto = Console.ReadLine();

            string user_saveimg = Environment.CurrentDirectory + "\\imgs\\postar.png";

            
            web.DownloadFile(new Uri(user_linkfoto), user_saveimg);
            string barra = "=";
            for (int i = 0; i < user_saveimg.Length; i++)
            {
                Console.Clear();
                string loading =  barra + "=";
                barra = loading;
                Console.WriteLine("[" + loading + "]");
            }
            var tweetinviLogoBinary = File.ReadAllBytes(user_saveimg);
            var uploadedImage = await userClient.Upload.UploadTweetImageAsync(tweetinviLogoBinary);
            var tweetWithImage = await userClient.Tweets.PublishTweetAsync(new PublishTweetParameters(user_legenda)
            {
                Medias = { uploadedImage }
            });

            File.Delete(user_saveimg);


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sucesso!");
            Console.ResetColor();
        }

        public static async Task tweetfrasesale()
        {
            

            string resp = string.Empty;
            

            Console.WriteLine(frase);
            Console.WriteLine("Deseja publicar? (Sim/Nao)");
            resp = Console.ReadLine();

            if (resp == "sim") {
                msg = "" + frase + "" + " Fonte: http://lerolero.com";
                var tweet = await userClient.Tweets.PublishTweetAsync(msg);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Sucesso!");
                Console.ResetColor();
            }
            else
            {
                //nada
            }
        }

        public static async Task autobot()
        {
            //while (inte_dm == true)
            //{
                //var messages = await userClient.Messages.GetMessagesAsync();
                //Console.WriteLine(messages);


            //}
        }


    }
}
