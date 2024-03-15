using Npgsql;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V120.DOM;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PaschoalottoTeste
{
    public static class Process
    {
        public static void StartProcess()
        {
            EdgeDriver driver = OpenBrowserAndWebSite(@"https://10fastfingers.com/typing-test/portuguese");

            if (driver == null) { MessageBox.Show("ERRO!"); return; }

        }

        private static EdgeDriver OpenBrowserAndWebSite(string url)
        {
            try
            {
                EdgeDriver driver = FuncSelenium.OpenNavigator();

                driver.Navigate().GoToUrl(url);
                driver.Manage().Window.Maximize();

                IWebElement CookieAllowButton = driver.FindElement(By.Id("CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll"));

                if (CookiePopUp(driver))
                {
                    driver.FindElement(By.Id("CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll")).Click();
                }

                if (!TextIsReady(driver)) { throw new Exception("Não foi possível encontrar as palavras a serem lidas na página."); }

                if (!ReadAndTypeText(driver)) { throw new Exception("Não foi possível encontrar as palavras a serem lidas na página."); }

                return driver;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static bool CookiePopUp(EdgeDriver driver)
        {
            try
            {
                IWebElement CookieButton = driver.FindElement(By.Id("CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TextIsReady(EdgeDriver driver)
        {
            try
            {
                IWebElement Text = driver.FindElement(By.Id("words"));
                return true;
            }
            catch { return false; }
        }

        private static bool ReadAndTypeText(EdgeDriver driver)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                IWebElement WordsDiv = driver.FindElement(By.Id("row1"));
                IReadOnlyCollection<IWebElement> WordsOnDiv = WordsDiv.FindElements(By.XPath(".//*"));

                IWebElement InputWord = driver.FindElement(By.Id("inputfield"));

                int WordsCount = 0;

                InputWord.SendKeys(driver.FindElement(By.XPath($"//div[@id='row1']//*[@wordnr='0']")).Text + " ");

                string Time = driver.FindElement(By.Id("timer")).Text;

                WordsCount++;

                while (Time != "0:00")
                {
                    if (WordsCount == WordsOnDiv.Count - 1)
                    {
                        InputWord.SendKeys(driver.FindElement(By.XPath($"//div[@id='row1']//*[@wordnr='{WordsCount}']")).Text + " ");

                        while (Time != "0:00")
                        {
                            Thread.Sleep(1000);
                            Time = driver.FindElement(By.Id("timer")).Text;
                        }

                        break;
                    }
                    else
                    {
                        InputWord.SendKeys(driver.FindElement(By.XPath($"//div[@id='row1']//*[@wordnr='{WordsCount}']")).Text + " ");
                    }

                    Thread.Sleep(50);

                    Time = driver.FindElement(By.Id("timer")).Text;
                    WordsCount++;
                }

                Thread.Sleep(5000);

                string WPM = driver.FindElement(By.XPath("//td[@id='wpm']//strong")).Text;
                string KeyStrokesCorrect = driver.FindElement(By.XPath("//tr[@id='keystrokes']//td[@class='value']//span[@class='correct']")).Text;
                string KeyStrokesWrong = driver.FindElement(By.XPath("//tr[@id='keystrokes']//td[@class='value']//span[@class='wrong']")).Text;
                string Acurracy = driver.FindElement(By.XPath("//tr[@id='accuracy']//td[@class='value']//strong")).Text;
                string CorrectWords = driver.FindElement(By.XPath("//tr[@id='correct']//td[@class='value']//strong")).Text;
                string WrongWords = driver.FindElement(By.XPath("//tr[@id='wrong']//td[@class='value']//strong")).Text;

                DataBase.InsertExecutionOnDataBase(WPM, KeyStrokesCorrect, KeyStrokesWrong, Acurracy, CorrectWords, WrongWords);

                driver.Quit();

                DataBase.MostrarUltimoRegistro();

                return true;

            }
            catch { return false; }
        }

        private static string[] TransformCollectionInStringArray(IReadOnlyCollection<IWebElement> WordsOnDiv)
        {
            string[] words = new string[WordsOnDiv.Count()];

            int aux = 0;

            foreach (IWebElement word in WordsOnDiv)
            {
                words[aux] = word.Text;
                aux++;
            }

            return words;
        }
    }

    public static class DataBase
    {
        private static void InsertExecution(Execution execution)
        {
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=159247;Database=PaschoalottoTeste;";

            // Criar uma conexão com o banco de dados
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    // Abrir a conexão
                    connection.Open();

                    Console.WriteLine("Conexão bem-sucedida!");

                    string sql = "INSERT INTO public.\"Execution\" (\"WPM\", \"KeystrokesQtde\", \"Accuracy\", \"CorrectWords\", \"WrongWords\") " +
                     "VALUES (@WPM, @KeystrokesQtde, @Accuracy, @CorrectWords, @WrongWords)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        // Parâmetros da instrução SQL
                        command.Parameters.AddWithValue("@WPM", execution.WPM);
                        command.Parameters.AddWithValue("@KeystrokesQtde", execution.KeystrokesQtde);
                        command.Parameters.AddWithValue("@Accuracy", execution.Accuracy);
                        command.Parameters.AddWithValue("@CorrectWords", execution.CorrectWords);
                        command.Parameters.AddWithValue("@WrongWords", execution.WrongWords);

                        // Executar a instrução SQL
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} linhas afetadas.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao conectar ao banco de dados: " + ex.Message);
                    return;
                }
            }

        }

        public static void MostrarUltimoRegistro()
        {
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=159247;Database=PaschoalottoTeste;";

            // Criar uma conexão com o banco de dados
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    // Abrir a conexão
                    connection.Open();

                    string sql = "SELECT * FROM public.\"Execution\" ORDER BY \"idExecution\" DESC LIMIT 1";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Extrair os valores do registro
                                int idExecution = reader.GetInt32(reader.GetOrdinal("idExecution"));
                                int WPM = reader.GetInt32(reader.GetOrdinal("WPM"));
                                int KeystrokesQtde = reader.GetInt32(reader.GetOrdinal("KeystrokesQtde"));
                                int Accuracy = reader.GetInt32(reader.GetOrdinal("Accuracy"));
                                int CorrectWords = reader.GetInt32(reader.GetOrdinal("CorrectWords"));
                                int WrongWords = reader.GetInt32(reader.GetOrdinal("WrongWords"));

                                // Exibir os valores na tela usando uma MessageBox
                                string mensagem = $"idExecution: {idExecution}\n" +
                                                  $"WPM: {WPM}\n" +
                                                  $"KeystrokesQtde: {KeystrokesQtde}\n" +
                                                  $"Accuracy: {Accuracy}\n" +
                                                  $"CorrectWords: {CorrectWords}\n" +
                                                  $"WrongWords: {WrongWords}";

                                MessageBox.Show(mensagem, "Último Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Nenhum registro encontrado na tabela 'Execution'.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao conectar ao banco de dados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void InsertExecutionOnDataBase(string WPM, string KeystrokesCorrect, string KeystrokesWrong, string Accuracy, string CorrectWords, string WrongWords)
        {
            Execution execution = CreateExecution(WPM, KeystrokesCorrect, KeystrokesWrong, Accuracy, CorrectWords, WrongWords);

            InsertExecution(execution);
        }

        private static Execution CreateExecution(string WPM, string KeystrokesCorrect, string KeystrokesWrong, string Accuracy, string CorrectWords, string WrongWords)
        {
            Execution execution = new Execution();

            try
            {
                //Transform WPM
                execution.WPM = ExtrairNumero(WPM);


                //Transform Keystrokes
                execution.KeystrokesQtde = ExtrairNumero(KeystrokesCorrect) + ExtrairNumero(KeystrokesWrong);

                //Transform Accuracy
                execution.Accuracy = ExtrairNumero(Accuracy);

                //Transform CorrectWords
                execution.CorrectWords = ExtrairNumero(CorrectWords);

                //Transfor WrongWords
                execution.WrongWords = ExtrairNumero(WrongWords);

                return execution;

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return null; }
        }

        static int ExtrairNumero(string texto)
        {

            MatchCollection matches = Regex.Matches(texto, @"\d+");
            int numero = 0;
            foreach (Match match in matches)
            {
                numero = numero * 10 + int.Parse(match.Value);
            }

            return numero;
        }
    }

    public class Execution
    {
        public int WPM { get; set; }
        public int KeystrokesQtde { get; set; }
        public int Accuracy { get; set; }
        public int CorrectWords { get; set; }
        public int WrongWords { get; set; }

    }
}
